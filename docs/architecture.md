# Arquitetura

## Camadas

### Domain

- entidades
- enums
- regras puras

### Application

- casos de uso
- DTOs
- interfaces

### Infrastructure

- EF Core
- repositorios
- DbContext

### Api

- controllers
- configuracao
- DI

---

## Principios

- Domain nao depende de ninguem
- Application orquestra regras
- Infrastructure implementa persistencia
- API apenas expoe endpoints

---

## Boas praticas

- Controllers finos
- Logica na Application
- Entidades sem dependencia externa
- DTOs separados das entidades

---

## Proximas melhorias planejadas

As regras abaixo ainda nao estao implementadas e devem respeitar a Clean Architecture existente.

- Agregacoes de clientes, como quantidade de telas e valor total agrupado, devem ser calculadas na Application a partir de consultas da Infrastructure.
- Regras de financeiro por cliente devem ficar na Application/Domain, nao nos controllers.
- Geracao automatica de lancamentos 5 dias antes do vencimento deve ser modelada como caso de uso explicito. Se houver job/agendador no futuro, ele deve apenas acionar caso de uso da Application.
- Calculo de custo mensal por creditos de servidor deve ficar fora do controller e deve ter DTOs proprios.
- Mudancas de modelo, como remocao de `LimiteClientes` e adicao de creditos/custo por credito, exigem migration futura.
- Pagamento manual deve continuar isolado da renovacao tecnica.
