using System;
using System.Collections.Generic;
using EV.DataConsumerService.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EV.DataConsumerService.API.Data;

public partial class EvdataAnalyticsMarketplaceDbContext : DbContext
{
    public EvdataAnalyticsMarketplaceDbContext()
    {
    }

    public EvdataAnalyticsMarketplaceDbContext(DbContextOptions<EvdataAnalyticsMarketplaceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessLog> AccessLogs { get; set; }

    public virtual DbSet<AccessPolicy> AccessPolicies { get; set; }

    public virtual DbSet<Analysis> Analyses { get; set; }

    public virtual DbSet<AnonymizationLog> AnonymizationLogs { get; set; }

    public virtual DbSet<ApiKey> ApiKeys { get; set; }

    public virtual DbSet<Consumer> Consumers { get; set; }

    public virtual DbSet<Dataset> Datasets { get; set; }

    public virtual DbSet<DatasetFile> DatasetFiles { get; set; }

    public virtual DbSet<DatasetVersion> DatasetVersions { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<RevenueShare> RevenueShares { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-BRF7Q691;Database=EVDataAnalyticsMarketplaceDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.HasKey(e => e.AccessLogId).HasName("PK__AccessLo__9ABDB71F0E9BB2D7");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.ActionAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IpAddress).HasMaxLength(100);
        });

        modelBuilder.Entity<AccessPolicy>(entity =>
        {
            entity.HasKey(e => e.AccessPolicyId).HasName("PK__AccessPo__A9497497E35F24C8");

            entity.Property(e => e.AllowedUse).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Analysis>(entity =>
        {
            entity.HasKey(e => e.AnalysisId).HasName("PK__Analyses__5B789DC867DD5721");

            entity.Property(e => e.AnalysisId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ReportUri).HasMaxLength(2000);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Visibility)
                .HasMaxLength(50)
                .HasDefaultValue("private");

            entity.HasOne(d => d.DatasetVersion).WithMany(p => p.Analyses)
                .HasForeignKey(d => d.DatasetVersionId)
                .HasConstraintName("FK_Analyses_Version");
        });

        modelBuilder.Entity<AnonymizationLog>(entity =>
        {
            entity.HasKey(e => e.AnonymizationId).HasName("PK__Anonymiz__B677CA8A55A11C91");

            entity.Property(e => e.AnonymizationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Method).HasMaxLength(200);
            entity.Property(e => e.PerformedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.DatasetVersion).WithMany(p => p.AnonymizationLogs)
                .HasForeignKey(d => d.DatasetVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Anonymization_Version");
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.ApiKeyId).HasName("PK__ApiKeys__2F1344F23BA25038");

            entity.Property(e => e.ApiKeyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.Organization).WithMany(p => p.ApiKeys)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apikeys_Org");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE9BAACDC0F3B");

            entity.HasIndex(e => e.OrganizationId, "UQ__Consumer__CADB0B132E352779").IsUnique();

            entity.Property(e => e.ConsumerId).ValueGeneratedNever();
            entity.Property(e => e.ContactEmail).HasMaxLength(320);

            entity.HasOne(d => d.Organization).WithOne(p => p.Consumer)
                .HasForeignKey<Consumer>(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumers_Org");
        });

        modelBuilder.Entity<Dataset>(entity =>
        {
            entity.HasKey(e => e.DatasetId).HasName("PK__Datasets__CCE574CB61186A96");

            entity.HasIndex(e => e.Category, "IX_Datasets_Category");

            entity.HasIndex(e => e.ProviderId, "IX_Datasets_ProviderId");

            entity.Property(e => e.DatasetId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BatteryTypes).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DataTypes).HasMaxLength(250);
            entity.Property(e => e.LicenseType)
                .HasMaxLength(100)
                .HasDefaultValue("research-only");
            entity.Property(e => e.Region).HasMaxLength(200);
            entity.Property(e => e.ShortDescription).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("pending");
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.VehicleTypes).HasMaxLength(200);
            entity.Property(e => e.Visibility)
                .HasMaxLength(50)
                .HasDefaultValue("private");

            entity.HasOne(d => d.Provider).WithMany(p => p.Datasets)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Datasets_Provider");
        });

        modelBuilder.Entity<DatasetFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__DatasetF__6F0F98BF1582ABF1");

            entity.Property(e => e.FileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Checksum).HasMaxLength(200);
            entity.Property(e => e.FileName).HasMaxLength(1000);
            entity.Property(e => e.FileUri).HasMaxLength(2000);

            entity.HasOne(d => d.DatasetVersion).WithMany(p => p.DatasetFiles)
                .HasForeignKey(d => d.DatasetVersionId)
                .HasConstraintName("FK_DatasetFiles_Version");
        });

        modelBuilder.Entity<DatasetVersion>(entity =>
        {
            entity.HasKey(e => e.DatasetVersionId).HasName("PK__DatasetV__85A7FBEC00CF8A26");

            entity.HasIndex(e => e.DatasetId, "IX_DatasetVersions_DatasetId");

            entity.Property(e => e.DatasetVersionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AnalysisReportUri).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FileFormat).HasMaxLength(50);
            entity.Property(e => e.PricePerDownload).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PricePerGb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("PricePerGB");
            entity.Property(e => e.SampleUri).HasMaxLength(2000);
            entity.Property(e => e.StorageUri).HasMaxLength(2000);
            entity.Property(e => e.VersionLabel).HasMaxLength(100);

            entity.HasOne(d => d.Dataset).WithMany(p => p.DatasetVersions)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatasetVersions_Dataset");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__CADB0B12934AA791");

            entity.Property(e => e.OrganizationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.OrgType).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A386850FECE");

            entity.Property(e => e.PaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");
            entity.Property(e => e.MarketplaceFee).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaidToProvider).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PaymentGateway).HasMaxLength(100);
            entity.Property(e => e.TransactionReference).HasMaxLength(200);
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK__Provider__B54C687D8848F97A");

            entity.HasIndex(e => e.OrganizationId, "UQ__Provider__CADB0B13160342F0").IsUnique();

            entity.Property(e => e.ProviderId).ValueGeneratedNever();
            entity.Property(e => e.ContactEmail).HasMaxLength(320);

            entity.HasOne(d => d.Organization).WithOne(p => p.Provider)
                .HasForeignKey<Provider>(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Providers_Org");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBE7A4AE6C6");

            entity.HasIndex(e => e.ConsumerOrgId, "IX_Purchases_ConsumerOrgId");

            entity.Property(e => e.PurchaseId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PurchasedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.TransactionId).HasMaxLength(200);

            entity.HasOne(d => d.ConsumerOrg).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.ConsumerOrgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_ConsumerOrg");

            entity.HasOne(d => d.DatasetVersion).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.DatasetVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Version");
        });

        modelBuilder.Entity<RevenueShare>(entity =>
        {
            entity.HasKey(e => e.RevenueShareId).HasName("PK__RevenueS__1DDE0DE306D335F1");

            entity.Property(e => e.RevenueShareId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");

            entity.HasOne(d => d.Payment).WithMany(p => p.RevenueShares)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RevenueShares_Payment");

            entity.HasOne(d => d.ToOrganization).WithMany(p => p.RevenueShares)
                .HasForeignKey(d => d.ToOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RevenueShares_Org");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A27612AA7");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6B0986F6C").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B249D87D99F08");

            entity.Property(e => e.SubscriptionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");
            entity.Property(e => e.RecurringPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.ConsumerOrg).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.ConsumerOrgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_ConsumerOrg");

            entity.HasOne(d => d.Dataset).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscriptions_Dataset");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC261E883");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534AA44BFEB").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRoles_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoles_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD7DD3519F");
                        j.ToTable("UserRoles");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
