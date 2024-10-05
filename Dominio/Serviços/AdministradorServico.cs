using MinimalApi.DTOs;
using MinimalApi.Entidades.Adm;
using MinimalAPIContexto;
using minimalApiInterfaceAdm;

namespace MinimalApiServicosAdm;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm =_contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
        
}
