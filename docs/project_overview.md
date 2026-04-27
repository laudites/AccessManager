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
- Clientes com quantidade de telas e valor agrupado das telas ativas
- Servidores com creditos disponiveis/comprados e custo por credito
- Renovacao tecnica manual de tela
- Troca manual de servidor
- Persistencia de historico para renovacao e troca de servidor
- Lancamentos financeiros por cliente com valor agrupado das telas ativas
- Competencia financeira calculada automaticamente pelo backend
- Marcacao manual de pagamento
- Consulta de lancamentos pendentes
- Consulta de lancamentos atrasados
- Endpoint/manual service para gerar pendentes 5 dias antes do vencimento acordado
- Dashboard com resumo financeiro, rendimento mensal, custo mensal, creditos e telas por servidor
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

## Estado atual das melhorias recentes

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

## Proximas melhorias planejadas

- Criar job/background service real para gerar pendencias automaticamente 5 dias antes do vencimento.
- Expor visualizacao completa e filtravel do historico tecnico.

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
- `api/lancamentos-financeiros/gerar-pendentes`
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
