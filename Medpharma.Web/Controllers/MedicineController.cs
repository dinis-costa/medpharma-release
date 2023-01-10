using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {

        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMedicineRepository _medicineRepository;

        public MedicineController(IImageHelper imageHelper,
                                  IConverterHelper converterHelper,
                                  IMedicineRepository medicineRepository)
        {
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _medicineRepository = medicineRepository;
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public IActionResult Index()
        {
            var dataContext = _medicineRepository.GetAll().OrderBy(p => p.Name).ToList();

            return View(dataContext);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var item = await _medicineRepository.GetByIdAsync(id.Value);

            if (item == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            return View(item);
        }

        [Authorize(Roles = "Admin,Clerk")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var item = await _medicineRepository.GetByIdAsync(id.Value);

            if (item == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var model = _converterHelper.ToMedicineViewModel(item as Medicine);

            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineViewModel model)
        {
            if (ModelState.IsValid)
            {

                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "medicine");

                var medicine = _converterHelper.ToMedicine(model, imageId, true);
                await _medicineRepository.CreateAsync(medicine);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicineViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "medicine");
                        //imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "medicine");
                    }

                    var medicine = _converterHelper.ToMedicine(model, imageId, false);

                    await _medicineRepository.UpdateAsync(medicine);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _medicineRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);

            await _medicineRepository.DeleteAsync(medicine);

            return RedirectToAction("Index");

        }


    }
}
