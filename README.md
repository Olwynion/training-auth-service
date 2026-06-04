# Training Auth Service

Микросервис аутентификации и авторизации. Входит в экосистему Training.

## Назначение

- Регистрация пользователей (Register)
- Вход в систему (Login)
- Обновление токенов (RefreshToken)
- Хранение пользователей и refresh-токенов в PostgreSQL

## Архитектура

Onion (Hexagonal) архитектура:

```
┌─────────────────────────────────────────┐
│  API (Training.Auth)                    │
│  - gRPC сервис (Grpc/)                  │
│  - CQRS Handlers (Handlers/)            │
│  - Startup/DI/Program                   │
├─────────────────────────────────────────┤
│  Services (Training.Auth.Services)      │
│  - Proto-файлы                          │
├─────────────────────────────────────────┤
│  Infrastructure (Training.Auth.Infrastructure)│
│  - Репозитории (Dapper + PostgreSQL)     │
│  - PasswordHasher (BCrypt)              │
│  - AuthTokenService (JWT)               │
│  - DbConnectionFactory                  │
├─────────────────────────────────────────┤
│  Domain (Training.Auth.Domain)          │
│  - Entity (User)                        │
│  - Interfaces (IUserRepository...)      │
│  - Domain Services (IAuthTokenService)  │
└─────────────────────────────────────────┘
```

## Как работает

1. Клиент (gateway/frontend) отправляет gRPC запрос на порт 5002
2. `AuthGrpcService` принимает запрос, парсит proto-сообщение
3. Через `IMediator` отправляет Command/Query в CQRS Handler
4. Handler выполняет бизнес-логику:
   - Register: хеширует пароль (BCrypt), создаёт пользователя, генерирует JWT
   - Login: проверяет пароль, генерирует JWT + refresh token
   - RefreshToken: проверяет refresh token, выдаёт новую пару токенов
5. Результат возвращается через proto-ответ

## Стек

- .NET 9
- gRPC (Grpc.AspNetCore)
- MediatR (CQRS)
- Dapper + Npgsql
- BCrypt.Net-Next (хэширование паролей)
- System.IdentityModel.Tokens.Jwt (JWT)
- PostgreSQL (через Docker)
- xUnit + Moq (тесты)

## Запуск

```bash
# Поднять БД
docker compose up -d

# Накатить миграции
./goose.exe -dir src/Training.Auth.Domain/Migrations postgres "host=localhost port=5434 user=postgres password=postgres dbname=training_auth sslmode=disable" up

# Запустить сервис
dotnet run --project src/Training.Auth/Training.Auth.csproj
```

Сервис слушает:
- gRPC: порт 5002
- HTTP/health: порт 5000

## Proto

Proto-контракты в подмодуле `proto/contracts/` (общий репозиторий [training-contracts](https://github.com/Olwynion/training-contracts)).
