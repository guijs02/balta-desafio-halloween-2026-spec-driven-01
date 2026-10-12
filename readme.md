<img width="100%" alt="Halloween 2026" src="https://baltaio.blob.core.windows.net/static/images/v4/challenges/halloween-2026/banner.jpg" />

## Halloween 2026 — Desafio 1

API didática para gerar senhas fortes e recuperá-las por GUID. A implementação segue a especificação em `specs/spec.md` e está organizada em projetos .NET 10 dentro de `src`.

### Executar a API

Na raiz do repositório:

```powershell
dotnet run --project src/Api/Api.csproj
```

A API cria o banco SQLite `passwords.db` no diretório do projeto da API. A string de conexão pode ser substituída pela configuração `ConnectionStrings:Passwords`.

### Endpoints

- `POST /passwords` — gera e persiste uma senha; retorna `id` e `password` após gravar com sucesso.
- `GET /passwords/{id}` — recupera a senha pelo GUID. GUID malformado ou ausente retorna HTTP 400 com ProblemDetails; GUID válido não encontrado retorna HTTP 404.

Senhas geradas têm pelo menos 16 caracteres, incluem pontuação ASCII imprimível e não contêm espaços em branco. As gerações recebem GUIDs próprios e registros anteriores permanecem consultáveis.

### Testes

```powershell
dotnet test src/Passwords.slnx
```

Os testes unitários cobrem as regras do domínio e do serviço. Os testes de integração hospedam a API com um banco SQLite em memória independente por teste.

### Limitações didáticas

As senhas são armazenadas sem criptografia, conforme o escopo da especificação. Esta implementação é exclusivamente didática e não é adequada para armazenar senhas reais ou uso em produção.
