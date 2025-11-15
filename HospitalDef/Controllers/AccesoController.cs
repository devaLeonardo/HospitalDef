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
            // 1. Buscamos al usuario por su nombreUsuario
            var usuario = await _context.Usuarios
                                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (usuario != null)
            {
                // 2. ¡VERIFICACIÓN SEGURA DE CONTRASEÑA!
                var verificationResult = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contraseña);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    // 3. Verificamos si el usuario está activo
                    if (usuario.Activo != true)
                    {
                        ViewData["Mensaje"] = "Su cuenta está desactivada.";
                        return View();
                    }

                    // 4. Creamos la "Identidad" (la credencial) del usuario
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                        new Claim(ClaimTypes.Email, usuario.Correo),
                        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
                        // (Opcional) Aquí podrías agregar roles
                    };

                    // 5. Creamos el principal de la identidad
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var props = new AuthenticationProperties
                    {
                        // ISPERSISTENT = FALSE (No la guardes en el disco duro del usuario)
                        IsPersistent = false
                        // No se necesita ExpiresUtc, ya que es una cookie de sesión
                    };

                    // 6. Inicia sesión en el sistema (CREA LA COOKIE)
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                            props);

                    // 7. Redirigimos al Home
                    return RedirectToAction("Index", "Pacientes");
                }
            }

            // Si el usuario no existe O la contraseña es incorrecta
            ViewData["Mensaje"] = "Nombre de usuario o contraseña incorrectos.";
            return View();
        }
    }
}
