using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Riaya.API.Middleware;
using Riaya.Application.Auth.Interfaces;
using Riaya.Application.Features.Admin.Interfaces;
using Riaya.Application.Features.Bookings.Interfaces;
using Riaya.Application.Features.Doctors.Interfaces;
using Riaya.Application.Features.Patients.Interfaces;
using Riaya.Application.Features.Payments.Interfaces;
using Riaya.Application.Features.Schedule.Interfaces;
using Riaya.Application.Features.TimeSlots.Interfaces;
using Riaya.Application.Validators;
using Riaya.Infrastructure.Seeders;
using Riaya.Infrastructure.Services.Admin;
using Riaya.Infrastructure.Services.Auth;
using Riaya.Infrastructure.Services.Bookings;
using Riaya.Infrastructure.Services.Doctors;
using Riaya.Infrastructure.Services.Patients;
using Riaya.Infrastructure.Services.Payments;
using Riaya.Infrastructure.Services.Schedule;
using Riaya.Infrastructure.Services.TimeSlots;
using Riaya.Persistence.Context;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================== CORS ==================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ================== Services ==================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================== Exception Handling ==================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ================== DbContext ==================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        ).MigrationsAssembly("Riaya.Persistence")
    ));

// ================== JWT ==================
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    jwtKey = "Riaya_Project_Super_Secret_Key_2026!@#";

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Riaya",

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Riaya",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = "id"
    };
});

builder.Services.AddAuthorization();

// ================== FluentValidation ==================
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// ================== Dependency Injection ==================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ================== Build ==================
var app = builder.Build();

// ================== Middleware ==================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Riaya API V1");
    c.RoutePrefix = string.Empty;
});

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ================== Database Migration & Seeding ==================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        context.Database.Migrate();
        AdminSeeder.SeedAsync(context).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration/Seeding Error: " + ex.Message);
    }
}

app.Run();
