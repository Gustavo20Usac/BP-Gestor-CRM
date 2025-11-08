using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Abstractions.Persistence;
using OeaMatriz.Application.Abstractions.Security;
using OeaMatriz.Application.Abstractions.Services;
using OeaMatriz.Infrastructure.Persistence;
using OeaMatriz.Infrastructure.Persistence.Repositories;
using OeaMatriz.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1) MVC (Razor) + HttpContext
// -----------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// (Opcional) Razor runtime compilation en dev
// builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// -----------------------------
// 2) EF Core - SQL Server
// -----------------------------
builder.Services.AddDbContext<OeaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------
// 3) Autenticación por cookies
// -----------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Denied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        // options.Cookie.Name = "OEA.Auth";
        // options.Cookie.SameSite = SameSiteMode.Lax;
    });

// -----------------------------
// 4) Autorización (políticas)
// -----------------------------
builder.Services.AddAuthorization(options =>
{
    // Usa claims "permiso" (agregados al autenticar)
    options.AddPolicy("CanManageClients", p => p.RequireClaim("permiso", "ManageClients"));
    options.AddPolicy("CanCreateEvaluations", p => p.RequireClaim("permiso", "CreateEvaluations"));
    // Agrega las que necesites...
});

// -----------------------------
// 5) DI: repos, servicios, hashing
// -----------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// (Opcional) AntiForgery para AJAX (Metronic/Fetch)
// builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

var app = builder.Build();

// -----------------------------
// 6) Pipeline
// -----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: primero autenticación, luego autorización
app.UseAuthentication();
app.UseAuthorization();

// (Opcional) middleware de semilla una sola vez
// using (var scope = app.Services.CreateScope())
// {
//     var ctx = scope.ServiceProvider.GetRequiredService<OeaDbContext>();
//     var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
//     var admin = await ctx.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == 1);
//     if (admin is not null && !admin.ClaveHash.Contains('.'))
//     {
//         admin.ClaveHash = hasher.Hash("Admin#2025!!");
//         ctx.Update(admin);
//         await ctx.SaveChangesAsync();
//     }
// }

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
