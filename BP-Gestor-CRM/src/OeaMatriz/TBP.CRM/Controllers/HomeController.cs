using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Abstractions.Services;
using System.Diagnostics;
using System.Security.Claims;
using TBP.CRM.Models;

namespace TBP.CRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthService _auth;

        public HomeController(ILogger<HomeController> logger, IAuthService auth)
        {
            _logger = logger;
            _auth = auth;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] OeaMatriz.Domain.Entities.DTOs.LoginRequest loginRequest)
        {
            var res = await _auth.LoginAsync(new(loginRequest.CorreoElectronico, loginRequest.Contrasena), default);
            if (!res.IsAuthenticated)
            {
                TempData["LoginError"] = res.Error ?? "Credenciales inválidas.";
                return View();
            }

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, res.UsuarioId!.Value.ToString()),
            new(ClaimTypes.Name, res.Nombre ?? loginRequest.CorreoElectronico),
            new(ClaimTypes.Role, res.PerfilId!.Value.ToString()), // o nombre del perfil si lo llevás
            new("pais_id", res.PaisId!.Value.ToString())
        };

            // Permisos como claims
            if (res.Permisos is not null)
                claims.AddRange(res.Permisos.Select(p => new Claim("permiso", p)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Denied() => View();

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
