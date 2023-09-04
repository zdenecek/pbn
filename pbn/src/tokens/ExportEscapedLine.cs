namespace pbn.tokens
{
    public record ExportEscapedLine : EscapedLine
    {
        public const string ExportLine = " EXPORT";


        public override string Typename => "Export Directive";

        public override void Serialize(System.IO.TextWriter to)
        {
            to.Write(EscapedLine.EscapeSequence);
            to.Write(ExportLine);
        }

    }
}