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
- repositórios
- DbContext

### Api
- controllers
- configuração
- DI

---

## Princípios

- Domain não depende de ninguém
- Application orquestra regras
- Infrastructure implementa persistência
- API apenas expõe endpoints

---

## Boas práticas

- Controllers finos
- Lógica na Application
- Entidades sem dependência externa
- DTOs separados das entidades