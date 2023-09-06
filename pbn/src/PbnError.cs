using System;

namespace pbn;

/// <summary>
///     Base class for all PBN errors.
/// </summary>
public class PbnError : Exception
{
    public PbnError(string message) : base(message)
    {
    }
}