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

## Contratos atuais

- Endpoints de clientes retornam agregados de quantidade de telas e valor total das telas ativas.
- Endpoints de servidores usam creditos disponiveis/comprados e custo por credito.
- Endpoints financeiros expoem criacao/consulta por cliente, com valor agrupado das telas ativas.
- `CompetenciaReferencia` nao e campo de entrada do usuario nos DTOs publicos.
- `DataVencimentoFinanceiro` deve ser campo de entrada para representar a data acordada de pagamento.
- A geracao de lancamento pendente 5 dias antes do vencimento possui endpoint manual em `POST /api/lancamentos-financeiros/gerar-pendentes`.
- Dashboard expoe dados agregados para rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor, clientes/telas por servidor e pendencias financeiras por cliente.
