using System.IO;

namespace pbn.tokens;

/// <summary>
///     Represents a line of text which is not recognized as any other PBN token.
/// </summary>
public record TextLine(string Content) : SemanticPbnToken
{
    public readonly string Content = Content;

    public override string Typename => "Unrecognized Text Line";

    public override void Serialize(TextWriter to)
    {
        to.Write(Content);
    }
}