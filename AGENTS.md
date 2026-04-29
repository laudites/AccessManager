# AGENTS.md

## Objetivo

Sistema interno de gestao de clientes e telas de acesso.

O MVP da Fase 1 esta funcional com backend e frontend.

---

## Stack obrigatoria

- Backend: .NET 8 Web API
- Frontend: React + Vite
- Banco: MySQL
- ORM: Entity Framework Core

---

## Arquitetura

- Clean Architecture
- SOLID
- Clean Code

Estrutura real do backend:

- `backend/src/AccessManager.Domain`
- `backend/src/AccessManager.Application`
- `backend/src/AccessManager.Infrastructure`
- `backend/src/AccessManager.Api`
- `backend/tests/AccessManager.UnitTests`

---

## Regras de dependencia (OBRIGATORIO)

- Domain NAO depende de ninguem
- Application depende apenas de Domain
- Infrastructure depende de Application e Domain
- Api depende de Application e usa Infrastructure apenas via DI

Nunca inverter essas dependencias.

---

## Estado atual da Fase 1

Implementado:

- CRUD de clientes
- CRUD de servidores
- CRUD de telas
- Filtros de telas por cliente e servidor
- Clientes exibem quantidade de telas e valor total das telas ativas
- Telas retornam status salvo/manual e status exibido/calculado separadamente
- Servidores usam `QuantidadeCreditos` e `ValorCustoCredito`
- Financeiro e por cliente e agrupa valores das telas ativas
- Renovacao tecnica manual com sugestao de +30 dias no frontend
- Troca manual de servidor
- Historico persistido para renovacao e troca de servidor
- CRUD financeiro manual
- Marcacao manual de pagamento
- Listagens financeiras de pendentes e atrasados
- Dashboard com resumo operacional e financeiro
- Dashboard financeiro exibe rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor e pendencias
- Geracao de pendencias financeiras usa `DiaPagamentoPreferido` e proximo vencimento real do cliente
- BackgroundService executa a mesma geracao de pendencias do endpoint manual
- Frontend React funcional com paginas de dashboard, clientes, servidores, telas, financeiro e historico

Observacao importante: a pagina de historico existe no frontend, mas a visualizacao dedicada do historico ainda deve ser tratada como melhoria futura caso nao haja endpoint especifico exposto para consulta.

---

## Regras principais

- Um cliente pode ter varias telas
- Cada tela e independente
- Cada tela possui:
  - servidor
  - usuario
  - senha
  - valor acordado
  - vencimento tecnico
  - marca da TV
  - app utilizado
  - MAC/ID opcional
  - chave secundaria opcional

---

## Regras tecnicas

- Renovacao e manual
- Troca de servidor e manual
- Renovacao altera o vencimento tecnico e pode alterar o valor acordado
- Na renovacao, o frontend sugere +30 dias a partir da maior data entre hoje e o vencimento tecnico atual
- O calendario da renovacao permanece editavel
- Troca de servidor altera apenas o servidor da tela
- Renovacao e troca de servidor devem gerar historico
- Pagamento financeiro nao altera vencimento tecnico
- `Status` da tela e o status salvo/manual
- `StatusExibicao` e calculado para apresentacao com base em `DataVencimentoTecnico`
- `StatusExibicao` preserva `Cancelado` e `Suspenso`
- Para os demais status, `StatusExibicao` e `Vencido` se `DataVencimentoTecnico` < hoje, `Vencendo` se estiver entre hoje e hoje + 3 dias, e `Ativo` caso contrario

---

## Regras financeiras

- Financeiro e separado do tecnico
- Pagamento nao renova automaticamente
- Lancamentos sao por cliente
- Valor do lancamento e calculado pela soma dos valores acordados das telas ativas do cliente
- `CompetenciaReferencia` e calculada pelo backend a partir de `DataVencimentoFinanceiro`
- `TelaClienteId` e opcional no lancamento financeiro
- Pode haver divida mesmo com tela ativa
- Valor financeiro deve ser maior que zero
- Geracao de lancamentos pendentes usa `DiaPagamentoPreferido` para calcular o proximo vencimento real
- Se faltarem ate 5 dias para o proximo vencimento financeiro, gera lancamento `Pendente`
- `DataVencimentoFinanceiro` recebe o proximo dia de pagamento calculado
- Meses curtos usam o ultimo dia valido do mes quando o dia preferido nao existe
- A geracao evita duplicidade por `ClienteId` + `DataVencimentoFinanceiro`
- Endpoint manual e BackgroundService usam o mesmo caso de uso

---

## Regras de servidores

- Servidor possui `QuantidadeCreditos`
- Servidor possui `ValorCustoCredito`
- `LimiteClientes` nao e mais regra funcional
- Custo mensal pode ser calculado por creditos utilizados/cadastrados no servidor
- Creditos de servidor sao regra operacional/financeira de custo, nao limite tecnico automatico de clientes

---

## Proximas melhorias planejadas

As regras principais de clientes, servidores, financeiro agrupado, status tecnico exibido, geracao de pendencias e dashboard financeiro ja foram implementadas. Pendencias futuras:

- Expor endpoint e tela completa para historico tecnico.

---

## Execucao local

Backend:

```powershell
dotnet restore backend/AccessManager.sln
dotnet ef database update --project backend/src/AccessManager.Infrastructure/AccessManager.Infrastructure.csproj --startup-project backend/src/AccessManager.Api/AccessManager.Api.csproj --context AccessManagerDbContext
dotnet run --project backend/src/AccessManager.Api/AccessManager.Api.csproj --launch-profile http
```

Frontend:

```powershell
cd frontend
npm install
npm run dev
```

URLs padrao:

- API: `http://localhost:5025`
- Frontend: `http://localhost:5173`

---

## Build obrigatorio antes de concluir mudancas de codigo

Backend:

```powershell
dotnet build backend/AccessManager.sln
dotnet test backend/AccessManager.sln
```

Frontend:

```powershell
cd frontend
npm run build
```

Para mudancas apenas em documentacao, build pode ser dispensado se nada de codigo foi alterado.

---

## Fora do escopo

- integracoes externas
- automacoes
- notificacoes
- cobranca automatica
- renovacao automatica por pagamento

---

## Boas praticas

- mudancas pequenas
- sempre garantir build quando houver codigo
- evitar complexidade desnecessaria
- nao adicionar dependencias sem necessidade
- controllers devem ser finos
- regras devem ficar na Application/Domain
- manter DTOs separados das entidades
- manter respostas consistentes pelo envelope atual da API
- nao misturar regras financeiras com regras tecnicas

---

## Ordem de evolucao recomendada

1. Ajustes de backend e regras
2. Entidades e DTOs
3. Banco, migrations e repositorios
4. Endpoints
5. Testes
6. Frontend
