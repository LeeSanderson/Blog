using System.Linq;

namespace BlogToHtml.UnitTests.Generators
{
    internal static class StringExtensions
    {
        public static string RemoveAllWhiteSpace(this string s) => new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }
}
