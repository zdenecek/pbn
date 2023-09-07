using System;
using System.Collections.Generic;

namespace pbn.model;

/// <summary>
///     Represents a position in bridge.
/// </summary>
public enum Position
{
    North,
    East,
    South,
    West
}

/// <summary>
///     Helper extension methods for <see cref="Position" />.
/// </summary>
public static class PositionHelpers
{
    /// <summary>
    ///     Convert string to position, case insensitive. Uses first letter of the string.
    /// </summary>
    public static Position FromString(string str)
    {
        return FromLetter(str[0]);
    }

    /// <summary>
    ///     Convert char to position, case insensitive.
    /// </summary>
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

    /// <summary>
    ///     Convert position to string. First letter is upper case.
    /// </summary>
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

    /// <summary>
    ///     Convert position to char upper case.
    /// </summary>
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

    /// <summary>
    ///     Get all positions. In order: North, East, South, West.
    /// </summary>
    public static IEnumerable<Position> All()
    {
        return new[] { Position.North, Position.East, Position.South, Position.West };
    }

    /// <summary>
    ///     True if position is North or South
    /// </summary>
    public static bool IsNs(this Position position)
    {
        return position == Position.North || position == Position.South;
    }

    /// <summary>
    ///     True if position is East or West
    /// </summary>
    public static bool IsEw(this Position position)
    {
        return position == Position.East || position == Position.West;
    }
}