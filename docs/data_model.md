# Modelo de Dados

## Cliente
- Id (Guid)
- Nome (string)
- Telefone (string)
- Observacao (string)
- DiaPagamentoPreferido (int?)
- DataCadastro (DateTime)
- Ativo (bool)

---

## Servidor
- Id (Guid)
- Nome (string)
- Descricao (string)
- Status (enum)
- LimiteClientes (int)
- UsuarioPainel (string)
- SenhaPainel (string)
- Observacao (string)
- Ativo (bool)

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
- Observacao (string)
- Ativo (bool)

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
- Observacao (string)
- DataCriacao (DateTime)

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
- Observacao (string)
- DataCriacao (DateTime)