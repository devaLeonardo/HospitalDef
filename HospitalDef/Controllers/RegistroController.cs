using Microsoft.AspNetCore.Mvc;
using HospitalDef.Models;
using Microsoft.AspNetCore.Identity; // Para IPasswordHasher
using Microsoft.EntityFrameworkCore; // Para FirstOrDefaultAsync

namespace HospitalDef.Controllers
{
    public class RegistroController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public RegistroController(HospitalContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Muestra la vista de registro
        [HttpGet]
        public IActionResult Registrar()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Procesa el formulario de registro
        [HttpPost]
        public async Task<IActionResult> Registrar(string correo, string nombreUsuario, string telefono, string contraseña)
        {
            // (Validación simple: verificar que el correo o usuario no existan)
            if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))
            {
                ViewData["Mensaje"] = $"El correo {correo} ya está registrado.";
                return View();
            }
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario))
            {
                ViewData["Mensaje"] = $"El nombre de usuario {nombreUsuario} ya existe.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                Correo = correo,
                NombreUsuario = nombreUsuario,
                Telefono = telefono,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            // ¡AQUÍ SE CREA EL HASH!
            nuevoUsuario.Contraseña = _passwordHasher.HashPassword(nuevoUsuario, contraseña);

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Lo mandamos a que se registre como paciente
            return RedirectToAction("Create", "Pacientes");
        }
    }
}
