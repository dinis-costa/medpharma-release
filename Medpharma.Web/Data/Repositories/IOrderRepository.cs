using Medpharma.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Data.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task AddPrescriptionToOrderAsync(Medicine model, int productQuantity, Customer customer);
        Task<int> CountCartItems(string userName);
        Task<List<OrderDetailTemp>> GetMedicinesCartByUserAsync(string userId);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
        Task DeleteDetailTempAsync(int id);
        Task<Order>? ConfirmOrderAsync(string customerId);
        Task<IQueryable<Order>> GetOrderAsync(string customerId);
        Task<Order> GetOrderById(int orderId);
        Task DeliveryOrder(int orderId);
        Task<bool> ConfirmOrderPrescriptionAsync(string customerId, List<Prescription> prescriptions);
        decimal TotalOrderRevenue();
    }
}
