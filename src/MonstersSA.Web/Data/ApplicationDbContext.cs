using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Models;

namespace MonstersSA.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Arquivo de contexto, mapeia as tabelas do banco e faz o data seeding dos torneios no banco
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabelas do banco:
        public DbSet<Player> Players { get; set; }
        public DbSet<TournamentDefinition> TournamentDefinitions { get; set; }
        public DbSet<StatementFile> StatementFiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PlayedTournament> PlayedTournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CONFIG DAS TABELAS:

            // Configuração da Tabela: Player
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.Name)
                     .HasMaxLength(100)
                     .IsRequired(); // NOT NULL
            });

            // Configuração da Tabela: TournamentDefinition
            modelBuilder.Entity<TournamentDefinition>(entity =>
            {
                entity.ToTable("TournamentDefinitions");
                entity.HasKey(t => t.TournamentDefinitionId);
                entity.Property(t => t.Name)
                     .HasMaxLength(255)
                     .IsRequired();
                
                entity.Property(t => t.BuyInAmount)
                     .HasColumnType("decimal(18, 2)") // Padrão profissional para valores monetários
                     .IsRequired();
            });

            // Configuração da Tabela: StatementFile
            modelBuilder.Entity<StatementFile>(entity =>
            {
                entity.ToTable("StatementFiles");
                entity.HasKey(f => f.StatementFileId);
                entity.Property(f => f.OriginalFileName)
                     .HasMaxLength(300)
                     .IsRequired();
                entity.Property(f => f.UploadDateUtc).IsRequired();
                entity.Property(f => f.PeriodStartDate).IsRequired();
                entity.Property(f => f.PeriodEndDate).IsRequired();

                // Define o relacionamento 
                entity.HasOne(f => f.Player) // Um arquivo processado tem um Player
                     .WithMany(p => p.StatementFiles) // Um Player tem muitos arquivos
                     .HasForeignKey(f => f.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade); // Se um Player for deletado, seus arquivos são deletados
            });

            // Configuração da Tabela: Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transactions");
                entity.HasKey(t => t.TransactionId);
                entity.Property(t => t.Description)
                     .HasMaxLength(255)
                     .IsRequired();
                
                entity.Property(t => t.ReferenceId)  // ReferenceId pode ser nulo (ex: transações de "Withdrawal" [4])
                     .HasMaxLength(50) 
                     .IsRequired(false); 

                entity.Property(t => t.CashAmount)
                     .HasColumnType("decimal(18, 2)")
                     .IsRequired();
                
                entity.Property(t => t.Points)
                     .HasColumnType("decimal(10, 2)")
                     .IsRequired();

                // Define o relacionamento 
                entity.HasOne(t => t.StatementFile)
                     .WithMany(f => f.Transactions)
                     .HasForeignKey(t => t.StatementFileId)
                     .OnDelete(DeleteBehavior.Cascade); // Se um arquivo for deletado, suas transações são deletadas
            });

            // Configuração da Tabela: PlayedTournament
            modelBuilder.Entity<PlayedTournament>(entity =>
            {
                entity.ToTable("PlayedTournaments");
                entity.HasKey(pt => pt.PlayedTournamentId);

                entity.Property(pt => pt.ReferenceId)
                     .HasMaxLength(50)
                     .IsRequired();
                
                entity.HasIndex(pt => pt.ReferenceId); // Adiciona um índice ao ReferenceId para otimizar futuras consultas

                entity.Property(pt => pt.TotalBuyIn)
                     .HasColumnType("decimal(18, 2)")
                     .IsRequired();
                
                entity.Property(pt => pt.TotalPayout)
                     .HasColumnType("decimal(18, 2)")
                     .IsRequired();
                
                entity.Property(pt => pt.NetResult)
                     .HasColumnType("decimal(18, 2)")
                     .IsRequired();

                // Relacionamento com Player
                entity.HasOne(pt => pt.Player)
                     .WithMany(p => p.PlayedTournaments)
                     .HasForeignKey(pt => pt.PlayerId)
                     .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento com TournamentDefinition
                entity.HasOne(pt => pt.TournamentDefinition)
                     .WithMany() // Sem propriedade de navegação de volta
                     .HasForeignKey(pt => pt.TournamentDefinitionId)
                     .OnDelete(DeleteBehavior.Restrict); // Impede que uma definição seja deletada se houver torneios jogados ligados a ela
            });


            // Data Seeding para carregar os dados da GRADE MAX LATE no banco
            modelBuilder.Entity<TournamentDefinition>().HasData(
            // Torneios de $5.50
            new TournamentDefinition { TournamentDefinitionId = 1, Name = "MICRO ROLLER", BuyInAmount = 5.50m, StartTime = new TimeOnly(23, 34) },
            new TournamentDefinition { TournamentDefinitionId = 2, Name = "MICRO NIGHTLY", BuyInAmount = 5.50m, StartTime = new TimeOnly(1, 54) },
          
            // Torneios de $11.00
            new TournamentDefinition { TournamentDefinitionId = 3, Name = "LUCKY 7s", BuyInAmount = 11.00m, StartTime = new TimeOnly(0, 20) },
            new TournamentDefinition { TournamentDefinitionId = 4, Name = "CRAZY 8s", BuyInAmount = 11.00m, StartTime = new TimeOnly(14, 30) },
            new TournamentDefinition { TournamentDefinitionId = 5, Name = "CRAZY KO", BuyInAmount = 11.00m, StartTime = new TimeOnly(23, 10) },
          
            // Torneios de $16.50
            new TournamentDefinition { TournamentDefinitionId = 6, Name = "LOW ROLLER", BuyInAmount = 16.50m, StartTime = new TimeOnly(22, 34) },
            new TournamentDefinition { TournamentDefinitionId = 7, Name = "CRAZY 8s MADRUGADA", BuyInAmount = 16.50m, StartTime = new TimeOnly(2, 10) },
          
            // Torneios de $9.90
            new TournamentDefinition { TournamentDefinitionId = 8, Name = "MINOR NINER", BuyInAmount = 9.90m, StartTime = new TimeOnly(3, 40) },
            new TournamentDefinition { TournamentDefinitionId = 9, Name = "EARLY NINER", BuyInAmount = 9.90m, StartTime = new TimeOnly(15, 30) },
          
            // Torneios de $4.40
            new TournamentDefinition { TournamentDefinitionId = 10, Name = "MICRO MONSTER", BuyInAmount = 4.40m, StartTime = new TimeOnly(1, 10) },
          
            // Torneios de $55.00
            new TournamentDefinition { TournamentDefinitionId = 11, Name = "NIGHTLY", BuyInAmount = 55.00m, StartTime = new TimeOnly(2, 30) },
          
            // Free Rolls
            new TournamentDefinition { TournamentDefinitionId = 12, Name = "FREEROLL", BuyInAmount = 0.00m, StartTime = new TimeOnly(0, 0) }
            );
          }
     }
}