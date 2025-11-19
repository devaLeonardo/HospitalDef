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

        // ACCIÓN PARA MOSTRAR LA VISTA DE LOGIN 
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Pacientes");
            }
            return View();
        }

        // ACCIÓN PARA PROCESAR EL FORMULARIO DE LOGIN
        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            var usuario = await _context.Usuarios//evita inyecciones sql
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);


            //validacion para no enviar formulario vacio
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                ViewData["Mensaje"] = "Todos los campos son obligatorios.";
                return View();
            }

            // Validación básica para evitar payload extraño (aunque EF lo parametriza),ya sea ; o ' evitando inyecciones
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

            // 1. Intentar login como texto plano
            if (usuario.Contraseña == contraseña)
            {
                passwordCorrecta = true;
                necesitaMigracion = true; // Se debe migrar la contraseña
            }
            else
            {
                //2. Intentar login usando HASH
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

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

            //5. Crear Claims, un claim es un objeto de tipo claim que sirve para describirlo, en este caso a nuestro usuario ya verificado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
            };

            if (paciente != null) { 
                claims.Add(new Claim(ClaimTypes.GivenName, paciente.Nombre));
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

                return RedirectToAction("Index", "Pacientes");

            }
            else
            {
                    var claims2 = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
                };
          
                    var identity = new ClaimsIdentity(claims2, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity)
                );
                    return RedirectToAction("Create", "Pacientes");
                
            }


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
