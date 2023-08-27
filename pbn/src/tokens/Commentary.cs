using System.IO;
using static pbn.tokens.Commentary;

namespace pbn.tokens;


public record class Commentary(Commentary.CommentaryFormat Format, bool StartsOnNewLine, string Content) : SemanticPbnToken
{
    public override string Typename => "Commentary";

    public enum CommentaryFormat
    {
        Singleline,
        Multiline
    }

    public override void Serialize(TextWriter to)
    {
        if (Format == CommentaryFormat.Singleline)
        {
            to.Write(";");
            to.Write(Content);
        }
        else
        {
            to.Write("{");
            to.Write(Content);
            to.Write("}");
        }
    }

}