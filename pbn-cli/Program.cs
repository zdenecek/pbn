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
        Application.Instance.Run(opts);
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        // do nothing
    }
}