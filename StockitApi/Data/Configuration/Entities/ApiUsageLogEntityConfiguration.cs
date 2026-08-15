using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockitApi.Entities;

namespace StockitApi.Data.Configuration.Entities;

public class ApiUsageLogEntityConfiguration : IEntityTypeConfiguration<ApiUsageLog>
{
    public void Configure(EntityTypeBuilder<ApiUsageLog> builder)
    {
        builder.ToTable("ApiUsageLog");

        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(x => x.Endpoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.HttpMethod)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => x.UserId, "IX_ApiUsageLog_UserId");
        builder.HasIndex(x => x.CreatedAt, "IX_ApiUsageLog_CreatedAt");

        builder.HasOne(x => x.User)
            .WithMany(u => u.ApiUsageLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
