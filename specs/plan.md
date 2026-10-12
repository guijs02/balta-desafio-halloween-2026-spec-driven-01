# Plano técnico

## Contexto
A solução será uma API em .NET 10 com C# e Minimal API para atender as necessidades de geração e recuperação de senhas fortes, conforme especificado em `specs/spec.md` e as regras de arquitetura e qualidade da `specs/constitution.md`.

O sistema deve permitir:
- gerar uma nova senha forte;
- persistir a senha associada a um GUID;
- consultar a senha por GUID;
- responder corretamente para casos de entrada inválida e recursos inexistentes;
- manter o comportamento consistente com os requisitos de negócio definidos para fins didáticos, sem ampliar o escopo para produção.

## Arquitetura
A aplicação será organizada em camadas para manter a regra de negócio fora do endpoint e do acesso HTTP, em conformidade com a constituição:

- `src/Api`: camada de entrada HTTP com Minimal API e endpoints.
- `src/Application`: serviços de aplicação, DTOs, contratos, validações e orquestração entre API e infraestrutura.
- `src/Domain`: entidades, regras de negócio e serviços de domínio, como geração e validação de senhas.
- `src/Infrastructure`: acesso a dados via EF Core e SQLite, incluindo persistência de senhas e GUID.
- `tests/Unit`: testes unitários para regras de negócio e validação.
- `tests/Integration`: testes de integração para endpoints e cenários de sucesso/erro.

Estrutura conceitual:
- Os endpoints apenas recebem requisições HTTP e delegam a serviços de aplicação.
- A lógica de geração de senha não fica no endpoint.
- O serviço de aplicação chama uma entidade/serviço de domínio para validar a senha e gerar o identificador.
- O repositório de persistência é responsável apenas por salvar e consultar registros no SQLite.

## Decisões
1. Stack e padrão
   - Usar .NET 10, C#, Minimal API e SQLite com EF Core.
   - Não adicionar dependências extras sem necessidade explícita, seguindo a regra da constituição.
   - O projeto de testes de integração usa `Microsoft.AspNetCore.Mvc.Testing` como infraestrutura de teste para hospedar a API e substituir o banco por uma conexão SQLite em memória isolada; essa dependência não é usada em execução pela aplicação.
   - Manter mensagens de erro em português e nomes em inglês, conforme convenção da constituição.

2. Persistência
   - Armazenar cada senha em tabela simples com identificador único e valor da senha.
   - O GUID será o identificador externo e permanecerá associado a um único registro.
   - A persistência será obrigatória antes de confirmar sucesso para a geração.

3. Geração da senha
   - A regra de senha forte será centralizada em um serviço de domínio.
   - A senha será gerada com caracteres ASCII imprimíveis, sem espaços, com no mínimo 16 caracteres e pelo menos um caractere especial.
   - A geração não precisa validar categorias extras de caracteres, apenas os critérios da especificação.

4. Tratamento de erros
   - Validação inválida do GUID ou ausência do parâmetro retorna HTTP 400 com `ProblemDetails`.
   - GUID inexistente retorna HTTP 404 para uma resposta coerente.
   - Falha de persistência não deve retornar sucesso indevido.

5. Testes
   - Cada regra de negócio terá testes unitários.
   - Cada endpoint terá pelo menos um teste de integração de caminho feliz.
   - Casos de erro e validação também serão cobertos em testes de integração.

## Modelo de dados
Entidade principal:

```csharp
public class PasswordRecord
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

Observações:
- `Id` representa o identificador único em formato GUID.
- `Value` contém a senha gerada, respeitando as regras da especificação.
- `CreatedAt` pode ser usado para auditoria e rastreio, sem afetar requisitos funcionais.
- Não será implementado mecanismo de expiração, edição ou exclusão, conforme o escopo da especificação.

Mapeamento EF Core:
- `PasswordRecords` como tabela de persistência.
- `Id` como chave primária.
- `Value` como campo obrigatório e não nulo.

## Contratos
### POST /passwords
Request
```json
{}
```

Response de sucesso
```json
{
  "id": "d4d2a79d-1bd2-4d20-95b0-2d17ff9834d0",
  "password": "A!bcdefghijklmnop"
}
```

Critérios:
- A senha deve ter pelo menos 16 caracteres.
- Deve conter pelo menos um caractere especial.
- Não pode conter espaços em branco.
- O `id` deve ser um GUID válido.

### GET /passwords/{id}
Response de sucesso
```json
{
  "id": "d4d2a79d-1bd2-4d20-95b0-2d17ff9834d0",
  "password": "A!bcdefghijklmnop"
}
```

Resposta de erro 400
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O identificador informado é inválido."
}
```

Resposta de erro 404
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Não encontrado",
  "status": 404,
  "detail": "Nenhuma senha foi encontrada para o identificador informado."
}
```

## Fluxo principal
1. O cliente chama `POST /passwords`.
2. O endpoint delega para o serviço de aplicação.
3. O serviço cria a senha forte usando a regra de domínio.
4. O serviço valida a senha conforme os critérios da especificação.
5. O repositório persiste o registro com GUID e senha.
6. Em caso de falha de persistência, a operação é abortada sem retorno de sucesso.
7. O endpoint retorna o `id` e a senha gerada.

## Fluxo de consulta
1. O cliente chama `GET /passwords/{id}`.
2. O endpoint valida o GUID recebido.
3. O serviço consulta a senha no repositório.
4. Se existir, retorna a senha com o mesmo identificador.
5. Se não existir, retorna 404.

## Testes planejados
### Unitários
- validar senha com 16 caracteres e caractere especial;
- rejeitar senha sem caractere especial;
- rejeitar senha com espaços em branco;
- aceitar caracteres ASCII permitidos;
- rejeitar caracteres fora do conjunto permitido;
- gerar GUID único para cada registro;
- garantir que um serviço de negócio não substitui senhas anteriores.

### Integração
- geração de senha com sucesso;
- consulta por GUID existente;
- consulta por GUID inválido retorna 400;
- consulta por GUID inexistente retorna 404;
- falha de persistência impede confirmação de sucesso;
- múltiplas gerações produzem identificadores distintos e preservam acesso aos registros anteriores.

## Riscos
- Segurança: a senha é armazenada sem criptografia e isso é intencional para o desafio, mas deve ser claramente documentado como não adequado para produção.
- Validação: a API precisa tratar corretamente entrada inválida, sem aceitar GUID malformado ou vazio.
- Persistência: falhas no SQLite devem ser tratadas para evitar retorno de resposta falsa de sucesso.
- Consistência: cada geração deve produzir um novo identificador e preservar os registros anteriores, sem sobrescrita.
- Escopo: manter a implementação focada no desafio, sem introduzir autenticação, expiração, UI ou mecanismos de produção.

## Entregáveis esperados
- API em .NET 10 com Minimal API.
- Persistência em SQLite via EF Core.
- Testes unitários cobrindo regras de negócio.
- Testes de integração para endpoints e casos de erro.
- Código organizado em camadas conforme a constituição.

## Observações finais
O plano foi estruturado para respeitar a constituição e manter a implementação fiel à especificação: foco em geração e recuperação de senhas fortes, persistência obrigatória, tratamento correto de erros e arquitetura limpa, sem ampliar o escopo para funcionalidades fora do desafio.