# ToDo Backend — WebService + WorkerService

A two-service backend for a To-Do application that demonstrates clean separation of concerns, message-driven communication, and minimal yet complete data modeling.

- **Todo.WebService** — Exposes REST API via Swagger, performs **validation only**, and publishes messages to **RabbitMQ**.
- **ToDo.WorkerService** — Consumes messages and performs **persistence to PostgreSQL** using **EF Core (Code First)**.
- **Relationships**: `User (1) → (many) Item`.
- **Soft Delete**: Deleting an item marks it as deleted instead of removing the row.

> The original assignment asks for two repositories. This codebase is organized as a **monorepo** for convenience; splitting instructions are included below.

---
## Overview

This project implements the backend only. Swagger acts as the client.  
The system is split into two deployable services with RabbitMQ in between. Writes flow through the message broker; the worker persists to Postgres. Reads can be proxied by the WebService or implemented directly per need.

---

## Main Objectives

- Provide clean **RESTful** endpoints for **User** and **Item**
- Enforce **validation** at the WebService boundary
- Use **RabbitMQ** for decoupled, reliable inter-service communication
- Use **EF Core (Code First)** with **migrations** against **PostgreSQL**
- Implement **Soft Delete** for items
- Keep the **data model minimal** yet practical

---
## Repository Structure

ToDo_Project/
├─ Todo.WebService/
│  ├─ ToDo.Core/                    # DTOs, Entities, Interfaces, Settings (RabbitMqSettings)
│  ├─ ToDo.Service/                 # Validation + publish to RabbitMQ
│  └─ Todo.WebService/              # Controllers, Program.cs, appsettings.json
│
├─ ToDo.WorkerService/
│  ├─ ToDo.Core/                    # Shared DTOs/Entities/Interfaces
│  ├─ ToDo.Data/                    # AppDbContext, Repositories, Migrations
│  └─ ToDo.WorkerService/           # Hosted Service, Consumers, Program.cs
│
└─ docker-compose.yml               # RabbitMQ + PostgreSQL for local development

---
## Architecture

```mermaid
flowchart LR
  Client["Swagger / REST Client"] -->|HTTP| Web["Todo.WebService (API + Validation)"]
  Web -->|publish JSON| MQ[("RabbitMQ")]
  MQ -->|consume user_queue| Worker["ToDo.WorkerService (Consumers)"]
  MQ -->|consume item_queue| Worker
  Worker -->|EF Core| DB[("PostgreSQL")]
