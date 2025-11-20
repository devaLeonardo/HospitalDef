using HospitalDef.Models;
using Microsoft.AspNetCore.Authentication;
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
    public class HistorialMedicoPacientesController : Controller
    {
        private readonly HospitalContext _context;

        public HistorialMedicoPacientesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: HistorialMedicoPacientes
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.HistorialMedicoPacientes.Include(h => h.IdPacienteNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: HistorialMedicoPacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialMedicoPaciente = await _context.HistorialMedicoPacientes
                .Include(h => h.IdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.IdHistorialMedicoPaciente == id);
            if (historialMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(historialMedicoPaciente);
        }

        // GET: HistorialMedicoPacientes/Create
        public IActionResult Create()
        {
            // Obtener Id del usuario loggeado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Acceso");

            int idUsuario = int.Parse(userId);

            // Buscar el PACIENTE asociado al usuario
            var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == idUsuario);

            if (paciente == null)
                return BadRequest("No se encontró un paciente asociado al usuario logueado.");

            // Enviar IdPaciente a la vista
            ViewBag.IdPaciente = paciente.IdPaciente;
            ViewBag.NombrePaciente = paciente.Nombre + " " + paciente.ApellidoP;


            return View();
        }

        // POST: HistorialMedicoPacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHistorialMedicoPaciente,IdPaciente,Peso,Altura,Alergias,TipoSangre,Padecimientos")] HistorialMedicoPaciente historialMedicoPaciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historialMedicoPaciente);
                await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login","Acceso");
            }
            // Recargar PACIENTE para que no se borre al fallar validación
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int idUsuario = int.Parse(userId);

            var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == idUsuario);

            if (paciente != null)
            {
                historialMedicoPaciente.IdPaciente = paciente.IdPaciente; // <--- IMPORTANTE
                ViewBag.IdPaciente = paciente.IdPaciente;
                ViewBag.NombrePaciente = paciente.Nombre + " " + paciente.ApellidoP;
            }
            return View(historialMedicoPaciente);
        }

        // GET: HistorialMedicoPacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialMedicoPaciente = await _context.HistorialMedicoPacientes.FindAsync(id);
            if (historialMedicoPaciente == null)
            {
                return NotFound();
            }
            // Recargar paciente para que no se borre al fallar validación
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int idUsuario = int.Parse(userId);

            var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == idUsuario);

            if (paciente != null)
            {
                ViewBag.IdPaciente = paciente.IdPaciente;
                ViewBag.NombrePaciente = paciente.Nombre + " " + paciente.ApellidoP;
            }

            return View(historialMedicoPaciente);

        }

        // POST: HistorialMedicoPacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHistorialMedicoPaciente,IdPaciente,Peso,Altura,Alergias,TipoSangre,Padecimientos")] HistorialMedicoPaciente historialMedicoPaciente)
        {
            if (id != historialMedicoPaciente.IdHistorialMedicoPaciente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historialMedicoPaciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistorialMedicoPacienteExists(historialMedicoPaciente.IdHistorialMedicoPaciente))
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
            ViewData["IdPaciente"] = new SelectList(_context.Pacientes, "IdPaciente", "IdPaciente", historialMedicoPaciente.IdPaciente);
            return View(historialMedicoPaciente);
        }

        // GET: HistorialMedicoPacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialMedicoPaciente = await _context.HistorialMedicoPacientes
                .Include(h => h.IdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.IdHistorialMedicoPaciente == id);
            if (historialMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(historialMedicoPaciente);
        }

        // POST: HistorialMedicoPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historialMedicoPaciente = await _context.HistorialMedicoPacientes.FindAsync(id);
            if (historialMedicoPaciente != null)
            {
                _context.HistorialMedicoPacientes.Remove(historialMedicoPaciente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistorialMedicoPacienteExists(int id)
        {
            return _context.HistorialMedicoPacientes.Any(e => e.IdHistorialMedicoPaciente == id);
        }
    }
}
