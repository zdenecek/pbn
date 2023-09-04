using System;

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
        return str[0] switch
        {
            'N' or 'n' => Position.North,
            'E' or 'e' => Position.East,
            'S' or 's' => Position.South,
            'W' or 'w' => Position.West,
            _ => throw new ArgumentException($"Unknown position: {str}")
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
}