using System;
using System.Collections.Generic;
using pbn.tokens.tags;

namespace pbn.tokens;

using TagFactoryMethod = Func<string, string, Tag>;

/// <summary>
///     Factory for creating tags, instantiates the correct tag type based on the tag name.
/// </summary>
public class TagFactory
{
    private readonly TagFactoryMethod defaultFactory = (name, content) => new Tag(name, content);
    private readonly Dictionary<string, TagFactoryMethod> stringFactories = new();

    public TagFactory RegisterTagFactoryMethod(string tagName, TagFactoryMethod factory)
    {
        if (stringFactories.ContainsKey(tagName))
            throw new InvalidOperationException($"factory for {tagName} was already set");

        stringFactories[tagName] = factory;

        return this;
    }

    public static TagFactory MakeDefault()
    {
        var factory = new TagFactory();


        factory.RegisterTagFactoryMethod(EventTag.TagName, (_, tagContent) => new EventTag(tagContent))
            .RegisterTagFactoryMethod(SiteTag.TagName, (_, tagContent) => new SiteTag(tagContent))
            .RegisterTagFactoryMethod(DateTag.TagName, (_, tagContent) => new DateTag(tagContent))
            .RegisterTagFactoryMethod(BoardTag.TagName, (_, tagContent) => new BoardTag(tagContent))
            .RegisterTagFactoryMethod(PlayerTag.West, (name, tagContent) => new PlayerTag(name, tagContent))
            .RegisterTagFactoryMethod(PlayerTag.North, (name, tagContent) => new PlayerTag(name, tagContent))
            .RegisterTagFactoryMethod(PlayerTag.East, (name, tagContent) => new PlayerTag(name, tagContent))
            .RegisterTagFactoryMethod(PlayerTag.South, (name, tagContent) => new PlayerTag(name, tagContent))
            .RegisterTagFactoryMethod(DealerTag.TagName, (_, tagContent) => new DealerTag(tagContent))
            .RegisterTagFactoryMethod(VulnerableTag.TagName, (_, tagContent) => new VulnerableTag(tagContent))
            .RegisterTagFactoryMethod(DealTag.TagName, (_, tagContent) => new DealTag(tagContent))
            .RegisterTagFactoryMethod(ScoringTag.TagName, (_, tagContent) => new ScoringTag(tagContent))
            .RegisterTagFactoryMethod(DeclarerTag.TagName, (_, tagContent) => new DeclarerTag(tagContent))
            .RegisterTagFactoryMethod(ContractTag.TagName, (_, tagContent) => new ContractTag(tagContent))
            .RegisterTagFactoryMethod(ResultTag.TagName, (_, tagContent) => new ResultTag(tagContent));


        return factory;
    }

    public Tag CreateTag(string tagName, string tagContent, int? lineNumber = null)
    {
        Tag tag;
        if (stringFactories.TryGetValue(tagName, out var factory))
            tag = factory.Invoke(tagName, tagContent);
        else
            tag = defaultFactory(tagName, tagContent);

        tag.OriginLine = lineNumber;
        return tag;
    }

    /// <summary>
    ///     Returns true if the tag name is one of known PBN table tags.
    /// </summary>
    public bool IsTableTag(string tagName)
    {
        return Tags.TableTags.Contains(tagName);
    }

    public TableTag CreateTableTag(string tagName, string tagContent, List<string> values, int? lineNumber = null)
    {
        var tag = new TableTag(tagName, tagContent, values)
        {
            OriginLine = lineNumber
        };
        return tag;
    }
}