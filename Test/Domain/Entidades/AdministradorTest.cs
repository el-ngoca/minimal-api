using MinimalApi.Entidades.Adm;

namespace Test.Domain.Entidades.AdministradorTest;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestarPropriedadeGetSet()
    {
        //arrange - Instanciamento da classe a ser testada

        var adm = new Administrador();

        //Act - Test para Set de propriedade
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste11";
        adm.Perfil = "Adm";


        //Assert - Test Get para testar os dados que se espera ter
        Assert.AreEqual(1,adm.Id);
        Assert.AreEqual("teste@teste.com",adm.Email);
        Assert.AreEqual("teste11",adm.Senha);
        Assert.AreEqual("Adm",adm.Perfil);
       


    }
}