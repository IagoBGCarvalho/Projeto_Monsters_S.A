mkdir src # Cria a pasta source

cd src

# Cria o projeto Blazor Web App com interatividade Server Global
dotnet new blazor -o MonstersSA.Web --interactivity Server --all-interactive

# Adiciona à solução
dotnet sln MonstersSA.slnx add MonstersSA.Web/MonstersSA.Web.csproj

# Instala as dependências do projeto
dotnet add MonstersSA.Web package Microsoft.EntityFrameworkCore.Sqlite
dotnet add MonstersSA.Web package Microsoft.EntityFrameworkCore.Design
dotnet add MonstersSA.Web package ClosedXML

# Criando as pastas que o Blazor não cria por padrão:
mkdir MonstersSA.Web/Data
mkdir MonstersSA.Web/Services
mkdir MonstersSA.Web/Models