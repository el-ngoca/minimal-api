using MinimalApi.DTOs;
using MinimalApi.Entidades.Adm;

namespace minimalApiInterfaceAdm;

public interface IAdministradorServico //Contrato
{
    Administrador? Login(LoginDTO loginDTO);
}
