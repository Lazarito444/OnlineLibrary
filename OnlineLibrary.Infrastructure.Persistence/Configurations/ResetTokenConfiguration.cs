using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibrary.Core.Domain.Entities;

namespace OnlineLibrary.Infrastructure.Persistence.Configurations;

public class ResetTokenConfiguration : IEntityTypeConfiguration<ResetToken>
{
    public void Configure(EntityTypeBuilder<ResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");
        builder.HasKey(rt => rt.Id);

        builder.HasOne(rt => rt.User)
            .WithOne()
            .HasForeignKey<ResetToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}