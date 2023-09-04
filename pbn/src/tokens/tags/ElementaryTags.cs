using System;
using pbn.model;

namespace pbn.tokens.tags;

/* Identification section is defined as follows:
 (1) Event      (the name of the tournament or match)
 (2) Site       (the location of the event)
 (3) Date       (the starting date of the game)
 (4) Board      (the board number)
 (5) West       (the west player)
 (6) North      (the north player)
 (7) East       (the east player)
 (8) South      (the south player)
 (9) Dealer     (the dealer)
(10) Vulnerable (the situation of vulnerability)
(11) Deal       (the dealt cards)
(12) Scoring    (the scoring method)
(13) Declarer   (the declarer of the contract)
(14) Contract   (the contract)
(15) Result     (the result of the game)
*/


public record BoardTag(string Value) : Tag(Tags.Board, Value)
{
    public static string TagName => "Board";
    public int BoardNumber { get; init; } = int.Parse(Value);
}

public record EventTag : Tag
{
    public static string TagName => "Event";
    public EventTag(string value) : base(Tags.Event, value)
    {
    }
}

public record SiteTag : Tag
{
    public static string TagName => "Site";
    public SiteTag(string value) : base(Tags.Site, value)
    {
    }
}

public record DateTag : Tag
{
    public static string TagName => "Date";
    public DateTag(string value) : base(Tags.Date, value)
    {
    }
}

public record PlayerTag : Tag
{
    public Position Position => PositionHelpers.FromString(Value);
    public PlayerTag(string name, string value) : base(name, value)
    {
    }
}

public record DealerTag : Tag
{
    public static string TagName => "Dealer";
    public Position Position => PositionHelpers.FromString(this.Value);

    public DealerTag(string value) : base(Tags.Dealer, value)
    {
    }
}

public record VulnerableTag : Tag
{
    public static string TagName => "Vulnerable";
    public Vulnerability Vulnerability => VulnerabilityHelpers.FromString(Value);
    
    public VulnerableTag(string value) : base(Tags.Vulnerable, value)
    {
    }
}

public record DealTag : Tag
{
    public static string TagName => "Deal";
    public DealTag(string value) : base(Tags.Deal, value)
    {
    }
}

public record ScoringTag : Tag
{
    public static string TagName => "Scoring";
    public ScoringTag(string value) : base(Tags.Scoring, value)
    {
    }
}

public record DeclarerTag : Tag
{
    public static string TagName => "Declarer";
    public DeclarerTag(string value) : base(Tags.Declarer, value)
    {
    }
}

public record ContractTag : Tag
{
    public static string TagName => "Contract";
    public ContractTag(string value) : base(Tags.Contract, value)
    {
    }
}

public record ResultTag : Tag
{
    public static string TagName => "Result";
    public ResultTag(string value) : base(Tags.Result, value)
    {
    }
}