using System.IO;

namespace pbn.tokens;

public record TextLine(string Content) : SemanticPbnToken
{
    public readonly string Content = Content;

    public override string Typename => "Unrecognized Text Line";

    public override void Serialize(TextWriter to)
    {
        to.Write(Content);
    }
}