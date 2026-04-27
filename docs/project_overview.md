# Visao do Projeto

## Nome

AccessManager

## Objetivo

Sistema interno para controle de clientes e acessos, onde cada tela e uma unidade independente de operacao tecnica e financeira.

---

## Problemas resolvidos

- controle manual desorganizado
- dificuldade de identificar vencimentos tecnicos
- dificuldade de gerenciar multiplas telas por cliente
- falta de controle financeiro simples
- falta de historico para renovacoes e trocas de servidor

---

## Solucao

Sistema centralizado com:

- controle de clientes
- controle de servidores
- controle de telas
- controle tecnico manual
- controle financeiro separado
- historico de alteracoes tecnicas
- dashboard operacional e financeiro

---

## Status do MVP (Fase 1)

Concluido e funcional.

Backend implementado em .NET 8 com Clean Architecture, EF Core e MySQL. Frontend implementado em React + Vite consumindo a API por Axios.

---

## Funcionalidades implementadas

### Backend

- CRUD de clientes
- CRUD de servidores
- CRUD de telas
- Filtros de telas por cliente e servidor
- Renovacao tecnica manual de tela
- Troca manual de servidor
- Persistencia de historico para renovacao e troca de servidor
- CRUD de lancamentos financeiros
- Marcacao manual de pagamento
- Consulta de lancamentos pendentes
- Consulta de lancamentos atrasados
- Dashboard com resumo e telas por servidor
- Testes unitarios de regras centrais e smoke tests de arquitetura

### Frontend

- Layout principal com navegacao
- Pagina de login visual
- Dashboard
- Clientes
- Servidores
- Telas
- Financeiro
- Historico

---

## Proximas melhorias planejadas

As proximas melhorias abaixo ainda nao estao implementadas. Elas devem evoluir o MVP sem perder o historico das regras atuais.

### Clientes

- Mostrar quantidade de telas por cliente na listagem e no detalhe.
- Mostrar valor total agrupado das telas do cliente.
- Calcular valor agrupado pela soma dos valores acordados das telas ativas do cliente.

### Servidores

- Remover o limite de clientes como conceito de dominio.
- Passar a registrar creditos disponiveis/comprados por servidor.
- Registrar custo por credito do servidor.
- Calcular custo mensal com base nos creditos utilizados ou cadastrados.

### Financeiro

- Mudar o lancamento financeiro para uma visao por cliente.
- Gerar valor do lancamento pela soma das telas ativas do cliente.
- Manter pagamento manual e separado da renovacao tecnica.
- Gerar automaticamente lancamento pendente 5 dias antes do vencimento financeiro acordado.
- Calcular `CompetenciaReferencia` internamente a partir de `DataVencimentoFinanceiro`.

### Dashboard

- Exibir rendimento mensal.
- Exibir custo mensal.
- Exibir quantidade total de clientes.
- Exibir clientes que ja pagaram no mes.
- Exibir creditos por servidor.
- Exibir clientes/telas por servidor.
- Exibir clientes pendentes no financeiro.

---

## Endpoints principais

- `api/clientes`
- `api/servidores`
- `api/telas`
- `api/telas/{id}/renovar`
- `api/telas/{id}/trocar-servidor`
- `api/lancamentos-financeiros`
- `api/lancamentos-financeiros/{id}/marcar-pago`
- `api/lancamentos-financeiros/pendentes`
- `api/lancamentos-financeiros/atrasados`
- `api/dashboard/resumo`
- `api/dashboard/telas-por-servidor`

---

## Fora do MVP

- automacao com servidores
- integracao com APIs externas
- notificacoes
- cobranca automatica
- autenticacao real
- perfis de usuario
- relatorios avancados
- visualizacao completa e filtravel do historico tecnico
