# Documentação de Arquitetura - Portal Elo APAE

**Disciplina:** Prática IV  
**Instituição:** Universidade do Oeste de Santa Catarina (UNOESC)  
**Orientador:** Prof. Maurício Gonzatto  

---

## 1. Visão Geral do Sistema

O **Portal Elo APAE** é uma plataforma web desenvolvida para apoiar a **APAE Videira** na gestão de eventos solidários e captação de voluntários/colaboradores. O sistema permite a divulgação pública de eventos institucionais, registro simplificado de interessados em contribuir (com diferentes tipos de apoio), controle administrativo de eventos e automação no envio de e-mails informativos para os voluntários.

---

## 2. Padrão Arquitetural

A solução adota uma **Arquitetura em Camadas (Layered Architecture)** com separação clara de responsabilidades:

```
[ Camada de Apresentação ]  -> APAEApplication (ASP.NET Core 10 MVC)
         │
         ▼
[ Camada de Infraestrutura/Dados ] -> Repository (EF Core 10 + MySQL Provider)
         │
         ▼
[ Camada de Domínio / Modelos ] -> Model (Entidades: Event, Collaborator, Address)
         │
         ▼
[ Camada Central / Biblioteca ] -> Biblioteca (Enums: EventStatus, HelpType)
```

### Detalhamento dos Módulos:

| Projeto | Camada | Responsabilidade |
| :--- | :--- | :--- |
| **`APAEApplication`** | Apresentação / Web | Controllers MVC, Views Razor (HTML5/CSS3/Bootstrap), Serviços (`EmailService`), Autenticação baseada em sessão administrativa. |
| **`Repository`** | Infraestrutura / Acesso a Dados | `AppDbContext`, repositórios de dados (`EventRepository`, `CollaboratorRepository`), mapeamento e persistência no banco MySQL. |
| **`Model`** | Domínio / Entidades | Modelos de negócio representativos das tabelas relacionais (`Collaborator`, `Event`, `Address`). |
| **`Biblioteca`** | Core / Compartilhado | Enums e tipos estruturais reutilizáveis em toda a solução (`EventStatus`, `HelpType`). |

---

## 3. Modelo de Entidade e Relacionamento (DER)

```mermaid
erDiagram
    EVENTS ||--o COLLABORATORS : "possui inscritos (1:N)"
    
    EVENTS {
        int Id PK
        string Title "Título do evento"
        string Date "Data prevista"
        string Description "Descrição/Objetivo"
        string ResourcesNeeded "Lista serializada em JSON"
        string Status "Ativo / Finalizado / Cancelado"
    }

    COLLABORATORS {
        int Id PK
        string Name "Nome completo"
        string Email "E-mail para contato"
        string Phone "WhatsApp / Telefone"
        string HelpTypes "Tipos de ajuda em JSON"
        bool EmailSent "Flag de disparo de e-mail"
        int EventId FK "Chave estrangeira para EVENTS"
    }

    ADDRESSES {
        int Id PK
        string Street "Rua/Avenida"
        string City "Cidade"
        string State "Estado/UF"
    }
```

---

## 4. Segurança e Boas Práticas Implementadas

1. **Proteção contra Cross-Site Request Forgery (CSRF):** Todos os formulários `POST` utilizam `[ValidateAntiForgeryToken]`.
2. **Controle de Acesso Quebrado (Broken Access Control - OWASP A01):** Endpoints administrativos protegidos por checagem de sessão ativa no `AdminController`, `EventController` e `CollaboratorController`.
3. **Isolamento de Credenciais Sensíveis:** Remoção de senhas de produção e credenciais de e-mail do repositório, com modelos de configuração seguros (`appsettings.Example.json`) e proteção via `.gitignore`.
4. **Resiliência de Banco de Dados:** Inicialização do banco com tratamento de exceções e criação automática das tabelas (`EnsureCreated`) junto com seed inicial para apresentação.
