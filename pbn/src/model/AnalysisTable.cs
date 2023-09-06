using System;
using System.Linq;

namespace pbn.model;

/// <summary>
///     Represents a double dummy and par contracts analysis.
/// </summary>
public class AnalysisTable
{
    private readonly int[,] ddTable;

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

    /// <summary>
    ///     A single contract that maximizes the score.
    /// </summary>
    public Contract MinimaxContract { get; }

    /// <summary>
    ///     Best possible score for <see cref="MinimaxContract" />.
    /// </summary>
    public int MinimaxScore { get; }

    /// <summary>
    ///     Creates a new AnalysisTable from given delegate.
    /// </summary>
    /// <remarks>
    ///     The delegate is used during the construction of the table to fill it with values and is not used afterwards.
    /// </remarks>
    public static AnalysisTable BuildAnalysisTable(Func<Position, Suit, int> ddTable, Contract minimaxContract,
        int minimaxScore)
    {
        var table = new int[5, 4];

        foreach (var suit in Enum.GetValues(typeof(Suit)).Cast<Suit>())
        foreach (var position in Enum.GetValues(typeof(Position)).Cast<Position>())
            table[(int)suit, (int)position] = ddTable(position, suit);

        return new AnalysisTable(table, minimaxContract, minimaxScore);
    }

    /// <returns>Double dummy tricks for a given trump suit and declarer position</returns>
    public int GetDoubleDummyTricks(Suit suit, Position position)
    {
        return ddTable[(int)suit, (int)position];
    }
}