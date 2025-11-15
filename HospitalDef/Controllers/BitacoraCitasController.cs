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
    public class BitacoraCitasController : Controller
    {
        private readonly HospitalContext _context;

        public BitacoraCitasController(HospitalContext context)
        {
            _context = context;
        }

        // GET: BitacoraCitas
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.BitacoraCitas.Include(b => b.FolioCitasNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: BitacoraCitas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraCita = await _context.BitacoraCitas
                .Include(b => b.FolioCitasNavigation)
                .FirstOrDefaultAsync(m => m.IdBitacoraCitas == id);
            if (bitacoraCita == null)
            {
                return NotFound();
            }

            return View(bitacoraCita);
        }

        // GET: BitacoraCitas/Create
        public IActionResult Create()
        {
            ViewData["FolioCitas"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas");
            return View();
        }

        // POST: BitacoraCitas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBitacoraCitas,FolioCitas,Estatus,Fecha,Costo,PoliticaCancelacion,MontoDevuelto")] BitacoraCita bitacoraCita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bitacoraCita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FolioCitas"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", bitacoraCita.FolioCitas);
            return View(bitacoraCita);
        }

        // GET: BitacoraCitas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraCita = await _context.BitacoraCitas.FindAsync(id);
            if (bitacoraCita == null)
            {
                return NotFound();
            }
            ViewData["FolioCitas"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", bitacoraCita.FolioCitas);
            return View(bitacoraCita);
        }

        // POST: BitacoraCitas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBitacoraCitas,FolioCitas,Estatus,Fecha,Costo,PoliticaCancelacion,MontoDevuelto")] BitacoraCita bitacoraCita)
        {
            if (id != bitacoraCita.IdBitacoraCitas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bitacoraCita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BitacoraCitaExists(bitacoraCita.IdBitacoraCitas))
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
            ViewData["FolioCitas"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", bitacoraCita.FolioCitas);
            return View(bitacoraCita);
        }

        // GET: BitacoraCitas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraCita = await _context.BitacoraCitas
                .Include(b => b.FolioCitasNavigation)
                .FirstOrDefaultAsync(m => m.IdBitacoraCitas == id);
            if (bitacoraCita == null)
            {
                return NotFound();
            }

            return View(bitacoraCita);
        }

        // POST: BitacoraCitas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bitacoraCita = await _context.BitacoraCitas.FindAsync(id);
            if (bitacoraCita != null)
            {
                _context.BitacoraCitas.Remove(bitacoraCita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BitacoraCitaExists(int id)
        {
            return _context.BitacoraCitas.Any(e => e.IdBitacoraCitas == id);
        }
    }
}
