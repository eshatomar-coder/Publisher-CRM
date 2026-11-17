using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Controllers;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// PHASE 1: THE BUILDER (All builder.Services go here)
// =================================================================
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1. ADD THIS: Point to PEMS's existing Data Protection keys
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\PEMS Files\DataProtectionKeys"))
    .SetApplicationName("PEMS"); // MUST exactly match PEMS

// 2. UPDATE THIS: Configure the shared cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.Name = ".Pegasus.SharedAuth"; // MUST match PEMS
        options.Cookie.Path = "/"; // CRITICAL: Allows PEMS and CRM to read the same cookie
    });

// Your custom Hierarchy Service MUST be here
builder.Services.AddScoped<IHierarchyService, HierarchyService>();

// =================================================================
// THE LOCK: DO NOT PUT ANY "builder.Services" BELOW THIS LINE
// =================================================================
var app = builder.Build();
// =================================================================

// =================================================================
// PHASE 2: THE APP (All app.Use... go here)
// =================================================================

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();