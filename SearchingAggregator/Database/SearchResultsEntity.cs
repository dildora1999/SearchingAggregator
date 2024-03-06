using System.ComponentModel.DataAnnotations;

namespace SearchingAggregator.Database;

public class SearchResultsEntity {
	[Key]
	public string Query { get; set; }
	public DateTime CreationDate { get; set; }
	public virtual List<SearchResultItemEntity> SearchResultItemEntities { get; set; }
}
