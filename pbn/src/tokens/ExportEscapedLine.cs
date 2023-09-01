namespace pbn.tokens
{
    public record ExportEscapedLine : EscapedLine
    {
        public const string ExportLine = EscapedLine.EscapeSequence + " " + "EXPORT";

        public override string Typename => "Export Directive";

        public override void Serialize(System.IO.TextWriter to)
        {
            to.Write(ExportLine);
        }

    }
}