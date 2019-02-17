@echo off
del %CD%\app.db
del /F /Q %CD%\Migrations\*.*
dotnet ef migrations add InitialCreate
dotnet ef database update