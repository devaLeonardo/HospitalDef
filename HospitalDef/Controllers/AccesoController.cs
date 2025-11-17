using Microsoft.AspNetCore.Mvc;
using HospitalDef.Models; // Para HospitalContext y Usuario
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Para las "credenciales" del usuario
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // Para IPasswordHasher

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

        // --- ACCIÓN PARA MOSTRAR LA VISTA DE LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Pacientes");
            }
            return View();
        }

        // --- ACCIÓN PARA PROCESAR EL FORMULARIO DE LOGIN ---
        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }

            bool passwordCorrecta = false;
            bool necesitaMigracion = false;

            // 🔹 1. Intentar login como texto plano
            if (usuario.Contraseña == contraseña)
            {
                passwordCorrecta = true;
                necesitaMigracion = true; // Se debe migrar el password
            }
            else
            {
                // 🔹 2. Intentar login usando HASH
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

            // 🔹 3. Si requiere migración (contraseña en texto plano), convertir a HASH
            if (necesitaMigracion)
            {
                usuario.Contraseña = _passwordHasher.HashPassword(usuario, contraseña);
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }

            // 🔹 4. Verificar si la cuenta está activa
            if ((bool)!usuario.Activo)
            {
                ViewData["Mensaje"] = "Su cuenta está desactivada.";
                return View();
            }

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

            // 🔹 5. Crear Claims
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
            };

            if (paciente != null)
                claims.Add(new Claim(ClaimTypes.GivenName, paciente.Nombre));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return RedirectToAction("Index", "Pacientes");
        }
        //cerrar sesion
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Acceso");
        }

    }
}
