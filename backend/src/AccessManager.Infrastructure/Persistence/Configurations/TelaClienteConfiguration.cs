using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessManager.Infrastructure.Persistence.Configurations;

public class TelaClienteConfiguration : IEntityTypeConfiguration<TelaCliente>
{
    public void Configure(EntityTypeBuilder<TelaCliente> builder)
    {
        builder.ToTable("telas_clientes");

        builder.HasKey(tela => tela.Id);

        builder.Property(tela => tela.NomeIdentificacao)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(tela => tela.UsuarioTela)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tela => tela.SenhaTela)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tela => tela.ValorAcordado)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(tela => tela.DataInicio)
            .IsRequired();

        builder.Property(tela => tela.DataVencimentoTecnico)
            .IsRequired();

        builder.Property(tela => tela.Status)
            .IsRequired();

        builder.Property(tela => tela.MarcaTv)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tela => tela.AppUtilizado)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tela => tela.MacOuIdApp)
            .HasMaxLength(100);

        builder.Property(tela => tela.ChaveSecundaria)
            .HasMaxLength(100);

        builder.Property(tela => tela.Observacao)
            .HasMaxLength(500);

        builder.Property(tela => tela.Ativo)
            .IsRequired();

        builder.HasMany(tela => tela.RenovacoesHistorico)
            .WithOne(renovacao => renovacao.TelaCliente)
            .HasForeignKey(renovacao => renovacao.TelaClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(tela => tela.LancamentosFinanceiros)
            .WithOne(lancamento => lancamento.TelaCliente)
            .HasForeignKey(lancamento => lancamento.TelaClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
