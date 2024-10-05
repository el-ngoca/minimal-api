using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Entidades.Veiculo;

namespace MinimalApiInterfaceVeiculo
{
    public interface IVeiculoServico
    {
        List<Veiculo> Todos(int? pagina= 1, string? Nome = null, string? Marca = null);

        Veiculo? BuscarPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);



    }
}