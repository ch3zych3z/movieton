namespace MovieTon.Database;

public class DbSimilarity(int similarToId, int similarFromId, double confidence)
{
    public int Id { get; set; }

    public int SimilarToId { get; set; } = similarToId;
    public DbMovie SimilarTo { get; set; } = null!;

    public int SimilarFromId { get; set; } = similarFromId;
    public DbMovie SimilarFrom { get; set; } = null!;

    public double Confidence { get; set; } = confidence;
}
