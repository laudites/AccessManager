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

## Regras implementadas recentes

As regras abaixo estao implementadas e devem continuar respeitando a Clean Architecture existente.

- Agregacoes de clientes, como quantidade de telas e valor total agrupado, devem ser calculadas na Application a partir de consultas da Infrastructure.
- Regras de financeiro por cliente devem ficar na Application/Domain, nao nos controllers.
- Geracao de lancamentos pendentes 5 dias antes do vencimento existe como caso de uso da Application, endpoint manual e BackgroundService na API. O job apenas aciona o caso de uso existente dentro de um escopo de DI.
- Calculo de custo mensal por creditos de servidor deve ficar fora do controller e deve ter DTOs proprios.
- Mudancas de modelo, como remocao de `LimiteClientes` e adicao de creditos/custo por credito, foram registradas na migration `ServerCreditsAndClientFinance`.
- Pagamento manual deve continuar isolado da renovacao tecnica.
- Filtros de listagem financeira por periodo devem ficar no service/repository e considerar `DataVencimentoFinanceiro`.
- Status financeiro do cliente deve ser calculado na Application a partir dos lancamentos do mes atual, sem persistir novo campo de dominio.
