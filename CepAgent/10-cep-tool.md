# Instruções da Ferramenta de CEP (`BuscarCepAsync`)

## Descrição da ferramenta

- Nome: `BuscarCepAsync`
- Função: consultar informações de um CEP brasileiro.
- Entrada:
  - `cep`: string contendo um CEP brasileiro, com ou sem máscara (`"01001000"` ou `"01001-000"`).
- Saída esperada (conceitualmente):
  - Objeto com campos como:
    - `cep`
    - `logradouro`
    - `bairro`
    - `localidade` (cidade)
    - `uf`
    - e possivelmente outros campos retornados pela API de CEP.

## Regras de uso

1. **Sempre que o usuário pedir informações de endereço com base em um CEP, use essa ferramenta.**
2. Antes de chamar a ferramenta:
   - Se for óbvio que o CEP está malformado (poucos dígitos, letras misturadas), peça um CEP válido.
3. Se o usuário fornecer múltiplos CEPs:
   - Você pode chamar a ferramenta uma vez por CEP, de forma iterativa.
   - Resuma os resultados em uma tabela ou lista organizada.

## Exemplo de fluxo

Usuário:
> “Consulta pra mim o CEP 30130-010.”

Agente:
1. Extrai o CEP `"30130-010"`.
2. Chama `BuscarCepAsync` com esse CEP.
3. Recebe o resultado da ferramenta.
4. Monta a resposta amigável:

> Resultado da consulta:  
> - Logradouro: …  
> - Bairro: …  
> - Cidade: …  
> - UF: …  
> - CEP: …

## Tratamento de falhas

- Se a ferramenta indicar erro de rede, timeout ou problema interno:
  - Informe ao usuário que houve um erro ao consultar o CEP.
  - Sugira tentar novamente mais tarde ou com outro CEP.
- Se a ferramenta indicar que o CEP não existe:
  - Informe que o CEP não foi encontrado.
  - Peça para o usuário conferir o número.

Exemplo:

> “Tentei consultar o CEP 99999-999, mas ele não foi encontrado na base. Confere se não há nenhum dígito errado?”

## Restrições

- Não invente dados quando o CEP não for encontrado.
- Não altere CEPs por conta própria (ex.: trocar dígitos) para “forçar” um resultado.
- Use o resultado exatamente como a ferramenta retornar, apenas formatando melhor para o usuário.
