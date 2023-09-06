using System;
using System.Collections.Generic;

namespace pbn.model;

/// Represents a position in bridge.
public enum Position
{
    North,
    East,
    South,
    West
}

/// Helper extension methods for <see cref="Position"/>.
public static class PositionHelpers
{
    /// Convert string to position, case insensitive. Uses first letter of the string.
    public static Position FromString(string str)
    {
        return FromLetter(str[0]);
    }
    
    /// Convert char to position, case insensitive.
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

    /// Convert position to string. First letter is upper case.
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

    /// Convert position to char upper case.
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

    /// Get all positions. In order: North, East, South, West.
    public static IEnumerable<Position> All()
    {
        return new[] { Position.North, Position.East, Position.South, Position.West };
    }

    /// <returns>True if position is North or South</returns>
    public static bool IsNs(this Position position)
    {
        return position == Position.North || position == Position.South;
    }
    
    /// <returns>True if position is East or West</returns>
    public static bool IsEw(this Position position)
    {
        return position == Position.East || position == Position.West;
    }
}