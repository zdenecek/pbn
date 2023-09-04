using System.IO;

namespace pbn.tokens
{
    public record VersionEscapedLine : EscapedLine
    {

        public const string VersionLinePrefix = " " + "PBN" + " ";

        private string versionString;

        public override string Typename => "Version Directive";

        public VersionEscapedLine(string versionString)
        {
            this.versionString = versionString;
        }

        public override void Serialize(TextWriter to)
        {
            to.Write(EscapedLine.EscapeSequence);
            to.Write(VersionLinePrefix);
            to.Write(this.versionString);
        }
    }

}