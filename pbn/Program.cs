using System;
using System.Collections.Generic;
using CommandLine;

namespace pbn_cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        var parser = new Parser(with =>
        {
            //ignore case for enum values
            with.CaseInsensitiveEnumValues = true;
            with.AutoHelp = true;
            with.HelpWriter = Parser.Default.Settings.HelpWriter;
        });

        parser.ParseArguments<Options>(args)
            .WithParsed(RunOptions);

    }

    private static void RunOptions(Options opts)
    {
        try
        {
            var app = new Application();
            app.Run(opts);
        }
        catch (Exception e)
        {
#if DEBUG
                Console.Error.WriteLine("Error: " + e.Message);
                Console.Error.WriteLine("Stack Trace: " + e.StackTrace);

                if (e.InnerException != null)
                {
                    Console.Error.WriteLine("Inner Exception: " + e.InnerException.Message);
                    Console.Error.WriteLine("Inner Stack Trace: " + e.InnerException.StackTrace);
                }

                // Rethrow the exception to preserve the original stack trace
                throw;
#else
            Console.Error.WriteLine("Error: " + e.Message);
#endif
        }
    }

}