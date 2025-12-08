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
    public class EmpleadoesController : Controller
    {
        private readonly HospitalContext _context;

        public EmpleadoesController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Empleadoes
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Empleados.Include(e => e.IdHorarioNavigation).Include(e => e.IdUsuarioNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Empleadoes/Details/5
        public async Task<IActionResult> Details()
        {


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Acceso");

            int idUsuario = int.Parse(userId);


            var empleado = await _context.Empleados
                .Include(m => m.IdHorarioNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == idUsuario);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleadoes/Create
        public IActionResult Create()
        {
            ViewData["IdHorario"] = new SelectList(_context.HorarioEmpleados, "IdHorario", "IdHorario");
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Empleadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,IdUsuario,IdHorario,Rfc,CuentaBancaria,Nombre,ApellidoP,ApellidoM,FechaNacimiento,Sexo,Edad,Calle,Colonia,Municipio,Estado,Activo")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHorario"] = new SelectList(_context.HorarioEmpleados, "IdHorario", "IdHorario", empleado.IdHorario);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", empleado.IdUsuario);
            return View(empleado);
        }

        // GET: Empleadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            ViewData["IdHorario"] = new SelectList(_context.HorarioEmpleados, "IdHorario", "IdHorario", empleado.IdHorario);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", empleado.IdUsuario);
            return View(empleado);
        }

        // POST: Empleadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,IdUsuario,IdHorario,Rfc,CuentaBancaria,Nombre,ApellidoP,ApellidoM,FechaNacimiento,Sexo,Edad,Calle,Colonia,Municipio,Estado,Activo")] Empleado empleado)
        {
            if (id != empleado.IdEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.IdEmpleado))
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
            ViewData["IdHorario"] = new SelectList(_context.HorarioEmpleados, "IdHorario", "IdHorario", empleado.IdHorario);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", empleado.IdUsuario);
            return View(empleado);
        }

        // GET: Empleadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.IdHorarioNavigation)
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.IdEmpleado == id);
        }
        public IActionResult DatosPersonalesDoctor()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var empleado = _context.Empleados
                .Include(e => e.IdHorarioNavigation)
                .Include(e => e.Doctors)
                .FirstOrDefault(e => e.IdUsuario == usuarioId);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        public IActionResult DatosPersonalesRecep()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var empleado = _context.Empleados
                .Include(e => e.IdHorarioNavigation)
                .Include(e => e.Recepcionista)
                .FirstOrDefault(e => e.IdUsuario == usuarioId);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        public IActionResult DatosPersonalesFar()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var empleado = _context.Empleados
                .Include(e => e.IdHorarioNavigation)
                .Include(e => e.Recepcionista)
                .FirstOrDefault(e => e.IdUsuario == usuarioId);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }


    }
}
