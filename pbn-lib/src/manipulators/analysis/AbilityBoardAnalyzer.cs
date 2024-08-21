using System.Collections.Generic;
using System.Text;
using pbn.dds;
using pbn.model;
using pbn.tokens;

namespace pbn.manipulators.analysis;

public class AbilityBoardAnalyzer : TagBasedBoardAnalyzer
{
    /// <summary>
    ///     Creates a minimax tag from a given analysis table.
    /// </summary>
    private Tag MakeMinimaxTag(AnalysisTable table)
    {
        var tokenString = new StringBuilder();

        if (table.MinimaxContract.HasValue)
        {
            tokenString.Append(table.MinimaxContract.Value.Level);
            tokenString.Append(table.MinimaxContract.Value.Suit.ToLetter());
            tokenString.Append(table.MinimaxContract.Value.Declarer.ToLetter());
            tokenString.Append(table.MinimaxScore);
        }

        var minimaxTag = TagFactory.CreateTag("Minimax", tokenString.ToString());
        return minimaxTag;
    }

    /// <summary>
    ///     Creates ability tag from a given analysis table.
    /// </summary>
    private Tag MakeAbilityTag(AnalysisTable table)
    {
        var tokenString = new StringBuilder();
        foreach (var position in new[] { Position.North, Position.East, Position.South, Position.West })
        {
            if (position != Position.North) tokenString.Append(' ');
            tokenString.Append(position.ToLetter()).Append(':');
            foreach (var suit in new[] { Suit.Notrump, Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs })
            {
                var value = table.GetDoubleDummyTricks(suit, position);
                tokenString.Append(value.ToString("X"));
            }
        }

        var abilityTag = TagFactory.CreateTag("Ability", tokenString.ToString());
        return abilityTag;
    }

    public AbilityBoardAnalyzer(DdsAnalysisService analysisService, TagFactory tagFactory) : base(analysisService, tagFactory)
    {
    }

    private static readonly HashSet<string> TagNames =
        new() {  "Minimax", "Ability" };
    
    protected override IEnumerable<string> AnalysisTagNames => TagNames;
    
    public override IEnumerable<SemanticPbnToken> CreateAnalysisTokens(AnalysisTable table)
    {
            return new[] { MakeAbilityTag(table), MakeMinimaxTag(table) };
    }

}