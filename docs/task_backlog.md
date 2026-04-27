# Backlog de Melhorias Futuras

A Fase 1 foi concluida. Este arquivo agora registra melhorias futuras, nao tarefas pendentes do MVP inicial.

---

## Prioridade alta

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
