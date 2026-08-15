## Generate new migrations
```bash
dotnet ef --startup-project .\StockitApi\ migrations add {{MigrationName}} -o Data/Migrations
```

## Apply migrations to the database
```bash
dotnet ef --startup-project .\StockitApi\ database update
```