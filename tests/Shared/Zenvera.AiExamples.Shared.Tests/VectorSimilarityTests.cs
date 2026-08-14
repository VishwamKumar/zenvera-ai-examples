namespace Zenvera.AiExamples.Shared.Tests;

public sealed class VectorSimilarityTests
{
    [Fact]
    public void Identical_vectors_score_one()
    {
        float[] vector = [1f, 0f, 0f];
        VectorSimilarity.Cosine(vector, vector).Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void Orthogonal_vectors_score_zero()
    {
        float[] left = [1f, 0f];
        float[] right = [0f, 1f];
        VectorSimilarity.Cosine(left, right).Should().BeApproximately(0f, 0.0001f);
    }
}
