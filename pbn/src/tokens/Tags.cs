using System.Collections.Generic;

namespace pbn.tokens;

public class Tags
{
    public const string Event = "Event";
    public const string Site = "Site";
    public const string Date = "Date";
    public const string Board = "Board";
    public const string West = "West";
    public const string North = "North";
    public const string East = "East";
    public const string South = "South";
    public const string Dealer = "Dealer";
    public const string Vulnerable = "Vulnerable";
    public const string Deal = "Deal";
    public const string Scoring = "Scoring";
    public const string Declarer = "Declarer";
    public const string Contract = "Contract";
    public const string Result = "Result";
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

    public static bool IsTagRecognized(string tagname)
    {
        return IdentificationSectionTags.Contains(tagname) || TableTags.Contains(tagname);
    }

    public static readonly IReadOnlySet<string> IdentificationSectionTags = new HashSet<string> {
            Event,
            Site,
            Date,
            Board,
            West,
            North,
            East,
            South,
            Dealer,
            Vulnerable,
            Deal,
            Scoring,
            Declarer,
            Contract,
            Result
        };

    public static readonly IReadOnlySet<string> TableTags = new HashSet<string> {
            DoubleDummyTricks,
            OptimumScore,
            OptimumResultTable,
            ActionTable,
            AuctionTimeTable,
            InstantScoreTable,
            OptimumPlayTable,
            PlayTimeTable,
            ScoreTable,
            ScoreTable,
            TotalScoreTable
        };


    public static bool IsBoardScopeTag(string tagName)
    {
        return IdentificationSectionTags.Contains(tagName) || TableTags.Contains(tagName);
    }

}
