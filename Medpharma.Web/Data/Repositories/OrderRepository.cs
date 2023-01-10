using Medpharma.Web.Data.Entities;
using Medpharma.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMedicineRepository _medicineRepository;

        public OrderRepository(DataContext context, IUserHelper userHelper, IMedicineRepository medicineRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _medicineRepository = medicineRepository;
        }


        public async Task<List<OrderDetailTemp>> GetMedicinesCartByUserAsync(string userId)
        {
            var result = await _context.OrderDetailTemp.Include(m => m.Medicine).Where(o => o.Customer.Email == userId).ToListAsync();
            return result;
        }

        public async Task AddPrescriptionToOrderAsync(Medicine model, int productQuantity, Customer customer)
        {
            var medicine = await _context.Medicines.FindAsync(model.Id);

            if (medicine == null)
            {
                return;
            }

            var orderDetailTemp = await _context.OrderDetailTemp.Where(odt => odt.Medicine == medicine).FirstOrDefaultAsync();

            if (orderDetailTemp == null)
            {

                orderDetailTemp = new OrderDetailTemp
                {
                    Medicine = medicine,
                    Price = model.Price,
                    Quantity = productQuantity,
                    Customer = customer,
                };

                _context.OrderDetailTemp.Add(orderDetailTemp);
            }
            //else
            //{
            //    orderDetailTemp.Quantity += productQuantity;

            //    _context.OrderDetailTemp.Update(orderDetailTemp);
            //}

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountCartItems(string userName)
        {
            return await _context.OrderDetailTemp.Where(o => o.Customer.Email == userName).CountAsync();
        }



        public decimal TotalOrderRevenue()
        {
            return _context.Order.Include("Items").Select(a => a.CValue).ToList().Sum();
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailTemp.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;



            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailTemp.Update(orderDetailTemp);
                await _context.SaveChangesAsync();
            }

            if (orderDetailTemp.Quantity == 0)
            {
                _context.OrderDetailTemp.Remove(orderDetailTemp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailTemp.FindAsync(id);

            if (orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailTemp.Remove(orderDetailTemp);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderPrescriptionAsync(string customerId, List<Prescription> prescriptions)
        {
            var customer = await _userHelper.GetUserByIdAsync(customerId) as Customer;

            if (customer == null)
            {
                return false;
            }

            var details = prescriptions.Select(o => new OrderDetail
            {
                Price = o.Medicine.Price,
                Product = o.Medicine,
                Quantity = o.Quantity
            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Customer = customer,
                Items = details
            };


            await CreateAsync(order);

            var selectedOrder = await this.GetOrderById(order.Id);

            foreach (var med in selectedOrder.Items.ToList())
            {

                med.Product.Stock -= (int)med.Quantity;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Order>? ConfirmOrderAsync(string customerId)
        {
            var customer = await _userHelper.GetUserByIdAsync(customerId) as Customer;

            if (customer == null)
            {
                return null;
            }

            var orderTmps = await _context.OrderDetailTemp
                .Include(o => o.Medicine)
                .Where(o => o.Customer == customer)
                .ToListAsync();

            if (orderTmps == null || orderTmps.Count == 0)
            {
                return null;
            }

            var details = orderTmps.Select(o => new OrderDetail
            {
                Price = o.Price,
                Product = o.Medicine,
                Quantity = o.Quantity
            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Customer = customer,
                Items = details
            };


            await CreateAsync(order);

            var selectedOrder = await this.GetOrderById(order.Id);

            foreach (var med in selectedOrder.Items.ToList())
            {

                med.Product.Stock -= (int)med.Quantity;
            }

            _context.OrderDetailTemp.RemoveRange(orderTmps);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string customerId)
        {
            var customer = await _userHelper.GetUserByIdAsync(customerId);

            if (customer == null)
            {
                return null;
            }

            return _context.Order
                .Include(o => o.Items)
                .ThenInclude(p => p.Product)
                .Where(o => o.Customer == customer)
                .OrderByDescending(o => o.OrderDate);
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Order
                .Include(c => c.Customer)
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(i => i.Id == orderId);
        }

        public async Task DeliveryOrder(int orderId)
        {
            var order = await _context.Order.FindAsync(orderId);

            if (order == null)
            {
                return;
            }

            order.DeliveryDate = DateTime.Now;
            order.OrderSent = 1;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
