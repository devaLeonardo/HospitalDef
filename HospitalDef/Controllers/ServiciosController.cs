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
    public class ServiciosController : Controller
    {
        private readonly HospitalContext _context;

        public ServiciosController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Servicios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Servicios.ToListAsync());
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(m => m.Idservicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servicio servicio)
        {
            servicio.Estatus = "disponible";

            if (!ModelState.IsValid)
                return View(servicio);

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Servicios/Edit/5
        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }


        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Servicio servicio)
        {
            if (!ModelState.IsValid)
                return View(servicio);

            var servicioDB = await _context.Servicios.FindAsync(servicio.Idservicio);
            if (servicioDB == null)
                return NotFound();

            servicioDB.NombreServicio = servicio.NombreServicio;
            servicioDB.Precio = servicio.Precio;
            servicioDB.HoraI = servicio.HoraI;
            servicioDB.HoraF = servicio.HoraF;
            servicioDB.Estatus = servicio.Estatus;

            await _context.SaveChangesAsync();

            
            TempData["ServicioEditado"] = true;

            return RedirectToAction(nameof(Index));
        }





        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(m => m.Idservicio == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.Idservicio == id);
        }
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            servicio.Estatus = "Inactivo"; // o 0 si es int
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return NotFound();

            servicio.Estatus = "disponible";
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
