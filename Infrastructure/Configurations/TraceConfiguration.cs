using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TraceConfiguration : IEntityTypeConfiguration<Trace>
    {
        public void Configure(EntityTypeBuilder<Trace> builder)
        {
            builder.ToTable("Trace_log");
            builder.HasKey(x => x.TraceId);
            builder.Property(e => e.TraceId).ValueGeneratedOnAdd();

            builder.Property(log => log.LogId). IsRequired();
            builder.Property(log => log.Timestamp)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()")
                   .HasConversion(
                        v => v, // Grava no banco normalmente
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler
                    );
            builder.Property(log => log.Level)
                   .IsRequired()
                   .HasConversion<string>()                 // Converte enum <-> string no banco
                   .HasMaxLength(50);
            builder.Property(log => log.Message)
                   .IsRequired()
                   .HasMaxLength(1000);
            builder.Property(log => log.StackTrace)
                   .HasMaxLength(4000)
                   .IsRequired(false);

            builder.HasOne(e => e.RequestLog)              // Cada Trace pertence a um RequestLog
                       .WithMany(r => r.Traces)            // Um RequestLog pode ter várias Traces
                       .HasForeignKey(e => e.LogId)        // LogId é a FK em Trace
                       .OnDelete(DeleteBehavior.Restrict); // Evita delete em cascata
        }
    }
}
