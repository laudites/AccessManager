# AccessManager

Sistema interno para gestao de clientes, telas de acesso, servidores, renovacoes tecnicas e controle financeiro manual.

## Estado atual

A Fase 1 (MVP) esta concluida. O sistema possui backend .NET 8 com Entity Framework Core e MySQL, frontend React com Vite, CRUDs principais, dashboard, operacoes tecnicas manuais e financeiro separado do tecnico.

## Conceito principal

Tela = unidade independente de controle tecnico e financeiro.

Cliente -> N Telas -> Servidor -> Financeiro

Uma tela pertence a um cliente e a um servidor. O pagamento financeiro nao renova a tela automaticamente.

## Estrutura do projeto

### Backend

- Caminho: `backend`
- Solution: `backend/AccessManager.sln`
- API: `backend/src/AccessManager.Api`
- Application: `backend/src/AccessManager.Application`
- Domain: `backend/src/AccessManager.Domain`
- Infrastructure: `backend/src/AccessManager.Infrastructure`
- Testes: `backend/tests/AccessManager.UnitTests`

### Frontend

- Caminho: `frontend`
- React + Vite
- Axios
- React Router
- Bootstrap

## Requisitos

- .NET 8 SDK
- Node.js e npm
- MySQL 8 ou compativel
- `dotnet-ef` instalado para aplicar migrations

Para instalar o `dotnet-ef`, se necessario:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
```

## Banco de dados

A connection string fica em:

```text
backend/src/AccessManager.Api/appsettings.json
```

Valor usado atualmente no projeto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=access_manager;User=root;Password=AccessManager12345;"
  }
}
```

Antes de rodar a API, ajuste usuario e senha conforme o MySQL local.

Aplicar migrations a partir da raiz do repositorio:

```powershell
dotnet ef database update --project backend/src/AccessManager.Infrastructure --startup-project backend/src/AccessManager.Api
```

## Executar backend

A partir da raiz do repositorio:

```powershell
dotnet restore backend/AccessManager.sln
dotnet run --project backend/src/AccessManager.Api/AccessManager.Api.csproj --launch-profile http
```

URL padrao da API em desenvolvimento:

```text
http://localhost:5025
```

O CORS de desenvolvimento permite o frontend em:

```text
http://localhost:5173
```

## Executar frontend

```powershell
cd frontend
npm install
npm run dev
```

URL padrao do Vite:

```text
http://localhost:5173
```

O frontend usa `VITE_API_BASE_URL` quando definida. Sem essa variavel, usa:

```text
http://localhost:5025
```

## Build e testes

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

## Funcionalidades do MVP

- CRUD de clientes
- CRUD de servidores
- CRUD de telas
- Filtros de telas por cliente e servidor
- Renovacao tecnica manual de tela
- Troca manual de servidor da tela
- Historico gravado em renovacoes e trocas de servidor
- CRUD de lancamentos financeiros
- Marcacao manual de pagamento
- Listagem de pendentes e atrasados
- Dashboard com totais de clientes, telas, vencimentos e valores em aberto

## Proximas melhorias planejadas

As regras abaixo ainda nao estao implementadas no codigo. Elas orientam a proxima etapa de desenvolvimento e devem ser tratadas com ajustes de dominio, DTOs, banco, API e frontend.

### Clientes

- A listagem e o detalhe de clientes devem exibir a quantidade de telas do cliente.
- A listagem e o detalhe de clientes devem exibir o valor total agrupado das telas do cliente.
- O valor agrupado deve ser calculado pela soma dos valores acordados das telas ativas do cliente.

### Servidores

- Remover o conceito de limite de clientes.
- Substituir por quantidade de creditos disponiveis/comprados no servidor.
- Adicionar valor de custo de cada credito do servidor.
- Permitir calculo de custo mensal com base nos creditos utilizados ou cadastrados.

### Financeiro

- O lancamento financeiro deve passar a ser feito por cliente.
- O valor do lancamento deve agrupar o valor das telas ativas do cliente.
- Se o cliente possui duas ou mais telas, o lancamento financeiro deve considerar a soma dessas telas.
- `CompetenciaReferencia` nao deve ser preenchida manualmente pelo usuario.
- `CompetenciaReferencia` pode ser mantida internamente para relatorios mensais, calculada automaticamente a partir de `DataVencimentoFinanceiro`.
- `DataVencimentoFinanceiro` representa a data acordada com o cliente para pagamento.
- O sistema deve gerar automaticamente lancamento financeiro pendente 5 dias antes do vencimento financeiro acordado.
- Pagamento continua manual.
- Pagamento nao renova tela.
- Financeiro continua separado do tecnico.

### Dashboard

- Exibir rendimento mensal.
- Exibir custo mensal.
- Exibir quantidade de clientes.
- Exibir quantidade de clientes que ja pagaram no mes.
- Exibir quantidade de creditos de cada servidor.
- Exibir quantidade de clientes/telas em cada servidor.
- Exibir lista de clientes/pessoas pendentes no financeiro.

## Fora do escopo atual

- Integracoes externas
- Automacoes com servidores
- Notificacoes
- Cobranca automatica
- Renovacao automatica por pagamento
