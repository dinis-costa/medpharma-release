using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Data.Repositories.Users;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        #region Fields & Constructors

        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IFlashMessage _flashMessage;

        public DoctorController(IUserHelper userHelper,
                                  IImageHelper imageHelper,
                                  IConverterHelper converterHelper,
                                  IDoctorRepository doctorRepository,
                                  ISpecialityRepository specialityRepository,
                                  IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _doctorRepository = doctorRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _specialityRepository = specialityRepository;
            _flashMessage = flashMessage;

        }

        #endregion

        #region GET

        [Authorize(Roles = "Admin,Clerk")]
        public ActionResult Index()
        {
            var dataContext = _userHelper.Get<Doctor>().Include("Speciality").ToList();
            return View(dataContext);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public IActionResult Create()
        {
            ViewData["Specialities"] = _specialityRepository.GetAll().ToList();
            return View();
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = _doctorRepository.GetById(id);

            if (item == null)
                return RedirectToAction("Error404", "Errors");

            return View(item);
        }

        [Authorize(Roles = "Admin,Doctor,Clerk")]
        public async Task<IActionResult> Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Error404", "Errors");

            var item = await _doctorRepository.GetByIdAsync(id);

            if (id == null)
                return RedirectToAction("Error404", "Errors");

            var model = _converterHelper.ToDoctorViewModel(item as Doctor);

            ViewData["Specialities"] = _specialityRepository.GetAll().ToList();

            ViewBag.IsDisabled = true;

            return View(model);
        }
        #endregion

        #region POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> Create(DoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "doctor");
                    }

                    var customer = _converterHelper.ToDoctor(model, imageId, true);
                    customer.EmailConfirmed = true;

                    await _userHelper.AddUserAsync(customer, "Cinel123!");
                    await _userHelper.AddUserToRoleAsync(customer, "Doctor");

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor,Clerk")]
        public async Task<IActionResult> Edit(string id, DoctorViewModel model)
        {
            if (id != model.Id)
                return RedirectToAction("Error404", "Errors");


            if (ModelState.IsValid)
            {
                var userExists = await _userHelper.GetUserByEmailAsync(model.Email);
                if (userExists == null || userExists.Id == model.Id)
                {
                    try
                    {
                        Guid imageId = model.ImageId;

                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                            imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "doctor");

                        var customer = _converterHelper.ToDoctor(model, imageId, false);

                        var user = await _userHelper.GetUserByIdAsync(customer.Id) as Doctor;
                        //var user = await _doctorRepository.GetByIdAsync(customer.Id);

                        user.Document = customer.Document;
                        user.Email = customer.Email;
                        user.FirstName = customer.FirstName;
                        user.LastName = customer.LastName;
                        user.SpecialityId = customer.SpecialityId;
                        user.ImageId = customer.ImageId;

                        await _userHelper.UpdateUserAsync(user);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await _userHelper.ExistAsync(model.Id))
                        {
                            return RedirectToAction("Details", "Doctor", new { id = model.Id });
                        }
                        else
                            throw;
                    }
                    _flashMessage.Confirmation("Changes Saved");

                    return RedirectToAction("Edit", "Doctor", new { id = model.Id });
                }
            }

            return View(model);
        }

        #endregion
    }
}
