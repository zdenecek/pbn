using System.IO;

namespace pbn.tokens;


public record Commentary(Commentary.CommentaryFormat Format, bool StartsOnNewLine, string Content) : SemanticPbnToken
{
    public override string Typename => "Commentary";

    public const string SinglelineCommentaryStartSequence = ";";
    public const string MultilineCommentaryStartSequence = "{";
    public const string MultilineCommentaryEndSequence = "}";

    public enum CommentaryFormat
    {
        Singleline,
        Multiline
    }

    public override void Serialize(TextWriter to)
    {
        if (Format == CommentaryFormat.Singleline)
        {
            to.Write(SinglelineCommentaryStartSequence);
            to.Write(Content);
        }
        else
        {
            to.Write(MultilineCommentaryStartSequence);
            to.Write(Content);
            to.Write(MultilineCommentaryEndSequence);
        }
    }

}