using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Blog.Api.Helper
{
    public static class SlugGenerator
    {
        public static string GenerateSlug(string title) {
            string lowerTitle = title.ToLowerInvariant();

            string normalized = lowerTitle.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            bool lastWasHyphen = false;

            foreach (char c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                    continue;

                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    lastWasHyphen = false;
                }
                else if (char.IsWhiteSpace(c) || c == '-' || c == '_')
                {
                    if (!lastWasHyphen && sb.Length > 0)
                    {
                        sb.Append('-');
                        lastWasHyphen = true;
                    }
                }
            }

            string slug = sb.ToString().Trim('-');

            return slug;
        }
    }
}
