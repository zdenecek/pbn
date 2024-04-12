using System;
using System.Collections.Generic;
using CommandLine;

namespace pbn_cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);

    }

    private static void RunOptions(Options opts)
    {
        try
        {
            var app = new Application();
            app.Run(opts);
        } catch (Exception e)
        {
            Console.Error.WriteLine("Error: " + e.Message);
        }
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        // do nothing
    }
}