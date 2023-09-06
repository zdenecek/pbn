using System.Linq;
using pbn.model;

namespace pbn.manipulators;

public enum RenumberMode
{
    /// <summary>
    ///    Shift board numbers by a number
    /// </summary>
    ShiftNumbers,

    /// <summary>
    ///     Renumber boards starting from a number
    /// </summary>
    AssignNewNumbers
}

/// <summary>
/// A record representing options for renumbering boards
/// </summary>
/// <param name="Mode">Mode of operation</param>
/// <param name="Number">Start number if mode is <see cref="RenumberMode.AssignNewNumbers"/>, shift constant if mode
/// is <see cref="RenumberMode.ShiftNumbers"/></param>
public record struct RenumberOptions(RenumberMode Mode, int Number)
{
    public static RenumberOptions Parse(string str)
    {
        var result = new RenumberOptions(RenumberMode.AssignNewNumbers, 1);

        if (str.Length == 0)
            return result;

        if (str[0] == '+' || str[0] == '-')
            result.Mode = RenumberMode.ShiftNumbers;

        return result with { Number = int.Parse(str) };
    }
}

public class PbnBoardManipulator
{

    /// <summary>
    /// Removes boards from a file
    /// </summary>
    public void RemoveBoards(PbnFile file, NumericRange rangeToRemove)
    {
         file.Boards
            .Where((b) => b.BoardNumber is not null && rangeToRemove.IsInRange(b.BoardNumber.Value))
            .ToList() // need to copy the list because we are modifying it
            .ForEach(file.RemoveBoardContext);
    }
    

    /// <summary>
    /// Renumbers boards in a file given options
    /// </summary>
    public void RenumberBoards(PbnFile file, RenumberOptions options)
    {
        switch (options.Mode)
        {
            case RenumberMode.ShiftNumbers:
            {
                foreach (var board in file.Boards)
                {
                    board.BoardNumber += options.Number;
                }

                break;
            }
            case RenumberMode.AssignNewNumbers:
            {
                var i = options.Number;
                foreach (var board in file.Boards)
                {
                    board.BoardNumber = i++;
                }

                break;
            }
        }
    }
}