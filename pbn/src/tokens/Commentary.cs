using System.IO;

namespace pbn.tokens;


/// <summary>
/// Represents a commentary in a PBN file.
/// </summary>
public record Commentary : SemanticPbnToken
{
    public enum CommentaryFormat
    {
        /// <summary>
        /// A single line commentary, i.e. beginning with a semicolon.
        /// </summary>
        Singleline,
        /// <summary>
        /// A multiline commentary, i.e. delimited with curly braces.
        /// </summary>
        Multiline
    }

    public const string SinglelineCommentaryStartSequence = ";";
    public const string MultilineCommentaryStartSequence = "{";
    public const string MultilineCommentaryEndSequence = "}";
    public override string Typename => "Commentary";
    public CommentaryFormat Format { get; init; }
    
    /// <summary>
    /// True if the commentary starts on a new line.
    /// False if the commentary starts right after the previous token.
    /// </summary>
    public bool StartsOnNewLine { get; init; }
    public string Content { get; init; }

    public Commentary(string content)
    {
        Format = content.Contains('\n') ? CommentaryFormat.Multiline : CommentaryFormat.Singleline;
        StartsOnNewLine = true;
        Content = content;
    }

    public Commentary(CommentaryFormat Format, bool StartsOnNewLine, string Content)
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
}