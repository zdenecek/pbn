using System;

namespace pbn;

public class PbnError : Exception
{
    public PbnError(string message) : base(message) {}
}