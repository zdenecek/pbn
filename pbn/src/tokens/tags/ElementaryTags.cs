﻿using System;
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


public record BoardTag : Tag
{
    public int BoardNumber { get; init; }

    public BoardTag(string content) : base(Tags.Board, content)
    {
        this.BoardNumber = int.Parse(content);
    }
}

public record EventTag : Tag
{
    public EventTag(string content) : base(Tags.Event, content)
    {
    }
}

public record SiteTag : Tag
{
    public SiteTag(string content) : base(Tags.Site, content)
    {
    }
}

public record DateTag : Tag
{
    public DateTag(string content) : base(Tags.Date, content)
    {
    }
}

public record PlayerTag : Tag
{
    public PlayerTag(string content, Position position) : base(position switch
    {
        Position.North => Tags.North,
        Position.East => Tags.East,
        Position.South => Tags.South,
        Position.West => Tags.West,
        _ => throw new ArgumentException()
    }, content)
    {
    }
}

public record DealerTag : Tag
{
    public DealerTag(Position position) : base(Tags.Dealer, position.ToString()[0..1])
    {
    }
}

public record VulnerableTag : Tag
{
    public VulnerableTag(string content) : base(Tags.Vulnerable, content)
    {
    }
}

public record DealTag : Tag
{
    public DealTag(string content) : base(Tags.Deal, content)
    {
    }
}

public record ScoringTag : Tag
{
    public ScoringTag(string content) : base(Tags.Scoring, content)
    {
    }
}

public record DeclarerTag : Tag
{
    public DeclarerTag(string content) : base(Tags.Declarer, content)
    {
    }
}

public record ContractTag : Tag
{
    public ContractTag(string content) : base(Tags.Contract, content)
    {
    }
}

public record ResultTag : Tag
{
    public ResultTag(string content) : base(Tags.Result, content)
    {
    }
}