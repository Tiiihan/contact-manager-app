using ContactManagerApp.Abstractions;
using ContactManagerApp.Data;
using ContactManagerApp.Middleware;
using ContactManagerApp.Services;
using ContactManagerApp.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ContactDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<ContactValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactValidationService, ContactValidationService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();