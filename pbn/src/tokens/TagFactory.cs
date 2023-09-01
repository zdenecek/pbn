using System;
using System.Collections.Generic;
using pbn.model;
using pbn.tokens.tags;


namespace pbn.tokens

{
    using TagFactoryMethod = Func<string, string, Tag>;

    public class TagFactory
    {
        private Dictionary<string, TagFactoryMethod> stringFactories = new();
        private TagFactoryMethod defaultFactory = (name, content) => new Tag(name, content);

        public TagFactory RegisterTagFactoryMethod(string tagName, TagFactoryMethod factory)
        {
            if (stringFactories.ContainsKey(tagName)) throw new InvalidOperationException($"factory for {tagName} was already set");

            stringFactories[tagName] = factory;

            return this;
        }

        public static TagFactory Default()
        {
            var factory = new TagFactory();


            factory.RegisterTagFactoryMethod(Tags.Event,  (_, tagContent) => new EventTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Site,  (_, tagContent) => new SiteTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Date,  (_, tagContent) => new DateTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Board,  (_, tagContent) => new BoardTag(tagContent))
            .RegisterTagFactoryMethod(Tags.West,  (_, tagContent) => new PlayerTag(tagContent, Position.West))
            .RegisterTagFactoryMethod(Tags.North,  (_, tagContent) => new PlayerTag(tagContent, Position.North))
            .RegisterTagFactoryMethod(Tags.East,  (_, tagContent) => new PlayerTag(tagContent, Position.East))
            .RegisterTagFactoryMethod(Tags.South,  (_, tagContent) => new PlayerTag(tagContent, Position.South))
            .RegisterTagFactoryMethod(Tags.Dealer,  (_, tagContent) => new DealerTag(PositionHelpers.FromString(tagContent)))
            .RegisterTagFactoryMethod(Tags.Vulnerable,  (_, tagContent) => new VulnerableTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Deal,  (_, tagContent) => new DealTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Scoring,  (_, tagContent) => new ScoringTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Declarer,  (_, tagContent) => new DeclarerTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Contract,  (_, tagContent) => new ContractTag(tagContent))
            .RegisterTagFactoryMethod(Tags.Result, (_, tagContent) => new ResultTag(tagContent));


            return factory;
        }

        public Tag CreateTag(string tagName, string tagContent)
        {

            if (this.stringFactories.ContainsKey(tagName))
                return this.stringFactories[tagName].Invoke(tagName, tagContent);

            return defaultFactory(tagName, tagContent);
        }

        public bool IsTableTag(string tagName)
        {
            return Tags.TableTags.Contains(tagName);
        }

        public TableTag CreateTableTag(string tagName, string tagContent, List<string> values)
        {
            return new TableTag(tagName, tagContent, values);
        }
    }
}