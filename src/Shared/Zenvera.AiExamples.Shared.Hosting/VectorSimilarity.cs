namespace Zenvera.AiExamples.Shared.Hosting;

public static class VectorSimilarity
{
    public static float Cosine(ReadOnlySpan<float> left, ReadOnlySpan<float> right)
    {
        if (left.Length != right.Length || left.Length == 0)
        {
            return 0f;
        }

        float dot = 0f;
        float magnitudeLeft = 0f;
        float magnitudeRight = 0f;

        for (var i = 0; i < left.Length; i++)
        {
            dot += left[i] * right[i];
            magnitudeLeft += left[i] * left[i];
            magnitudeRight += right[i] * right[i];
        }

        return magnitudeLeft == 0f || magnitudeRight == 0f
            ? 0f
            : dot / (MathF.Sqrt(magnitudeLeft) * MathF.Sqrt(magnitudeRight));
    }
}
