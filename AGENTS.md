
# AGENTS.md

## Objetivo

Sistema interno de gestão de clientes e telas de acesso.

---

## Stack obrigatória

- Backend: .NET 8 Web API
- Frontend: React
- Banco: MySQL
- ORM: Entity Framework Core

---

## Arquitetura

- Clean Architecture
- SOLID
- Clean Code

---

## Regras de dependência (OBRIGATÓRIO)

- Domain NÃO depende de ninguém
- Application depende apenas de Domain
- Infrastructure depende de Application e Domain
- Api depende de Application (e Infrastructure via DI)

Nunca inverter essas dependências.

---

## Regras principais

- Um cliente pode ter várias telas
- Cada tela é independente
- Cada tela possui:
  - servidor
  - usuário
  - senha
  - valor acordado
  - vencimento técnico
  - marca da TV
  - app utilizado
  - MAC/ID opcional
  - chave secundária opcional

---

## Regras técnicas

- Renovação é manual
- Troca de servidor é manual
- Alterações devem gerar histórico

---

## Regras financeiras

- Financeiro é separado do técnico
- Pagamento não renova automaticamente
- Lançamentos são manuais
- Pode haver dívida mesmo com tela ativa

---

## Fora do escopo

- integrações externas
- automações
- notificações
- cobrança automática

---

## Boas práticas

- mudanças pequenas
- sempre garantir build
- evitar complexidade desnecessária
- não adicionar dependências sem necessidade
- controllers devem ser finos
- regras devem ficar na Application/Domain

---

## Ordem de execução

1. Backend
2. Entidades
3. Banco (EF Core + MySQL)
4. CRUDs
5. Regras de negócio
6. Frontend