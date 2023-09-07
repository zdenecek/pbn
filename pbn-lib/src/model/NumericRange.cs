namespace pbn.model;

/// <summary>
/// Represents an integer range. The range is inclusive. A range can be a single number.
/// </summary>
/// <param name="Start">Inclusive start</param>
/// <param name="End">Inclusive end</param>
public readonly record struct NumericRange(int Start, int End)
{
    private const string Separator = "-";

    /// <summary>
    /// Parses a string into a NumericRange. The string can be a single number or a range of numbers.
    /// If a single number is provided, the range will be from that number to that number.
    /// </summary>
    public static NumericRange FromString(string str)
    {
        if (!str.Contains(Separator))
        {
            var number = int.Parse(str);
            return new NumericRange(number, number);
        }
        
        var parts = str.Split(Separator, 2);
        return new NumericRange(int.Parse(parts[0]), int.Parse(parts[1]));
    }
    
    /// <summary>
    /// Returns true if the number is in the range. Bounds are inclusive.
    /// </summary>
    public bool IsInRange(int number)
    {
        return number >= Start && number <= End;
    }
    
    
}
