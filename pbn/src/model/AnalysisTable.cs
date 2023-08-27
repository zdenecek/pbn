using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnalysisTable
{
    private readonly int[,] ddTable;

    public static AnalysisTable BuildAnalysisTable(Func<Position, Suit, int> ddTable)
    {
        var table = new int[5, 4];

        foreach (var suit in Enum.GetValues(typeof(Suit)).Cast<Suit>())
        {
            foreach (var position in Enum.GetValues(typeof(Position)).Cast<Position>())
            {
                table[(int)suit, (int)position] = ddTable(position, suit);
            }
        }

        return new AnalysisTable(table);
    }

    /// <summary>
    /// Creates a new AnalysisTable from a double dummy table.
    /// </summary>
    /// <param name="ddTable">Twodimension array, first index is strain, second is position</param>
    public AnalysisTable(int[,] ddTable)
    {
        this.ddTable = ddTable;
    }

    public int GetDoubleDummyTricks(Suit suit, Position position)
    {
        return ddTable[(int)suit, (int)position];
    }


}
