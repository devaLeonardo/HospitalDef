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
    public class RecepcionistumsController : Controller
    {
        private readonly HospitalContext _context;

        public RecepcionistumsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Recepcionistums
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return NotFound("Ocurrio un error inesperado por favor regresa");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var recepcionista = await _context.Recepcionista
                .Include(r => r.IdEmpleadoNavigation)
                .ThenInclude(e => e.IdHorarioNavigation)
                .Where(r => r.IdEmpleadoNavigation.IdUsuario == userId)
                .FirstOrDefaultAsync();

            return View(recepcionista);
        }

        // GET: Recepcionistums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionistum = await _context.Recepcionista
                .Include(r => r.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdRecepcionista == id);
            if (recepcionistum == null)
            {
                return NotFound();
            }

            return View(recepcionistum);
        }

        // GET: Recepcionistums/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado");
            return View();
        }

        // POST: Recepcionistums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRecepcionista,IdEmpleado")] Recepcionistum recepcionistum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recepcionistum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", recepcionistum.IdEmpleado);
            return View(recepcionistum);
        }

        // GET: Recepcionistums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionistum = await _context.Recepcionista.FindAsync(id);
            if (recepcionistum == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", recepcionistum.IdEmpleado);
            return View(recepcionistum);
        }

        // POST: Recepcionistums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRecepcionista,IdEmpleado")] Recepcionistum recepcionistum)
        {
            if (id != recepcionistum.IdRecepcionista)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recepcionistum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecepcionistumExists(recepcionistum.IdRecepcionista))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", recepcionistum.IdEmpleado);
            return View(recepcionistum);
        }

        // GET: Recepcionistums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionistum = await _context.Recepcionista
                .Include(r => r.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdRecepcionista == id);
            if (recepcionistum == null)
            {
                return NotFound();
            }

            return View(recepcionistum);
        }

        // POST: Recepcionistums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recepcionistum = await _context.Recepcionista.FindAsync(id);
            if (recepcionistum != null)
            {
                _context.Recepcionista.Remove(recepcionistum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecepcionistumExists(int id)
        {
            return _context.Recepcionista.Any(e => e.IdRecepcionista == id);
        }
    }
}
