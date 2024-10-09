using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.DTOs;
using MinimalApi.Entidades.Adm;
using MinimalApi.Entidades.Veiculo;
using minimalApiAdministradoresDTO;
using minimalApiAModelViewAdministradores;
using MinimalAPIContexto;
using minimalApiEnumsPerfil;
using minimalApiInterfaceAdm;
using MinimalApiInterfaceVeiculo;
using minimalApiModelViewAdmLogado;
using MinimalApiModelViewHome;
using minimalApiModelViewValidacao;
using MinimalApiServicosAdm;
using MinimalAPiServicosVeiculo;
using minimalApiVeiculoDTO;

#region  builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(options =>
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
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(options => {
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

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));

var app = builder.Build();
#endregion

#region  Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Email, administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken (
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials

    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores", ([FromBody] AdministradoresDTO administradoresDTO, IAdministradorServico administradorServico) => 
{
    var validacao = new Validacao {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradoresDTO.Email))
    {
        validacao.Mensagens.Add("O Email nao pode estar vazio");
    }
    if (string.IsNullOrEmpty(administradoresDTO.Senha))
    {
        validacao.Mensagens.Add("A Senha nao pode estar vazia");
    };
    if (string.IsNullOrEmpty(administradoresDTO.Perfil.ToString()))
    {
        validacao.Mensagens.Add("O perfil nao pode estar vazio");
    };

    var adm = new Administrador 
    {
        Email = administradoresDTO.Email,
        Senha = administradoresDTO.Senha,
        Perfil = administradoresDTO.Perfil.ToString() ?? Perfil.editor.ToString()
    };

    administradorServico.Incluir(adm);

    return Results.Created($"/administradores/{adm.Id}", adm);

}).RequireAuthorization().RequireAuthorization( new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Todos(pagina);
    var adms =  new List<AdministradorModelView> ();

    foreach (var administradores in adm)
    {
        adms.Add(new AdministradorModelView {
            Id = administradores.Id,
            Email = administradores.Email,
            Perfil = administradores.Perfil
        });
    }

    return Results.Ok(adms);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administradores");

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    var adm = administradorServico.Login(loginDTO);
    
    if  (adm != null) 
    {
        string token = GerarTokenJwt(adm);

        return Results.Ok(new AdministradorLogado {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();

}).WithTags("Administradores");
#endregion

#region Veiculos
Validacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new Validacao 
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome)){
        validacao.Mensagens.Add("O Nome nao pode ser vazio");
    }
    if (string.IsNullOrEmpty(veiculoDTO.Marca)){
        validacao.Mensagens.Add("A Marca nao pode ser vazio");
    }
    if (string.IsNullOrEmpty(veiculoDTO.Modelo)){
        validacao.Mensagens.Add("O Modelo nao pode ser vazio");
    }
    if (veiculoDTO.Ano < 1960){
        validacao.Mensagens.Add("O Modelo e muito antigo nao pode fazer parte da lista");
    }

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico ) => 
{
    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);


    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Modelo = veiculoDTO.Modelo,
        Ano = veiculoDTO.Ano
    };

    VeiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.RequireAuthorization(new AuthorizeAttribute {Roles = "editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico VeiculoServico) =>
{
    var veiculos = VeiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.RequireAuthorization(new AuthorizeAttribute {Roles = "editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico VeiculoServico) =>
{
    var veiculo = VeiculoServico.BuscarPorId(id);
    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.RequireAuthorization(new AuthorizeAttribute {Roles = "editor"})
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico)=>
{
    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = VeiculoServico.BuscarPorId(id);
    
    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Modelo = veiculoDTO.Modelo;
    veiculo.Ano = veiculoDTO.Ano;

    VeiculoServico.Atualizar(veiculo);


    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.RequireAuthorization(new AuthorizeAttribute {Roles = "editor"})
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Veiculos");

#endregion
#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

#endregion