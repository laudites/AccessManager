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

Proximas melhorias planejadas:

- DTOs de listagem/detalhe de cliente devem incluir quantidade de telas.
- DTOs de listagem/detalhe de cliente devem incluir valor total agrupado das telas ativas.
- Esses campos podem ser calculados por consulta/projecao e nao precisam necessariamente ser persistidos na tabela `Cliente`.

---

## Servidor

- Id (Guid)
- Nome (string)
- Descricao (string?)
- Status (enum)
- LimiteClientes (int)
- UsuarioPainel (string)
- SenhaPainel (string)
- Observacao (string?)
- Ativo (bool)

Observacoes praticas:

- `LimiteClientes` nao pode ser negativo.
- O status do servidor e independente do status das telas.
- A troca de servidor de uma tela nao altera automaticamente dados financeiros.

Proximas melhorias planejadas:

- `LimiteClientes` deve ser removido do modelo de dominio e substituido em migration futura.
- Adicionar campo para quantidade de creditos disponiveis/comprados no servidor.
- Adicionar campo para valor de custo por credito.
- O custo mensal pode ser calculado como `creditos utilizados * custo por credito` ou por outra regra definida antes da implementacao.
- Avaliar se creditos utilizados serao persistidos ou calculados pela quantidade de telas/clientes associados ao servidor.

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
- TelaClienteId (Guid)
- CompetenciaReferencia (DateTime)
- Descricao (string)
- Valor (decimal)
- DataVencimentoFinanceiro (DateTime)
- DataPagamento (DateTime?)
- StatusFinanceiro (enum)
- Observacao (string?)
- DataCriacao (DateTime)

Observacoes praticas:

- Lancamento sempre referencia cliente e tela.
- A tela deve pertencer ao cliente informado.
- `Valor` deve ser maior que zero.
- `StatusFinanceiro` padrao na criacao e `Pendente` quando nao informado.
- Marcar como pago define `StatusFinanceiro = Pago` e preenche `DataPagamento`.
- Pagamento nao altera a tela, nao renova vencimento tecnico e nao cria historico tecnico.

Proximas melhorias planejadas:

- Lancamento financeiro deve passar a representar cobranca por cliente.
- Avaliar tornar `TelaClienteId` opcional, remover do fluxo principal ou substituir por relacionamento de detalhe caso seja necessario auditar quais telas compuseram o valor.
- `Valor` deve ser calculado pela soma dos valores acordados das telas ativas do cliente.
- `CompetenciaReferencia` nao deve ser preenchida manualmente pelo usuario.
- `CompetenciaReferencia` pode permanecer no banco como campo interno para relatorios mensais.
- Quando mantida, `CompetenciaReferencia` deve ser calculada automaticamente a partir de `DataVencimentoFinanceiro`.
- `DataVencimentoFinanceiro` deve representar a data acordada com o cliente.
- Criar mecanismo para gerar lancamento pendente automaticamente 5 dias antes do vencimento financeiro acordado.

---

## Dashboard e consultas agregadas

Proximas melhorias planejadas:

- Criar consultas agregadas para rendimento mensal.
- Criar consultas agregadas para custo mensal baseado em creditos de servidores.
- Criar consultas para clientes que pagaram no mes.
- Criar consultas para creditos por servidor.
- Criar consultas para clientes/telas por servidor.
- Criar consulta para clientes pendentes no financeiro.

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

Observacao:

- As novas regras planejadas exigem migration futura, mas nenhuma migration deve ser criada nesta etapa de documentacao.

Comando pratico para aplicar migrations:

```powershell
dotnet ef database update --project backend/src/AccessManager.Infrastructure --startup-project backend/src/AccessManager.Api
```
