using System;
using System.Collections.Generic;

namespace pbn.model;

public enum Position
{
    North,
    East,
    South,
    West
}

public static class PositionHelpers
{
    public static Position FromString(string str)
    {
        return FromLetter(str[0]);
    }
    
    public static Position FromLetter(char c)
    {
        return c switch
        {
            'N' or 'n' => Position.North,
            'E' or 'e' => Position.East,
            'S' or 's' => Position.South,
            'W' or 'w' => Position.West,
            _ => throw new ArgumentException($"Unknown position: {c}")
        };
    }

    public static string ToString(this Position position)
    {
        return position switch
        {
            Position.North => "North",
            Position.East => "East",
            Position.South => "South",
            Position.West => "West",
            _ => throw new ArgumentException($"Unknown position: {position}")
        };
    }

    public static char ToLetter(this Position position)
    {
        return position switch
        {
            Position.North => 'N',
            Position.East => 'E',
            Position.South => 'S',
            Position.West => 'W',
            _ => throw new ArgumentException($"Unknown position: {position}")
        };
    }

    public static IEnumerable<Position> All()
    {
        return new[] { Position.North, Position.East, Position.South, Position.West };
    }

    public static bool IsNS(this Position position)
    {
        return position == Position.North || position == Position.South;
    }
}