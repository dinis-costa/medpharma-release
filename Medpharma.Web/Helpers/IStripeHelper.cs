using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public interface IStripeHelper
    {
        Task<Response> PayAsync(string cardNumber, string expMonth, string expYear, string cvc, int amountToPay);
    }
}
