using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessManager.Infrastructure.Persistence.Configurations;

public class LancamentoFinanceiroConfiguration : IEntityTypeConfiguration<LancamentoFinanceiro>
{
    public void Configure(EntityTypeBuilder<LancamentoFinanceiro> builder)
    {
        builder.ToTable("lancamentos_financeiros");

        builder.HasKey(lancamento => lancamento.Id);

        builder.Property(lancamento => lancamento.CompetenciaReferencia)
            .IsRequired();

        builder.Property(lancamento => lancamento.Descricao)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(lancamento => lancamento.Valor)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(lancamento => lancamento.DataVencimentoFinanceiro)
            .IsRequired();

        builder.Property(lancamento => lancamento.StatusFinanceiro)
            .IsRequired();

        builder.Property(lancamento => lancamento.Observacao)
            .HasMaxLength(500);

        builder.Property(lancamento => lancamento.DataCriacao)
            .IsRequired();

        builder.HasOne(lancamento => lancamento.Cliente)
            .WithMany(cliente => cliente.LancamentosFinanceiros)
            .HasForeignKey(lancamento => lancamento.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(lancamento => lancamento.TelaCliente)
            .WithMany(tela => tela.LancamentosFinanceiros)
            .HasForeignKey(lancamento => lancamento.TelaClienteId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
