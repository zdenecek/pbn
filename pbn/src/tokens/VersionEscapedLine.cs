using System.IO;

namespace pbn.tokens;

/// <summary>
/// Represents the version escaped line directive, e.g. "% PBN 2.1"
/// </summary>
public record VersionEscapedLine : EscapedLine
{
    public const string VersionLinePrefix = " " + "PBN" + " ";

    private readonly string versionString;

    public readonly PbnVersion Version;

    public VersionEscapedLine(string versionString)
    {
        this.versionString = versionString;
        Version = PbnVersionHelper.FromString(versionString);
    }

    public override string Typename => "Version Directive";

    public override void Serialize(TextWriter to)
    {
        to.Write(EscapeSequence);
        to.Write(VersionLinePrefix);
        to.Write(versionString);
    }
}