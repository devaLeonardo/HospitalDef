using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDef.Models;

namespace HospitalDef.Controllers
{
    public class HistorialCitasMedicoPacientesController : Controller
    {
        private readonly HospitalContext _context;

        public HistorialCitasMedicoPacientesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: HistorialCitasMedicoPacientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.HistorialCitasMedicoPacientes.ToListAsync());
        }

        // GET: HistorialCitasMedicoPacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialCitasMedicoPaciente = await _context.HistorialCitasMedicoPacientes
                .FirstOrDefaultAsync(m => m.IdHcmp == id);
            if (historialCitasMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(historialCitasMedicoPaciente);
        }

        // GET: HistorialCitasMedicoPacientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HistorialCitasMedicoPacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHcmp,Fecha,Hora")] HistorialCitasMedicoPaciente historialCitasMedicoPaciente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historialCitasMedicoPaciente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(historialCitasMedicoPaciente);
        }

        // GET: HistorialCitasMedicoPacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialCitasMedicoPaciente = await _context.HistorialCitasMedicoPacientes.FindAsync(id);
            if (historialCitasMedicoPaciente == null)
            {
                return NotFound();
            }
            return View(historialCitasMedicoPaciente);
        }

        // POST: HistorialCitasMedicoPacientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHcmp,Fecha,Hora")] HistorialCitasMedicoPaciente historialCitasMedicoPaciente)
        {
            if (id != historialCitasMedicoPaciente.IdHcmp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historialCitasMedicoPaciente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistorialCitasMedicoPacienteExists(historialCitasMedicoPaciente.IdHcmp))
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
            return View(historialCitasMedicoPaciente);
        }

        // GET: HistorialCitasMedicoPacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historialCitasMedicoPaciente = await _context.HistorialCitasMedicoPacientes
                .FirstOrDefaultAsync(m => m.IdHcmp == id);
            if (historialCitasMedicoPaciente == null)
            {
                return NotFound();
            }

            return View(historialCitasMedicoPaciente);
        }

        // POST: HistorialCitasMedicoPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historialCitasMedicoPaciente = await _context.HistorialCitasMedicoPacientes.FindAsync(id);
            if (historialCitasMedicoPaciente != null)
            {
                _context.HistorialCitasMedicoPacientes.Remove(historialCitasMedicoPaciente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistorialCitasMedicoPacienteExists(int id)
        {
            return _context.HistorialCitasMedicoPacientes.Any(e => e.IdHcmp == id);
        }
    }
}
