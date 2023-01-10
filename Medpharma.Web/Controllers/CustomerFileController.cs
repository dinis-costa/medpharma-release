using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class CustomerFileController : Controller
    {
        #region Fields & Constructor
        private readonly ICustomerFileRepository _filesRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly string ServerDirectory;

        public CustomerFileController(ICustomerFileRepository filesRepository,
                                      ICustomerRepository customerRepository)
        {
            _filesRepository = filesRepository;
            _customerRepository = customerRepository;
            ServerDirectory = Environment.CurrentDirectory + "\\ServerFiles\\";
        }
        #endregion

        #region GET
        public async Task<IActionResult> Index()
        {
            var customerString = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);

            var dataContext = _filesRepository.GetAllCustomerFiles(customerString.Id);
            ViewBag.From = "Index";
            return View(dataContext);
        }

        public async Task<IActionResult> Uploaded(string? customerString)
        {
            if (string.IsNullOrEmpty(customerString))
            {
                var customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                customerString = customer.Id;
            }

            var dataContext = _filesRepository.GetUploadedByCustomerId(customerString);
            return View("Index", dataContext);
        }

        public async Task<IActionResult> Invoices(string? customerString)
        {
            if (string.IsNullOrEmpty(customerString))
            {
                var customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                customerString = customer.Id;
            }

            var dataContext = _filesRepository.GetInvoicesByCustomerId(customerString);
            return View("Index", dataContext);
        }

        public async Task<IActionResult> ShopInvoices(string? customerString)
        {
            if (string.IsNullOrEmpty(customerString))
            {
                var customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                customerString = customer.Id;
            }

            var dataContext = _filesRepository.GetShopInvoicesByCustomerId(customerString);
            return View("Index", dataContext);
        }

        public async Task<IActionResult> Prescriptions(string? customerString)
        {
            if (string.IsNullOrEmpty(customerString))
            {
                var customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                customerString = customer.Id;
            }

            var dataContext = _filesRepository.GetPrescriptionsByCustomerId(customerString);

            return View("Index", dataContext);
        }

        public async Task<IActionResult> Exams(string? customerString)
        {
            if (string.IsNullOrEmpty(customerString))
            {
                var customer = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);
                customerString = customer.Id;
            }

            var dataContext = _filesRepository.GetExamsByCustomerId(customerString);
            return View("Index", dataContext);
        }

        public IActionResult Appointment(int id)
        {
            var dataContext = _filesRepository.GetFilesByAppointmentId(id);
            return View("Index", dataContext);
        }

        #endregion

        public async Task<IActionResult> Create()
        {
            var customerString = await _customerRepository.GetUserByEmailAsync(User.Identity.Name);

            CustomerFileViewModel model = new CustomerFileViewModel();
            model.CustomerId = customerString.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CustomerFileViewModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                FileInfo fi = new FileInfo(model.File.FileName);

                if (fi.Extension == string.Empty)
                {
                    ModelState.AddModelError(string.Empty, "Upload failed: Invalid file extension");
                    return View(model);
                }
                model.FileExtension = fi.Extension;
            }

            if (ModelState.IsValid)
            {
                var user = await _customerRepository.GetByIdAsync(model.CustomerId);
                if (user != null)
                {
                    Guid fileId = Guid.Empty;

                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}{model.FileExtension}";

                    string path = Path.Combine(ServerDirectory, "files", file);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

                    fileId = Guid.Parse(guid);

                    var customerFile = new CustomerFile();
                    customerFile.CustomerId = model.CustomerId;
                    customerFile.FileId = fileId;
                    customerFile.Description = model.Description;
                    customerFile.FileExtension = model.FileExtension;

                    await _filesRepository.CreateAsync(customerFile);
                }
                return RedirectToAction("Index", "CustomerFile", new { customerString = user.Id });
            }

            return RedirectToAction("Index", "Customers");
        }

        [HttpGet]
        public async virtual Task<ActionResult> Download(int id)
        {
            var file = await _filesRepository.GetByIdAsync(id);

            string serverFiles = Path.Combine(ServerDirectory, "files", $"{file.FileId}{file.FileExtension}");

            var fileFinal = await System.IO.File.ReadAllBytesAsync(serverFiles);

            return File(fileFinal, "application/octet-stream", $"{file.FileId}{file.FileExtension}");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var file = await _filesRepository.GetByIdAsync(id);

            string path = Path.Combine(ServerDirectory, "files", $"{file.FileId}{file.FileExtension}");

            System.IO.File.Delete(path);

            await _filesRepository.DeleteAsync(file);

            return RedirectToAction("Index", "CustomerFile", new { customerString = file.CustomerId });
        }
    }
}
