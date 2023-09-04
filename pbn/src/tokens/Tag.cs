using System.IO;

namespace pbn.tokens
{
    public record Tag(string Name, string Value) : SemanticPbnToken
    {
        public override string Typename => "Tag";
        public const string TagOpeningDelimiter = "[";
        public const string TagClosingDelimiter = "]";
        public const string ValueDelimiter = "\"";

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
}
}