---
name: ef-code-first-migrations
description: Manage Kangla database schema changes with Entity Framework Core code-first. Use whenever a request adds, removes, renames, changes, or relates persisted entity data, modifies PlantsContext configuration, or requires an EF migration.
---

# EF Code-First Migrations

Use this workflow for the SQLite-backed Kangla database.

## Change the model first

1. Inspect the affected entities in `src/kangla.Domain/Entities`, `PlantsContext`, DTOs, mapping, repositories, services, controllers, and the Angular client as applicable.
2. Edit the domain entities and relationships first. Add `DbSet` and Fluent API configuration in `src/kangla.Infrastructure/PlantsContext.cs` when required.
3. Update the application/API layers that expose or depend on the changed data. Do not expose credentials or internal-only fields.
4. Build the infrastructure project before scaffolding:

   ```powershell
   dotnet build src/kangla.Infrastructure/kangla.Infrastructure.csproj --no-restore
   ```

## Scaffold migrations with EF CLI

Never hand-author a migration, designer file, or model snapshot. Do not alter generated migration code to force a schema operation.

Use a concise PascalCase name that describes the model change. For this project, use the design-time factory and run:

```powershell
dotnet-ef migrations add <MigrationName> --no-build `
  --project src/kangla.Infrastructure/kangla.Infrastructure.csproj `
  --startup-project src/kangla.Infrastructure/kangla.Infrastructure.csproj
```

The generated migration and its `.Designer.cs` file belong in `src/kangla.Infrastructure/Migrations`. EF updates `PlantsContextModelSnapshot.cs`; preserve its existing line-ending convention.

If `dotnet-ef` is unavailable, install the version matching the EF Core packages before scaffolding. Do not substitute a manually created migration.

## Verify before handoff

1. Review the generated migration and its SQL:

   ```powershell
   dotnet-ef migrations script <PreviousMigration> <MigrationName> --no-build `
     --project src/kangla.Infrastructure/kangla.Infrastructure.csproj `
     --startup-project src/kangla.Infrastructure/kangla.Infrastructure.csproj
   ```

2. For SQLite operations such as dropping or altering a column, keep the EF-generated migration intact. EF uses the generated target model to rebuild the affected table safely.
3. Confirm model and migration agree:

   ```powershell
   dotnet-ef migrations has-pending-model-changes --no-build `
     --project src/kangla.Infrastructure/kangla.Infrastructure.csproj `
     --startup-project src/kangla.Infrastructure/kangla.Infrastructure.csproj
   ```

4. Run a backend build and `git diff --check`. Inspect the diff for line-ending-only churn.

Never remove, rewrite, or edit a migration that has been applied to a shared or production database. Create a new corrective migration instead.
