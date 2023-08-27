using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Suit
{
    NOTRUMP,
    SPADES,
    HEARTS,
    DIAMONDS,
    CLUBS,
}


public static class SuitHelpers
{

    public static Suit FromString(string str)
    {
        return str[0] switch
        {
            'N' or 'n' => Suit.NOTRUMP,
            'S' or 's' => Suit.SPADES,
            'H' or 'h' => Suit.HEARTS,
            'D' or 'd' => Suit.DIAMONDS,
            'C' or 'c' => Suit.CLUBS,
            _ => throw new ArgumentException($"Unknown suit: {str}")
        };
    }

    public static string ToString(this Suit suit)
    {

        return suit switch
        {
            Suit.NOTRUMP => "No trumps",
            Suit.SPADES => "Spades",
            Suit.HEARTS => "Hearts",
            Suit.DIAMONDS => "Diamonds",
            Suit.CLUBS => "Clubs",
            _ => throw new ArgumentException($"Unknown suit: {suit}")
        };
    }
}