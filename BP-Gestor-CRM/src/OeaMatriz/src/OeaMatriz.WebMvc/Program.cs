using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Provide access to HttpContext for razor views (used in _Layout.cshtml)
builder.Services.AddHttpContextAccessor();

// Configure cookie-based authentication for the MVC front-end.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Redirect unauthenticated users to the login page.
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddAuthorization();

// Register HttpClient for API access. Base address is configured in appsettings.json (ApiBaseAddress).
builder.Services.AddHttpClient("Api", client =>
{
    var baseAddress = builder.Configuration["ApiBaseAddress"];
    if (string.IsNullOrEmpty(baseAddress))
    {
        throw new InvalidOperationException("ApiBaseAddress must be configured in appsettings.json for MVC to call the Web API.");
    }
    client.BaseAddress = new Uri(baseAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();