using System;
using System.Collections.Generic;
using pbn.tokens.tags;


namespace pbn.tokens

{
    using TagFactoryMethod = Func<string, string, Tag>;

    public class TagFactory
    {
        private Dictionary<string, TagFactoryMethod> stringFactories = new();
        private TagFactoryMethod defaultFactory = (name, content) => new Tag(name, content);

        public TagFactory registerTagFactoryMethod(string tagName, TagFactoryMethod factory)
        {
            if (stringFactories.ContainsKey(tagName)) throw new InvalidOperationException($"factory for {tagName} was already set");

            stringFactories[tagName] = factory;

            return this;
        }

        public static TagFactory Default()
        {
            var factory = new TagFactory();


            factory.registerTagFactoryMethod(Tags.Event,  (tagName, tagContent) => new EventTag(tagContent))
            .registerTagFactoryMethod(Tags.Site,  (tagName, tagContent) => new SiteTag(tagContent))
            .registerTagFactoryMethod(Tags.Date,  (tagName, tagContent) => new DateTag(tagContent))
            .registerTagFactoryMethod(Tags.Board,  (tagName, tagContent) => new BoardTag(tagContent))
            .registerTagFactoryMethod(Tags.West,  (tagName, tagContent) => new PlayerTag(tagContent, Position.WEST))
            .registerTagFactoryMethod(Tags.North,  (tagName, tagContent) => new PlayerTag(tagContent, Position.NORTH))
            .registerTagFactoryMethod(Tags.East,  (tagName, tagContent) => new PlayerTag(tagContent, Position.EAST))
            .registerTagFactoryMethod(Tags.South,  (tagName, tagContent) => new PlayerTag(tagContent, Position.SOUTH))
            .registerTagFactoryMethod(Tags.Dealer,  (tagName, tagContent) => new DealerTag(PositionHelpers.FromString(tagContent)))
            .registerTagFactoryMethod(Tags.Vulnerable,  (tagName, tagContent) => new VulnerableTag(tagContent))
            .registerTagFactoryMethod(Tags.Deal,  (tagName, tagContent) => new DealTag(tagContent))
            .registerTagFactoryMethod(Tags.Scoring,  (tagName, tagContent) => new ScoringTag(tagContent))
            .registerTagFactoryMethod(Tags.Declarer,  (tagName, tagContent) => new DeclarerTag(tagContent))
            .registerTagFactoryMethod(Tags.Contract,  (tagName, tagContent) => new ContractTag(tagContent))
            .registerTagFactoryMethod(Tags.Result, (tagName, tagContent) => new ResultTag(tagContent));


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