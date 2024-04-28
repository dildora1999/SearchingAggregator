using Microsoft.EntityFrameworkCore;

namespace SearchingAggregator.Database;

public class SearchResultsDbContext(DbContextOptions<SearchResultsDbContext> options) : DbContext {
	public DbSet<SearchResultsEntity> SearchResultsEntities { get; set; }
	public DbSet<SearchResultItemEntity> SearchResultItems { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		if (!optionsBuilder.IsConfigured) {
			optionsBuilder.UseSqlServer("Server=localhost;Database=SearchResultsDatabase;TrustServerCertificate=True");
		}
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<SearchResultsEntity>(entity => {
			entity.HasKey(e => e.Query);
		});

		modelBuilder.Entity<SearchResultItemEntity>(entity => {
			entity.HasKey(e => e.Title);
			entity.HasOne(d => d.SearchResultsEntity)
				.WithMany(p => p.SearchResultItemEntities);
		});
	}
}
