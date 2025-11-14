using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CepAgent.Infrastructure.Configuration;
using CepAgent.Application.UseCases;
using CepAgent.Application.DTOs;

namespace CepAgent.Console
{
    /// <summary>
    /// Aplicacao console simples para testar o caso de uso de consulta de CEP sem MCP.
    /// Constrói um host, registra a infraestrutura e o caso de uso, e permite consultar CEPs via console.
    /// </summary>
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            builder.Services.AddInfrastructure();
            builder.Services.AddScoped<ConsultarCepUseCase>();

            var host = builder.Build();

            using var scope = host.Services.CreateScope();
            var provider = scope.ServiceProvider;
            var logger = provider.GetRequiredService<ILogger<Program>>();
            var useCase = provider.GetRequiredService<ConsultarCepUseCase>();

            logger.LogInformation("CepAgent.Console iniciado. Informe um CEP (8 digitos) ou 'exit'.");

            while (true)
            {
                System.Console.Write("CEP> ");
                var input = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                try
                {
                    var request = new ConsultaCepRequest(input.Trim());
                    var result = await useCase.ExecuteAsync(request).ConfigureAwait(false);

                    if (result.Encontrado)
                    {
                        System.Console.WriteLine($"Resultado da consulta para {result.CepFormatado}:");
                        System.Console.WriteLine($"- Logradouro: {result.Logradouro}");
                        System.Console.WriteLine($"- Bairro: {result.Bairro}");
                        System.Console.WriteLine($"- Cidade: {result.Localidade}");
                        System.Console.WriteLine($"- UF: {result.Uf}");
                    }
                    else
                    {
                        System.Console.WriteLine($"Consulta sem resultado: {result.Mensagem}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao executar consulta de CEP");
                    System.Console.WriteLine("Ocorreu um erro ao consultar o CEP. Veja logs para mais detalhes.");
                }
            }

            logger.LogInformation("Encerrando CepAgent.Console.");
            return 0;
        }
    }
}
