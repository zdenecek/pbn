using System.IO;

namespace pbn.tokens;

/// <summary>
/// Represents an escaped line without a special meaning.
/// </summary>
public record CustomEscapedLine(string Content) : EscapedLine
{
    public override string Typename => "Escaped Line";

    public override void Serialize(TextWriter to)
    {
        to.Write(EscapeSequence);
        to.Write(Content);
    }
}