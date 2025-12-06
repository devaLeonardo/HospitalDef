using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalDef.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalContext _context;

        public DoctorsController(HospitalContext context)
        {
            _context = context;
        }

        

        // GET: Doctors/Details/5
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Acceso");

            int idUsuario = int.Parse(userId);


            var doctor = await _context.Doctors
                .Include(d => d.IdConsultorioNavigation)
                .Include(d => d.IdEmpleadoNavigation)
                .Include(d => d.IdEspecialidadNavigation)
                .FirstOrDefaultAsync(m => m.IdDoctor == idUsuario);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["IdConsultorio"] = new SelectList(_context.Consultorios, "IdConsultorio", "IdConsultorio");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado");
            ViewData["IdEspecialidad"] = new SelectList(_context.Especialidades, "IdEspecialidad", "IdEspecialidad");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDoctor,IdEmpleado,IdEspecialidad,IdConsultorio,CedulaProf")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdConsultorio"] = new SelectList(_context.Consultorios, "IdConsultorio", "IdConsultorio", doctor.IdConsultorio);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", doctor.IdEmpleado);
            ViewData["IdEspecialidad"] = new SelectList(_context.Especialidades, "IdEspecialidad", "IdEspecialidad", doctor.IdEspecialidad);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["IdConsultorio"] = new SelectList(_context.Consultorios, "IdConsultorio", "IdConsultorio", doctor.IdConsultorio);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", doctor.IdEmpleado);
            ViewData["IdEspecialidad"] = new SelectList(_context.Especialidades, "IdEspecialidad", "IdEspecialidad", doctor.IdEspecialidad);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDoctor,IdEmpleado,IdEspecialidad,IdConsultorio,CedulaProf")] Doctor doctor)
        {
            if (id != doctor.IdDoctor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.IdDoctor))
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
            ViewData["IdConsultorio"] = new SelectList(_context.Consultorios, "IdConsultorio", "IdConsultorio", doctor.IdConsultorio);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", doctor.IdEmpleado);
            ViewData["IdEspecialidad"] = new SelectList(_context.Especialidades, "IdEspecialidad", "IdEspecialidad", doctor.IdEspecialidad);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.IdConsultorioNavigation)
                .Include(d => d.IdEmpleadoNavigation)
                .Include(d => d.IdEspecialidadNavigation)
                .FirstOrDefaultAsync(m => m.IdDoctor == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.IdDoctor == id);
        }
    }
}
