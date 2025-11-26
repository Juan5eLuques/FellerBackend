using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Amazon.S3;
using FellerBackend.Data;
using FellerBackend.Services;
using FellerBackend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURACIÓN DE BASE DE DATOS - PostgreSQL + EF Core
// ========================================
builder.Services.AddDbContext<FellerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// CONFIGURACIÓN DE JWT AUTHENTICATION
// ========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========================================
// CONFIGURACIÓN DE AWS S3
// ========================================
var awsOptions = builder.Configuration.GetSection("AWS");
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new Amazon.S3.AmazonS3Config
    {
        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsOptions["Region"]!)
    };
    
    return new AmazonS3Client(
        awsOptions["AccessKey"],
        awsOptions["SecretKey"],
        config
    );
});

// ========================================
// INYECCIÓN DE DEPENDENCIAS - SERVICIOS
// ========================================
// Servicios de infraestructura
builder.Services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();
builder.Services.AddScoped<IImagenService, ImagenService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Servicios de negocio
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// ========================================
// CONFIGURACIÓN DE CORS
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
 "http://localhost:3000",
       "http://localhost:5173", 
        "http://localhost:4200",
        "http://localhost:5000",
          "https://localhost:5000",
   "http://localhost:8080",
   "http://127.0.0.1:3000",
   "http://127.0.0.1:5173",
                "http://127.0.0.1:4200",
 "http://127.0.0.1:5000"
      )
   .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

    // Política más permisiva para desarrollo
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
          .AllowAnyHeader()
   .AllowAnyMethod();
    });
});

// ========================================
// CONFIGURACIÓN DE CONTROLLERS
// ========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ========================================
// CONFIGURACIÓN DE SWAGGER CON JWT Y SOPORTE PARA ARCHIVOS
// ========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Feller Automotores API",
        Version = "v1",
     Description = "API para gestión de venta de autos, motos y servicio de lavado",
        Contact = new OpenApiContact
        {
            Name = "Feller Automotores",
            Email = "contacto@fellerautomotores.com"
        }
    });

    // Configuración de JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
    Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
      In = ParameterLocation.Header,
 Description = "Ingrese su token JWT en el formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
        {
 new OpenApiSecurityScheme
     {
    Reference = new OpenApiReference
              {
          Type = ReferenceType.SecurityScheme,
    Id = "Bearer"
  }
 },
            Array.Empty<string>()
  }
    });

    // Configuración para manejar IFormFile en Swagger
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
 Format = "binary"
    });
});

var app = builder.Build();

// ========================================
// MIDDLEWARE PIPELINE
// ========================================

// Habilitar Swagger en todos los ambientes (comentar en producción si es necesario)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Feller Automotores API v1");
    options.RoutePrefix = string.Empty; // Swagger en la raíz: http://localhost:5000/
    options.DocumentTitle = "Feller Automotores API";
});

// Solo redirigir a HTTPS en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// IMPORTANTE: El orden es crítico
// Usar política más permisiva en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowFrontend");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========================================
// INICIALIZAR BASE DE DATOS
// ========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
 var context = services.GetRequiredService<FellerDbContext>();
        
        // Verificar si la BD existe y está accesible
        var canConnect = context.Database.CanConnect();
        if (canConnect)
        {
            Console.WriteLine("? Conexión a PostgreSQL exitosa");
          Console.WriteLine($"   Base de datos: {context.Database.GetDbConnection().Database}");
      }
        else
        {
            Console.WriteLine("? No se pudo conectar a PostgreSQL");
            Console.WriteLine("   Verifica que PostgreSQL esté corriendo y las credenciales sean correctas");
   }
    }
    catch (Exception ex)
    {
var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "? Error al inicializar la base de datos");
  Console.WriteLine($"? Error: {ex.Message}");
    }
}

Console.WriteLine("\n?? ???????????????????????????????????????????????????????");
Console.WriteLine("   FELLER AUTOMOTORES API - INICIADA CORRECTAMENTE");
Console.WriteLine("   ???????????????????????????????????????????????????????");
Console.WriteLine($"   ?? Swagger UI:    http://localhost:5000");
Console.WriteLine($"   ?? Swagger HTTPS: https://localhost:7000");
Console.WriteLine($"   ?? API Base URL:  http://localhost:5000/api");
Console.WriteLine($"   ?? Auth:     /api/auth/login");
Console.WriteLine("   ???????????????????????????????????????????????????????\n");

app.Run();
