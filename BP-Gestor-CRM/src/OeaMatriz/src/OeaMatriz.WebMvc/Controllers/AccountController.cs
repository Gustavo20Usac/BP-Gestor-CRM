using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OeaMatriz.WebMvc.Controllers;

/// <summary>
/// Provides user account related actions such as login and logout.
/// </summary>
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Displays the login page.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Processes the login form. On success, authenticates the user and redirects
    /// to the specified return URL or the home page.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var client = _httpClientFactory.CreateClient("Api");
        try
        {
            var response = await client.PostAsJsonAsync("api/auth/login", new { username = model.Username, password = model.Password });
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                ModelState.AddModelError(string.Empty, "Respuesta inválida del servidor.");
                return View(model);
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(loginResponse.Token);
            var claimsIdentity = new ClaimsIdentity(token.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error al iniciar sesión: {ex.Message}");
            return View(model);
        }
    }

    /// <summary>
    /// Logs the user out and redirects to the login page.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    /// <summary>
    /// View model used for login operations.
    /// </summary>
    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Username { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    private class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }
}