namespace ContactManager.Utilities
{
    public class Levenshtein
    {
        public static float PercentDiff(string? source1, string? source2)
        {
            if (source1 == null || source2 == null) return 0f;
            int maxDiff = Math.Max(source1.Length, source2.Length);
            if (maxDiff == 0) return 100f;  // Both strings are empty — identical
            return 100 * (maxDiff - Quickenshtein.Levenshtein.GetDistance(source1, source2)) / maxDiff;
        }
    }
}
