using System;
using CepAgent.Domain.Services;
using CepAgent.Infrastructure.Http.Cep;
using Microsoft.Extensions.DependencyInjection;

namespace CepAgent.Infrastructure.Configuration
{
    /// <summary>
    /// Extensões para registrar os serviços da camada Infrastructure no contêiner de DI.
    /// Segue o padrão descrito nas instructions: registra HttpClient específico para o serviço de CEP.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra a infraestrutura necessária (serviços HTTP, clientes, etc.).
        /// Deve ser chamado a partir do projeto composition root (ex.: McpServer/Program).
        /// </summary>
        /// <param name="services">Instância de <see cref="IServiceCollection"/>.</param>
        /// <returns>Mesma instância para encadeamento.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            // Registrar HttpClient específico para ViaCEP
            services.AddHttpClient<ICepLookupService, HttpCepLookupService>(client =>
            {
                client.BaseAddress = new Uri("https://viacep.com.br/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CepAgent.Infrastructure/1.0");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}