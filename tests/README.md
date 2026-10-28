# PostgreSQL provider integration tests

These NUnit tests build the provider's package project and run its bulk-insert implementation with a real `UmbracoDatabase` against PostgreSQL. They do not require a separate Umbraco source checkout or a running CMS website. They complement, rather than replace, Umbraco's own unit and integration suites.

## Run locally

Requirements: .NET 10 SDK and a disposable PostgreSQL 16, 17, or 18 database. For example:

```sh
docker run --detach --rm --name umbraco-postgresql-tests \
  -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=provider_tests \
  -p 127.0.0.1:55432:5432 postgres:18

# Wait until this reports "accepting connections" before running the tests.
docker exec umbraco-postgresql-tests pg_isready -U postgres -d provider_tests

export POSTGRES_TEST_CONNECTION_STRING='Host=localhost;Port=55432;Database=provider_tests;Username=postgres;Password=postgres'
TZ=Europe/Oslo dotnet test tests/Our.Umbraco.PostgreSql.Tests/Our.Umbraco.PostgreSql.Tests.csproj --configuration Release

docker stop umbraco-postgresql-tests
```

The connection string must identify a test database where the user can create schemas. Each test creates and drops its own uniquely named schema. An absent connection string or unavailable database fails the tests rather than silently skipping them. Running with a non-UTC timezone makes the local `DateTime` conversion regression observable. CI uses `Europe/Oslo` for this reason.

## What is verified

- Raw Npgsql connections and connections wrapped by the production retry interceptor, with and without MiniProfiler, execute binary `COPY`.
- A PostgreSQL trigger records `current_query()` for inserted rows, so row-by-row `INSERT` cannot accidentally satisfy the COPY assertions.
- Provider-owned transactions commit, making rows visible to an independent connection; caller-owned transactions remain open and can be rolled back.
- Failed batches roll back without partial writes or a stuck connection; caller-owned failures remain the caller's responsibility to abort.
- The existing fallback for unknown connection wrappers completes its own transaction and preserves caller-owned transactions.
- Result/generated columns are excluded, column mappers and nulls are respected, and UTC/local/unspecified dates match ordinary inserts.
- Empty input opens no connection and starts no transaction.

The GitHub Actions workflow runs this suite against PostgreSQL 16, 17, and 18 and uploads TRX results. These tests establish provider behavior for the cases above, not full CMS/package compatibility or a measured throughput improvement.
