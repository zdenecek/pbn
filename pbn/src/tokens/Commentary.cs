using System.IO;

namespace pbn.tokens;

public record Commentary : SemanticPbnToken
{
    public enum CommentaryFormat
    {
        Singleline,
        Multiline
    }

    public const string SinglelineCommentaryStartSequence = ";";
    public const string MultilineCommentaryStartSequence = "{";
    public const string MultilineCommentaryEndSequence = "}";
    public override string Typename => "Commentary";
    public Commentary.CommentaryFormat Format { get; init; }
    public bool StartsOnNewLine { get; init; }
    public string Content { get; init; }

    public Commentary(string content)
    {
        Format = content.Contains('\n') ? CommentaryFormat.Multiline : CommentaryFormat.Singleline;
        StartsOnNewLine = true;
        Content = content;
    }

    public Commentary(Commentary.CommentaryFormat Format, bool StartsOnNewLine, string Content)
    {
        this.Format = Format;
        this.StartsOnNewLine = StartsOnNewLine;
        this.Content = Content;
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

    public void Deconstruct(out Commentary.CommentaryFormat Format, out bool StartsOnNewLine, out string Content)
    {
        Format = this.Format;
        StartsOnNewLine = this.StartsOnNewLine;
        Content = this.Content;
    }
}