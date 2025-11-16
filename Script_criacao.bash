mkdir src # Cria a pasta source

cd src

dotnet new sln -n MonstersSA # Cria a nova Solução

dotnet new mvc -n MonstersSA.Web -o MonstersSA.Web # Cria o projeto MVC

dotnet sln MonstersSA.slnx add MonstersSA.Web/MonstersSA.Web.csproj # Adiciona o projeto à solução

# Criando as pastas que o MVC não cria por padrão:
mkdir MonstersSA.Web/Data
mkdir MonstersSA.Web/Services