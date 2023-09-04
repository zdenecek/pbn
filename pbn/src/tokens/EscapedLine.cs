using System.Text.RegularExpressions;

namespace pbn.tokens;

public abstract record EscapedLine : SemanticPbnToken
{
    public const string EscapeSequence = "%";

    private static readonly Regex VersionRegex = new(@"\d\.\d$");

    public bool IsDirective => this is VersionEscapedLine or ExportEscapedLine;


    public static EscapedLine Create(string contents)
    {
        if (contents == ExportEscapedLine.ExportLine) return new ExportEscapedLine();

        if (IsVersionLine(contents))
            return new VersionEscapedLine(contents.Substring(VersionEscapedLine.VersionLinePrefix.Length));

        return new CustomEscapedLine(contents);
    }

    private static bool IsVersionLine(string line)
    {
        if (!line.StartsWith(VersionEscapedLine.VersionLinePrefix))
            return false;

        var versionCandidate = line.Substring(VersionEscapedLine.VersionLinePrefix.Length);
        return VersionRegex.IsMatch(versionCandidate);
    }
}