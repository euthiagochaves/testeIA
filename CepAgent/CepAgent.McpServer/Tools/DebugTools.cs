using System;
using System.Threading.Tasks;
using CepAgent.McpServer.Mcp;

namespace CepAgent.McpServer.Tools
{
    /// <summary>
    /// Ferramentas de debug expostas via MCP: Echo e ReverseEcho.
    /// </summary>
    public sealed class DebugTools
    {
        public DebugTools()
        {
        }

        /// <summary>
        /// Retorna a mesma mensagem enviada, usada para testes de conectividade.
        /// </summary>
        /// <param name="message">Mensagem a ser ecoada.</param>
        /// <returns>Mesma mensagem com prefixo.</returns>
        [McpTool("Echo")]
        public Task<string> Echo(string message)
        {
            var prefix = "Echo: ";
            return Task.FromResult(prefix + (message ?? string.Empty));
        }

        /// <summary>
        /// Retorna a string invertida, usada para testes de round-trip.
        /// </summary>
        /// <param name="message">Mensagem a ser invertida.</param>
        /// <returns>Mensagem invertida.</returns>
        [McpTool("ReverseEcho")]
        public Task<string> ReverseEcho(string message)
        {
            if (message is null) return Task.FromResult(string.Empty);
            var chars = message.ToCharArray();
            Array.Reverse(chars);
            return Task.FromResult(new string(chars));
        }
    }
}
