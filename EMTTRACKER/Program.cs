using EMTTRACKER.Data;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
{
    config.AccessDeniedPath = "/Managed/ErrorAcceso";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMINONLY", policy => policy.RequireRole("ADMIN"));
});

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();

builder.Services.AddTransient<IRepositoryLogin, RepositoryLogin>();
builder.Services.AddTransient<IRepositoryEmt, RepositoryEmt>();
builder.Services.AddTransient<IRepositoryInterurbano, RepositoryInterurbano>();
builder.Services.AddTransient<IRepositoryCercanias, RepositoryCercanias>();
builder.Services.AddTransient<IRepositoryIncidencias, RepositoryIncidencias>();
builder.Services.AddTransient<IRepositoryGestionParadas, RepositoryGestionParadas>();
string connectionString = builder.Configuration.GetConnectionString("SqlEmt");
builder.Services.AddDbContext<EmtContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// MIDDLEWARE DE AUTENTICACIÓN
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(name: "default",
        template: "{controller=Menu}/{action=Index}/{id?}");
});

app.Run();
