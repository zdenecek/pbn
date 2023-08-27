using System;
using System.IO;

namespace pbn.tokens
{
    public record class EmptyLine : SemanticPbnToken
    {

        public override string Typename => "Empty Line";

        public override void Serialize(TextWriter to)
        {
            // do nothing
        }

    }
}