using Medpharma.Web.Data.Entities;
using System.Linq;

namespace Medpharma.Web.Data.Repositories
{
    public interface ICustomerFileRepository : IGenericRepository<CustomerFile>
    {
        IQueryable GetUploadedByCustomerId(string customerId);
        IQueryable GetInvoicesByCustomerId(string customerId);
        IQueryable GetPrescriptionsByCustomerId(string customerId);
        IQueryable GetExamsByCustomerId(string customerId);
        IQueryable GetFilesByAppointmentId(int appointmentId);
        IQueryable GetAllCustomerFiles(string customerId);
        IQueryable GetShopInvoicesByCustomerId(string customerId);
    }
}
