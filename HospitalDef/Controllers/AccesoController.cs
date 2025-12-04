using Microsoft.AspNetCore.Mvc;
using HospitalDef.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace HospitalDef.Controllers
{
    public class AccesoController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AccesoController(HospitalContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Pacientes");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                ViewData["Mensaje"] = "Todos los campos son obligatorios.";
                return View();
            }

            if (nombreUsuario.Contains("'") || nombreUsuario.Contains(";") || nombreUsuario.Length > 50)
            {
                ViewData["Mensaje"] = "Nombre de usuario inválido.";
                return View();
            }

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }

            bool passwordCorrecta = false;
            bool necesitaMigracion = false;

            if (usuario.Contraseña == contraseña)
            {
                passwordCorrecta = true;
                necesitaMigracion = true;
            }
            else
            {
                var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);
                if (result == PasswordVerificationResult.Success)
                {
                    passwordCorrecta = true;
                }
            }

            if (!passwordCorrecta)
            {
                ViewData["Mensaje"] = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }

            if (necesitaMigracion)
            {
                usuario.Contraseña = _passwordHasher.HashPassword(usuario, contraseña);
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }

            if ((bool)!usuario.Activo)
            {
                ViewData["Mensaje"] = "Su cuenta está desactivada.";
                return View();
            }

            // Buscar paciente o empleado
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.IdUsuario == usuario.IdUsuario);

            Doctor? doctor = null;
            Farmaceutico? farmaceutico = null;
            Recepcionistum? recepcionista = null;

            if (empleado != null)
            {
                doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdEmpleado == empleado.IdEmpleado);
                farmaceutico = await _context.Farmaceuticos.FirstOrDefaultAsync(f => f.IdEmpleado == empleado.IdEmpleado);
                recepcionista = await _context.Recepcionista.FirstOrDefaultAsync(r => r.IdEmpleado == empleado.IdEmpleado);
            }

            // CREACIÓN DE CLAIMS GENERALES
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
            };

    
            //  ASIGNAR ROL SEGUN EL TIPO
            if (doctor != null)
                claims.Add(new Claim(ClaimTypes.Role, "Doctor"));
            else if (farmaceutico != null)
                claims.Add(new Claim(ClaimTypes.Role, "Farmaceutico"));
            else if (recepcionista != null)
                claims.Add(new Claim(ClaimTypes.Role, "Recepcionista"));
            else if (paciente != null)
                claims.Add(new Claim(ClaimTypes.Role, "Paciente"));
            else
                claims.Add(new Claim(ClaimTypes.Role, "SinRol"));

            // FIRMAR SESIÓN
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );


            //  REDIRECCIÓN SEGÚN EL ROL
            if (doctor != null)
                return RedirectToAction("Index", "Doctors");

            if (farmaceutico != null)
                return RedirectToAction("Index", "Farmaceuticoes");

            if (recepcionista != null)
                return RedirectToAction("Index", "Recepcionistums");

            if (paciente != null)
                return RedirectToAction("Index", "Pacientes");

            // SI NO TIENE PERFIL, CREAR PACIENTE
            return RedirectToAction("Create", "Pacientes");
        }

        // CERRAR SESIÓN
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Acceso");
        }
    }
}
