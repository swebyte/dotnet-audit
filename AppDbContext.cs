using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CaseTypeA> CaseTypeAs => Set<CaseTypeA>();
    public DbSet<CaseTypeB> CaseTypeBs => Set<CaseTypeB>();
    public DbSet<CaseTypeC> CaseTypeCs => Set<CaseTypeC>();
    public DbSet<CaseAudit> CaseAudits => Set<CaseAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Case>()
            .HasOne(c => c.CaseTypeA)
            .WithOne()
            .HasForeignKey<CaseTypeA>(x => x.CaseId);

        modelBuilder.Entity<Case>()
            .HasOne(c => c.CaseTypeB)
            .WithOne()
            .HasForeignKey<CaseTypeB>(x => x.CaseId);

        modelBuilder.Entity<Case>()
            .HasOne(c => c.CaseTypeC)
            .WithOne()
            .HasForeignKey<CaseTypeC>(x => x.CaseId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.AddInterceptors(
            new AuditInterceptor()
        );
    }
}
