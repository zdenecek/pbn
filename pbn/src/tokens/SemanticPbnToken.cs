using System.IO;

namespace pbn.tokens;

/// <summary>
/// Represents a semantic token in a PBN file.
/// </summary>
public abstract record SemanticPbnToken
{
    /// <summary>
    /// A string that identifies the type of this token.
    /// Artificially introduced identifier.
    /// </summary>
    public abstract string Typename { get; }
    
    /// <summary>
    /// Line number in the original file, if parsed from a file.
    /// </summary>
    public int? OriginLine { get; internal set; }

    public PbnFile.BoardContext? OwningBoardContext { get; internal set; }

    public override string ToString()
    {
        using MemoryStream stream = new();
        using StreamWriter writer = new(stream);
        Serialize(writer);
        return stream.ToString() ?? "";
    }

    public abstract void Serialize(TextWriter to);
}