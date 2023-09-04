using System;
using System.Collections.Generic;
using System.Linq;
using pbn.tokens;

namespace pbn.utils;

public static class ListUtils
{
    public static IEnumerable<Tag> GetAllTagsByNames(this IEnumerable<SemanticPbnToken> tokens,
        IReadOnlySet<string> names)
    {
        return from token in tokens
            where token is Tag
            let tag = token as Tag
            where names.Contains(tag.Name)
            select tag;
    }

    public static IEnumerable<Tag> GetAllTagsThatSatisfy(this IEnumerable<SemanticPbnToken> tokens,
        Func<Tag, bool> predicate)
    {
        return from token in tokens
            where token is Tag
            let tag = token as Tag
            where predicate(tag)
            select tag;
    }

    public static bool ContainsAll(this IEnumerable<Tag> tags, IReadOnlySet<string> names)
    {
        HashSet<string> found = new();
        foreach (var (tagName, _) in tags)
            if (names.Contains(tagName))
                found.Add(tagName);
        return found.Count == names.Count;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind, int startAt = 0 )
    {
        int i = startAt;
        foreach (T element in self.Skip(startAt))
        {
            if (Equals(element, elementToFind))
                return i;
            i++;
        }

        return -1;
    }
}