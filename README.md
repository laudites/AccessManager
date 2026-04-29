# AccessManager

Sistema interno para gestao de clientes, telas de acesso, servidores, renovacoes tecnicas e controle financeiro.

## Estado atual

A Fase 1 (MVP) esta concluida. O sistema possui backend .NET 8 com Entity Framework Core e MySQL, frontend React com Vite, CRUDs principais, dashboard, operacoes tecnicas manuais, financeiro por cliente e controle de custos por creditos de servidor.

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
dotnet ef database update --project backend/src/AccessManager.Infrastructure/AccessManager.Infrastructure.csproj --startup-project backend/src/AccessManager.Api/AccessManager.Api.csproj --context AccessManagerDbContext
```

Sempre que houver mudanca no modelo de dados ou nova migration, execute `database update` antes de rodar a API. A migration `ServerCreditsAndClientFinance` atualiza servidores para creditos/custo por credito e ajusta lancamentos financeiros para o modelo por cliente.

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
- Status tecnico exibido/calculado para telas, separado do status salvo/manual
- Renovacao tecnica manual de tela com sugestao automatica de +30 dias
- Troca manual de servidor da tela
- Historico gravado em renovacoes e trocas de servidor
- CRUD de lancamentos financeiros
- Marcacao manual de pagamento
- Listagem de pendentes e atrasados
- Status financeiro exibido/calculado separado do status salvo/manual
- Dashboard com totais de clientes, telas, vencimentos e valores em aberto
- Clientes exibem quantidade de telas e valor agrupado das telas ativas
- Servidores usam quantidade de creditos e valor de custo por credito
- Financeiro cria lancamentos por cliente, agrupando valores das telas ativas
- Competencia financeira e calculada automaticamente pelo backend
- Geracao manual e automatica de lancamentos pendentes com base no dia de pagamento preferido do cliente
- Dashboard com rendimento mensal, custo mensal, clientes pagos no mes, creditos por servidor e pendencias financeiras

## Regras recentes importantes

### Telas

- `Status` e o status salvo/manual da tela.
- `StatusExibicao` e calculado pelo backend para apresentacao, sem sobrescrever o status salvo.
- `StatusExibicao` preserva `Cancelado` e `Suspenso`.
- Para os demais status, usa `DataVencimentoTecnico`: vencido quando a data e anterior a hoje, vencendo quando esta entre hoje e hoje + 3 dias, e ativo nos demais casos.
- Ao renovar, o frontend sugere nova data com +30 dias a partir da maior data entre hoje e o vencimento tecnico atual.
- O calendario da renovacao continua editavel.

### Financeiro

- `StatusFinanceiro` e o status salvo/manual do lancamento.
- `StatusFinanceiroExibicao` e calculado pelo backend para apresentacao, sem sobrescrever o status salvo.
- `StatusFinanceiroExibicao` preserva `Pago`, `Cancelado` e `Atrasado` quando estes forem o status salvo.
- Lancamento salvo como `Pendente`, sem `DataPagamento`, com `DataVencimentoFinanceiro` anterior a hoje, e exibido como `Atrasado`.
- A listagem de pendentes nao inclui vencidos calculados como atrasados.
- A listagem de atrasados inclui lancamentos salvos como `Atrasado` e pendentes vencidos por data.
- O dashboard nao conta o mesmo lancamento como pendente e atrasado.
- No frontend Financeiro, badges/listagem/detalhe usam `StatusFinanceiroExibicao`; formularios continuam usando `StatusFinanceiro`.
- A geracao de pendentes usa `DiaPagamentoPreferido` do cliente para calcular o proximo vencimento real.
- Se faltarem ate 5 dias para esse vencimento, o sistema gera um lancamento `Pendente`.
- `DataVencimentoFinanceiro` recebe o proximo dia de pagamento calculado.
- Em meses curtos, dias 29, 30 ou 31 usam o ultimo dia valido do mes.
- O valor do lancamento e a soma das telas ativas do cliente.
- A duplicidade e evitada por `ClienteId` + `DataVencimentoFinanceiro`.
- Pagamento continua manual e nao renova tela.

## Proximas melhorias planejadas

- Expor consulta completa do historico tecnico.

## Fora do escopo atual

- Integracoes externas
- Automacoes com servidores
- Notificacoes
- Cobranca automatica
- Renovacao automatica por pagamento
