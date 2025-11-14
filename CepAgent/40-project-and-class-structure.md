# Estrutura de Solução, Projetos e Classes

Estas instruções definem **como a solução .NET 10 / C# 14 deve ser organizada**, quais **projetos** devem existir e quais **classes/interfaces** cada projeto precisa ter para o cenário de:

- Consulta de CEP via API externa (ex.: ViaCEP/BrasilAPI).
- Exposição dessa funcionalidade via **MCP server** (tool `BuscarCepAsync`).
- Tools de debug (`Echo`, `ReverseEcho`).

A IA **deve seguir esta estrutura** sempre que for criar, alterar ou sugerir código.

---

## 1. Visão geral da solução

Nome sugerido da solução: `CepAgent.sln`

### Projetos obrigatórios

1. `CepAgent.Domain`
2. `CepAgent.Application`
3. `CepAgent.Infrastructure`
4. `CepAgent.McpServer`

### Projeto opcional (se quiser um console “normal” para testar sem MCP)

5. `CepAgent.Console`

---

## 2. Projeto `CepAgent.Domain`

**Objetivo:** conter **modelo de domínio** (Entidades, Value Objects, regras básicas) e contratos de serviços de domínio.

### 2.1. Configuração (conceitual)

- `TargetFramework`: `net10.0`
- Sem referência a infraestrutura (HTTP, banco, etc).
- Tudo em C# 14 com foco em imutabilidade quando possível.

### 2.2. Namespaces principais

- `CepAgent.Domain`
- `CepAgent.Domain.ValueObjects`
- `CepAgent.Domain.Entities`
- `CepAgent.Domain.Services` (para interfaces de domínio)
- `CepAgent.Domain.Errors` (opcional)

### 2.3. Classes e interfaces obrigatórias

#### 2.3.1. Value Object: `Cep`

**Arquivo:** `ValueObjects/Cep.cs`  
**Namespace:** `CepAgent.Domain.ValueObjects`

Responsabilidade:
- Representar um CEP brasileiro como VO.
- Normalizar e validar formato (8 dígitos, remover máscara, etc).

Assinatura conceitual:

```csharp
namespace CepAgent.Domain.ValueObjects;

public readonly record struct Cep
{
    public string Value { get; }

    public Cep(string value)
    {
        // Normalizar (remover traço/ponto/espaço)
        // Validar quantidade de dígitos
        // Se inválido, lançar exceção de domínio ou usar padrão de erro definido.
        Value = /* valor normalizado */;
    }

    public override string ToString() => Value;
}
