using Microsoft.EntityFrameworkCore;
using HospitalDef.Models; // Para tu HospitalContext y Usuario
using Microsoft.AspNetCore.Authentication.Cookies; // Para la autenticación de Cookies
using Microsoft.AspNetCore.Identity; // Para el IPasswordHasher

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAR EL DBCONTEXT (Tu "Traductor") ---
var connectionString = builder.Configuration.GetConnectionString("Conexion");
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. CONFIGURAR LA AUTENTICACIÓN POR COOKIES ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Define la página de Login. Si un usuario no logueado intenta
        // entrar a una página [Authorize], será redirigido aquí.
        options.LoginPath = "/Acceso/Login";

        // (Opcional) Página si no tiene permisos (Roles)
        options.AccessDeniedPath = "/Home/AccessDenied";

        // (Opcional) Tiempo de expiración de la cookie
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// --- 3. REGISTRAR EL SERVICIO DE HASHING ---
// Esto nos permite inyectar IPasswordHasher para encriptar
// y verificar contraseñas de forma segura.
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// --- 4. SERVICIOS ESTÁNDAR ---
builder.Services.AddControllersWithViews();

// --- Fin de la configuración de servicios ---

var app = builder.Build();

// ... (Configuración de entorno, https, etc.)

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- 5. HABILITAR EL MIDDLEWARE DE AUTENTICACIÓN ---
// ¡El orden es CRUCIAL!
// Debe ir DESPUÉS de UseRouting() y ANTES de MapControllerRoute()

app.UseAuthentication(); // <-- Este identifica quién es el usuario (lee la cookie)
app.UseAuthorization();  // <-- Este verifica si tiene permiso (usa [Authorize])

// --- 6. MAPEO DE RUTAS ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();