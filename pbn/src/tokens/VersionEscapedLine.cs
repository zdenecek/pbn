using System.IO;

namespace pbn.tokens;

public record VersionEscapedLine : EscapedLine
{
    public const string VersionLinePrefix = " " + "PBN" + " ";

    private readonly string versionString;

    public VersionEscapedLine(string versionString)
    {
        this.versionString = versionString;
    }

    public override string Typename => "Version Directive";

    public override void Serialize(TextWriter to)
    {
        to.Write(EscapeSequence);
        to.Write(VersionLinePrefix);
        to.Write(versionString);
    }
}