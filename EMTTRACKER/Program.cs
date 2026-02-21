using EMTTRACKER.Data;
using EMTTRACKER.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar el esquema de Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // A dónde redirigir si no está logeado
        options.AccessDeniedPath = "/Home/Index"; // A dónde ir si no tiene rol suficiente
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Duración de la sesión
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IRepositoryLogin, RepositoryLogin>();
builder.Services.AddTransient<IRepositoryEmt, RepositoryEmt>();
string connectionString = builder.Configuration.GetConnectionString("SqlEmt");
builder.Services.AddDbContext<EmtContext>
    (options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// MIDDLEWARE DE AUTENTICACIÓN
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Menu}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
