using System.ComponentModel.DataAnnotations;

namespace SearchingAggregator.Database;

public class SearchResultItemEntity {
	public string Query { get; set; }
	[Key]
	public string Title { get; set; }
	public string Link { get; set; }
	public string Description { get; set; }
	public virtual SearchResultsEntity SearchResultsEntity { get; set; }
}
