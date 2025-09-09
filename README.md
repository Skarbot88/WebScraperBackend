# WebScraper Backend - Local Setup

This guide will help you set up the **WebScraper backend** locally, connecting to a Dockerized PostgreSQL database. Migrations are applied automatically, so no extra setup is needed.

## Prerequisites

1. [Docker](https://docs.docker.com/get-docker/) installed and running.
2. [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installed.
3. [PostgreSQL](https://www.postgresql.org/download/) installed (optional if using Docker only, but recommended for direct database access/testing).
4. Terminal/PowerShell access.
5. Basic knowledge of running shell commands.

## 1. Clone the repository

```bash
git clone https://github.com/Skarbot88/WebScraperBackend.git
cd WebScraperBackend
```

## 2. Create environment file

In the **repo root**, create a `.env` file:

```env
DB_PASSWORD=YourStrongPasswordHere
```

> You can choose any secure password. This password will be used for the local Postgres database.

## 3. Start Dockerized Postgres

Run:

```bash
docker-compose up -d
```

- This will start a Postgres container on `localhost:5433`.
- Database: `WebScraper`
- User: `webscraper_admin`
- Password: from your `.env`

Check that the container is running:

```bash
docker ps
```

## 4. Load environment variable for the backend

In the **same terminal**, run:

```bash
export $(grep -v '^#' .env | xargs)
```

- Ensures the backend picks up `DB_PASSWORD` at runtime.

## 5. Run the backend

```bash
cd API
dotnet run
```

- Backend runs on: [http://localhost:5220](http://localhost:5220)
- Automatic migrations are applied at startup.

## 6. Verify setup

- Check logs in the terminal — you should see:

```
Applying migration 'Initial'.
Now listening on: http://localhost:5220
```

- The database `WebScraper` now has all tables created.

## 7. Additional notes

- **Frontend** runs on [http://localhost:3000](http://localhost:3000) — CORS is already configured for the backend.
- **Changing password**: Users can modify `.env` and restart the Postgres container.
- **Resetting the database**: To clean Postgres and start fresh, run:

```bash
docker-compose down -v
docker-compose up -d
```

- No plaintext passwords are stored in the repository.

---

This setup ensures any developer can **clone the repo, set a password, start Dockerized Postgres, and run the backend with migrations applied automatically**, ready to connect to the frontend.

