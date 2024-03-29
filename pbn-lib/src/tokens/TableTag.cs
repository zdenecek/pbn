using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pbn.tokens;

/// <summary>
///     Represents a table tag in a PBN file, ie. tag with values.
/// </summary>
public record
    TableTag(string Name, string Value, IList<string> Values) : Tag(Name, Value)
{
    /// <summary>
    ///     Column information for this table.
    /// </summary>
    public IList<ColumnInfo> Columns { get; init; } = ParseColumnInfo(Value);

    public override string Typename => "Table Tag";

    /// <summary>
    ///     Parses a single column information from a string.
    /// </summary>
    private static ColumnInfo ParseSingleColumnInfo(string info)
    {
        var str = info;
        var colInfo = new ColumnInfo();

        if (str[0] == ColumnInfo.AscendingOrderingChar || str[0] == ColumnInfo.DescendingOrderingChar)
        {
            colInfo.Ordering = str[0] == ColumnInfo.AscendingOrderingChar
                ? ColumnInfo.ColumnOrdering.Ascending
                : ColumnInfo.ColumnOrdering.Descending;
            str = str[1..];
        }

        if (str.Contains(ColumnInfo.OrderingInfoSeparator))
        {
            var parts = str.Split(ColumnInfo.OrderingInfoSeparator);
            if (parts.Length != 2)
                throw new FormatException("Invalid column format: " + info);
            colInfo.Name = parts[0];
            if (parts[1].EndsWith(ColumnInfo.RightAlignChar) || parts[1].EndsWith(ColumnInfo.LeftAlignChar))
            {
                colInfo.Alignment = parts[1].EndsWith(ColumnInfo.LeftAlignChar)
                    ? ColumnInfo.ColumnAlignment.Left
                    : ColumnInfo.ColumnAlignment.Right;
                parts[1] = parts[1].Remove(parts[1].Length - 1);
            }

            try
            {
                colInfo.AlignmentWidth = int.Parse(parts[1]);
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid column format: " + info);
            }
        }
        else
        {
            colInfo.Name = str;
        }

        return colInfo;
    }

    /// <summary>
    ///     Parses column information from a string.
    /// </summary>
    private static IList<ColumnInfo> ParseColumnInfo(string tagContent)
    {
        var infos = new List<ColumnInfo>();
        var infoStrs = tagContent.Split(';');
        if (infoStrs.Length == 0) throw new FormatException("Invalid column format: " + tagContent);

        foreach (var colInfo in infoStrs) infos.Add(ParseSingleColumnInfo(colInfo));

        var orderedColumnsCount = infos.Count(info => info.Ordering != ColumnInfo.ColumnOrdering.None);
        if (orderedColumnsCount > 1)
            throw new FormatException("Only one column can be ordered: " + tagContent);

        return infos;
    }


    public override void Serialize(TextWriter to)
    {
        base.Serialize(to);
        to.Write("\n");
        foreach (var row in Values.Chunk(Columns.Count))
        {
            var first = true;
            foreach (var (item, col) in row.Zip(Columns))
            {
                if (!first)
                    to.Write(' ');
                else
                    first = false;
                to.Write(
                    col.Alignment switch
                    {
                        ColumnInfo.ColumnAlignment.None => item,
                        ColumnInfo.ColumnAlignment.Left => item.PadLeft(col.AlignmentWidth),
                        ColumnInfo.ColumnAlignment.Right => item.PadRight(col.AlignmentWidth),
                        _ => throw new ArgumentOutOfRangeException()
                    }
                );
            }

            to.Write("\n");
        }
    }

    /// <summary>
    ///     Represents column information for a table tag.
    /// </summary>
    public record struct ColumnInfo(string Name, ColumnInfo.ColumnOrdering Ordering,
        ColumnInfo.ColumnAlignment Alignment, int AlignmentWidth)
    {
        public enum ColumnAlignment
        {
            Left,
            Right,
            None
        }

        public enum ColumnOrdering
        {
            Ascending,
            Descending,
            None
        }

        public const char LeftAlignChar = 'L';
        public const char RightAlignChar = 'R';
        public const char OrderingInfoSeparator = '\\';
        public const char AscendingOrderingChar = '+';
        public const char DescendingOrderingChar = '-';

        public ColumnInfo() : this("", ColumnOrdering.None, ColumnAlignment.None, 0)
        {
        }
    }
}