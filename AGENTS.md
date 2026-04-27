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
- Renovacao tecnica manual
- Troca manual de servidor
- Historico persistido para renovacao e troca de servidor
- CRUD financeiro manual
- Marcacao manual de pagamento
- Listagens financeiras de pendentes e atrasados
- Dashboard com resumo operacional e financeiro
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
- Lancamentos sao manuais
- Pode haver divida mesmo com tela ativa
- Lancamento financeiro pertence a um cliente e a uma tela
- A tela informada no lancamento deve pertencer ao cliente informado
- Valor financeiro deve ser maior que zero

---

## Proximas melhorias planejadas

As regras desta secao ainda nao estao implementadas. Na proxima etapa de codigo, atualizar Domain, Application, Infrastructure, Api, frontend e testes mantendo a separacao entre tecnico e financeiro.

### Clientes

- A aba Clientes deve mostrar quantidade de telas do cliente.
- A aba Clientes deve mostrar valor total agrupado das telas do cliente.
- Valor agrupado = soma dos `ValorAcordado` das telas ativas do cliente.
- Esse valor e informativo para cliente/financeiro e nao deve alterar vencimento tecnico.

### Servidores

- Remover o conceito de `LimiteClientes`.
- Substituir por quantidade de creditos disponiveis/comprados no servidor.
- Adicionar valor de custo por credito.
- Permitir calculo de custo mensal do servidor com base nos creditos utilizados ou cadastrados.
- Creditos de servidor sao regra operacional/financeira de custo, nao limite tecnico automatico de clientes.

### Financeiro

- Lancamento financeiro deve ser feito por cliente, nao por tela individual como regra principal.
- O valor do lancamento deve agrupar a soma das telas ativas do cliente.
- Cliente com duas ou mais telas deve ter lancamento considerando a soma dos valores acordados dessas telas.
- `CompetenciaReferencia` nao deve ser preenchida manualmente pelo usuario.
- `CompetenciaReferencia` pode continuar existindo internamente para relatorios mensais, calculada automaticamente a partir de `DataVencimentoFinanceiro`.
- `DataVencimentoFinanceiro` representa a data acordada com o cliente para pagamento.
- O sistema deve gerar automaticamente lancamento financeiro pendente 5 dias antes da `DataVencimentoFinanceiro`.
- Pagamento continua manual.
- Pagamento nao renova tela.
- Financeiro continua separado do tecnico.

### Dashboard

- Deve exibir rendimento mensal.
- Deve exibir custo mensal.
- Deve exibir quantidade de clientes.
- Deve exibir quantidade de clientes que ja pagaram no mes.
- Deve exibir quantidade de creditos de cada servidor.
- Deve exibir quantidade de clientes/telas em cada servidor.
- Deve exibir lista de clientes/pessoas pendentes no financeiro.

---

## Execucao local

Backend:

```powershell
dotnet restore backend/AccessManager.sln
dotnet ef database update --project backend/src/AccessManager.Infrastructure --startup-project backend/src/AccessManager.Api
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
