using System;
using System.Text.RegularExpressions;

namespace pbn.tokens
{
    public abstract record class EscapedLine : SemanticPbnToken
    {
        public const string EscapeSequence = "%";


        public static EscapedLine Create(string contents)
        {
            if (contents == ExportLine)
            {
                return new ExportEscapedLine();
            }
            if (IsVersionLine(contents))
            {
                return new VersionEscapedLine(contents.Substring(VersionEscapedLine.VersionLinePrefix.Length));
            }

            return new CustomEscapedLine(contents);
        }

        private static bool IsVersionLine(string line)
        {
            if (!line.StartsWith(VersionEscapedLine.VersionLinePrefix))
            {
                return false;
            }
            else
            {
                return VersionRegex.IsMatch(line);
            }
        }

        public bool IsDirective => this is VersionEscapedLine or ExportEscapedLine;

        private static readonly Regex VersionRegex = new Regex(VersionEscapedLine.VersionLinePrefix + @"\d\.\d$");
        private const string ExportLine = " Export";
    }

}