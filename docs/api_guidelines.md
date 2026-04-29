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
- Endpoints de clientes retornam o status financeiro do cliente no mes atual: `Atrasado`, `Pendente`, `Pago` ou `Sem lancamento`.
- Endpoints de servidores usam creditos disponiveis/comprados e custo por credito.
- Endpoints de telas retornam `Status` como status salvo/manual.
- Endpoints de telas retornam `StatusExibicao` como status calculado para apresentacao.
- `StatusExibicao` preserva `Cancelado` e `Suspenso`; nos demais casos usa `DataVencimentoTecnico` para retornar `Vencido`, `Vencendo` ou `Ativo`.
- Endpoints financeiros expoem criacao/consulta por cliente, com valor agrupado das telas ativas.
- `CompetenciaReferencia` nao e campo de entrada do usuario nos DTOs publicos.
- `DataVencimentoFinanceiro` deve ser campo de entrada para representar a data acordada de pagamento.
- A listagem de lancamentos financeiros aceita filtros opcionais por `mes` e `ano`, considerando `DataVencimentoFinanceiro`.
- A geracao de lancamento pendente possui endpoint manual em `POST /api/lancamentos-financeiros/gerar-pendentes` e tambem e executada automaticamente por BackgroundService da API.
- A geracao calcula o proximo vencimento real pelo `DiaPagamentoPreferido` do cliente e gera pendencia quando faltam ate 5 dias.
- A geracao usa o ultimo dia valido do mes quando o dia preferido nao existe no mes.
- A geracao evita duplicidade por `ClienteId` + `DataVencimentoFinanceiro`.
- `GET /api/clientes` e `GET /api/clientes/{id}` devem manter o status financeiro calculado no contrato de resposta, sem exigir campo de entrada do usuario.
- Dashboard expoe dados agregados para rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor, clientes/telas por servidor e pendencias financeiras por cliente.
