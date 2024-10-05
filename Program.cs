using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.DTOs;
using MinimalApi.Entidades.Veiculo;
using MinimalAPIContexto;
using minimalApiInterfaceAdm;
using MinimalApiInterfaceVeiculo;
using MinimalApiModelViewHome;
using MinimalApiServicosAdm;
using MinimalAPiServicosVeiculo;
using minimalApiVeiculoDTO;

#region  builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));

var app = builder.Build();
#endregion

#region  Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Administradores

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if  (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();

});
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico VeiculoServico ) => 
{
    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Modelo = veiculoDTO.Modelo,
        Ano = veiculoDTO.Ano
    };

    VeiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
});

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico VeiculoServico) =>
{
    var veiculos = VeiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
});

#endregion
#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

#endregion