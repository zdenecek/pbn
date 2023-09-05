using System;

namespace pbn.model;

public enum Suit
{
    Notrump,
    Spades,
    Hearts,
    Diamonds,
    Clubs
}

public static class SuitHelpers
{
    public static Suit FromString(string str)
    {
        return FromLetter(str[0]);
    }

    public static Suit FromLetter(char c)
    {
        return c switch
        {
            'N' or 'n' => Suit.Notrump,
            'S' or 's' => Suit.Spades,
            'H' or 'h' => Suit.Hearts,
            'D' or 'd' => Suit.Diamonds,
            'C' or 'c' => Suit.Clubs,
            _ => throw new ArgumentException($"Unknown suit: {c}")
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
    
    public static char ToLetter(this Suit suit)
    {
        return suit switch
        {
            Suit.Notrump => 'N',
            Suit.Spades => 'S',
            Suit.Hearts => 'H',
            Suit.Diamonds => 'D',
            Suit.Clubs => 'C',
            _ => throw new ArgumentException($"Unknown suit: {suit}")
        };
    }
    
    public static string ToLetters(this Suit suit)
    {
        return suit switch
        {
            Suit.Notrump => "NT",
            Suit.Spades => "S",
            Suit.Hearts => "H",
            Suit.Diamonds => "D",
            Suit.Clubs => "C",
            _ => throw new ArgumentException($"Unknown suit: {suit}")
        };
    }
}