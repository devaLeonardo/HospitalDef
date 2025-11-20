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
            // Obtener Id del usuario loggeado desde los Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.IdUsuario = userId;

            if (userId == null)
                return RedirectToAction("Login", "Acceso");

            // Convertir a int
            int idUsuario = int.Parse(userId);

            // Buscar la información del usuario (opcional si solo usas su Id)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);

            if (usuario == null)
                return BadRequest("No se encontró el usuario logueado.");

            // Enviar datos del usuario a la vista
            ViewBag.IdUsuario = idUsuario;
            ViewBag.NombreUsuario = usuario.NombreUsuario;

            return View();

        }

        // POST: Pacientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Paciente paciente)
            {
            // VALIDACIONES
            if (string.IsNullOrWhiteSpace(paciente.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(paciente.ApellidoP))
                ModelState.AddModelError("ApellidoP", "El apellido paterno es obligatorio.");

            if (paciente.IdUsuario <= 0)
                ModelState.AddModelError("IdUsuario", "No se asignó un usuario válido.");

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (paciente.FechaNacimiento == default || paciente.FechaNacimiento > hoy)
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no es válida.");

            paciente.Edad = CalcularEdad(paciente.FechaNacimiento);

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

            //  esto hace que no se borre el usuario por si no se cumple una validacion
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int idUsuario = int.Parse(userId);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);

            ViewBag.IdUsuario = idUsuario;
            ViewBag.NombreUsuario = usuario.NombreUsuario;
         

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
                return NotFound();

            // Validaciones
            if (string.IsNullOrWhiteSpace(paciente.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
          
            
            var hoy = DateOnly.FromDateTime(DateTime.Today); //convierte la fecha actual en dateonly (01/01/0001)

            if (paciente.FechaNacimiento == default || paciente.FechaNacimiento > hoy)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no es válida.");
            }

            // Recalcular edad
            paciente.Edad = CalcularEdad(paciente.FechaNacimiento);

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
           
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

        //metodo para calcular edad
        private int CalcularEdad(DateOnly fechaNacimiento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            int edad = hoy.Year - fechaNacimiento.Year;

            // Si aún no cumplió años este año, restar 1
            if (fechaNacimiento > hoy.AddYears(-edad))
                edad--;

            return edad;
        }
    }
}
