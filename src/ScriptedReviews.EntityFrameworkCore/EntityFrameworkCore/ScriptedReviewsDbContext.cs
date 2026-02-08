using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using ScriptedReviews.Chapters;
using ScriptedReviews.Ratings;
using ScriptedReviews.Seasons;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using ScriptedReviews.Notifications;
using ScriptedReviews.MonitoringLogs;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;


namespace ScriptedReviews.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class ScriptedReviewsDbContext :
    AbpDbContext<ScriptedReviewsDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Serie> Series { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ScriptedReviews.Watchlists.Watchlist> Watchlists { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ApiMonitoringLog> ApiMonitoringLogs { get; set; }
    public DbSet<ErrorLogs.ErrorLog> ErrorLogs { get; set; }

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public ScriptedReviewsDbContext(DbContextOptions<ScriptedReviewsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */
        builder.Entity<Serie>(s =>
        {
            s.ToTable(ScriptedReviewsConsts.DbTablePrefix + "Series",
                ScriptedReviewsConsts.DbSchema);
            s.ConfigureByConvention();  //auto configure for base class props
            s.Property(x => x.Title).IsRequired().HasMaxLength(128);
        });

        builder.Entity<Season>(s =>
        {
            s.ToTable(ScriptedReviewsConsts.DbTablePrefix + "Seasons",
                ScriptedReviewsConsts.DbSchema);
            s.ConfigureByConvention();  //auto configure for base class props
        });

        builder.Entity<Chapter>(s =>
        {
            s.ToTable(ScriptedReviewsConsts.DbTablePrefix + "Chapters",
                ScriptedReviewsConsts.DbSchema);
            s.ConfigureByConvention();  //auto configure for base class props
        });

        builder.Entity<ScriptedReviews.Watchlists.Watchlist>(b =>
        {
            b.ToTable(ScriptedReviewsConsts.DbTablePrefix + "Watchlists",
                ScriptedReviewsConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasMany(w => w.Series)
             .WithMany()
             .UsingEntity(j => j.ToTable("AppWatchListSeries"));
        });

        builder.Entity<Notification>(b =>
        {
            b.ToTable("Notifications");
            b.Property(n => n.Description).IsRequired().HasMaxLength(1024);
            b.Property(n => n.Type).IsRequired().HasMaxLength(128);
            b.Property(n => n.WasRead).HasDefaultValue(false);
        });

        builder.Entity<Rating>(b =>
        {
            b.ToTable("Ratings");
            b.Property(r => r.RatingNumber).IsRequired();
            b.Property(r => r.Comment).HasMaxLength(1000);
            b.HasOne(r => r.Serie)
                .WithMany()
                .HasForeignKey(r => r.SeriesId)
                .OnDelete(DeleteBehavior.Cascade);
            b.ConfigureByConvention();
        });

        builder.Entity<ApiMonitoringLog>(b =>
        {
            b.ToTable("ApiMonitoringLogs");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd(); //  Esto permite que SQL Server genere automáticamente el ID
            b.Property(a => a.Endpoint).IsRequired().HasMaxLength(256);
            b.Property(a => a.HttpMethod).IsRequired().HasMaxLength(10);
            b.Property(a => a.ResponseTime).IsRequired();
            b.Property(a => a.HttpStatusCode).IsRequired();
            b.Property(a => a.UserAgent).HasMaxLength(512);
            b.Property(a => a.IPAddress).HasMaxLength(45);
            b.Property(a => a.CreatedAt).IsRequired();
        });

        builder.Entity<ErrorLogs.ErrorLog>(b =>
        {
            b.ToTable("ErrorLogs");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(e => e.Level).IsRequired().HasMaxLength(20);
            b.Property(e => e.Message).IsRequired().HasMaxLength(4000);
            b.Property(e => e.Exception).HasMaxLength(8000);
            b.Property(e => e.Source).HasMaxLength(256);
            b.Property(e => e.Endpoint).HasMaxLength(256);
            b.Property(e => e.HttpMethod).HasMaxLength(10);
            b.Property(e => e.UserId).HasMaxLength(128);
            b.Property(e => e.IpAddress).HasMaxLength(45);
            b.Property(e => e.CreatedAt).IsRequired();
        });

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(ScriptedReviewsConsts.DbTablePrefix + "YourEntities", ScriptedReviewsConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
