namespace Zenvera.AiExamples.Rag.ManualConsoleHost.Retrieval;

public static class CosineSimilarity
{
    public static float Compute(ReadOnlySpan<float> left, ReadOnlySpan<float> right)
        => VectorSimilarity.Cosine(left, right);
}
