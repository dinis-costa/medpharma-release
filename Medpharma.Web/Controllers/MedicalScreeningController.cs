using Medpharma.Web.Data.Entities;
using Medpharma.Web.Data.Repositories;
using Medpharma.Web.Helpers;
using Medpharma.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medpharma.Web.Controllers
{
    [Authorize]
    public class MedicalScreeningController : Controller
    {
        #region Fields & Controller

        private readonly IPriorityRepository _priorityRepository;
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IMedicalScreeningRepository _medicalScreeningRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IAdmissionRepository _admissionRepository;

        public MedicalScreeningController(
            IPriorityRepository priorityRepository,
            ISpecialityRepository specialityRepository,
            IMedicalScreeningRepository medicalScreeningRepository,
            IUserHelper userHelper,
             IConverterHelper converterHelper,
             IAdmissionRepository admissionRepository
            )
        {
            _priorityRepository = priorityRepository;
            _specialityRepository = specialityRepository;
            _medicalScreeningRepository = medicalScreeningRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _admissionRepository = admissionRepository;
        }

        #endregion

        #region GET

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public IActionResult Index() // Not in use.
        {
            return View();
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Registered()
        {
            List<MedicalScreening> dc = null;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _userHelper.GetUserByEmailAsync(User.Identity.Name) as Doctor;
                dc = _medicalScreeningRepository.GetBySpeciality(doctor.SpecialityId).Where(ms => ms.IsAccepted == false).ToList();
            }
            else
                dc = _medicalScreeningRepository.GetAllWithInfo().Where(ms => ms.IsAccepted == false).ToList();

            return View(dc);
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NotFound();

            var admission = await _admissionRepository.GetByIdWithInfo(id.Value);

            var model = new MedicalScreeningViewModel
            {
                Admission = admission,
                Priorities = _priorityRepository.GetComboPriorities(),
                Specialities = _specialityRepository.GetComboSpeciality(),
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalScreening = await _medicalScreeningRepository.GetByIdWithInfo(id.Value);

            if (medicalScreening == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToMedicalScreeningViewModel(medicalScreening, false);

            model.Priorities = _priorityRepository.GetComboPriorities();
            model.Specialities = _specialityRepository.GetComboSpeciality(); //Replace by only available Specialities.

            return View(model);
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _medicalScreeningRepository.GetByIdWithInfo(id.Value);

            if (model == null)
                return NotFound();

            return View(model);
        }

        #endregion

        #region POST

        [HttpPost]
        [Authorize(Roles = "Admin,Clerk,Doctor")]
        public async Task<IActionResult> Create(MedicalScreeningViewModel model)
        {
            if (ModelState.IsValid)
            {
                var medicaScreening = _converterHelper.ToMedicalScreening(model, true);

                await _medicalScreeningRepository.CreateAsync(medicaScreening);

                var admission = await _admissionRepository.GetByIdAsync(medicaScreening.AdmissionId);

                admission.Registered = 1;

                await _admissionRepository.UpdateAsync(admission);
            }

            return RedirectToAction("Registered");
        }

        [Authorize(Roles = "Admin,Clerk,Doctor")]
        [HttpPost]
        public async Task<IActionResult> Edit(MedicalScreeningViewModel model)
        {
            if (ModelState.IsValid)
            {
                var medicaScreening = _converterHelper.ToMedicalScreening(model, false);

                await _medicalScreeningRepository.UpdateAsync(medicaScreening);

                return RedirectToAction("Registered");
            }

            return View(model);
        }

        #endregion





    }
}
