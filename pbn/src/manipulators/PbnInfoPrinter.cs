using System.IO;
using System.Linq;
using pbn.tokens;

namespace pbn.manipulators;

/// <summary>
/// Used to execute the --info command.
/// </summary>
class PbnInfoPrinter
{

    /// <summary>
    /// Prints the overview of the file to the given stream.
    /// </summary>
    public static void PrintOverview(string filename, PbnFile file, TextWriter outStream)
    {
        outStream.WriteLine($"File: {filename}");
        outStream.Write("Boards: ");
        if (file.Boards.Count == 0)
            outStream.WriteLine("None");
        else
        {
            outStream.Write($"{file.Boards[0].BoardNumber} - {file.Boards.Last().BoardNumber} ");
            outStream.WriteLine($"({file.Boards.Count} boards)");
        }

        outStream.WriteLine("Analyses: No");

        var generator = GetGeneratorInfo(file);
        if (!string.IsNullOrEmpty(generator))
            outStream.WriteLine($"Generated by: {generator}");
    }

    /// <summary>
    /// Tries to find generator info encoded in the Generator tag. 
    /// </summary>
    /// <returns>Contents of the generator tag, null if no such tag is found</returns>
    public static string GetGeneratorInfo(PbnFile file)
    {
        var token = file.Tokens.FirstOrDefault(t =>
        {
            if (t is Tag tag && tag.Tagname == "Generator")
                return true;
            return false;
        });

        if (token == null)
            return "";

        return (token as Tag)?.Content ?? null;
    }
}