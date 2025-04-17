namespace Flow.Launcher.Plugin.Need
{
public static class StringHelper
{
    public static string TruncateWithEllipsis(string s, int length)
    {
        if ("...".Length > length)
        {
            return s;
        }
        if (s.Length > length)
        {
            return s.Substring(0, length - "...".Length) + "...";
        }
        return s;
    }
}
}