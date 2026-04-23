# AccessManager

Sistema interno para gestão de clientes, telas (acessos), servidores, renovações técnicas e controle financeiro simples.

## Objetivo

Permitir controle completo de:

- clientes
- telas (pontos)
- servidores
- vencimentos técnicos
- cobranças manuais
- histórico

## Conceito principal

Tela = unidade independente de controle técnico e financeiro

Cliente → N Telas → Servidor → Financeiro

---

## Estrutura do projeto

### Backend
- .NET 8 Web API
- Clean Architecture
- Entity Framework Core
- MySQL

### Frontend
- ReactJS
- Axios
- React Router
- Bootstrap

---

## Fase atual

Fase 1 (MVP):

- sem integrações externas
- sem automações
- controle manual completo

---

## Execução

### Backend

1. Configurar connection string no `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=accessmanager;user=root;password=senha"
}

dotnet ef database update

dotnet run

npm install
npm start