# Regras de Negocio

---

## Cliente

- pode ter varias telas
- possui dia de pagamento preferido opcional
- nome e obrigatorio
- dia de pagamento preferido, quando informado, deve estar entre 1 e 31
- cliente pode ser marcado como ativo/inativo
- A listagem e o detalhe de clientes devem exibir quantidade de telas do cliente.
- A listagem e o detalhe de clientes devem exibir valor total agrupado das telas do cliente.
- Valor total agrupado = soma de `ValorAcordado` das telas ativas do cliente.
- Telas inativas nao compoem o valor agrupado.
- A listagem e o detalhe de clientes devem exibir status financeiro do mes atual.
- Status financeiro do cliente considera lancamentos do cliente no mes/ano atual pela `DataVencimentoFinanceiro`.
- Prioridade do status financeiro do cliente: `Atrasado` > `Pendente` > `Pago` > `Sem lançamento`.
- `Atrasado` tambem cobre lancamento sem pagamento, nao pago/cancelado, com vencimento financeiro anterior a hoje.

---

## Tela

- pertence a um cliente
- pertence a um servidor
- e independente
- possui:
  - usuario
  - senha
  - valor acordado
  - vencimento tecnico
  - marca da TV
  - app utilizado
  - MAC/ID opcional
  - chave secundaria opcional

Regras implementadas:

- ClienteId e ServidorId sao obrigatorios
- cliente informado deve existir
- servidor informado deve existir
- nome de identificacao, usuario e senha sao obrigatorios
- valor acordado deve ser maior ou igual a zero
- data de vencimento tecnico e obrigatoria
- status deve ser valido
- textos opcionais vazios sao normalizados para `null`
- `Status` e o status salvo/manual da tela
- `StatusExibicao` e calculado pelo backend para apresentacao e nao sobrescreve `Status`
- `StatusExibicao` preserva `Cancelado` e `Suspenso`
- Para os demais status, `StatusExibicao` e `Vencido` quando `DataVencimentoTecnico` < hoje
- Para os demais status, `StatusExibicao` e `Vencendo` quando `DataVencimentoTecnico` esta entre hoje e hoje + 3 dias
- Para os demais status, `StatusExibicao` e `Ativo` quando nao estiver vencido nem vencendo

---

## Servidor

- possui credenciais proprias
- pode atender varias telas
- possui status proprio
- possui quantidade de creditos disponiveis/comprados
- possui valor de custo por credito
- quantidade de creditos nao pode ser negativa
- valor de custo por credito nao pode ser negativo
- `LimiteClientes` nao e mais regra funcional
- O custo mensal do servidor deve poder ser calculado com base nos creditos utilizados ou cadastrados.
- No dashboard atual, custo estimado por servidor usa a quantidade de telas ativas multiplicada pelo custo do credito.

---

## Renovacao Tecnica

- manual
- altera vencimento tecnico
- pode alterar valor acordado
- nao altera financeiro
- gera historico em `RenovacaoTelaHistorico`

Regras implementadas:

- nova data de vencimento tecnico e obrigatoria
- novo valor acordado e opcional
- se novo valor for informado, deve ser maior ou igual a zero
- historico guarda vencimento anterior, novo vencimento, valor anterior, novo valor e observacao
- no frontend, ao abrir a renovacao, a nova data sugerida e +30 dias sobre a maior data entre hoje e o vencimento tecnico atual
- o campo de calendario permanece editavel pelo usuario

---

## Troca de Servidor

- manual
- altera o servidor da tela
- nao altera vencimento tecnico
- nao altera valor acordado
- nao altera financeiro
- gera historico em `RenovacaoTelaHistorico`

Regras implementadas:

- ServidorNovoId e obrigatorio
- servidor novo deve existir
- historico guarda servidor anterior, servidor novo, vencimento tecnico atual, valor acordado atual e observacao

---

## Financeiro

- separado do tecnico
- manual
- pode ficar pendente mesmo com tela ativa
- possui vencimento proprio
- pagamento nao renova tela
- cliente pode dever mesmo com acesso ativo
- cada tela pode ter valor diferente

Regras implementadas:

- lancamento financeiro pertence a um cliente
- `TelaClienteId` e opcional
- valor do lancamento e calculado pela soma dos valores acordados das telas ativas do cliente
- `CompetenciaReferencia` e calculada automaticamente pelo backend com base em `DataVencimentoFinanceiro`
- listagem de lancamentos permite filtro opcional por mes e ano, considerando `DataVencimentoFinanceiro`
- vencimento financeiro e obrigatorio
- status financeiro deve ser valido
- ao criar lancamento sem status, o status padrao e `Pendente`
- ao marcar como pago, status vira `Pago` e `DataPagamento` recebe a data atual em UTC
- pagamento nao altera `DataVencimentoTecnico` da tela

### Geracao de pendentes

- Existe endpoint/manual service para gerar lancamentos pendentes de clientes elegiveis.
- Existe BackgroundService na API para executar a mesma geracao automaticamente todos os dias.
- A geracao usa `DiaPagamentoPreferido` para calcular o proximo vencimento real do cliente.
- Se o dia preferido ja passou no mes atual, o vencimento calculado usa o proximo mes.
- Se o mes nao possuir o dia preferido, o vencimento calculado usa o ultimo dia valido do mes.
- A janela de geracao e inclusiva entre hoje e hoje + 5 dias.
- Quando o proximo vencimento esta nessa janela, `DataVencimentoFinanceiro` recebe esse proximo dia de pagamento.
- O valor gerado e a soma de `ValorAcordado` das telas ativas do cliente.
- A geracao evita duplicar lancamento para o mesmo `ClienteId` + `DataVencimentoFinanceiro`.
- Lancamentos gerados automaticamente iniciam como `Pendente`.
- A rotina automatica nao marca pagamentos, nao renova telas e nao altera vencimentos tecnicos.

---

## Dashboard

### Estado atual

- Exibe resumo operacional e financeiro do MVP.
- Classifica telas por vencimento tecnico.
- Resume lancamentos pendentes, atrasados e total em aberto.
- Exibe rendimento mensal.
- Exibe custo mensal.
- Exibe quantidade de clientes.
- Exibe quantidade de clientes que ja pagaram no mes.
- Exibe quantidade de creditos de cada servidor.
- Exibe quantidade de clientes/telas em cada servidor.
- Exibe lista de clientes/pessoas pendentes no financeiro.

---

## Status Tela

- Ativo
- Vencendo
- Vencido
- Suspenso
- Cancelado

### Regra de calculo exibida

- `Status` salvo/manual permanece separado do status exibido.
- Se `Status` salvo for `Cancelado` ou `Suspenso` -> preservar esse status como exibicao.
- Se `DataVencimentoTecnico` < hoje -> Vencido
- Se `DataVencimentoTecnico` >= hoje e <= hoje + 3 dias -> Vencendo
- Se `DataVencimentoTecnico` > hoje + 3 dias -> Ativo

Observacao: a exibicao usa essa regra sem gravar o resultado no status manual da tela.

---

## Status Financeiro

- Pendente
- Pago
- Atrasado
- Cancelado

### Regra de atraso usada no dashboard

Um lancamento e considerado atrasado quando:

- status e `Atrasado`; ou
- nao possui pagamento, nao esta `Pago`, nao esta `Cancelado` e o vencimento financeiro e anterior a hoje.

---

## Regras importantes

- renovacao tecnica nao e pagamento
- pagamento nao e renovacao tecnica
- financeiro e tecnico devem evoluir separadamente
- alteracoes tecnicas relevantes devem manter historico
