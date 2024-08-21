using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using pbn.dds;
using pbn.model;
using pbn.tokens;

namespace pbn.manipulators.analysis;

public class PsBridgeBoardAnalyzer : BaseBoardAnalyzer
{
    public PsBridgeBoardAnalyzer(DdsAnalysisService analysisService) : base(analysisService)
    {
    }

    public override IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table)
    {
        var stringBuilder = new StringBuilder("%!R ");
        // The ESWN order is part of the PS Bridge format
        foreach (var position in new[] { Position.East, Position.South, Position.West, Position.North }) 
        {
            foreach (var suit in new[] { Suit.Notrump, Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs })
            {
                stringBuilder.Append(table.GetDoubleDummyTricks(suit, position).ToString("X"));
            }
        }
        
        return new[] { new CustomEscapedLine(stringBuilder.ToString()) };
    }

    private readonly Regex matchingRegex = new Regex(@"^%!R [0-9A-D]{20}$", RegexOptions.Compiled);
    private IEnumerable<CustomEscapedLine> GetAnalysisLines(IEnumerable<SemanticPbnToken> tokens)
    {
        return from token in tokens
            where token is CustomEscapedLine line
                  && matchingRegex.IsMatch(line.Content)
            select token as CustomEscapedLine;
    }
    
    public override void PurgeAnalysesTokens(PbnFile file)
    {
        var toDelete = GetAnalysisLines(file.Tokens);
        foreach (var line in toDelete) file.DeleteToken(line);
    }

    public override void PurgeAnalysesTokens(PbnFile file, PbnFile.BoardContext board)
    {
        var toDelete = GetAnalysisLines(board.Tokens);
        foreach (var line in toDelete) file.DeleteToken(line);
    }

    public override bool HasAnalysis(PbnFile.BoardContext board)
    {
        return GetAnalysisLines(board.Tokens).Any();
    }
}