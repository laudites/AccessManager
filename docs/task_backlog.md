# Backlog de Melhorias Futuras

A Fase 1 foi concluida. Este arquivo agora registra melhorias futuras, nao tarefas pendentes do MVP inicial.

---

## Prioridade alta

### Regras planejadas para Clientes

- Incluir quantidade de telas na listagem e no detalhe de clientes.
- Incluir valor total agrupado das telas ativas do cliente.
- Calcular o valor agrupado pela soma de `ValorAcordado` das telas ativas.
- Atualizar DTOs, services, repositories, endpoints e tela de Clientes.
- Adicionar testes para cliente com uma tela, varias telas e nenhuma tela ativa.

### Regras planejadas para Servidores

- Remover `LimiteClientes` do dominio, DTOs, validacoes e UI.
- Criar campos de creditos disponiveis/comprados e custo por credito.
- Definir regra exata de creditos utilizados: por tela, por cliente, ou por cadastro manual.
- Implementar calculo de custo mensal do servidor.
- Criar migration para substituir o modelo atual de limite por creditos.

### Regras planejadas para Financeiro

- Alterar lancamento financeiro para ser criado por cliente.
- Agrupar automaticamente o valor das telas ativas do cliente no lancamento.
- Remover preenchimento manual de `CompetenciaReferencia` pelo usuario.
- Calcular `CompetenciaReferencia` automaticamente a partir de `DataVencimentoFinanceiro`.
- Usar `DataVencimentoFinanceiro` como data acordada com o cliente.
- Gerar lancamento financeiro pendente automaticamente 5 dias antes do vencimento acordado.
- Manter pagamento manual.
- Garantir que pagamento nao renove tela.
- Garantir que financeiro continue separado do tecnico.

### Regras planejadas para Dashboard

- Exibir rendimento mensal.
- Exibir custo mensal.
- Exibir quantidade de clientes.
- Exibir quantidade de clientes que ja pagaram no mes.
- Exibir creditos por servidor.
- Exibir quantidade de clientes/telas em cada servidor.
- Exibir lista de clientes/pessoas pendentes no financeiro.

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
