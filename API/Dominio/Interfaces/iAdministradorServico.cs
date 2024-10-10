using MinimalApi.DTOs;
using MinimalApi.Entidades.Adm;

namespace minimalApiInterfaceAdm;

public interface IAdministradorServico //Contrato
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador? Incluir(Administrador administrador);

    List<Administrador> Todos( int?  pagina = 1, string? Perfil = null);
}
