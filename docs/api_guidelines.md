# API Guidelines

## Padrao

- REST
- endpoints no plural
- nomes dos endpoints seguem o padrao atual em portugues: `api/clientes`, `api/servidores`, `api/telas`, `api/lancamentos-financeiros`

---

## Exemplos atuais

GET /api/clientes
POST /api/clientes
PUT /api/clientes/{id}
GET /api/clientes/{id}

---

## Regras

- controllers nao devem conter logica
- validacoes ficam na Application
- responses devem ser consistentes
- usar DTOs

---

## Proximas melhorias planejadas

As proximas mudancas de API ainda nao estao implementadas.

- Endpoints de clientes devem retornar agregados de quantidade de telas e valor total das telas ativas quando fizer sentido para listagem/detalhe.
- Endpoints de servidores devem substituir limite de clientes por creditos disponiveis/comprados e custo por credito.
- Endpoints financeiros devem expor criacao/consulta por cliente, com valor agrupado das telas ativas.
- `CompetenciaReferencia` nao deve ser campo de entrada do usuario nos DTOs publicos.
- `DataVencimentoFinanceiro` deve ser campo de entrada para representar a data acordada de pagamento.
- A geracao automatica de lancamento pendente 5 dias antes do vencimento deve ter caso de uso proprio e endpoints administrativos apenas se necessario.
- Dashboard deve expor dados agregados para rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor, clientes/telas por servidor e pendencias financeiras por cliente.
