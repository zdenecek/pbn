using CommandLine;
using System.Collections.Generic;

namespace pbn;

internal class Program
{
    static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
          .WithParsed(RunOptions)
          .WithNotParsed(HandleParseError);
    }
    static void RunOptions(Options opts)
    {
        Application.Instance.Run(opts);
    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }
}