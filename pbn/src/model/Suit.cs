using System;

namespace pbn.model;

public enum Suit
{
    Notrump,
    Spades,
    Hearts,
    Diamonds,
    Clubs,
}


public static class SuitHelpers
{

    public static Suit FromString(string str)
    {
        return str[0] switch
        {
            'N' or 'n' => Suit.Notrump,
            'S' or 's' => Suit.Spades,
            'H' or 'h' => Suit.Hearts,
            'D' or 'd' => Suit.Diamonds,
            'C' or 'c' => Suit.Clubs,
            _ => throw new ArgumentException($"Unknown suit: {str}")
        };
    }

    public static string ToString(this Suit suit)
    {

        return suit switch
        {
            Suit.Notrump => "No trumps",
            Suit.Spades => "Spades",
            Suit.Hearts => "Hearts",
            Suit.Diamonds => "Diamonds",
            Suit.Clubs => "Clubs",
            _ => throw new ArgumentException($"Unknown suit: {suit}")
        };
    }
}