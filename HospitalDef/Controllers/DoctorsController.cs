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
                .FirstOrDefaultAsync(m => m.IdEmpleadoNavigation.IdUsuario == idUsuario);
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
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(d => d.IdDoctor == id);

            if (doctor == null)
                return NotFound();

            ViewBag.IdHorario = new SelectList(
                _context.HorarioEmpleados,
                "IdHorario",
                "Dias",
                doctor.IdEmpleadoNavigation.IdHorario
            );

            ViewBag.IdEspecialidad = new SelectList(
                _context.Especialidades,
                "IdEspecialidad",
                "Especialidades",
                doctor.IdEspecialidad
            );

            ViewBag.IdConsultorio = new SelectList(
                _context.Consultorios,
                "IdConsultorio",
                "Numero",
                doctor.IdConsultorio
            );

            return View(doctor);
        }



        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Doctor model)
        {
            var doctorDb = await _context.Doctors
                .Include(d => d.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(d => d.IdDoctor == id);

            if (doctorDb == null)
                return NotFound();

            // ================= EMPLEADO =================
            await TryUpdateModelAsync(
                doctorDb.IdEmpleadoNavigation,
                "IdEmpleadoNavigation",
                e => e.Nombre,
                e => e.ApellidoP,
                e => e.ApellidoM,
                e => e.Rfc,
                e => e.CuentaBancaria,
                e => e.Sexo,
                e => e.Calle,
                e => e.Colonia,
                e => e.Municipio,
                e => e.Estado,
                e => e.IdHorario
            );

            // ================= DOCTOR =================
            await TryUpdateModelAsync(
                doctorDb,
                "",
                d => d.IdEspecialidad,
                d => d.IdConsultorio,
                d => d.CedulaProf
            );

            await _context.SaveChangesAsync();

            TempData["DoctorEditado"] = true;

            // 
            return RedirectToAction("Index", "vistaDoctores");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(d => d.IdDoctor == id);

            if (doctor == null)
                return NotFound();

            // VERIFICAR CITAS PENDIENTES
            bool tieneCitasPendientes = await _context.Citas.AnyAsync(c =>
                c.idDoctor == id &&
                c.fechaCita >= DateTime.Today &&     // futuras o de hoy
                c.estatusAtencion == "PAGADO"               // ajusta según tu modelo
            );

            if (tieneCitasPendientes)
            {
                TempData["DoctorNoDesactivado"] = true;
                return RedirectToAction("Index", "vistaDoctores");
            }

            // ✅ DESACTIVAR
            doctor.IdEmpleadoNavigation.Activo = false;
            await _context.SaveChangesAsync();

            TempData["DoctorDesactivado"] = true;
            return RedirectToAction("Index", "vistaDoctores");
        }





    }


}
