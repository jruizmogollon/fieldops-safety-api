# FieldOps Safety API

API pequeña para registrar incidentes de seguridad en operaciones de campo. La idea está inspirada en procesos que pueden aparecer en mantenimiento, industria o minería, pero el proyecto no pretende representar todo un sistema minero.

## Qué demuestra

- API REST con ASP.NET Core y C#.
- Persistencia local con SQLite.
- Configuración opcional para PostgreSQL o Supabase.
- Validación de datos y cambio de estados.
- Separación entre la API, el dominio y la persistencia.
- Pruebas automatizadas con xUnit.

## Ejecutar sin Supabase

No necesitas crear una cuenta ni mantener abierto Supabase para probar el proyecto. Por defecto usa SQLite y crea el archivo `fieldops.db` automáticamente.

```powershell
dotnet run --project FieldOps.Safety/FieldOps.Safety.csproj --urls http://127.0.0.1:5190
```

La API queda disponible en `http://127.0.0.1:5190`.

## Usar PostgreSQL o Supabase de forma opcional

La conexión se configura mediante variables de entorno para no guardar contraseñas en GitHub:

```powershell
$env:DatabaseProvider = "postgres"
$env:ConnectionStrings__Default = "Host=tu-host;Port=5432;Database=postgres;Username=postgres;Password=tu-clave;SSL Mode=Require;Trust Server Certificate=true"
dotnet run --project FieldOps.Safety/FieldOps.Safety.csproj --urls http://127.0.0.1:5190
```

Si no defines esas variables, el proyecto sigue usando SQLite. De esta manera, una pausa o falta de actividad en un proyecto gratuito de Supabase no rompe la demostración local.

## Endpoints

- `GET /health`
- `GET /api/incidents`
- `GET /api/incidents?status=Open`
- `GET /api/incidents/{id}`
- `POST /api/incidents`
- `PATCH /api/incidents/{id}/status`

Ejemplo de registro:

```json
{
  "title": "Derrame de aceite",
  "description": "Se detectó aceite cerca del taller",
  "area": "Taller",
  "severity": "High"
}
```

## Pruebas

```powershell
dotnet test
```
