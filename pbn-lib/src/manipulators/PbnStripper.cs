using System.Collections.Generic;
using pbn.tokens;

namespace pbn.manipulators;

/// <summary>
///     Used to purge a pbn file of unwanted tokens.
///     Used to execute the --strip command.
/// </summary>
public class PbnStripper
{
    /// <summary>
    ///     Creates a file stripper with predefined allowed tags.
    ///     The stripper removes all tokens except directives and allowed tags.
    ///     Predefined tags are: Generator, Board, Dealer, Vulnerable, Deal, Ability, Minimax, OptimumScore, OptimumResultTable
    /// </summary>
    public PbnStripper()
    {
        AllowedTags = new List<string>
        {
            "Generator",
            "Board",
            "Dealer",
            "Vulnerable",
            "Deal",
            "Ability",
            "Minimax",
            "OptimumScore",
            "OptimumResultTable"
        };
    }

    /// <summary>
    ///     Creates a file stripper with the given allowed tags.
    ///     The stripper removes all tokens except directives and allowed tags.
    /// </summary>
    /// <param name="allowedTags">List of allowed tags</param>
    public PbnStripper(List<string> allowedTags)
    {
        AllowedTags = allowedTags;
    }

    public List<string> AllowedTags { get; }

    /// <summary>
    ///     The strip method removes all tokens except directives and allowed tags.
    /// </summary>
    /// <param name="file">File to strip</param>
    public void Strip(PbnFile file)
    {
        for (var i = 0; i < file.Tokens.Count; i++)
        {
            var token = file.Tokens[i];

            if (token is EscapedLine escapedLine && escapedLine.IsDirective)
                continue;

            if (token is Tag tag && AllowedTags.Contains(tag.Name))
                continue;

            file.DeleteToken(token);
            i--;
        }
    }
}