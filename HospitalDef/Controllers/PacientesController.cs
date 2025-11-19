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
    public class PacientesController : Controller
    {
        private readonly HospitalContext _context;

        public PacientesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Pacientes.Include(p => p.IdUsuarioNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPaciente == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        public IActionResult Create()
        {
           

            ViewBag.PacienteId = new SelectList(_context.Usuarios,"IdUsuario","NombreUsuario");


            return View();
        }

        // POST: Pacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Paciente paciente)
            {
                // 1. Verificar si los datos cumplen las reglas (Strings no nulos, fechas válidas, etc.)
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(paciente);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Create", "HistorialMedicoPacientes");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "No se pudo guardar los cambios. " + ex.Message);
                    }
                }

                ViewBag.PacienteId = new SelectList(_context.Usuarios, "IdUsuario", "NombreUsuario", paciente.IdUsuario);

                // Regresamos a la vista para que el usuario corrija los errores
                return View(paciente);
            }


        

        // GET: Pacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", paciente.IdUsuario);
            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaciente,IdUsuario,Nombre,ApellidoP,ApellidoM,FechaNacimiento,Sexo,Edad,Calle,Colonia,Municipio,Estado")] Paciente paciente)
        {
            if (id != paciente.IdPaciente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(paciente.IdPaciente))
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
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", paciente.IdUsuario);
            return View(paciente);
        }

        // GET: Pacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdPaciente == id);
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.IdPaciente == id);
        }
    }
}
