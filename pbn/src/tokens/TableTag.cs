using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static pbn.tokens.TableTag;
using static pbn.tokens.TableTag.ColumnInfo;

namespace pbn.tokens;



public record class TableTag : Tag
{
    public IList<ColumnInfo> Columns { get; init; }
    public IList<string> Values { get; init; }

    public TableTag(string Tagname, string Content, IList<string> àows) : base(Tagname, Content)
    {
        this.Columns = parseColumnInfo(Content);
    }

    public record struct ColumnInfo(string Name, ColumnInfo.ColumnOrdering Ordering, ColumnInfo.ColumnAlignment Alignment, int AlignmentWidth)
    {
        public const char LEFT_ALIGN_CHAR = 'L';
    public const char RIGHT_ALIGN_CHAR = 'R';
    public const char ORDERING_INFO_SEPARATOR = '\\';
    public const char ASCENDING_ORDERING_CHAR = '+';
    public const char DESCENDING_ORDERING_CHAR = '-';
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

    public ColumnInfo() : this("", ColumnOrdering.None, ColumnAlignment.None, 0) { }
}


public static ColumnInfo ParseSingleColumnInfo(string info)
{
    var str = info;
    var col_info = new ColumnInfo();

    if (str[0] == ColumnInfo.ASCENDING_ORDERING_CHAR || str[0] == ColumnInfo.DESCENDING_ORDERING_CHAR)
    {
        col_info.Ordering = str[0] == ColumnInfo.ASCENDING_ORDERING_CHAR ? ColumnInfo.ColumnOrdering.Ascending : ColumnInfo.ColumnOrdering.Descending;
        str = str[1..];
    }

    if (str.Contains(ColumnInfo.ORDERING_INFO_SEPARATOR))
    {
        var parts = str.Split(ColumnInfo.ORDERING_INFO_SEPARATOR);
        if (parts.Length != 2)
            throw new Exception("Invalid column format: " + info);
        col_info.Name = parts[0];
        if (parts[1].EndsWith(ColumnInfo.RIGHT_ALIGN_CHAR) || parts[1].EndsWith(ColumnInfo.LEFT_ALIGN_CHAR))
        {
            col_info.Alignment = parts[1].EndsWith(ColumnInfo.LEFT_ALIGN_CHAR) ? ColumnInfo.ColumnAlignment.Left : ColumnInfo.ColumnAlignment.Right;
            parts[1] = parts[1].Remove(parts[1].Length - 1);
        }
        try
        {
            col_info.AlignmentWidth = int.Parse(parts[1]);
        }
        catch (FormatException)
        {
            throw new Exception("Invalid column format: " + info);
        }
    }
    else
    {
        col_info.Name = str;
    }

    return col_info;
}

private static IList<ColumnInfo> parseColumnInfo(string tagContent)
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