using Microsoft.EntityFrameworkCore;
using teste_pratico.Models.Entities;

namespace teste_pratico.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                // Configuração da chave primária
                entity.HasKey(e => e.Id);

                // Configuração do Id como auto-incremento
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Configuração dos campos obrigatórios
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Cep)
                    .IsRequired()
                    .HasMaxLength(8);

                // Configuração dos campos opcionais
                entity.Property(e => e.Street)
                    .HasMaxLength(200);

                entity.Property(e => e.City)
                    .HasMaxLength(100);

                entity.Property(e => e.State)
                    .HasMaxLength(2);

                // Índices para melhorar performance (InMemory compatible)
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.Cep);

                // Configuração de timestamps (InMemory compatible)
                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);

            });

            // Seed data para testes iniciais (opcional)
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Leonardo Testa",
                    Email = "leotesta@example.com",
                    Cep = "14302156",
                    Street = "Rua Prof Jose Marques",
                    City = "Batatais",
                    State = "SP",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automaticamente atualizar o UpdatedAt antes de salvar
            foreach (var entry in ChangeTracker.Entries<Customer>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}