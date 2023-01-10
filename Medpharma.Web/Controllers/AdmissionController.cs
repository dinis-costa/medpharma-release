using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class AdmissionController : Controller
    {
        private readonly IAdmissionRepository _admissionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConverterHelper _converterHelper;

        public AdmissionController(IAdmissionRepository admissionRepository,
                                   ICustomerRepository customerRepository,
                                   IConverterHelper converterHelper)
        {
            _admissionRepository = admissionRepository;
            _customerRepository = customerRepository;
            _converterHelper = converterHelper;
        }

        [Authorize(Roles = "Clerk,Doctor,Admin")]
        public IActionResult Index()
        {
            var dataContext = _admissionRepository.GetAll().Where(ad => ad.Registered == 0).Include("Customer");

            if (User.IsInRole("Customer"))
            {
                return NotFound();
            }
            else
                return View(dataContext);
        }

        [Authorize(Roles = "Clerk")]
        public async Task<IActionResult> Create(string customerString)
        {
            var customer = await _customerRepository.GetByIdAsync(customerString);

            var model = new AdmissionViewModel
            {
                Customer = customer,
                CustomerId = customer.Id,
            };

            return View(model);
        }

        [Authorize(Roles = "Clerk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admission = _converterHelper.ToAdmission(model, true);

                await _admissionRepository.CreateAsync(admission);

                return RedirectToAction("Index", "Admission");

            }
            return View(model);
        }

        [Authorize(Roles = "Clerk, Doctor, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var admission = await _admissionRepository.GetByIdAsync(id);
            await _admissionRepository.DeleteAsync(admission);
            return RedirectToAction(nameof(Index));
        }
    }
}
