using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StreamHubApi.Context;
using StreamHubApi.Interfaces;
using StreamHubApi.Services;

// 1. Cargar variables desde .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Configurar SQLite con Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=streamhub.db";
builder.Services.AddDbContext<StreamHubDbContext>(options =>
    options.UseSqlite(connectionString));

// 3. Configurar Autenticación JWT
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? throw new InvalidOperationException("Falta JWT_SECRET_KEY en el archivo .env");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "StreamHubApi";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "StreamHubClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<IContenidoService, ContenidoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ISuscripcionService, SuscripcionService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPagoService, PagoService>();

// 4. Configurar Swagger con botón Authorize
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StreamHub API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresá el token JWT así: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "StreamHub API v1");
    options.DocumentTitle = "StreamHub API — Documentación";
    options.InjectJavascript("/swagger-es.js");
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();