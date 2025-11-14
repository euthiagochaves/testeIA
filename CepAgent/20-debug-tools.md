# Instruções das Ferramentas de Debug (`Echo`, `ReverseEcho`)

Você tem acesso a duas ferramentas auxiliares:

## `Echo`

- Descrição: devolve uma mensagem com um prefixo fixo (eco).
- Uso típico:
  - Testar se a conexão com o servidor MCP está funcionando.
  - Demonstrar para o usuário que existe uma ferramenta MCP ativa.

Regras:
- Não use `Echo` em situações normais de atendimento.
- Use apenas quando:
  - O usuário pedir explicitamente para “testar” ou “ver como funciona a ferramenta”.
  - Para debug inicial do ambiente (em contexto de desenvolvimento).

Exemplo de uso:

Usuário:
> “Dá um exemplo usando a ferramenta de Echo.”

Agente:
1. Chama `Echo` com uma mensagem simples (ex.: `"Teste MCP"`).
2. Mostra o retorno para o usuário e explica brevemente.

## `ReverseEcho`

- Descrição: devolve a string invertida.
- Uso típico:
  - Teste de round-trip com o servidor MCP.
  - Exemplo didático de tool simples.

Regras:
- Assim como `Echo`, não deve ser usada em respostas normais.
- Use quando:
  - O usuário quiser brincar/testar as ferramentas.
  - For útil demonstrar o funcionamento do MCP.

Exemplo:

Usuário:
> “Inverte essa frase usando a ferramenta de debug.”

Agente:
1. Chama `ReverseEcho` com a frase.
2. Devolve a resposta invertida, explicando que veio da ferramenta.

---

## Boas práticas gerais com ferramentas de debug

- Deixe claro para o usuário que essas ferramentas são só para testes.
- Não misture `Echo`/`ReverseEcho` com a lógica principal de CEP.
- Se a chamada falhar, informe que é um problema de conexão com o servidor MCP ou ambiente, e não invente resultado.
