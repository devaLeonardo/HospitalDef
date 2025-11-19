using Microsoft.AspNetCore.Mvc;
using HospitalDef.Models;
using Microsoft.AspNetCore.Identity; // Para el hashseo de las contraseñas
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
            // Validación simple: verificar que el correo o usuario no existan
            if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))//esta linea evita inyecciones sql
            {
                ViewData["Mensaje"] = $"El correo {correo} ya está registrado.";
                return View();
            }
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario))
            {
                ViewData["Mensaje"] = $"El nombre de usuario {nombreUsuario} ya existe.";
                return View();
            }


            //valirdar que el formulario este vacio
            if (string.IsNullOrWhiteSpace(correo) ||
                 string.IsNullOrWhiteSpace(nombreUsuario) ||
                 string.IsNullOrWhiteSpace(telefono) ||
                 string.IsNullOrWhiteSpace(contraseña))
            {
                ViewData["Mensaje"] = "Todos los campos son obligatorios.";
                return View();
            }

            // Validar formato de correo
            if (!correo.Contains("@") || !correo.Contains("."))
            {
                ViewData["Mensaje"] = "El formato del correo no es válido.";
                return View();
            }
            // Validar que teléfono solo tenga números
            if (string.IsNullOrEmpty(telefono) || !telefono.All(char.IsDigit))
            {
                ViewData["Mensaje"] = "El teléfono solo puede contener números.";
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

            // AQUI SE HACE EL HASH DE LAS CONTRASEÑAS NUEVAS
            nuevoUsuario.Contraseña = _passwordHasher.HashPassword(nuevoUsuario, contraseña);

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Lo mandamos a que se registre como paciente
            return RedirectToAction("Login", "Acceso");
        }
    }
}
