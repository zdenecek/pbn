using System;
using System.IO;
using System.Linq;
using pbn;

public class DebugUtils
{
    /// <summary>
    /// Serializes a PbnFile to an output stream including token type names.
    /// </summary>
    /// <param name="file">File to serialize.</param>
    /// <param name="outStream">Stream to serialize the file to.</param>
    public static void SerializePbnFile(PbnFile file, TextWriter outStream)
    {
        var i = 0;
        foreach (var token in file.Tokens())
        {
            outStream.Write($"{i,4}");
            outStream.Write(": ");
            outStream.Write($"{("<" + token.Typename + ">"),-25}");

            token.Serialize(outStream);
            outStream.WriteLine();
            i++;
        }
    }

    /// <summary>
    /// Prints file BoardContext ranges to the stream.
    /// </summary>
    /// <param name="file">File to print BoardContext ranges from.</param>
    /// <param name="outStream">Stream to print to.</param>
    public static void PrintBoardContextRanges(PbnFile file, TextWriter outStream)
    {
        Console.WriteLine("Board Contexts:");

        foreach (var context in file.BoardContexts)
        {
            outStream.Write("Board ");
            outStream.Write(context.BoardNumber);
            outStream.Write(": ");

            var range = file.BoardContextIdToTokenRange[context.Id];
            outStream.Write($"[{range.StartIndex}, {range.StartIndex + range.TokenCount - 1}]");
            outStream.WriteLine();
        }
    }


    public static void PrintAnalysisTable(AnalysisTable table, TextWriter outStream)
    {

        outStream.WriteLine("    NT   ♠   ♥   ♦   ♣");
        foreach (var position in Enum.GetValues(typeof(Position)).Cast<Position>())

        {

            outStream.Write($" {PositionHelpers.ToString(position)[0]}");

            foreach (var suit in Enum.GetValues(typeof(Suit)).Cast<Suit>())
            {
                outStream.Write($"{table.GetDoubleDummyTricks(suit, position),4}");
            }
            outStream.WriteLine();
        }

        
    }


    public static void Playground()
    {
        var table = new AnalysisServiceDDS().AnalyzePbn("N:A873.AJ83.52.Q32 95.74.AQJ84.T754 QT62.KQ62.K93.K8 KJ4.T95.T76.AJ96",
            Vulnerability.NONE);

        PrintAnalysisTable(table, Console.Out);
    }
}