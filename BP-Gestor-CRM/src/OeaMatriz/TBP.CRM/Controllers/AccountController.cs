
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Abstractions.Services;
using System.Security.Claims;

namespace OeaMatriz.WebMvc.Controllers;

/// <summary>
/// Provides user account related actions such as login and logout.
/// </summary>
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthService _auth;
    public AccountController(IAuthService auth) => _auth = auth;


    /// <summary>
    /// Displays the login page.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

 
}