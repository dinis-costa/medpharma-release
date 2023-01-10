using Medpharma.Mobile.Models;
using System.Threading.Tasks;

namespace Medpharma.Mobile.Services
{
    public interface IApiService
    {
        Task<Response> Login<T>(string url, BindingLogin model);
        Task<Response> GetListAsync<T>(string url);
    }
}