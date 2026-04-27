using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessManager.Infrastructure.Persistence.Configurations;

public class ServidorConfiguration : IEntityTypeConfiguration<Servidor>
{
    public void Configure(EntityTypeBuilder<Servidor> builder)
    {
        builder.ToTable("servidores");

        builder.HasKey(servidor => servidor.Id);

        builder.Property(servidor => servidor.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(servidor => servidor.Descricao)
            .HasMaxLength(500);

        builder.Property(servidor => servidor.Status)
            .IsRequired();

        builder.Property(servidor => servidor.QuantidadeCreditos)
            .IsRequired();

        builder.Property(servidor => servidor.ValorCustoCredito)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(servidor => servidor.UsuarioPainel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(servidor => servidor.SenhaPainel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(servidor => servidor.Observacao)
            .HasMaxLength(500);

        builder.Property(servidor => servidor.Ativo)
            .IsRequired();

        builder.HasMany(servidor => servidor.Telas)
            .WithOne(tela => tela.Servidor)
            .HasForeignKey(tela => tela.ServidorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
