using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Game");
            builder.HasKey(g => g.GameId);
            builder.Property(g => g.GameId).ValueGeneratedOnAdd().HasColumnType("INT");

            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.Property(g => g.Description).IsRequired().HasMaxLength(200);
            builder.Property(g => g.Genre).IsRequired().HasMaxLength(30);
            builder.Property(g => g.Price).IsRequired().HasColumnType("DECIMAL(18,2)");
            builder.Property(g => g.ReleaseDate).IsRequired().HasColumnType("DATE");
            builder.Property(g => g.Rating).IsRequired(false).HasColumnType("INT");
            builder.Property(g => g.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(
                    v => v, // Grava no banco normalmente
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler
                );
            builder.Property(g => g.UpdatedAt)
                .IsRequired(false)
                .HasConversion(
                    v => v, // Grava no banco normalmente  
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null // Força Kind como UTC ao ler  
                );
        }
    }
}
