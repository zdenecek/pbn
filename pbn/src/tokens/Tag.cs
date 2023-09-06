using System.IO;

namespace pbn.tokens;

/// <summary>
///     Represents a tag in a PBN file.
///     Example of a tag: '[Name "Value"]'
/// </summary>
/// <param name="Name">Name of the tag</param>
/// <param name="Value">Value of the tag</param>
public record Tag(string Name, string Value) : SemanticPbnToken
{
    public const string TagOpeningDelimiter = "[";
    public const string TagClosingDelimiter = "]";
    public const string ValueDelimiter = "\"";
    public override string Typename => "Tag";

    public override void Serialize(TextWriter to)
    {
        to.Write(TagOpeningDelimiter);
        to.Write(Name);
        to.Write(" ");
        to.Write(ValueDelimiter);
        to.Write(Value);
        to.Write(ValueDelimiter);
        to.Write(TagClosingDelimiter);
    }

    public override string ToString()
    {
        return $"{Name}:{Value}";
    }
}