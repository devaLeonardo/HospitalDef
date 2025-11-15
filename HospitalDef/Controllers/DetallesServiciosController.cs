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
    public class DetallesServiciosController : Controller
    {
        private readonly HospitalContext _context;

        public DetallesServiciosController(HospitalContext context)
        {
            _context = context;
        }

        // GET: DetallesServicios
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.DetallesServicios.Include(d => d.FolioTicketNavigation).Include(d => d.IdServicioNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: DetallesServicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesServicio = await _context.DetallesServicios
                .Include(d => d.FolioTicketNavigation)
                .Include(d => d.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdDs == id);
            if (detallesServicio == null)
            {
                return NotFound();
            }

            return View(detallesServicio);
        }

        // GET: DetallesServicios/Create
        public IActionResult Create()
        {
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket");
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "Idservicio", "Idservicio");
            return View();
        }

        // POST: DetallesServicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDs,FolioTicket,IdServicio,Precio,SubTotal")] DetallesServicio detallesServicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallesServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesServicio.FolioTicket);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "Idservicio", "Idservicio", detallesServicio.IdServicio);
            return View(detallesServicio);
        }

        // GET: DetallesServicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesServicio = await _context.DetallesServicios.FindAsync(id);
            if (detallesServicio == null)
            {
                return NotFound();
            }
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesServicio.FolioTicket);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "Idservicio", "Idservicio", detallesServicio.IdServicio);
            return View(detallesServicio);
        }

        // POST: DetallesServicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDs,FolioTicket,IdServicio,Precio,SubTotal")] DetallesServicio detallesServicio)
        {
            if (id != detallesServicio.IdDs)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesServicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesServicioExists(detallesServicio.IdDs))
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
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesServicio.FolioTicket);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "Idservicio", "Idservicio", detallesServicio.IdServicio);
            return View(detallesServicio);
        }

        // GET: DetallesServicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesServicio = await _context.DetallesServicios
                .Include(d => d.FolioTicketNavigation)
                .Include(d => d.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdDs == id);
            if (detallesServicio == null)
            {
                return NotFound();
            }

            return View(detallesServicio);
        }

        // POST: DetallesServicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallesServicio = await _context.DetallesServicios.FindAsync(id);
            if (detallesServicio != null)
            {
                _context.DetallesServicios.Remove(detallesServicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesServicioExists(int id)
        {
            return _context.DetallesServicios.Any(e => e.IdDs == id);
        }
    }
}
