# Инструкции по разработке микросервисов

## Общие правила
- Не менять версию .NET, csproj или зависимости без согласования
- `Nullable` reference types: `<Nullable>enable</Nullable>`
- 3 официальных гайдлайна (Microsoft + Google style)
- Архитектура: Onion (Hexagonal)

## Архитектура
- **Domain (Core)** — сущности, VO, интерфейсы репозиториев
- **Application/Services** — use cases, бизнес-логика
- **Infrastructure** — реализации репозиториев, БД, внешние API
- **Presentation** — API, gRPC, Consumers
- Зависимости внутрь, Domain без внешних зависимостей
- Интерфейсы в Domain, реализации в Infrastructure

## DDD
- `Entities/`, `ValueObjects/`, `Repositories/`, `Enums/` — в Domain
- `Mappings/` — в Infrastructure

## Именование
- `PascalCase` для классов/методов/свойств
- `_camelCase` для приватных полей
- `I` для интерфейсов: `IUserRepository`

## Организация кода
- Каждый класс в отдельном файле, имя = класс

## Асинхронность
- `async/await`, без `.Result/.Wait()`, `ConfigureAwait(false)`

## БД
- Dapper, async методы, строки не в коде

## Тесты
- xUnit/NUnit/MSTest, Moq/NSubstitute
- `<ClassName>Tests`

## Proto
- `platform grpc restore --proto-source vendor.protogen` для генерации

## Before commit
- `dotnet build`
- `dotnet test`

## Фичи C#
- File-scoped namespaces (C# 10+)
- primary constructors (C# 12+)
