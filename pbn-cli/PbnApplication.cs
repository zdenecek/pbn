using System;
using System.IO;
using CommandLine;
using pbn;
using pbn.dds;
using pbn.debug;
using pbn.manipulators;
using pbn.tokens;

namespace pbn_cli;

/// <summary>
/// The CLI Application class. This is a singleton.
/// </summary>
public class Application
{
    /// <summary>
    /// Version of the application.
    /// </summary>
    public const string Version = "0.1.0";

    static Application()
    {
        if (Instance != null)
            throw new InvalidOperationException("PbnApplication already exists");
        Instance = new Application();
    }

    private Application()
    {
    }
    
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static Application Instance { get; }
    
    /// <summary>
    /// True if the application was started with the verbose flag.
    /// </summary>
    public bool Verbose { get; set; }
    
    public void Run(Options options)
    {
        if (options.Version)
        {
            Console.WriteLine($"pbn {Version}");
            return;
        }

        if (options.Verbose)
            Verbose = true;

        if (string.IsNullOrWhiteSpace(options.InputFile))
        {
            Console.Error.WriteLine("No input file specified");
            return;
        }

        var filename = options.InputFile!;

        HandleFile(filename, options);
    }

    private void HandleFile(string filename, Options options)
    {
        using var inputFile = new StreamReader(filename);
        var tagFactory = TagFactory.MakeDefault();
        var parser = new PbnParser(PbnParser.RecoveryMode.Strict, tagFactory);

        PbnFile file;

        try
        {
            file = parser.Parse(inputFile);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error parsing file: {e.Message}");
            return;
        }

        if (options.Debug)
        {
            DebugUtils.SerializePbnFile(file, Console.Out);
            DebugUtils.PrintBoardContextRanges(file, Console.Out);
            DebugUtils.Playground();
            return;
        }

        if (options.Info)
        {
            PbnInfoPrinter.PrintOverview(filename, file, Console.Out);
            return;
        }

        if (options.Strip)
        {
            var stripper = new PbnStripper();
            stripper.Strip(file);
        }

        if (options.Analyze)
        {
            var service = new DdsAnalysisService();
            var analyzer = new PbnBoardAnalyzer(service, tagFactory);
            analyzer.AddAnalyses(file);
        }

        var serializer = new PbnSerializer();

        if (!string.IsNullOrWhiteSpace(options.Output))
            serializer.Serialize(file, options.Output);
        else if (options.Overwrite)
            serializer.Serialize(file, filename);
        else
            serializer.Serialize(file, Console.Out);
    }
}

/// <summary>
/// Holds the information on what flags, options and arguments were given when running the program.
/// </summary>
public class Options
{
    [Option('h', "help", HelpText = "Produce help message")]
    public bool Help { get; set; }

    [Option("version", HelpText = "Print version information")]
    public bool Version { get; set; }

    [Option('v', "verbose", HelpText = "Print additional information about the file")]
    public bool Verbose { get; set; }

    [Option('s', "strip", HelpText = "Remove all results, site and event information")]
    public bool Strip { get; set; }

    [Option('a', "analyze", HelpText = "Create double dummy analyses for each board")]
    public bool Analyze { get; set; }

    [Value(1, MetaName = "output-file",
        HelpText = "Output file name, if not specified, the program will use the input file name")]
    public string? Output { get; set; }

    [Option('w', "overwrite", HelpText = "Overwrite the input file with output")]
    public bool Overwrite { get; set; }

    [Option("info", HelpText = "Print information about the file")]
    public bool Info { get; set; }

    [Value(0, MetaName = "input-file", Required = true, HelpText = "Input file name")]
    public string? InputFile { get; set; }

    [Option("debug", Hidden = true)] public bool Debug { get; set; }
}