using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum Position
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public static class PositionHelpers
{

    public static Position FromString(string str)
    {
        return str[0] switch
        {
            'N' or 'n' => Position.NORTH,
            'E' or 'e' => Position.EAST,
            'S' or 's' => Position.SOUTH,
            'W' or 'w' => Position.WEST,
            _ => throw new ArgumentException($"Unknown position: {str}" )
        };
    }

    public static string ToString(this Position position)
    {

        return position switch
        {
            Position.NORTH =>  "North",
            Position.EAST =>  "East",
            Position.SOUTH =>  "South",
            Position.WEST => "West",
            _ => throw new ArgumentException($"Unknown position: {position}")
        };
    }
}