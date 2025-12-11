# Análise de dados da Bodog

## 📊 Sobre o Projeto

Sistema desenvolvido para automatizar a análise de desempenho em torneios de poker na plataforma Bodog. O sistema processa extratos detalhados (.xlsx) fornecidos pela Bodog e gera relatórios com métricas de performance por torneio, incluindo ROI, lucro líquido e estatísticas de jogo.

Recentemente refatorado de MVC para **Blazor Server**, o projeto agora oferece uma experiência mais fluida e interativa, com processamento em tempo real e lógica aprimorada para identificação de torneios.

## 🎯 Objetivo

Automatizar o processo de análise de desempenho semanal/mensal que anteriormente era feito manualmente, proporcionando insights rápidos e precisos sobre os torneios mais lucrativos, eliminando erros de cálculo e inconsistências de fuso horário.

## 🚀 Funcionalidades

- **Upload via SignalR**: Processamento rápido de arquivos .xlsx utilizando streams de memória, sem necessidade de reload de página.
- **Análise de Performance**: Cálculo de ROI, lucro líquido e estatísticas por torneio.
- **Algoritmo de Matching Inteligente**:
  - **Correção de Fuso**: Conversão automática de UTC (Bodog) para Horário de Brasília.
  - **Lógica Circular**: Identificação precisa de torneios da madrugada (ex: jogos às 02:00 pertencentes à grade das 22:00).
- **Relatórios Interativos**: Visualização imediata dos resultados ordenados por rentabilidade.
- **Gestão de Dados**: Botão integrado para limpeza do banco de dados antes de novos uploads.

## 🛠️ Tecnologias Utilizadas

- **.NET 10** - Framework principal
- **Blazor Web App (Interactive Server)** - Arquitetura web com renderização no servidor
- **Entity Framework Core** - ORM com suporte a `IDbContextFactory` para concorrência
- **SQLite** - Banco de dados local
- **ClosedXML** - Processamento robusto de arquivos Excel
- **Bootstrap** - Interface de usuário responsiva

## 📁 Estrutura do Projeto

```
MonstersSA.Web/ 
├── Components/ 
│ ├── Layout/ 
│ │ ├── MainLayout.razor 
│ │ └── NavMenu.razor 
│ └── Pages/ 
│ ├── Upload.razor # Página Inicial (Home) 
│ └── Reports.razor # Visualização dos Relatórios ├── Models/ 
│ ├── Player.cs 
│ ├── TournamentDefinition.cs 
│ ├── PlayedTournament.cs 
│ ├── Transaction.cs 
│ ├── StatementFile.cs 
│ └── TournamentPerformanceDto.cs 
├── Services/ 
│ ├── StatementProcessingService.cs 
│ └── ReportsService.cs 
├── Data/ 
│ └── ApplicationDbContext.cs 
├── wwwroot/ 
└── Program.cs
```

## 🔄 Fluxo da Aplicação

1. **Acesso Inicial** (domínio:porta) - Carregamento da SPA Blazor.
2. **Upload de Extrato** - Leitura do arquivo .xlsx via stream segura (SignalR).
   - O sistema ignora metadados do cabeçalho e busca a âncora de dados ("Date").
   - Aplica conversão de timezone e matemática modular para matching de torneios.
3. **Persistência** - Dados salvos em SQLite com verificação de duplicidade.
4. **Relatório** (`/Reports`) - Navegação automática para a visualização de ROI e Lucro Líquido.

## 📊 Métricas Calculadas

- **ROI (Return on Investment)**: Retorno percentual sobre o investimento.
- **Resultado Líquido**: Lucro/prejuízo total por torneio (Payout - BuyIn).
- **Total de Entradas**: Quantidade de vezes que cada torneio foi jogado.

## 🎮 Como Usar

### 1. Obter Extrato da Bodog
- Solicitar extrato semanal/mensal na plataforma Bodog.
- Download do arquivo .xlsx com todas as transações.

### 2. Upload no Sistema
- Acessar a página inicial.
- (Opcional) Clicar em **"⚠️ Limpar Banco de Dados"** para resetar análises anteriores.
- Selecionar arquivo .xlsx.
- Aguardar o processamento automático.

### 3. Analisar Resultados
- O sistema redireciona automaticamente para `/reports`.
- Identificar torneios "NÃO MAPEADOS" e analisar o ROI dos torneios conhecidos.

## ⚙️ Configuração e Execução

### Pré-requisitos
- .NET 10 SDK
- Visual Studio 2022 ou VS Code

### Execução Local
Recomendado o uso de Linux ou Windows (configurado via `.gitattributes` para compatibilidade).

```bash
git clone <repositorio>
cd src
cd MonstersSA.Web/
dotnet restore
dotnet run
```

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