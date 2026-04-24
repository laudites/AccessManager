using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessManager.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(cliente => cliente.Id);

        builder.Property(cliente => cliente.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(cliente => cliente.Telefone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(cliente => cliente.Observacao)
            .HasMaxLength(500);

        builder.Property(cliente => cliente.DataCadastro)
            .IsRequired();

        builder.Property(cliente => cliente.Ativo)
            .IsRequired();

        builder.HasMany(cliente => cliente.Telas)
            .WithOne(tela => tela.Cliente)
            .HasForeignKey(tela => tela.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cliente => cliente.LancamentosFinanceiros)
            .WithOne(lancamento => lancamento.Cliente)
            .HasForeignKey(lancamento => lancamento.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
