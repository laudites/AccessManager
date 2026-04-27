# Regras de Negocio

---

## Cliente

- pode ter varias telas
- possui dia de pagamento preferido opcional
- nome e obrigatorio
- dia de pagamento preferido, quando informado, deve estar entre 1 e 31
- cliente pode ser marcado como ativo/inativo

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
- limite de clientes nao pode ser negativo

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

- lancamento pertence a um cliente e a uma tela
- tela informada deve pertencer ao cliente informado
- competencia de referencia e obrigatoria
- valor deve ser maior que zero
- vencimento financeiro e obrigatorio
- status financeiro deve ser valido
- ao criar lancamento sem status, o status padrao e `Pendente`
- ao marcar como pago, status vira `Pago` e `DataPagamento` recebe a data atual em UTC
- pagamento nao altera `DataVencimentoTecnico` da tela

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
