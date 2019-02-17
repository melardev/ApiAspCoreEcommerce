@echo off
REM Remove database
rm app.db
REM remove Migrations folder, /q : quiet mode, do not ask for confirmation; /s remove recursively
rd /q /s Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update