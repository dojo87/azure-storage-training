using Azure;
using Azure.Data.Tables;

namespace MovieApp.ViewModel
{
    public class MovieModel : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Image { get; set; }
        public string Title { get; set; } 
        public double Rank { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class MoviesModel
    {
        public MovieModel[] Movies { get; set; } 
        public string ContinuationToken { get; set; }
    }
}
