using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Entidades.Adm;
using MinimalAPIContexto;
using MinimalApiServicosAdm;

namespace Test.Entidades.Servicos.AdministradoresServicosTest;

[TestClass]

public class AdministradoresServicosTest
{
    //Configuracao do Configuration Builder
     private DbContexto CriarContextoDeTeste ()
     {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));
        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
     }

    [TestMethod]
    public void TestandoSalvarAdministrador ()
    {
        //arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste11";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);
        
        //Act
        administradorServico.Incluir(adm);

        //Assert - Test Get para testar os dados que se espera ter
        Assert.AreEqual(1,administradorServico.Todos(1).Count());
    }
}
