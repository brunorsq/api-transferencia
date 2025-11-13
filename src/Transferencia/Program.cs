using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Transferencia.Auth;
using Transferencia.Data.context;
using Transferencia.Data.Repository;
using Transferencia.Services;
using Transferencia.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Data
var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var dbPath = Path.Combine(projectRoot, "database", "contacorrente.db");

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    .Replace("{DB_PATH}", dbPath);

builder.Services.AddSingleton<DataContext>(new DataContext(connectionString));

// Repositories
builder.Services.AddScoped<TransferenciaRepository>();

// Services
builder.Services.AddScoped<TransferenciaService>();

// Auth
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<TokenJWT>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<TokenJWT>();

var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Http

builder.Services.Configure<Api>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<TransferenciaService>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<Api>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});

// Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transferencia API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite '{seu token}' para autenticação"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
