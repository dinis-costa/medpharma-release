using Medpharma.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Medpharma.Web.Controllers
{
    public class ErrorController : Controller
    {
        //[Route("Error/404")]
        //public IActionResult Error404()
        //{
        //    return View();
        //}


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
