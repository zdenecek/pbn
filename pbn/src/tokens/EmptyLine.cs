using System.IO;

namespace pbn.tokens;

/// <summary>
///     Represents an empty line in a PBN file.
/// </summary>
public record EmptyLine : SemanticPbnToken
{
    public override string Typename => "Empty Line";

    public override void Serialize(TextWriter to)
    {
        // do nothing
    }
}