using System.IO;

namespace pbn.tokens;

public abstract record SemanticPbnToken
{
    public override string ToString()
    {
        using MemoryStream stream = new();
        using StreamWriter writer = new(stream);
        Serialize(writer);
        return stream.ToString() ?? "";
    }

    public abstract void Serialize(TextWriter to);

    public abstract string Typename
    {
        get;
    }

}