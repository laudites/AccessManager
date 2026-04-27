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
- Servidores usam `QuantidadeCreditos` e `ValorCustoCredito`
- Financeiro e por cliente e agrupa valores das telas ativas
- Renovacao tecnica manual
- Troca manual de servidor
- Historico persistido para renovacao e troca de servidor
- CRUD financeiro manual
- Marcacao manual de pagamento
- Listagens financeiras de pendentes e atrasados
- Dashboard com resumo operacional e financeiro
- Dashboard financeiro exibe rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor e pendencias
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
- Troca de servidor altera apenas o servidor da tela
- Renovacao e troca de servidor devem gerar historico
- Pagamento financeiro nao altera vencimento tecnico

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
- Geracao de lancamentos pendentes 5 dias antes do vencimento esta preparada como endpoint manual, sem job agendado real

---

## Regras de servidores

- Servidor possui `QuantidadeCreditos`
- Servidor possui `ValorCustoCredito`
- `LimiteClientes` nao e mais regra funcional
- Custo mensal pode ser calculado por creditos utilizados/cadastrados no servidor
- Creditos de servidor sao regra operacional/financeira de custo, nao limite tecnico automatico de clientes

---

## Proximas melhorias planejadas

As regras principais de clientes, servidores, financeiro agrupado e dashboard financeiro ja foram implementadas. Pendencias futuras:

- Criar job/background service real para gerar pendencias automaticamente.
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
