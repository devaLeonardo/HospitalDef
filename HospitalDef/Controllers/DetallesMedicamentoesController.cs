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
    public class DetallesMedicamentoesController : Controller
    {
        private readonly HospitalContext _context;

        public DetallesMedicamentoesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: DetallesMedicamentoes
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.DetallesMedicamentos.Include(d => d.FolioTicketNavigation).Include(d => d.IdMedicamentoNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: DetallesMedicamentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesMedicamento = await _context.DetallesMedicamentos
                .Include(d => d.FolioTicketNavigation)
                .Include(d => d.IdMedicamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdDm == id);
            if (detallesMedicamento == null)
            {
                return NotFound();
            }

            return View(detallesMedicamento);
        }

        // GET: DetallesMedicamentoes/Create
        public IActionResult Create()
        {
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket");
            ViewData["IdMedicamento"] = new SelectList(_context.Medicamentos, "IdMedicamento", "IdMedicamento");
            return View();
        }

        // POST: DetallesMedicamentoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDm,FolioTicket,IdMedicamento,Precio,Cantidad,SubTotal")] DetallesMedicamento detallesMedicamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallesMedicamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesMedicamento.FolioTicket);
            ViewData["IdMedicamento"] = new SelectList(_context.Medicamentos, "IdMedicamento", "IdMedicamento", detallesMedicamento.IdMedicamento);
            return View(detallesMedicamento);
        }

        // GET: DetallesMedicamentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesMedicamento = await _context.DetallesMedicamentos.FindAsync(id);
            if (detallesMedicamento == null)
            {
                return NotFound();
            }
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesMedicamento.FolioTicket);
            ViewData["IdMedicamento"] = new SelectList(_context.Medicamentos, "IdMedicamento", "IdMedicamento", detallesMedicamento.IdMedicamento);
            return View(detallesMedicamento);
        }

        // POST: DetallesMedicamentoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDm,FolioTicket,IdMedicamento,Precio,Cantidad,SubTotal")] DetallesMedicamento detallesMedicamento)
        {
            if (id != detallesMedicamento.IdDm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesMedicamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesMedicamentoExists(detallesMedicamento.IdDm))
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
            ViewData["FolioTicket"] = new SelectList(_context.Tickets, "FolioTicket", "FolioTicket", detallesMedicamento.FolioTicket);
            ViewData["IdMedicamento"] = new SelectList(_context.Medicamentos, "IdMedicamento", "IdMedicamento", detallesMedicamento.IdMedicamento);
            return View(detallesMedicamento);
        }

        // GET: DetallesMedicamentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesMedicamento = await _context.DetallesMedicamentos
                .Include(d => d.FolioTicketNavigation)
                .Include(d => d.IdMedicamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdDm == id);
            if (detallesMedicamento == null)
            {
                return NotFound();
            }

            return View(detallesMedicamento);
        }

        // POST: DetallesMedicamentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallesMedicamento = await _context.DetallesMedicamentos.FindAsync(id);
            if (detallesMedicamento != null)
            {
                _context.DetallesMedicamentos.Remove(detallesMedicamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesMedicamentoExists(int id)
        {
            return _context.DetallesMedicamentos.Any(e => e.IdDm == id);
        }
    }
}
