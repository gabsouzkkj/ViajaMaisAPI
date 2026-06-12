using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using ViajaMaisAPI.Models;

namespace ViajaMaisAPI.Data;

public class ViajaMaisContext : DbContext
{
    public ViajaMaisContext(DbContextOptions<ViajaMaisContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Destino> Destinos { get; set; }
    public DbSet<Hotel> Hoteis { get; set; }
    public DbSet<PacoteViagem> PacotesViagem { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().HasKey(c => c.Id);
        modelBuilder.Entity<Destino>().HasKey(d => d.Id);
        modelBuilder.Entity<Hotel>().HasKey(h => h.Id);
        modelBuilder.Entity<PacoteViagem>().HasKey(p => p.Id);
        modelBuilder.Entity<Reserva>().HasKey(r => r.Id);

        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.Destino)
            .WithMany(d => d.Hoteis)
            .HasForeignKey(h => h.DestinoId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<PacoteViagem>()
            .HasOne(p => p.Destino)
            .WithMany(d => d.PacotesViagem)
            .HasForeignKey(p => p.DestinoId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Cliente)
            .WithMany(c => c.Reservas)
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.PacoteViagem)
            .WithMany(p => p.Reservas)
            .HasForeignKey(r => r.PacoteViagemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PacoteViagem>()
            .Property(p => p.Preco)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Destino>().HasData(
            new Destino { Id = 1, Cidade = "Rio de Janeiro", EstadoOuPais = "RJ", Descricao = "A Cidade Maravilhosa com suas praias icônicas e o Cristo Redentor." },
            new Destino { Id = 2, Cidade = "Gramado", EstadoOuPais = "RS", Descricao = "Charme europeu, arquitetura alpina e a famosa Rota do Chocolate." }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Nome = "Copacabana Palace", Endereco = "Av. Atlântica, 1702", DestinoId = 1 },
            new Hotel { Id = 2, Nome = "Hotel Alpestre", Endereco = "Rua Leopoldo Rosenfeld, 67", DestinoId = 2 }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nome = "Ana Paula", Documento = "111.222.333-44", Email = "ana@email.com", Telefone = "14999990001", DataCadastro = new DateTime(2026, 1, 10) },
            new Cliente { Id = 2, Nome = "Bruno Costa", Documento = "555.666.777-88", Email = "bruno@email.com", Telefone = "14999990002", DataCadastro = new DateTime(2026, 2, 5) }
        );

        modelBuilder.Entity<PacoteViagem>().HasData(
            new PacoteViagem { Id = 1, NomePacote = "Férias no Rio de Janeiro", Preco = 1850.00m, VagasDisponiveis = 20, DataSaida = new DateTime(2026, 11, 10), DataRetorno = new DateTime(2026, 11, 17), DestinoId = 1 },
            new PacoteViagem { Id = 2, NomePacote = "Natal Luz em Gramado", Preco = 2490.90m, VagasDisponiveis = 15, DataSaida = new DateTime(2026, 12, 15), DataRetorno = new DateTime(2026, 12, 22), DestinoId = 2 }
        );

        modelBuilder.Entity<Reserva>().HasData(
            new Reserva { Id = 1, DataCompra = new DateTime(2026, 6, 1), QuantidadePassageiros = 2, StatusPagamento = "Confirmado", ClienteId = 1, PacoteViagemId = 1 },
            new Reserva { Id = 2, DataCompra = new DateTime(2026, 6, 5), QuantidadePassageiros = 1, StatusPagamento = "Pendente", ClienteId = 2, PacoteViagemId = 2 }
        );
    }
}