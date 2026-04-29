# Backlog de Melhorias Futuras

A Fase 1 foi concluida. Este arquivo agora registra melhorias futuras, nao tarefas pendentes do MVP inicial.

---

## Prioridade alta

### Implementado: Clientes

- Quantidade de telas na listagem e no detalhe de clientes.
- Valor total agrupado das telas ativas do cliente.
- Calculo pela soma de `ValorAcordado` das telas ativas.
- Status financeiro do cliente no mes atual.
- Prioridade do status financeiro: Atrasado, Pendente, Pago e Sem lançamento.
- DTOs, services, repositories, endpoints, frontend e testes atualizados.

### Implementado: Servidores

- `LimiteClientes` deixou de ser regra funcional.
- Campos de creditos disponiveis/comprados e custo por credito implementados.
- Calculo de custo mensal estimado do servidor implementado no dashboard.
- Migration `ServerCreditsAndClientFinance` criada e aplicada no banco local.

### Implementado: Financeiro

- Lancamento financeiro por cliente.
- Valor agrupado automaticamente pelas telas ativas do cliente.
- `CompetenciaReferencia` calculada automaticamente pelo backend a partir de `DataVencimentoFinanceiro`.
- `DataVencimentoFinanceiro` usada como data acordada com o cliente.
- Endpoint/manual service para gerar lancamento pendente 5 dias antes do vencimento acordado.
- BackgroundService na API para executar a geracao real de pendencias automaticamente todos os dias.
- Filtro por mes/ano na listagem financeira, considerando `DataVencimentoFinanceiro`.
- Pagamento manual mantido.
- Pagamento nao renova tela.
- Financeiro continua separado do tecnico.

### Implementado: Dashboard financeiro

- Rendimento mensal.
- Custo mensal.
- Quantidade de clientes.
- Quantidade de clientes que ja pagaram no mes.
- Creditos por servidor.
- Quantidade de clientes/telas em cada servidor.
- Lista de clientes/pessoas pendentes no financeiro.

### Implementado: geracao automatica real

- BackgroundService real criado na API para gerar pendencias 5 dias antes do vencimento acordado.
- Endpoint/manual service mantido para execucao manual.
- Regra de duplicidade mantida por cliente e vencimento financeiro.
- Rotina automatica mantida sem integracao externa e sem alterar renovacao tecnica.

### Historico tecnico

- Criar endpoint para consultar `RenovacaoTelaHistorico`
- Permitir filtros por cliente, tela, servidor e periodo
- Conectar a pagina `Historico` do frontend a dados reais da API
- Exibir tipo da operacao de forma clara: renovacao ou troca de servidor

### Validacoes e feedback

- Padronizar mensagens de erro exibidas no frontend
- Melhorar validacoes de formulario antes do envio
- Garantir estados vazios claros para listas sem registros
- Revisar campos obrigatorios em telas de criacao e edicao

### Testes

- Aumentar cobertura dos services da Application
- Adicionar testes para repositories quando houver banco de teste ou estrategia definida
- Adicionar testes de fluxo para renovacao, troca de servidor e financeiro
- Adicionar verificacoes de contrato basicas para endpoints principais

---

## Prioridade media

### Usabilidade do frontend

- Melhorar filtros e busca em clientes, servidores, telas e financeiro
- Adicionar paginacao ou carregamento incremental nas listas
- Melhorar indicacoes visuais de vencido, vencendo, pendente e atrasado
- Revisar responsividade das telas principais
- Substituir pagina de login visual por autenticacao real quando entrar no escopo

### Dashboard

- Adicionar cards ou tabelas com proximos vencimentos tecnicos
- Adicionar resumo por servidor com capacidade e ocupacao
- Adicionar resumo financeiro por periodo
- Permitir filtro por periodo nas metricas financeiras

### Financeiro

- Permitir baixa parcial somente se a regra for definida
- Adicionar edicao controlada de lancamentos pagos
- Adicionar cancelamento com motivo
- Adicionar relatorio simples de valores em aberto por cliente

---

## Prioridade baixa

### Organizacao tecnica

- Remover ou revisar pastas antigas/geradas que nao fazem parte da solution atual, se confirmado que sao sobras
- Revisar documentacao de API para refletir endpoints reais em portugues (`api/clientes`, `api/servidores`, etc.)
- Avaliar versionamento de respostas da API se o sistema crescer
- Avaliar seed de dados para ambiente local

### Operacao

- Criar guia de backup e restore do banco MySQL
- Criar checklist de deploy
- Criar configuracao separada para ambiente de producao
- Documentar variaveis de ambiente suportadas

---

## Fora do escopo ate decisao futura

- integracoes externas
- automacoes com servidores
- notificacoes
- cobranca automatica
- renovacao automatica por pagamento
- aplicativo mobile
- multiempresa
