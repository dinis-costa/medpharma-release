using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace Medpharma.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialityController : Controller
    {
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IFlashMessage _flashMessage;

        public SpecialityController(ISpecialityRepository specialityRepository, IFlashMessage flashMessage)
        {
            _specialityRepository = specialityRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            var dataContext = _specialityRepository.GetAll().OrderBy(p => p.Name).ToList();

            return View(dataContext);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var speciality = await _specialityRepository.GetByIdAsync(id.Value);

            if (speciality == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            return View(speciality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                await _specialityRepository.CreateAsync(speciality);

                _flashMessage.Info("New Speciality Added");

                return RedirectToAction("Index");
            }
            return View(speciality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _specialityRepository.UpdateAsync(speciality);

                    _flashMessage.Info("Changes Saved");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _specialityRepository.ExistAsync(speciality.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(speciality);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var speciality = await _specialityRepository.GetByIdAsync(id);

            try
            {
                await _specialityRepository.DeleteAsync(speciality);

                _flashMessage.Info("Speciality Deleted");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "This Speciality is in use";
                ViewBag.ErrorMessage = "Consider deleting all dependencies appended and try again.";
                return RedirectToAction("Error", "Error");
            }

        }
    }
}
