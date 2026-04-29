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
- Status exibido/calculado das telas separado do status salvo/manual
- Clientes com quantidade de telas e valor agrupado das telas ativas
- Servidores com creditos disponiveis/comprados e custo por credito
- Renovacao tecnica manual de tela com sugestao de +30 dias no frontend
- Troca manual de servidor
- Persistencia de historico para renovacao e troca de servidor
- Lancamentos financeiros por cliente com valor agrupado das telas ativas
- Competencia financeira calculada automaticamente pelo backend
- Filtro de lancamentos financeiros por mes e ano de vencimento
- Status financeiro calculado por cliente no mes atual
- Marcacao manual de pagamento
- Consulta de lancamentos pendentes
- Consulta de lancamentos atrasados
- Endpoint/manual service e BackgroundService para gerar pendentes pelo proximo vencimento real do cliente
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
- Mostrar status financeiro do cliente no mes atual com prioridade Atrasado, Pendente, Pago e Sem lançamento.

### Servidores

- Remover o limite de clientes como conceito de dominio.
- Passar a registrar creditos disponiveis/comprados por servidor.
- Registrar custo por credito do servidor.
- Calcular custo mensal com base nos creditos utilizados ou cadastrados.

### Financeiro

- Mudar o lancamento financeiro para uma visao por cliente.
- Gerar valor do lancamento pela soma das telas ativas do cliente.
- Manter pagamento manual e separado da renovacao tecnica.
- Gerar automaticamente lancamento pendente quando faltam ate 5 dias para o proximo vencimento financeiro real.
- Calcular o proximo vencimento com base em `DiaPagamentoPreferido`.
- Usar o proximo mes quando o dia preferido ja passou no mes atual.
- Usar o ultimo dia valido em meses curtos.
- Calcular `CompetenciaReferencia` internamente a partir de `DataVencimentoFinanceiro`.
- Filtrar listagem por mes/ano usando `DataVencimentoFinanceiro`.
- Executar geracao automatica diaria de pendencias por BackgroundService da API, mantendo o endpoint manual.
- Evitar duplicidade de pendencias para o mesmo cliente e vencimento financeiro.

### Telas

- Manter `Status` como status salvo/manual.
- Exibir `StatusExibicao` calculado pela data de vencimento tecnico.
- Preservar `Cancelado` e `Suspenso` como status exibido quando forem o status manual.
- Classificar as demais telas como `Vencido`, `Vencendo` ou `Ativo` pela regra de vencimento tecnico.
- Sugerir renovacao com +30 dias a partir da maior data entre hoje e o vencimento tecnico atual.
- Manter o calendario de renovacao editavel.

### Dashboard

- Exibir rendimento mensal.
- Exibir custo mensal.
- Exibir quantidade total de clientes.
- Exibir clientes que ja pagaram no mes.
- Exibir creditos por servidor.
- Exibir clientes/telas por servidor.
- Exibir clientes pendentes no financeiro.
- Apoiar leitura de margem operacional pela comparacao entre rendimento mensal e custo mensal.

---

## Proximas melhorias planejadas

- Expor visualizacao completa e filtravel do historico tecnico.

---

## Endpoints principais

- `api/clientes`
- `api/servidores`
- `api/telas`
- `api/telas/{id}/renovar`
- `api/telas/{id}/trocar-servidor`
- `api/lancamentos-financeiros`
- `api/lancamentos-financeiros?mes={mes}&ano={ano}`
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
