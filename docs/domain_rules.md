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

- Existe endpoint/manual service para gerar lancamentos pendentes de clientes elegiveis 5 dias antes do vencimento acordado.
- Existe BackgroundService na API para executar a mesma geracao automaticamente todos os dias.
- A geracao evita duplicar lancamento para o mesmo cliente e vencimento.
- A geracao usa `DiaPagamentoPreferido` para identificar clientes cujo vencimento financeiro ocorre em 5 dias.
- Lancamentos gerados automaticamente iniciam como `Pendente`.

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

### Regra de calculo usada no dashboard

- Se `DataVencimentoTecnico` < hoje -> Vencido
- Se `DataVencimentoTecnico` >= hoje e <= hoje + 3 dias -> Vencendo
- Se `DataVencimentoTecnico` > hoje + 3 dias -> Ativo

Observacao: o dashboard classifica vencimento pela data tecnica, independentemente do status salvo na tela.

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
