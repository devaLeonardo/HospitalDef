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
    public class RecetumsController : Controller
    {
        private readonly HospitalContext _context;

        public RecetumsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Recetums
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Receta.Include(r => r.FolioCitaNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Recetums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta
                .Include(r => r.FolioCitaNavigation)
                .FirstOrDefaultAsync(m => m.FolioReceta == id);
            if (recetum == null)
            {
                return NotFound();
            }

            return View(recetum);
        }

        // GET: Recetums/Create
        public IActionResult Create()
        {
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas");
            return View();
        }

        // POST: Recetums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolioReceta,FolioCita,Observaciones,Tratamientos,Diagnostico")] Recetum recetum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recetum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", recetum.FolioCita);
            return View(recetum);
        }

        // GET: Recetums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta.FindAsync(id);
            if (recetum == null)
            {
                return NotFound();
            }
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", recetum.FolioCita);
            return View(recetum);
        }

        // POST: Recetums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FolioReceta,FolioCita,Observaciones,Tratamientos,Diagnostico")] Recetum recetum)
        {
            if (id != recetum.FolioReceta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recetum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecetumExists(recetum.FolioReceta))
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
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", recetum.FolioCita);
            return View(recetum);
        }

        // GET: Recetums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta
                .Include(r => r.FolioCitaNavigation)
                .FirstOrDefaultAsync(m => m.FolioReceta == id);
            if (recetum == null)
            {
                return NotFound();
            }

            return View(recetum);
        }

        // POST: Recetums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recetum = await _context.Receta.FindAsync(id);
            if (recetum != null)
            {
                _context.Receta.Remove(recetum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecetumExists(int id)
        {
            return _context.Receta.Any(e => e.FolioReceta == id);
        }
    }
}
