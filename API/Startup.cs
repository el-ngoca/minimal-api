using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIContexto;
using minimalApiInterfaceAdm;
using MinimalApiInterfaceVeiculo;
using MinimalApiServicosAdm;
using MinimalAPiServicosVeiculo;

namespace Minimal_api.API.Startup;

public class Startup
{
    private string key = "";

    public IConfiguration Configuration {get; set; } = default!;

    public Startup (IConfiguration configuration)
    {
        Configuration = configuration;

        key = Configuration?.GetSection("JWT")?.ToString() ?? "";
    }
    
    #region services
    public void ConfigurationServices(IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        services.AddSwaggerGen();
        services.AddEndpointsApiExplorer();
        services.AddAuthorization();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description ="Insira o seu Token JWT desta maneira: Bearer {Seu token}"
                
            });

            options.AddSecurityRequirement( new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string [] {}
                }
            });
        });

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();
        services.AddDbContext<DbContexto>(options => 
            options.UseMySql(Configuration.GetConnectionString("mysql"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))));
        }
    #endregion

    
}
