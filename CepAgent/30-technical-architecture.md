# Instruções Técnicas e Arquitetura do Projeto

## Contexto

Este projeto é um **MCP server + console application** em **.NET 10** usando **C# 14**, com foco em:

- Consultar CEP via API externa (ex.: ViaCEP, BrasilAPI).
- Expor tools MCP (ex.: `BuscarCepAsync`, `Echo`, `ReverseEcho`).
- Servir de laboratório para integração de IA + MCP + DDD em C#.

Todas as gerações de código devem respeitar esta arquitetura.

---

## Stack técnica

- **Runtime**: .NET 10 (`<TargetFramework>net10.0</TargetFramework>`).
- **Linguagem**: C# 14 (`<LangVersion>latest</LangVersion>` ou equivalente).
- **Estilo**:
  - Usar recursos modernos (records, pattern matching, primary constructors, etc.) desde que compatíveis.
  - Preferir código limpo, explícito, fácil de ler.
- **DI e Hosting**:
  - `Microsoft.Extensions.Hosting` / `GenericHost`.
  - `Microsoft.Extensions.DependencyInjection` para DI.
  - `Microsoft.Extensions.Logging` para log.

---

## Organização em camadas (DDD)

Estrutura recomendada de projetos (pode ajustar nomes, mas manter a separação):

- **`CepAgent.Domain`**
  - Entidades, Value Objects, agregados e interfaces de repositório.
  - Regras de negócio puras, sem dependência de infraestrutura.
  - Exemplos:
    - `Cep` (Value Object)
    - `Endereco` (Entity/VO)
    - Interfaces: `ICepLookupService` (contrato de consulta de CEP na linguagem do domínio).

- **`CepAgent.Application`**
  - Casos de uso / Application Services.
  - Orquestra fluxo de domínio, mas sem detalhes de infraestrutura.
  - Exemplos:
    - `ConsultarCepUseCase` / `ConsultarCepHandler`.
    - Interfaces de portas de saída que o domínio usa (quando fizer sentido).

- **`CepAgent.Infrastructure`**
  - Implementações técnicas:
    - `HttpCepLookupService` (implementação de `ICepLookupService` usando HttpClient).
    - Configuração de `HttpClientFactory`.
    - Mapeamento de JSON da API externa para modelos de domínio.
  - Acesso a arquivos, rede, banco, etc., se adicionados no futuro.

- **`CepAgent.McpServer`**
  - Projeto console responsável por expor as **tools MCP**.
  - Usa o pacote MCP `.NET` e registra os tools via atributos ou builder.
  - As tools **não** contêm regra de negócio pesada: apenas chamam Application/Domain.
  - Exemplo:
    - Classe estática `CepTools` com métodos MCP chamando `ConsultarCepUseCase`.
    - Classe `DebugTools` com `Echo`, `ReverseEcho`.

- **`CepAgent.Console`**
  - (Opcional, se houver console separado do MCP server)
  - Aplicação console para interação direta com o usuário (sem MCP).
  - Pode chamar a API de IA ou fazer testes diretos com os casos de uso.

---

## Regras para o domínio (Domain Layer)

- Domínio deve ser **livre de dependências** de:
  - HTTP, MCP, bancos de dados, frameworks UI, etc.
- Usar **Value Objects** sempre que fizer sentido:
  - Ex.: `Cep` como VO, com validação interna de formato.
- Preferir imutabilidade (`record`, `init`-only setters) quando possível.
- Colocar validações de negócio no domínio:
  - Ex.: normalização e validação de CEP.
- Não retornar tipos técnicos (HttpResponseMessage, DTOs de API externa) no domínio.
  - Sempre mapear para tipos de domínio (`Endereco`, `Cep`, etc.).

---

## Regras para Application Layer

- Cada caso de uso deve ser expresso como um **serviço/coordenador**:
  - Ex.: `ConsultarCepUseCase` com método:
    - `Task<ResultadoConsultaCep> HandleAsync(Cep cep, CancellationToken ct = default);`
- Application chama:
  - Interfaces do domínio/infrastructure (`ICepLookupService`), **nunca** MCP diretamente.
- Fazer o mapeamento entre:
  - Dados técnicos (DTOs da infra / API) e modelos de domínio.
- Tratar erros esperados (CEP não encontrado, erro de validação de CEP) e propagá-los como tipos de resultado amigáveis (ex.: `Result<T>`, union types ou padrões equivalentes).

---

## Regras para Infrastructure Layer

- Toda integração externa (HTTP, banco, arquivos) fica aqui.
- Usar `IHttpClientFactory` via DI:
  - Nada de `new HttpClient()` jogado em qualquer lugar.
- Implementar `ICepLookupService` com:
  - Chamada à API de CEP (ViaCEP/BrasilAPI).
  - Tratamento de status HTTP.
  - Parse de JSON em DTOs internos.
  - Mapeamento para modelos de domínio.
- Os DTOs de API externa devem ficar na infra, não no domínio.

---

## Regras para o MCP Server

- O MCP server deve:
  - Ser um projeto console (`CepAgent.McpServer`) com `Host.CreateApplicationBuilder(args)`.
  - Registrar serviços de domínio, application e infra pelo DI.
  - Registrar tools MCP apontando para serviços/casos de uso da camada Application.

- Tools MCP:
  - Métodos simples, pequenos, sem lógica de negócio complexa.
  - Exemplo (conceitual):

    ```csharp
    [McpServerTool]
    public static async Task<EnderecoDto> BuscarCepAsync(
        string cep,
        [FromServices] ConsultarCepUseCase useCase,
        CancellationToken ct = default)
    {
        // 1. Validar/normalizar CEP (pode delegar para VO Cep).
        // 2. Chamar o use case da Application.
        // 3. Mapear o resultado para DTO amigável para a IA.
    }
    ```

  - Tools de debug (`Echo`, `ReverseEcho`):
    - Devem ser mantidas simples, sem dependência do domínio.
    - Usadas apenas para teste.

---

## Convenções de código

- Usar **async/await** em IO-bound (HTTP, disco, etc.).
- Nomes em português do brasil (PT-BR) para código (classes, métodos, namespaces, comentários, tudo em português), pode responder em português pro usuário.
- Seguir convenções padrão:
  - PascalCase para classes, métodos, propriedades.
  - camelCase para parâmetros e variáveis locais.
- Separar arquivos por tipo (uma classe principal por arquivo).
- Manter construtores enxutos e preferir injeção de dependência.

---

## Testes (opcional, mas recomendado)

- Criar projeto `CepAgent.Tests` com:
  - Testes de unidade para:
    - VO `Cep` (validação/normalização).
    - `ConsultarCepUseCase` (mockando `ICepLookupService`).
  - Testes não devem depender de HTTP real (usar mocks/dublês).

---

## Resumo de responsabilidade por camada

- **Domain**: regra de negócio, modelos de domínio, invariantes.
- **Application**: orquestra casos de uso, chama domínios/infra, traduz tipos.
- **Infrastructure**: adaptações técnicas (HTTP, APIs externas, DB).
- **McpServer**: expõe tools MCP chamando Application.
- **Console (se existir)**: interface linha de comando para testes/uso direto.

Qualquer código novo gerado pelo agente deve procurar respeitar essa divisão.
