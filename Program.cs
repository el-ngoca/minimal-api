using Microsoft.EntityFrameworkCore;
using MinimalAPIContexto;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddDbContext<DbContexto>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));

app.MapGet("/", () => "Eduardo");

app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO) => {
    if  (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login com Sucesso");
    else
        return Results.Unauthorized();

});

app.Run();

