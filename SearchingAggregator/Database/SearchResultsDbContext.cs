using Microsoft.EntityFrameworkCore;

namespace SearchingAggregator.Database;

public class SearchResultsDbContext(DbContextOptions<SearchResultsDbContext> options) : DbContext {
	public DbSet<SearchResultsEntity> SearchResultsEntities { get; set; }
	public DbSet<SearchResultItemEntity> SearchResultItems { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		if (!optionsBuilder.IsConfigured) {
			optionsBuilder.UseSqlServer(@"Server=.;Database=SearchResultsDbContext;Trusted_Connection=True;");
		}
	}
}
