using System.ComponentModel.DataAnnotations;

namespace SearchingAggregator.Database.Entities;

public class SearchResultItemEntity {
	[Key]
	public string Title { get; set; }
	public string Link { get; set; }
	public string Description { get; set; }
	public virtual SearchResultsEntity SearchResultsEntity { get; set; }
}
