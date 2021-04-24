using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinaryPatientFollow.Models;

namespace VeterinaryPatientFollow.Controllers
{
    public class PatientsController : Controller
    {
        private readonly PatientRegistrationContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PatientsController(PatientRegistrationContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var patients = from p in _context.Patients
                           select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(s => s.Name.Contains(searchString)
                                       || s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    patients = patients.OrderByDescending(p => p.Name);
                    break;
                case "owner_desc":
                    patients = patients.OrderByDescending(p => p.Owner);
                    break;
                case "genus_desc":
                    patients = patients.OrderByDescending(p => p.Genus);
                    break;
                default:
                    patients = patients.OrderBy(p => p.Diagnosis);
                    break;

            }
            int pageSize = 3;
            return View(await PaginatedList<Patient>.CreateAsync(patients.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        public IActionResult Create(int id = 0)
        {
            if (id == 0)
                return View(new Patient());
            else
                return View(_context.Patients.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientID,Name,ImageURL,Owner,Genus,Diagnosis,File")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "img");

                if (Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var fullFileName = Path.Combine(filePath, patient.File.FileName);

                using (var fileStream = new FileStream(fullFileName, FileMode.Create))
                {
                    await patient.File.CopyToAsync(fileStream);
                }

                patient.ImageURL = patient.File.FileName;

                if (patient.PatientID == 0)
                    _context.Add(patient);
                else
                    _context.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientID,Name,ImageURL,Owner,Genus,Diagnosis")] Patient patient)
        {
            if (id != patient.PatientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientID))
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
            return View(patient);
        }

        private bool PatientExists(int patientID)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

