using System.IO;

namespace pbn.tokens
{
    public record Tag(string Tagname, string Content) : SemanticPbnToken
    {
        public override string Typename => "Tag";

    public override void Serialize(TextWriter to)
    {
        to.Write("[");
        to.Write(Tagname);
        to.Write(" \"");
        to.Write(Content);
        to.Write("\"]");
    }
}
}