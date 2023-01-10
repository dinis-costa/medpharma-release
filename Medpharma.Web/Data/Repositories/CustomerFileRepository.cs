using Medpharma.Web.Data.Entities;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public class CustomerFileRepository : GenericRepository<CustomerFile>, ICustomerFileRepository
    {
        private readonly DataContext _context;

        public CustomerFileRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllCustomerFiles(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId);
        }

        public IQueryable GetUploadedByCustomerId(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId && cf.Type == 1);
        }

        public IQueryable GetInvoicesByCustomerId(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId && cf.Type == 2);
        }

        public IQueryable GetShopInvoicesByCustomerId(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId && (cf.Type == 5 || cf.Type == 6));
        }

        public IQueryable GetPrescriptionsByCustomerId(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId && cf.Type == 3);
        }

        public IQueryable GetExamsByCustomerId(string customerId)
        {
            return _context.CustomerFiles.Where(cf => cf.CustomerId == customerId && cf.Type == 4);
        }

        public IQueryable GetFilesByAppointmentId(int appointmentId)
        {
            return _context.CustomerFiles.Where(cf => cf.AppointmentId == appointmentId);
        }




    }
}
