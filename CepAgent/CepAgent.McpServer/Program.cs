using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CepAgent.Infrastructure.Configuration;
using CepAgent.Application.UseCases;
using CepAgent.McpServer.Mcp;

namespace CepAgent.McpServer
{
    /// <summary>
    /// Entrypoint do servidor MCP. Configura o host, logging, DI (incluindo a infraestrutura)
    /// e registra o servidor MCP com transporte stdio e tools do assembly atual.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Logging conforme instrucoes da arquitetura tecnica
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => { });
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Registrar infraestrutura e casos de uso
            builder.Services.AddInfrastructure(); // Extensao do projeto CepAgent.Infrastructure

            // Registrar o caso de uso da camada Application
            builder.Services.AddScoped<ConsultarCepUseCase>();

            // Configurar o servidor MCP conforme instrucoes
            builder.WithStdioServerTransport()
                   .WithToolsFromAssembly(Assembly.GetExecutingAssembly());

            // Registrar hosted service que inicializa o MCP server
            builder.Services.AddHostedService<McpHostedService>();

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Iniciando CepAgent.McpServer...");

            await host.RunAsync();
        }
    }
}

namespace CepAgent.McpServer.Mcp
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Reflection;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Atributo usado para expor m?todos como tools MCP.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class McpToolAttribute : Attribute
    {
        public string? Name { get; }

        public McpToolAttribute()
        {
        }

        public McpToolAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Opcoes de configuracao do MCP server simplificado.
    /// </summary>
    public sealed class McpOptions
    {
        public bool UseStdio { get; set; }

        public Assembly? ToolsAssembly { get; set; }
    }

    /// <summary>
    /// Extensoes para configurar o builder com transporte e assembly de tools.
    /// </summary>
    public static class McpBuilderExtensions
    {
        public static HostApplicationBuilder WithStdioServerTransport(this HostApplicationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Configure<McpOptions>(opts => opts.UseStdio = true);
            return builder;
        }

        public static HostApplicationBuilder WithToolsFromAssembly(this HostApplicationBuilder builder, Assembly assembly)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            builder.Services.Configure<McpOptions>(opts => opts.ToolsAssembly = assembly);
            return builder;
        }
    }

    /// <summary>
    /// Hosted service responsavel por inicializar e publicar as tools MCP encontradas no assembly configurado.
    /// Implementacao simples que apenas registra e loga as tools para fins de laboratorio / desenvolvimento.
    /// </summary>
    public sealed class McpHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<McpHostedService> _logger;
        private readonly McpOptions _options;

        public McpHostedService(IServiceProvider serviceProvider, ILogger<McpHostedService> logger, IOptions<McpOptions> options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MCP Hosted Service iniciando...");

            if (_options.ToolsAssembly is null)
            {
                _logger.LogWarning("Nenhum assembly de tools foi configurado para o MCP. Nenhuma tool foi registrada.");
                return Task.CompletedTask;
            }

            // Descobrir metodos marcados com [McpTool]
            var toolMethods = DiscoverToolMethods(_options.ToolsAssembly);

            if (!toolMethods.Any())
            {
                _logger.LogWarning("Nenhum metodo marcado com [McpTool] encontrado no assembly {AssemblyName}.", _options.ToolsAssembly.FullName);
            }
            else
            {
                _logger.LogInformation("Foram encontradas {Count} tools MCP no assembly {AssemblyName}:", toolMethods.Count, _options.ToolsAssembly.FullName);
                foreach (var tm in toolMethods)
                {
                    _logger.LogInformation(" - {ToolName}: {DeclaringType}.{MethodName}", tm.Attribute?.Name ?? tm.Method.Name, tm.Method.DeclaringType?.FullName, tm.Method.Name);
                }
            }

            if (_options.UseStdio)
            {
                _logger.LogInformation("MCP transport configurado para stdio. (Simulado)");
            }

            // Em ambiente real aqui inicializar servidor MCP e ligar transportes.

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MCP Hosted Service finalizando...");
            return Task.CompletedTask;
        }

        private static List<(MethodInfo Method, McpToolAttribute? Attribute)> DiscoverToolMethods(Assembly assembly)
        {
            var result = new List<(MethodInfo, McpToolAttribute?)>();

            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    var attr = method.GetCustomAttribute<McpToolAttribute>();
                    if (attr is not null)
                    {
                        result.Add((method, attr));
                    }
                }
            }

            return result;
        }
    }
}
