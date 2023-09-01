using System.IO;

namespace pbn.tokens
{
    public record VersionEscapedLine : EscapedLine
    {

        public const string VersionLinePrefix = EscapeSequence + " " + "PBN" + " ";

        private string versionString;

        public override string Typename => "Version Directive";

        public VersionEscapedLine(string versionString)
        {
            this.versionString = versionString;
        }

        public override void Serialize(TextWriter to)
        {
            to.Write(VersionLinePrefix);
            to.Write(this.versionString);
        }
    }

}