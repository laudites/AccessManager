using AccessManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessManager.Infrastructure.Persistence.Configurations;

public class RenovacaoTelaHistoricoConfiguration : IEntityTypeConfiguration<RenovacaoTelaHistorico>
{
    public void Configure(EntityTypeBuilder<RenovacaoTelaHistorico> builder)
    {
        builder.ToTable("renovacoes_telas_historico");

        builder.HasKey(renovacao => renovacao.Id);

        builder.Property(renovacao => renovacao.DataVencimentoTecnicoAnterior)
            .IsRequired();

        builder.Property(renovacao => renovacao.NovaDataVencimentoTecnico)
            .IsRequired();

        builder.Property(renovacao => renovacao.ValorAcordadoAnterior)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(renovacao => renovacao.ValorAcordadoNovo)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(renovacao => renovacao.Observacao)
            .HasMaxLength(500);

        builder.Property(renovacao => renovacao.DataCriacao)
            .IsRequired();

        builder.HasOne(renovacao => renovacao.Cliente)
            .WithMany()
            .HasForeignKey(renovacao => renovacao.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(renovacao => renovacao.ServidorAnterior)
            .WithMany()
            .HasForeignKey(renovacao => renovacao.ServidorAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(renovacao => renovacao.ServidorNovo)
            .WithMany()
            .HasForeignKey(renovacao => renovacao.ServidorNovoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
