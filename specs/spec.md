# Gerador de Senhas Fortes

## Problema
- Aplicações e usuários precisam gerar senhas fortes de maneira simples e padronizada, garantindo que atendam a critérios mínimos de composição. Além disso, é necessário permitir a recuperação de uma senha gerada anteriormente sem precisar gerá-la novamente.

## Objetivo
- Disponibilizar uma API que permita gerar senhas fortes seguindo critérios predefinidos e recuperar uma senha previamente gerada por meio de um identificador único.
- Cada senha gerada deve ser armazenada para permitir sua consulta posterior.

## Usuários
- Consumidores da API: pessoas ou aplicações que desejam gerar senhas fortes e recuperar senhas anteriormente geradas.

- Desenvolvedores: pessoas que utilizam o projeto para aprender e praticar o desenvolvimento orientado por especificações (SDD).

## Histórias
Como consumidor da API, quero gerar uma senha forte para obter uma senha que atenda aos critérios mínimos definidos.

Como consumidor da API, quero receber um identificador único após gerar uma senha para poder consultá-la posteriormente.

Como consumidor da API, quero consultar uma senha utilizando seu identificador único para recuperar uma senha gerada anteriormente.

Como consumidor da API, quero receber uma resposta adequada quando consultar um identificador inexistente para entender que nenhuma senha foi encontrada.

## Requisitos funcionais
RF01 — Gerar senha: a API deve disponibilizar um endpoint para gerar uma nova senha forte e retornar a senha gerada junto com seu identificador único.

RF02 — Armazenar senha: toda senha gerada deve ser armazenada para permitir sua recuperação posterior.

RF03 — Retornar identificador: após a geração e o armazenamento bem-sucedido de uma senha, a API deve retornar um identificador único no formato GUID associado à senha gerada.

RF04 — Consultar senha: a API deve disponibilizar um endpoint que receba um GUID e retorne a senha associada a ele, caso exista.

RF05 — Tratar identificador inexistente: caso o GUID informado não esteja associado a nenhuma senha armazenada, a API deve retornar HTTP 404 indicando que o recurso não foi encontrado.

RF06 — Informar falha no armazenamento: se não for possível armazenar a senha gerada, a API não deve informar que a operação foi concluída com sucesso nem retornar um identificador como se a senha estivesse disponível para consulta.

RF07 — Informar entrada inválida: se o identificador estiver em formato inválido ou não for informado, a API deve retornar HTTP 400 com detalhes do problema.

## Regras de negócio
RN01 — Comprimento mínimo: toda senha gerada deve conter, no mínimo, 16 caracteres.

RN02 — Caracteres especiais: toda senha gerada deve conter pelo menos um caractere especial.

RN03 — Proibição de espaços em branco: a senha gerada não pode conter espaços em branco, incluindo espaços entre caracteres ou nas extremidades.

RN04 — Identificador único: cada senha gerada deve possuir um GUID próprio, utilizado para identificá-la nas consultas posteriores.

RN05 — Persistência obrigatória: toda senha gerada com sucesso deve ser armazenada antes de a API confirmar a conclusão da operação.

RN06 — Associação entre senha e identificador: cada GUID deve estar associado à senha correspondente, permitindo sua recuperação posterior.

RN07 — Independência das gerações: cada solicitação de geração deve produzir uma nova senha e um novo identificador, sem substituir os registros anteriores.

RN08 — Escopo de segurança: para fins exclusivamente didáticos, as senhas serão armazenadas sem criptografia ou outros mecanismos adicionais de proteção. Essa simplificação não torna o projeto adequado para uso em produção.

RN09 — Definição de senha forte: para esta especificação, uma senha forte é aquela que atende aos critérios de comprimento mínimo, presença de caractere especial e ausência de espaços em branco. Não são exigidas categorias adicionais de caracteres.

RN10 — Conjunto de caracteres: a senha deve conter somente letras ASCII, dígitos ASCII e pontuação ASCII imprimível. Pontuação especial corresponde aos caracteres dos intervalos `!` a `/`, `:` a `@`, `[` a `` ` `` e `{` a `~`; espaço e caracteres de controle não são permitidos.

RN11 — Retenção: uma senha armazenada deve permanecer disponível para consulta sem prazo de expiração definido. Esta especificação não define uma operação de remoção.

RN12 — Unicidade do conteúdo: cada solicitação deve gerar uma nova senha, mas não há garantia de que o texto da senha seja diferente do texto de todas as senhas geradas anteriormente. Os identificadores devem ser próprios e distintos.

## Casos de borda
Consulta de um GUID válido sintaticamente, mas inexistente no armazenamento.

Consulta com um GUID em formato inválido, retornando HTTP 400 com detalhes do problema.

Consulta sem informar o identificador, retornando HTTP 400 com detalhes do problema.

Falha no armazenamento durante a geração de uma senha.

Geração de uma senha com exatamente 16 caracteres.

Tentativa de gerar uma senha que não contenha caracteres especiais.

Tentativa de gerar uma senha que contenha espaços em branco.

Solicitações consecutivas de geração, garantindo que cada senha tenha seu próprio identificador e que os registros anteriores permaneçam disponíveis.

Recuperação de uma senha após a conclusão da geração e do armazenamento.

## Fora de escopo
Criptografia, hash ou qualquer outro mecanismo de proteção das senhas armazenadas.

Autenticação, autorização e gerenciamento de usuários.

Políticas de expiração, validade ou troca periódica de senhas.

Recuperação de senhas por e-mail ou outros canais externos.

Personalização dos critérios de geração pelo consumidor da API.

Histórico de consultas, auditoria e rastreamento de operações.

Limitação de requisições e mecanismos de proteção contra abuso.

Interface gráfica ou aplicação web para interação com a API.

Uso em ambientes de produção ou armazenamento de senhas reais e sensíveis.

## Critérios de aceite
CA01: ao solicitar a geração de uma senha, a API deve retornar a senha e seu GUID; a senha deve conter pelo menos 16 caracteres, pelo menos um caractere especial e nenhum espaço em branco.

CA02: após gerar e armazenar uma senha com sucesso, a API deve retornar um GUID válido associado a ela.

CA03: ao consultar o GUID retornado na geração, a API deve recuperar exatamente a senha originalmente gerada.

CA04: a senha deve continuar disponível para consulta após a conclusão da solicitação de geração.

CA05: ao consultar um GUID inexistente, a API deve retornar uma resposta de recurso não encontrado.

CA06: ao consultar um GUID em formato inválido ou sem informar o identificador, a API deve retornar HTTP 400 com detalhes do problema.

CA07: ao solicitar uma geração sem que seja possível armazenar a senha, a API deve informar a falha sem retornar a senha ou o GUID e sem confirmar sucesso indevidamente.

CA08: ao realizar múltiplas solicitações de geração, cada operação bem-sucedida deve produzir uma nova senha e um GUID próprio, preservando a possibilidade de consultar as senhas anteriores.

CA09: nenhuma senha gerada pode conter espaços em branco, mesmo quando os caracteres forem combinados aleatoriamente.

CA10: o comportamento da API deve permanecer consistente com os requisitos e as regras de negócio definidos nesta especificação.

CA11: uma senha armazenada deve permanecer disponível para consulta sem prazo de expiração definido.

CA12: solicitações distintas devem receber GUIDs próprios, mas podem, excepcionalmente, receber senhas com o mesmo texto.