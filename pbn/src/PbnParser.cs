using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using pbn.tokens;
using static pbn.tokens.Commentary;

namespace pbn;

/// <summary>
///     Class used to parse a pbn file from an input stream.
/// </summary>
public class PbnParser
{
    /// <summary>
    ///     Represents the recovery mode of the parser, what it should do when a lexical or syntactical error is encountered
    ///     Only strict mode is supported at the moment
    /// </summary>
    public enum RecoveryMode
    {
        /// <summary>
        ///     Strict mode. If the parser encounters an error, it will throw an exception.
        /// </summary>
        Strict,


        /// <summary>
        ///     Relaxed mode, if an error is encountered, the parser will try to recover and parse next tag.
        /// </summary>
        SkipToNextTag,

        /// <summary>
        ///     Relaxed mode, if an error is encountered, the parser will try to recover and parse next board.
        ///     If there is no board context and an error is encountered, parser will try to skip and parse next tag.
        /// </summary>
        SkipToNextBoard
    }

    private readonly TagFactory tagFactory;
    private readonly char[] whiteSpaceCharacters = " \t\n\v\f\r".ToCharArray();

    /// <summary>
    ///     Line counter
    /// </summary>
    private int currentLine;

    public PbnParser() : this(RecoveryMode.Strict, TagFactory.MakeDefault())
    {
    }

    public PbnParser(RecoveryMode mode, TagFactory factory)
    {
        if (mode != RecoveryMode.Strict)
            throw new NotImplementedException("Only strict mode is supported at the moment");
        Mode = mode;

        tagFactory = factory;
    }

    public RecoveryMode Mode { get; init; }

    private string? GetLine(StreamReader inputStream)
    {
        currentLine++;
        return inputStream.ReadLine();
    }

    private List<string> GetTableValues(ref string line, StreamReader inputStream)
    {
        var values = new List<string>();


        if (line.Length == 0)
        {
            var newLine = GetLine(inputStream);
            if (newLine == null)
                return values;
            line = newLine;
        }

        while (!(line.StartsWith("[") || line.Length == 0 ||
                 line.StartsWith(SinglelineCommentaryStartSequence)))
        {
            var newValues = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            values.AddRange(newValues);
            var newLine = GetLine(inputStream)?.Trim();
            if (newLine == null)
            {
                line = "";
                break;
            }

            line = newLine;
        }

        return values;
    }

    /// <summary>
    ///     Parses a token from a line. The line is modified to remove the parsed token.
    ///     Possibly reads more lines from the input stream.
    /// </summary>
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

        var firstValidCharacter = line[0];
        if (firstValidCharacter == '[')
        {
            var tag = ParseTag(ref line, ref inputStream, startedOnNewLine);
            return tag;
        }

        if (firstValidCharacter == ';')
        {
            var token = new Commentary(CommentaryFormat.Singleline, true, line[1..]);
            line = "";
            return token;
        }

        if (firstValidCharacter == '{') return ParseMultilineComment(ref line, ref inputStream, startedOnNewLine);

        {
            var token = new TextLine(line);
            line = "";
            return token;
        }
    }

    /// <summary>
    ///     Parses a tag from a line. The line is modified to remove the parsed tag.
    ///     Possibly reads more lines from the input stream.
    /// </summary>
    public Tag ParseTag(ref string line, ref StreamReader inputStream, bool startedOnNewLine)
    {
        var regex1 = new Regex(@"(\s*\[\s*(\w+)\s*""(.*)""\s*\]\s*)");
        var matches = regex1.Match(line);

        if (!matches.Success) throw new InvalidOperationException("At line " + currentLine + ": Invalid tag: " + line);

        var tagString = matches.Groups[1].Value;
        var tagName = matches.Groups[2].Value;
        var tagContent = matches.Groups[3].Value;

        line = line.Remove(0, tagString.Length);

        Tag tag;

        if (tagFactory.IsTableTag(tagName))
        {
            var tableValues = GetTableValues(ref line, inputStream);
            tag = tagFactory.CreateTableTag(tagName, tagContent, new List<string>(tableValues));
        }
        else
        {
            tag = tagFactory.CreateTag(tagName, tagContent);
        }

        if (line.Trim(whiteSpaceCharacters).Length == 0) line = "";

        return tag;
    }

    /// <summary>
    ///     Parses a multiline comment from a line. The line is modified to remove the parsed comment.
    ///     Consumes lines from the input stream until the comment is closed.
    /// </summary>
    private Commentary ParseMultilineComment(ref string line, ref StreamReader inputStream, bool startedOnNewLine)
    {
        var start = line.IndexOf('{');
        var lineno = currentLine;

        line = line.Substring(start);
        var content = "";
        while (line.IndexOf('}') == -1)
        {
            content += line + "\n";
            line = GetLine(inputStream) ??
                   throw new InvalidOperationException("Multiline comment not closed at line " + lineno);
        }

        var end = line.IndexOf("}", StringComparison.Ordinal);
        content += line.Substring(0, end);

        line = line.Remove(0, end);

        var token = new Commentary(CommentaryFormat.Multiline, startedOnNewLine, content);

        if (line.Trim(whiteSpaceCharacters).Length == 0) line = "";

        return token;
    }


    /// <summary>
    ///     Parses a PBN file from an input stream.
    /// </summary>
    public PbnFile Parse(StreamReader inputStream)
    {
        var file = new PbnFile();

        string? line;
        while ((line = inputStream.ReadLine()) != null)
        {
            file.AppendToken(ParseToken(ref line, inputStream, false));
            while (!string.IsNullOrEmpty(line)) file.AppendToken(ParseToken(ref line, inputStream, false));
        }

        return file;
    }
}