namespace ContactManager.Utilities
{
    public class Levenshtein
    {
        public static float PercentDiff(string source1, string source2)
        {
            int maxDiff = Math.Max(source1.Length, source2.Length);
            return 100*(maxDiff - Quickenshtein.Levenshtein.GetDistance(source1, source2))/maxDiff;
        }
    }
}
