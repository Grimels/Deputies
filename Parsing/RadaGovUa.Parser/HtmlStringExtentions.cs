namespace RadaGovUa.Parser
{
    public static class HtmlStringExtentions
    {
        public static string ClearHtml(this string s)
        {
            var result = s.Replace("&nbsp;", "").Trim(' ', '\t', '\n', '\r');
            return result.Length > 0 ? result : null;
        }
    }
}