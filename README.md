# FieldOps Safety API

Small backend project for recording safety incidents in field operations. It is a portfolio project inspired by industrial and mining workflows, but it does not pretend to model a complete mining operation.

## What it demonstrates

- REST endpoints with ASP.NET Core and C#.
- Input validation and status transitions.
- Dependency inversion through a small store interface.
- Filtering incidents by status.
- Automated tests for the main workflow.

## Run locally

```powershell
dotnet run --project FieldOps.Safety/FieldOps.Safety.csproj --urls http://127.0.0.1:5190
```

The API will be available at `http://127.0.0.1:5190`.

## Endpoints

- `GET /health`
- `GET /api/incidents`
- `GET /api/incidents?status=Open`
- `POST /api/incidents`
- `PATCH /api/incidents/{id}/status`

Example request:

```json
{
  "title": "Derrame de aceite",
  "description": "Se detectó aceite cerca del taller",
  "area": "Taller",
  "severity": "High"
}
```

The first version uses an in-memory store to keep setup simple. A PostgreSQL/Supabase adapter is the next planned step once the workflow is validated.

## Test

```powershell
dotnet test
```
