using System.IO;

namespace pbn.tokens;

public record ExportEscapedLine : EscapedLine
{
    public const string ExportLine = " EXPORT";


    public override string Typename => "Export Directive";

    public override void Serialize(TextWriter to)
    {
        to.Write(EscapeSequence);
        to.Write(ExportLine);
    }
}