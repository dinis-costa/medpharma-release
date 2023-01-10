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
    public class ExamsController : Controller
    {
        private readonly IExamRepository _examsRepository;
        private readonly IFlashMessage _flashMessage;

        public ExamsController(IExamRepository examsRepository, IFlashMessage flashMessage)
        {
            _examsRepository = examsRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            var context = _examsRepository.GetAll().OrderBy(p => p.Name).ToList();

            return View(context);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                await _examsRepository.CreateAsync(exam);

                _flashMessage.Info("New Category Added");

                return RedirectToAction("Index");

            }

            //_flashMessage.Danger("Error! There's already a user with that email!");

            return View(exam);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            var exam = await _examsRepository.GetByIdAsync(id.Value);

            if (exam == null)
            {
                return RedirectToAction("Error404", "Errors");
            }

            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _examsRepository.UpdateAsync(exam);


                    _flashMessage.Info("Changes Saved");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _examsRepository.ExistAsync(exam.Id))
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
            return View(exam);
        }

        // DELETE POST
        public async Task<IActionResult> Delete(int id)
        {
            var speciality = await _examsRepository.GetByIdAsync(id);

            try
            {
                await _examsRepository.DeleteAsync(speciality);

                _flashMessage.Info("Exam Category Deleted");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "This Exam Category is in use";
                ViewBag.ErrorMessage = "Consider deleting all dependencies appended and try again.";
                return RedirectToAction("Error", "Error");
            }
        }
    }
}
