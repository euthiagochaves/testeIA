using System.Threading;
using System.Threading.Tasks;
using CepAgent.Domain.Entities;
using CepAgent.Domain.ValueObjects;

namespace CepAgent.Domain.Services
{
    /// <summary>
    /// Contrato do serviço de consulta de CEP no domínio.
    /// Implementações devem ser fornecidas pela camada Infrastructure (ex.: chamada HTTP pra ViaCEP).
    /// </summary>
    public interface ICepLookupService
    {
        /// <summary>
        /// Consulta um CEP e retorna o endereço correspondente se encontrado.
        /// </summary>
        /// <param name="cep">CEP a ser consultado (Value Object do domínio).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Endereco quando encontrado; caso contrário, null.</returns>
        Task<Endereco?> BuscarPorCepAsync(Cep cep, CancellationToken cancellationToken = default);
    }
}
