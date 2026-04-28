# Modelo de Dados

O modelo atual esta implementado no backend com EF Core e MySQL. As entidades ficam em `backend/src/AccessManager.Domain/Entities` e as configuracoes de banco ficam em `backend/src/AccessManager.Infrastructure/Persistence/Configurations`.

---

## Cliente

- Id (Guid)
- Nome (string)
- Telefone (string)
- Observacao (string?)
- DiaPagamentoPreferido (int?)
- DataCadastro (DateTime)
- Ativo (bool)

Observacoes praticas:

- `Nome` e obrigatorio pela regra de aplicacao.
- `DiaPagamentoPreferido` e opcional, mas quando informado deve estar entre 1 e 31.
- Cliente pode existir sem lancamento financeiro.
- Cliente pode ter varias telas.

- DTOs de listagem/detalhe de cliente incluem quantidade de telas.
- DTOs de listagem/detalhe de cliente incluem valor total agrupado das telas ativas.
- DTOs de listagem/detalhe de cliente incluem status financeiro calculado para o mes atual.
- Esses campos podem ser calculados por consulta/projecao e nao precisam necessariamente ser persistidos na tabela `Cliente`.
- O status financeiro do cliente usa a prioridade: `Atrasado` > `Pendente` > `Pago` > `Sem lançamento`.

---

## Servidor

- Id (Guid)
- Nome (string)
- Descricao (string?)
- Status (enum)
- QuantidadeCreditos (int)
- ValorCustoCredito (decimal)
- UsuarioPainel (string)
- SenhaPainel (string)
- Observacao (string?)
- Ativo (bool)

Observacoes praticas:

- `QuantidadeCreditos` nao pode ser negativo.
- `ValorCustoCredito` nao pode ser negativo.
- `LimiteClientes` foi substituido por `QuantidadeCreditos` na migration `ServerCreditsAndClientFinance` e nao deve ser usado como regra funcional.
- O status do servidor e independente do status das telas.
- A troca de servidor de uma tela nao altera automaticamente dados financeiros.
- O custo mensal estimado pode ser calculado com base nos creditos utilizados/cadastrados.

---

## TelaCliente

- Id (Guid)
- ClienteId (Guid)
- NomeIdentificacao (string)
- ServidorId (Guid)
- UsuarioTela (string)
- SenhaTela (string)
- ValorAcordado (decimal)
- DataInicio (DateTime)
- DataVencimentoTecnico (DateTime)
- Status (enum)
- MarcaTv (string)
- AppUtilizado (string)
- MacOuIdApp (string?)
- ChaveSecundaria (string?)
- Observacao (string?)
- Ativo (bool)

Observacoes praticas:

- Tela e a principal unidade operacional do sistema.
- `ClienteId` e `ServidorId` devem apontar para registros existentes.
- `ValorAcordado` aceita zero, mas nao aceita valor negativo.
- `DataVencimentoTecnico` controla as classificacoes de vencida, vencendo e ativa no dashboard.
- `MacOuIdApp`, `ChaveSecundaria` e `Observacao` sao opcionais e strings vazias sao normalizadas para `null`.
- Financeiro da tela e registrado separadamente em `LancamentoFinanceiro`.

---

## RenovacaoTelaHistorico

- Id (Guid)
- TelaClienteId (Guid)
- ClienteId (Guid)
- ServidorAnteriorId (Guid?)
- ServidorNovoId (Guid?)
- DataVencimentoTecnicoAnterior (DateTime)
- NovaDataVencimentoTecnico (DateTime)
- ValorAcordadoAnterior (decimal)
- ValorAcordadoNovo (decimal)
- Observacao (string?)
- DataCriacao (DateTime)

Observacoes praticas:

- A mesma tabela registra renovacao tecnica e troca de servidor.
- Em renovacao, `ServidorAnteriorId` e `ServidorNovoId` ficam nulos no fluxo atual.
- Em troca de servidor, os vencimentos e valores sao repetidos para preservar o contexto da alteracao.
- Atualmente o historico e gravado nas operacoes tecnicas, mas a consulta dedicada do historico ainda pode evoluir.

---

## LancamentoFinanceiro

- Id (Guid)
- ClienteId (Guid)
- TelaClienteId (Guid?)
- CompetenciaReferencia (DateTime)
- Descricao (string)
- Valor (decimal)
- DataVencimentoFinanceiro (DateTime)
- DataPagamento (DateTime?)
- StatusFinanceiro (enum)
- Observacao (string?)
- DataCriacao (DateTime)

Observacoes praticas:

- Lancamento sempre referencia cliente.
- `TelaClienteId` e opcional para manter compatibilidade historica, mas o fluxo principal atual e por cliente.
- `Valor` e calculado pelo backend pela soma das telas ativas do cliente.
- `StatusFinanceiro` padrao na criacao e `Pendente` quando nao informado.
- `CompetenciaReferencia` e calculada automaticamente a partir de `DataVencimentoFinanceiro`.
- Marcar como pago define `StatusFinanceiro = Pago` e preenche `DataPagamento`.
- Pagamento nao altera a tela, nao renova vencimento tecnico e nao cria historico tecnico.
- `DataVencimentoFinanceiro` representa a data acordada com o cliente.
- A listagem financeira pode ser filtrada por mes e ano usando `DataVencimentoFinanceiro`.
- Existe mecanismo manual/endpoint e BackgroundService para gerar lancamento pendente 5 dias antes do vencimento financeiro acordado.
- A geracao automatica usa `DiaPagamentoPreferido` do cliente para calcular o vencimento financeiro alvo.

---

## Dashboard e consultas agregadas

- Existem consultas agregadas para rendimento mensal.
- Existem consultas agregadas para custo mensal baseado em creditos de servidores.
- Existem consultas para clientes que pagaram no mes.
- Existem consultas para creditos por servidor.
- Existem consultas para clientes/telas por servidor.
- Existe consulta para clientes pendentes no financeiro.

---

## Banco e migrations

- DbContext: `AccessManagerDbContext`
- DbSets:
  - `Clientes`
  - `Servidores`
  - `TelasClientes`
  - `RenovacoesTelasHistorico`
  - `LancamentosFinanceiros`
- Migration inicial existente: `20260424191340_InitialCreate`
- Migration de creditos/financeiro por cliente: `20260427211202_ServerCreditsAndClientFinance`

Observacao:

- Mudancas no modelo de dados exigem nova migration e execucao de `database update`.

Comando pratico para aplicar migrations:

```powershell
dotnet ef database update --project backend/src/AccessManager.Infrastructure/AccessManager.Infrastructure.csproj --startup-project backend/src/AccessManager.Api/AccessManager.Api.csproj --context AccessManagerDbContext
```
