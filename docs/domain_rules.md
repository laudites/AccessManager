# Regras de Negócio

---

## Cliente

- pode ter várias telas
- possui dia de pagamento preferido

---

## Tela

- pertence a um cliente
- pertence a um servidor
- é independente
- possui:
  - usuário
  - senha
  - valor acordado
  - vencimento técnico
  - marca da TV
  - app utilizado
  - MAC/ID opcional
  - chave secundária opcional

---

## Servidor

- possui credenciais próprias
- pode atender várias telas

---

## Renovação Técnica

- manual
- altera vencimento técnico
- pode trocar servidor
- gera histórico

---

## Financeiro

- separado do técnico
- manual
- pode ficar pendente mesmo com tela ativa
- possui vencimento próprio

---

## Status Tela

- Ativo
- Vencendo
- Vencido
- Suspenso
- Cancelado

### Regra de cálculo

- Se DataVencimentoTecnico < hoje → Vencido
- Se DataVencimentoTecnico <= hoje + 3 dias → Vencendo
- Caso contrário → Ativo

---

## Status Financeiro

- Pendente
- Pago
- Atrasado
- Cancelado

---

## Regras importantes

- renovação ≠ pagamento
- cliente pode dever mesmo com acesso ativo
- cada tela pode ter valor diferente