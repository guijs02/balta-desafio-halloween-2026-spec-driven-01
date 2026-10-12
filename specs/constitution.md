# Constituição do projeto

## Stack
- .NET 10, C#, Minimal API
- Persistência em SQLite com EF Core
- Testes com xUnit

## Arquitetura
- Endpoint não acessa banco diretamente, sempre via serviço de aplicação
- Regra de negócio não vive no endpoint nem no handler HTTP
- Nenhuma dependência nova entra sem justificativa escrita no plano

## Qualidade
- Toda regra de negócio precisa ter teste unitário
- Todo endpoint precisa ter ao menos um teste de integração de caminho feliz
- Erro de validação sempre retorna 400 com ProblemDetails

## Convenções
- Utilizar nome claros de variaveis objetos seguindo Pascal case para objetos e camel case para variaveis locais
- Utilizar nomes de variveis, classes, arquivos em ingles e mensagens de erros em português.
- Commit no formato: tipo(escopo): descriçãop, com o identificador do requisito

## Governança
- Se o plano contradiz a constituição, quem muda é o plano
- Se a constituição precisa mudar, muda em commit separado