using System;

namespace pbn.model;

/// Represents contract suit, ie. including notrump.
public enum Suit
{
    Notrump,
    Spades,
    Hearts,
    Diamonds,
    Clubs
}

/// Helper extension methods for <see cref="Suit"/>.
public static class SuitHelpers
{
    /// Convert string to suit, case insensitive. Uses first letter of the string.
    public static Suit FromString(string str)
    {
        return FromLetter(str[0]);
    }

    /// Convert char to suit, case insensitive.
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

    /// Convert suit to string. First letter is upper case. Notrump is "No trumps".
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
    
    /// Convert suit to char upper case.
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
    
    /// Convert suit to char upper case. Notrump is "NT".
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