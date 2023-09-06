using System.Collections.Generic;
using pbn.tokens.tags;

namespace pbn.tokens;


/// <summary>
/// Utility class for PBN tags.
/// </summary>
public static class Tags
{
    public const string DoubleDummyTricks = "DoubleDummyTricks";
    public const string OptimumScore = "OptimumScore";
    public const string OptimumResultTable = "OptimumResultTable";
    public const string ActionTable = "ActionTable";
    public const string AuctionTimeTable = "AuctionTimeTable";
    public const string InstantScoreTable = "InstantScoreTable";
    public const string OptimumPlayTable = "OptimumPlayTable";
    public const string PlayTimeTable = "PlayTimeTable";
    public const string ScoreTable = "ScoreTable";
    public const string TotalScoreTable = "TotalScoreTable";

    public static readonly IReadOnlySet<string> IdentificationSectionTags = new HashSet<string>
    {
        EventTag.TagName,
        SiteTag.TagName,
        DateTag.TagName,
        BoardTag.TagName,
        PlayerTag.North,
        PlayerTag.East,
        PlayerTag.South,
        PlayerTag.West,
        DealerTag.TagName,
        VulnerableTag.TagName,
        DealTag.TagName,
        ScoringTag.TagName,
        DeclarerTag.TagName,
        ContractTag.TagName,
        ResultTag.TagName,
    };

    public static readonly IReadOnlySet<string> TableTags = new HashSet<string>
    {
        OptimumResultTable,
        ActionTable,
        AuctionTimeTable,
        InstantScoreTable,
        OptimumPlayTable,
        PlayTimeTable,
        ScoreTable,
        TotalScoreTable
    };

    public static bool IsTagRecognized(string tagName)
    {
        return IdentificationSectionTags.Contains(tagName) || TableTags.Contains(tagName);
    }


    public static bool IsBoardScopeToken(SemanticPbnToken token)
    {
        if (token is not Tag tag) return false;
        return IdentificationSectionTags.Contains(tag.Name) || TableTags.Contains(tag.Name);
    }
}