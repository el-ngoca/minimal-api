using Microsoft.EntityFrameworkCore;
using MinimalApi.DTOs;
using MinimalApi.Entidades.Adm;
using minimalApiAdministradoresDTO;
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
    public Administrador? Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public List<Administrador> Todos(int? pagina = 1, string? Perfil = null)
    {
        var query = _contexto.Administradores.AsQueryable();

        if(string.IsNullOrEmpty(Perfil))
        {
            query.Where(V => EF.Functions.Like(V.Perfil.ToLower(), $"%{Perfil}%"));
        }

        int itensPorPagina = 10;

        if (pagina != null){
            query = query.Skip(((int)pagina -1) * itensPorPagina).Take(itensPorPagina);
        }
        
        return query.ToList();
    }
}
