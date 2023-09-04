using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using pbn.tokens;
using static pbn.tokens.Commentary;

namespace pbn;

///  Class used to parse a pbn file from an input stream.
public class PbnParser
{
    ///  Represents the recovery mode of the parser, what it should do when a lexical or syntactical error is encountered
    /// @see PbnParser::PbnParser(RecoveryMode mode)
    /// Only strict mode is supported at the moment
    public enum RecoveryMode
    {
        /**
     *  Strict mode. If the parser encounters an error, it will throw an exception.
     */
        Strict,

        /**
     *  Relaxed mode, if an error is encountered, the parser will try to recover and parse next tag.
     */
        SkipToNextTag,

        /**
     *  Relaxed mode, if an error is encountered, the parser will try to recover and parse next board.
     * If there is no board context and an error is encountered, parser will try to skip and parse next tag.
     */
        SkipToNextBoard
    }

    public PbnParser() : this(RecoveryMode.Strict, TagFactory.Default())
    {
    }

    public PbnParser(RecoveryMode mode, TagFactory factory)
    {
        if (mode != RecoveryMode.Strict)
            throw new NotImplementedException("Only strict mode is supported at the moment");
        Mode = mode;

        this.tagFactory = factory;
    }

    public RecoveryMode Mode { get; init; }

    private readonly TagFactory tagFactory;
    private readonly char[] whiteSpaceCharacters = " \t\n\v\f\r".ToCharArray();
    private int currentLine;

    private string? Getline(StreamReader inputStream)
    {
        currentLine++;
        return inputStream.ReadLine();
    }

    public List<string> GetTableValues(ref string line, StreamReader inputStream)
    {
        List<string> values = new List<string>();

        if (line.Length == 0 && (line = Getline(inputStream)) == null )
            return values;

        while (!(line.StartsWith("[") || line.Length == 0 ||
                 line.StartsWith(Commentary.SinglelineCommentaryStartSequence)))
        {
            var newValues = line.Split(" ",  StringSplitOptions.RemoveEmptyEntries);
            values.AddRange(newValues);
            line = Getline(inputStream)?.Trim();
            if (line == null)
            {
                line = "";
                break;
            }
        }

        return values;
    }

    public SemanticPbnToken ParseToken(ref string line, StreamReader inputStream, bool startedOnNewLine)
    {
        line = line.TrimStart();
        if (line == "")
            return new EmptyLine();
        if (line.StartsWith(EscapedLine.EscapeSequence))
        {
            var token = EscapedLine.Create(line.Substring(1));
            line = "";
            return token;
        }

        char firstValidCharacter = line[0];
        if (firstValidCharacter == '[')
        {
            var tag = ParseTag(ref line, ref inputStream, startedOnNewLine);
            return tag;
        }
        else if (firstValidCharacter == ';')
        {
            var token = new Commentary(CommentaryFormat.Singleline, true, line[1..]);
            line = "";
            return token;
        }
        else if (firstValidCharacter == '{')
        {
            return ParseMultilineComment(ref line, ref inputStream, startedOnNewLine);
        }
        else
        {
            var token = new TextLine(line);
            line = "";
            return token;
        }
    }

    public Tag ParseTag(ref string line, ref StreamReader inputStream, bool startedOnNewLine)
    {
        Regex regex1 = new Regex(@"(\s*\[\s*(\w+)\s*""(.*)""\s*\]\s*)");
        Match matches = regex1.Match(line);

        if (!matches.Success)
        {
            throw new InvalidOperationException("At line " + currentLine + ": Invalid tag: " + line);
        }

        string tagString = matches.Groups[1].Value;
        string tagName = matches.Groups[2].Value;
        string tagContent = matches.Groups[3].Value;

        line = line.Remove(0, tagString.Length);

        Tag tag;

        if (this.tagFactory.IsTableTag(tagName))
        {
            List<string> tableValues = GetTableValues(ref line, inputStream);
            tag = tagFactory.CreateTableTag(tagName, tagContent, new List<string>(tableValues));
        }
        else
        {
            tag = tagFactory.CreateTag(tagName, tagContent);
        }

        if (line.Trim(whiteSpaceCharacters).Length == 0)
        {
            line = "";
        }

        return tag;
    }

    public Commentary ParseMultilineComment(ref string line, ref StreamReader inputStream, bool startedOnNewLine)
    {
        int start = line.IndexOf('{');
        int lineno = currentLine;

        line = line.Substring(start);
        string content = "";
        while (line.IndexOf('}') == -1)
        {
            content += line + "\n";
            if ((line = Getline(inputStream)) == null)
            {
                throw new InvalidOperationException("Multiline comment not closed at line " + lineno);
            }
        }

        int end = line.IndexOf("}", StringComparison.Ordinal);
        content += line.Substring(0, end);

        line = line.Remove(0, end);

        Commentary token = new Commentary(CommentaryFormat.Multiline, startedOnNewLine, content);

        if (line.Trim(whiteSpaceCharacters).Length == 0)
        {
            line = "";
        }

        return token;
    }

    public PbnFile Parse(StreamReader inputStream)
    {
        PbnFile file = new PbnFile();

        string? line;
        while ((line = inputStream.ReadLine()) != null)
        {
            file.AppendToken(ParseToken(ref line, inputStream, false));
            while (!string.IsNullOrEmpty(line))
            {
                file.AppendToken(ParseToken(ref line, inputStream, false));
            }
        }

        return file;
    }
}