using System.Collections.Generic;
using pbn.tokens.tags;

namespace pbn.tokens;

/// <summary>
///     Utility class for PBN tags.
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
        ResultTag.TagName
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

    /// <summary>
    ///     Returns true if the tag name is one of known PBN tags.
    ///     Unknown tags are not invalid.
    /// </summary>
    public static bool IsTagRecognized(string tagName)
    {
        return IdentificationSectionTags.Contains(tagName) || TableTags.Contains(tagName);
    }

    /// <summary>
    ///     Returns true if the token is a tag that is in the scope of a board.
    /// </summary>
    public static bool IsBoardScopeToken(SemanticPbnToken token)
    {
        if (token is not Tag tag) return false;
        return IdentificationSectionTags.Contains(tag.Name) || TableTags.Contains(tag.Name);
    }
}