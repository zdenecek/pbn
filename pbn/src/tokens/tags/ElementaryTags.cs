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

public record BoardTag(string Value) : Tag(TagName, Value)
{
    public static string TagName => "Board";
    public int BoardNumber { get; init; } = int.Parse(Value);
}

public record EventTag : Tag
{
    public EventTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Event";
}

public record SiteTag : Tag
{
    public SiteTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Site";
}

public record DateTag : Tag
{
    public DateTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Date";
}

public record PlayerTag : Tag
{
    public PlayerTag(string name, string value) : base(name, value)
    {
    }

    public const string West = "West";
    public const string North = "North";
    public const string East = "East";
    public const string South = "South";
    
    public Position Position => PositionHelpers.FromString(Value);
}

public record DealerTag : Tag
{
    public DealerTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Dealer";
    public Position Position => PositionHelpers.FromString(Value);
}

public record VulnerableTag : Tag
{
    public VulnerableTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Vulnerable";
    public Vulnerability Vulnerability => VulnerabilityHelpers.FromString(Value);
}

public record DealTag : Tag
{
    public DealTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Deal";
}

public record ScoringTag : Tag
{
    public ScoringTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Scoring";
}

public record DeclarerTag : Tag
{
    public DeclarerTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Declarer";
}

public record ContractTag : Tag
{
    public ContractTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Contract";
}

public record ResultTag : Tag
{
    public ResultTag(string value) : base(TagName, value)
    {
    }

    public static string TagName => "Result";
}