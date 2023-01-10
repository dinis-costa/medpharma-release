using Medpharma.Web.Data.Entities;
using Medpharma.Web.Models;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public interface ICheckoutHelper
    {
        Task<CheckOutPrescriptionInfoViewModel> GetPrescriptionRemainingAsync(Appointment appointment);

    }
}
