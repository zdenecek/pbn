using System;
using System.Linq;

namespace pbn.model;

public class AnalysisTable
{
    private readonly int[,] ddTable;
    
    public Contract MinimaxContract { get; init; }
    public int MinimaxScore { get; init; }

    /// <summary>
    ///     Creates a new AnalysisTable from a double dummy table.
    /// </summary>
    /// <param name="ddTable">Twodimension array, first index is strain, second is position</param>
    /// <param name="minimaxContract"></param>
    /// <param name="minimaxScore"></param>
    public AnalysisTable(int[,] ddTable, Contract minimaxContract, int minimaxScore)
    {
        this.ddTable = ddTable;
        MinimaxContract = minimaxContract;
        MinimaxScore = minimaxScore;
    }

    public static AnalysisTable BuildAnalysisTable(Func<Position, Suit, int> ddTable, Contract minimaxContract, int minimaxScore)
    {
        var table = new int[5, 4];

        foreach (var suit in Enum.GetValues(typeof(Suit)).Cast<Suit>())
        foreach (var position in Enum.GetValues(typeof(Position)).Cast<Position>())
            table[(int)suit, (int)position] = ddTable(position, suit);

        return new AnalysisTable(table, minimaxContract, minimaxScore);
    }

    public int GetDoubleDummyTricks(Suit suit, Position position)
    {
        return ddTable[(int)suit, (int)position];
    }
}