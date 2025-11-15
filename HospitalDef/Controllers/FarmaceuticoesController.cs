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
    public class FarmaceuticoesController : Controller
    {
        private readonly HospitalContext _context;

        public FarmaceuticoesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Farmaceuticoes
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Farmaceuticos.Include(f => f.IdEmpleadoNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Farmaceuticoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmaceutico = await _context.Farmaceuticos
                .Include(f => f.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdFarmaceutico == id);
            if (farmaceutico == null)
            {
                return NotFound();
            }

            return View(farmaceutico);
        }

        // GET: Farmaceuticoes/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado");
            return View();
        }

        // POST: Farmaceuticoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFarmaceutico,IdEmpleado")] Farmaceutico farmaceutico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(farmaceutico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", farmaceutico.IdEmpleado);
            return View(farmaceutico);
        }

        // GET: Farmaceuticoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmaceutico = await _context.Farmaceuticos.FindAsync(id);
            if (farmaceutico == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", farmaceutico.IdEmpleado);
            return View(farmaceutico);
        }

        // POST: Farmaceuticoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFarmaceutico,IdEmpleado")] Farmaceutico farmaceutico)
        {
            if (id != farmaceutico.IdFarmaceutico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmaceutico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmaceuticoExists(farmaceutico.IdFarmaceutico))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", farmaceutico.IdEmpleado);
            return View(farmaceutico);
        }

        // GET: Farmaceuticoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmaceutico = await _context.Farmaceuticos
                .Include(f => f.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdFarmaceutico == id);
            if (farmaceutico == null)
            {
                return NotFound();
            }

            return View(farmaceutico);
        }

        // POST: Farmaceuticoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmaceutico = await _context.Farmaceuticos.FindAsync(id);
            if (farmaceutico != null)
            {
                _context.Farmaceuticos.Remove(farmaceutico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FarmaceuticoExists(int id)
        {
            return _context.Farmaceuticos.Any(e => e.IdFarmaceutico == id);
        }
    }
}
