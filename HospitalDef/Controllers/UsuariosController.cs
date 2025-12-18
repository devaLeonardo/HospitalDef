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
    public class UsuariosController : Controller
    {
        private readonly HospitalContext _context;

        public UsuariosController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Correo,Contraseña,NombreUsuario,Telefono,FechaRegistro,Activo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }



        // GET: Usuarios/CreateUsuario
        [HttpGet]
        public IActionResult CreateUsuario()
        {
            ViewBag.Especialidades = _context.Especialidades.ToList();
            ViewBag.Consultorios = _context.Consultorios.ToList();
            ViewBag.Horarios = _context.HorarioEmpleados.ToList();
            return View();
        }


        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsuario(Usuario usuario)
        {
            string tipo = Request.Form["TipoUsuario"];

            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            if (tipo == "Paciente")
            {
                var paciente = new Paciente
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = Request.Form["Nombre"],
                    ApellidoP = Request.Form["ApellidoP"],
                    ApellidoM = Request.Form["ApellidoM"],
                    FechaNacimiento = DateOnly.Parse(Request.Form["FechaNacimiento"]),
                    Edad = CalcularEdad(DateOnly.Parse(Request.Form["FechaNacimiento"])),
                    Sexo = Request.Form["Sexo"],
                    Calle = Request.Form["Calle"],
                    Colonia = Request.Form["Colonia"],
                    Municipio = Request.Form["Municipio"],
                    Estado = Request.Form["Estado"]
                };

                _context.Pacientes.Add(paciente);
            }
            else
            {
                var empleado = new Empleado
                {
                    IdUsuario = usuario.IdUsuario,
                    IdHorario = int.Parse(Request.Form["IdHorario"]),
                    Nombre = Request.Form["Nombre"],
                    ApellidoP = Request.Form["ApellidoP"],
                    ApellidoM = Request.Form["ApellidoM"],
                    FechaNacimiento = DateOnly.Parse(Request.Form["FechaNacimiento"]),
                    Edad = CalcularEdad(DateOnly.Parse(Request.Form["FechaNacimiento"])),
                    Sexo = Request.Form["Sexo"],
                    Calle = Request.Form["Calle"],
                    Colonia = Request.Form["Colonia"],
                    Municipio = Request.Form["Municipio"],
                    Estado = Request.Form["Estado"],
                    Rfc = Request.Form["Rfc"],
                    CuentaBancaria = Request.Form["CuentaBancaria"],
                    Activo = true
                };

                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync();

                if (tipo == "Doctor")
                {
                    _context.Doctors.Add(new Doctor
                    {
                        IdEmpleado = empleado.IdEmpleado,
                        CedulaProf = Request.Form["CedulaProf"],
                        IdEspecialidad = int.Parse(Request.Form["IdEspecialidad"]),
                        IdConsultorio = int.Parse(Request.Form["IdConsultorio"])
                    });
                }

                if (tipo == "Recepcionista")
                    _context.Recepcionista.Add(new Recepcionistum
                    {
                        IdEmpleado = empleado.IdEmpleado
                    });

                if (tipo == "Farmaceutico")
                    _context.Farmaceutico.Add(new Farmaceutico
                    {
                        IdEmpleado = empleado.IdEmpleado
                    });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Recepcionistums");
        }

        private int CalcularEdad(DateOnly fecha)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            int edad = hoy.Year - fecha.Year;
            if (fecha > hoy.AddYears(-edad)) edad--;
            return edad;
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Correo,Contraseña,NombreUsuario,Telefono,FechaRegistro,Activo")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
