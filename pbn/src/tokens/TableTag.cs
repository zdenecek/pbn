using System;
using System.Collections.Generic;
using System.Linq;

namespace pbn.tokens;

public record
    TableTag(string Tagname, string Content, IList<string> Values) : Tag(Tagname, Content)
{
    public IList<ColumnInfo> Columns { get; init; } = ParseColumnInfo(Content);

    public record struct ColumnInfo(string Name, ColumnInfo.ColumnOrdering Ordering,
        ColumnInfo.ColumnAlignment Alignment, int AlignmentWidth)
    {
        public const char LeftAlignChar = 'L';
        public const char RightAlignChar = 'R';
        public const char OrderingInfoSeparator = '\\';
        public const char AscendingOrderingChar = '+';
        public const char DescendingOrderingChar = '-';

        public enum ColumnOrdering
        {
            Ascending,
            Descending,
            None
        }

        public enum ColumnAlignment
        {
            Left,
            Right,
            None
        }

        public ColumnInfo() : this("", ColumnOrdering.None, ColumnAlignment.None, 0)
        {
        }
    }


    public static ColumnInfo ParseSingleColumnInfo(string info)
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
                throw new Exception("Invalid column format: " + info);
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
                throw new Exception("Invalid column format: " + info);
            }
        }
        else
        {
            colInfo.Name = str;
        }

        return colInfo;
    }

    private static IList<ColumnInfo> ParseColumnInfo(string tagContent)
    {
        var infos = new List<ColumnInfo>();
        var infoStrs = tagContent.Split(';');
        if (infoStrs.Length == 0)
        {
            throw new FormatException("Invalid column format: " + tagContent);
        }

        foreach (var colInfo in infoStrs)
        {
            infos.Add(ParseSingleColumnInfo(colInfo));
        }

        var orderedColumnsCount = infos.Count(info => info.Ordering != ColumnInfo.ColumnOrdering.None);
        if (orderedColumnsCount > 1)
            throw new FormatException("Only one column can be ordered: " + tagContent);

        return infos;
    }
}