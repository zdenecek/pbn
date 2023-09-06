using System;
using System.Collections.Generic;
using System.Linq;
using pbn.tokens;

namespace pbn.utils;

/// <summary>
///     Utility methods for working with lists.
/// </summary>
public static class ListUtils
{
    /// <summary>
    ///     Returns all tags that have one of the given names.
    /// </summary>
    public static IEnumerable<Tag> GetAllTagsByNames(this IEnumerable<SemanticPbnToken> tokens,
        IReadOnlySet<string> names)
    {
        return from token in tokens
            where token is Tag
            let tag = token as Tag
            where names.Contains(tag.Name)
            select tag;
    }

    /// <summary>
    ///     Returns all tags that satisfy the given predicate.
    /// </summary>
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

    /// <summary>
    ///     IndexOf for IReadOnlyList.
    /// </summary>
    public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind, int startAt = 0)
    {
        var i = startAt;
        foreach (var element in self.Skip(startAt))
        {
            if (Equals(element, elementToFind))
                return i;
            i++;
        }

        return -1;
    }
}