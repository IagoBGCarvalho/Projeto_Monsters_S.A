# Análise de dados da Bodog - README

## 📊 Sobre o Projeto

Sistema desenvolvido para automatizar a análise de desempenho em torneios de poker na plataforma Bodog. O sistema processa extratos detalhados (.xlsx) fornecidos pela Bodog e gera relatórios com métricas de performance por torneio, incluindo ROI, lucro líquido e estatísticas de jogo.

## 🎯 Objetivo

Automatizar o processo de análise de desempenho semanal/mensal que anteriormente era feito manualmente, proporcionando insights rápidos e precisos sobre os torneios mais lucrativos.

## 🚀 Funcionalidades

- **Upload de Extratos**: Processamento automático de arquivos .xlsx da Bodog
- **Análise de Performance**: Cálculo de ROI, lucro líquido e estatísticas por torneio
- **Relatórios Inteligentes**: Ordenação por ROI para identificar os torneios mais rentáveis
- **Interface Web**: Interface intuitiva para upload e visualização de relatórios

## 🛠️ Tecnologias Utilizadas

- **.NET 10** - Framework principal
- **ASP.NET Core MVC** - Arquitetura web
- **Entity Framework Core** - ORM e acesso a dados
- **SQLite** - Banco de dados local
- **ClosedXML** - Processamento de arquivos Excel
- **Bootstrap** - Interface de usuário

## 📁 Estrutura do Projeto

```
MonstersSA.Web/
├── Controllers/
│   ├── HomeController.cs
│   ├── UploadController.cs
│   └── ReportsController.cs
├── Models/
│   ├── Player.cs
│   ├── TournamentDefinition.cs
│   ├── PlayedTournament.cs
│   ├── Transaction.cs
│   ├── StatementFile.cs
│   └── TournamentPerformanceDto.cs
├── Services/
│   ├── IStatementProcessingService.cs
│   ├── StatementProcessingService.cs
│   ├── IReportsService.cs
│   └── ReportsService.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Upload/
│   │   └── Index.cshtml
│   └── Reports/
│       └── Index.cshtml
└── wwwroot/
```

## 🔄 Fluxo da Aplicação

1. **Acesso Inicial** (domínio:porta) - Página inicial padrão
2. **Upload de Extrato** (`/Upload`) - Seleção do arquivo .xlsx
3. **Processamento** - Análise automática das transações e redirecionamento para /Reports
4. **Relatório** (`/Reports`) - Visualização dos resultados ordenados por ROI

## 📊 Métricas Calculadas

- **ROI (Return on Investment)**: Retorno percentual sobre o investimento
- **Resultado Líquido**: Lucro/prejuízo total por torneio
- **Total de Entradas**: Quantidade de vezes que cada torneio foi jogado

## 🎮 Como Usar

### 1. Obter Extrato da Bodog
- Solicitar extrato semanal/mensal na plataforma Bodog
- Download do arquivo .xlsx com todas as transações

### 2. Upload no Sistema
- Acessar `/Upload`
- Selecionar arquivo .xlsx
- Clicar em "Enviar Extrato"

### 3. Analisar Resultados
- Acessar `/Reports` (redirecionamento acontece automaticamente)
- Ver torneios ordenados por ROI (mais rentáveis primeiro)
- Identificar padrões de sucesso e oportunidades de melhoria

## ⚙️ Configuração e Execução

### Pré-requisitos
- .NET 10 SDK
- Visual Studio 2022 ou VS Code

### Execução Local
```bash
git clone <repositorio>
cd src
cd MonstersSA.Web/
dotnet restore
dotnet run
```

### OBS
Por enquanto, a funcionalidade de limpar o banco a cada uso (mais especificamente a tabela que contém cada linha do arquivo .xlsx) ainda não foi implementada, então é necessário **APAGAR** o banco de dados `monsters.db` ao final de cada uso para que o próximo resultado não seja poluído pelos dados do arquivo anterior.

## 📈 Exemplo de Saída

| Torneio | Jogos | Resultado Líquido | ROI |
|---------|-------|------------------|-----|
| MICRO ROLLER | 15 | R$ 210,18 | 254,76% |
| MICRO NIGHTLY | 13 | R$ 90,90 | 127,13% |
| NIGHTLY | 1 | R$ 39,50 | 71,82% |

## 🔍 Casos de Uso

- **Análise Semanal**: Identificar torneios com melhor performance recente
- **Otimização de Bankroll**: Direcionar recursos para torneios mais rentáveis
- **Ajuste de Estratégia**: Baseado em dados concretos de ROI
- **Tracking de Progresso**: Monitorar evolução ao longo do tempo

## 🤝 Contribuição

Este projeto segue o **GitHub Flow** para versionamento:
- `main` - Branch principal estável
- `feature:*` - Novas funcionalidades
- `fix:*` - Correções de bugs
- `refactor:*` - Refatoração de código
- `docs:*` - Alterações na documentação

## 📄 Licença

...

## 👥 Desenvolvimento

Desenvolvido por Iago Batista Gomes de Carvalho.