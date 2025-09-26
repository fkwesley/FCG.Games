using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RequestLogConfiguration : IEntityTypeConfiguration<RequestLog>
    {
        public void Configure(EntityTypeBuilder<RequestLog> builder)
        {
            builder.ToTable("Request_log");

            builder.HasKey(log => log.LogId);
            builder.Property(log => log.LogId).IsRequired();

            builder.Property(log => log.UserId)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(log => log.HttpMethod)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(log => log.Path)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(log => log.StatusCode).IsRequired();

            builder.Property(log => log.RequestBody).IsRequired(false);
            builder.Property(log => log.ResponseBody).IsRequired(false);

            builder.Property(log => log.StartDate)
                .IsRequired()
                .HasConversion(
                    v => v, // Grava no banco normalmente
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler
                );
            builder.Property(log => log.EndDate)
                .IsRequired()
                .HasConversion(
                    v => v, // Grava no banco normalmente
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler
                );
            builder.Property(log => log.Duration).IsRequired();
        }
    }
}
