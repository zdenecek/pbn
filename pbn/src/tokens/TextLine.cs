using System;
using System.IO;

namespace pbn.tokens
{
    public record class TextLine : SemanticPbnToken
    {
        protected string content;

        public override string Typename => "Unrecognized Text Line";

        public TextLine(string content)
        {
            this.content = content;
        }

        public override void Serialize(TextWriter to)
        {
            to.Write(content);
        }

    }
}