using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using pbn.model;
using pbn.tokens;
using pbn.tokens.tags;
using pbn.utils;

namespace pbn;

/// <summary>
///     Represents a PBN file.
/// </summary>
public class PbnFile
{
    /// All board contexts in the file.
    private readonly List<BoardContext> boards = new();

    /// All tokens in the file.
    private readonly List<SemanticPbnToken> tokens = new();

    /// <summary>
    ///     Returns all boards contexts in the file.
    ///     <seealso cref="BoardContext" />
    /// </summary>
    public IReadOnlyList<BoardContext> Boards => boards;

    /// <summary>
    ///     Returns true if the file is in export format, i.e. contains the export directive.
    /// </summary>
    public bool IsExportFormat => tokens.GetRange(0, 10).Any(t => t is ExportEscapedLine);

    /// <summary>
    ///     All the tokens of a file
    /// </summary>
    public IReadOnlyList<SemanticPbnToken> Tokens => tokens;


    /// <summary>
    ///     Returns the token at specified position in file.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the index is out of range</exception>
    public SemanticPbnToken this[int index]
    {
        get
        {
            if (index < 0 || index > tokens.Count) throw new ArgumentOutOfRangeException();
            return tokens[index];
        }
    }

    /// <summary>
    ///     Returns true if at least one board with the given number is present in the file.
    /// </summary>
    public bool HasBoardWithNumber(int number)
    {
        return boards.Any(context => context.BoardNumber == number);
    }

    /// <summary>
    ///     Returns the deal with the given number.
    /// </summary>
    public BoardContext? GetBoard(int number)
    {
        return boards.FirstOrDefault(context => context.BoardNumber == number);
    }

    /// <summary>
    ///     Appends a token to the end of the file.
    /// </summary>
    public void AppendToken(SemanticPbnToken token)
    {
        tokens.Add(token);

        if (!(token is Tag tag) || !Tags.IsBoardScopeTag(tag.Name)) return;

        var createNewBoardContext = boards.Count == 0 || !boards.Last().AcceptsToken(tag);

        var index = tokens.Count - 1;

        if (createNewBoardContext)
        {
            var newBoardContext = new BoardContext(this, new TokenRange { StartIndex = index, EndOffset = 0 });
            boards.Add(newBoardContext);
            newBoardContext.ApplyToken(tag);
        }
        else
        {
            var context = boards.Last();
            context.TokenRange = context.TokenRange with { EndOffset = context.TokenRange.EndOffset + 1 };
            context.ApplyToken(tag);
        }
    }

    /// <summary>
    ///     Inserts a token at the given index. All following tokens, including the one at the index, are shifted.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void InsertToken(int at, SemanticPbnToken token)
    {
        if (at < 0 || at > tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Insert token: Index out of range");

        foreach (var boardContext in Boards)
        {
            var range = boardContext.TokenRange;
            if (range.StartIndex >= at)
            {
                boardContext.TokenRange = range with { StartIndex = range.StartIndex + 1 };
            }
            else if (range.EndIndex >= at)
            {
                boardContext.TokenRange = range with { EndOffset = range.EndOffset + 1 };
                if (boardContext.AcceptsToken(token, at - range.StartIndex))
                {
                }

                if (token is Tag tag) boardContext.ApplyToken(tag);
            }
        }

        tokens.Insert(at, token);
    }

    /// <summary>
    ///     Replace token at a given index with a new token. The replaced token is discarded
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ReplaceToken(int at, SemanticPbnToken with)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Replace token: Index out of range");

        var from = tokens[at];
        var context = from.OwningBoardContext;
        if (context is not null)
        {
            if (from is Tag tagFrom) context.UnapplyToken(tagFrom);

            if (with is Tag tagWith) context.ApplyToken(tagWith);
        }

        tokens[at] = with;
    }

    /// <summary>
    ///     Replace a given token with a new token. The old token is discarded
    /// </summary>
    /// <exception cref="ArgumentException">If the token to be replaced is not found</exception>
    public void ReplaceToken(SemanticPbnToken from, SemanticPbnToken to)
    {
        var index = tokens.FindIndex(p => p == from);
        if (index < 0)
            throw new ArgumentException("Replace Token: Token not found in token vector");

        ReplaceToken(index, to);
    }

    /// <summary>
    ///     Delete token at a given index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void DeleteTokenAt(int at)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Delete token at: Index out of range");

        tokens.RemoveAt(at);
    }

    /// <summary>
    ///     Delete a given token
    /// </summary>
    /// <exception cref="ArithmeticException">If the token is not found in the file</exception>
    public void DeleteToken(SemanticPbnToken token)
    {
        var index = tokens.FindIndex(p => p == token);
        if (index < 0)
            throw new ArgumentException();

        DeleteToken(index);
    }

    /// <summary>
    ///     Serializes all of the files tokens to a stream writer.
    /// </summary>
    public void Serialize(TextWriter writer)
    {
        foreach (var token in tokens)
        {
            token.Serialize(writer);
            writer.WriteLine();
        }
    }


    private BoardContext? FindOwningBoardContext(int tokenIndex)
    {
        return boards.FirstOrDefault(context =>
            context.TokenRange.StartIndex <= tokenIndex && context.TokenRange.EndIndex >= tokenIndex);
    }

    private IEnumerable<SemanticPbnToken> GetContextTokens(BoardContext context)
    {
        var range = context.TokenRange;
        return tokens.GetRange(range.StartIndex, range.TokenCount).Where(
            token => token.OwningBoardContext == context);
    }

    /// <summary>
    ///     Delete given token. Does nothing if token is not in the file.
    /// </summary>
    private void DeleteToken(int at)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentException("Iterator is outside of token list", nameof(at));


        foreach (var (context, range) in boards.Select(b => (b, b.TokenRange)))
            if (range.StartIndex > at)
            {
                context.TokenRange = range with { StartIndex = range.StartIndex - 1 };
            }
            else if (range.StartIndex <= at && range.EndIndex >= at)
            {
                context.TokenRange = range with { EndOffset = range.EndOffset - 1 };
                if (tokens[at] is Tag tag) context.UnapplyToken(tag);
            }

        tokens.RemoveAt(at);

        // TODO validate state
    }

    /// <summary>
    ///     A BoardContext is a range of tags in a PbnFile that are relevant for a single board.
    /// </summary>
    public record BoardContext
    {
        private readonly PbnFile pbnFile;
        private readonly Dictionary<string, List<Tag>> tagsByName = new();

        internal BoardContext(PbnFile pbnFile, TokenRange range)
        {
            this.pbnFile = pbnFile;
            TokenRange = range;
        }

        /// <summary>
        ///     Range in which all the context's tokens are.
        ///     Not all tokens in the range must pertain to the context.
        /// </summary>
        public TokenRange TokenRange { get; set; }

        public int? BoardNumber => this.GetTag<BoardTag>(BoardTag.TagName)?.BoardNumber;

        /// <summary>
        ///     Returns the tokens that are part of this context.
        ///     Invalidates when a token is added or removed from this context.
        /// </summary>
        public IEnumerable<SemanticPbnToken> Tokens => pbnFile.GetContextTokens(this);

        private T? GetTag<T>(string name) where T : Tag
        {
            if (!tagsByName.ContainsKey(name)) return null;
            return tagsByName[name]?.FirstOrDefault() as T;
        }

        /// <summary>
        ///     Apply the given token to this context. Used to validate file state.
        /// </summary>
        public void ApplyToken(SemanticPbnToken token)
        {
            token.OwningBoardContext = this;
            if (token is not Tag tag) return;
            if (tag.Name == Tags.Board)
            {
                Debug.Assert(BoardNumber is null, "Internal error: Board number is already set.");
            }

            if (!tagsByName.ContainsKey(tag.Name)) tagsByName[tag.Name] = new List<Tag>();
            tagsByName[tag.Name].Add(tag);
        }

        /// <summary>
        ///     Remove tag from this context. Used to validate file state.
        /// </summary>
        public void UnapplyToken(SemanticPbnToken token)
        {
            token.OwningBoardContext = null;
            if (token is not Tag tag) return;

            if (tag is BoardTag && BoardNumber != null)
            {
                throw new InvalidOperationException("Internal error: Board number cannot be changed.");
            }

            tagsByName[tag.Name].Remove(tag);
        }

        /// <summary>
        ///     Check whether a given token can be applied to the context.
        /// </summary>
        /// <param name="token" />
        /// <param name="atIndex">Where would the token be added. Null means after the last token currently in the context.</param>
        public bool AcceptsToken(SemanticPbnToken token, int? atIndex = null)
        {
            atIndex ??= Tokens.Count();

            if (token is not Tag(var tagName, var _)) return true;

            if (Tags.IsTagRecognized(tagName))
                return !Tokens.Any(x =>
                {
                    if (x is not Tag(var tagName2, var _)) return false;

                    return tagName2 == tagName;
                });

            return true;
        }

        public Board AsBoard()
        {
            var vulnerability = GetTag<VulnerableTag>(VulnerableTag.TagName)?.Vulnerability;
            var dealer = GetTag<DealerTag>(DealerTag.TagName)?.Position;
            var cards = GetTag<DealTag>(DealTag.TagName)?.Value;
            var number = BoardNumber;

            if (!vulnerability.HasValue || !dealer.HasValue || cards is null || !number.HasValue)
                throw new PbnError("Cannot create board: Invalid board context ");
            
            return new Board(
                number.Value,
                vulnerability.Value,
                dealer.Value,
                cards
            );
        }

        public void AppendToken(SemanticPbnToken token)
        {
            pbnFile.InsertToken(this.TokenRange.EndIndex + 1, token);
            Debug.Assert(token.OwningBoardContext == this);
        }

        public void InsertToken(SemanticPbnToken token, int atIndex)
        {
            var index = pbnFile.Tokens.IndexOf(this.Tokens.ElementAt(atIndex));
            pbnFile.InsertToken(index, token);
            Debug.Assert(token.OwningBoardContext == this);
        }
    }

    /// Maps board context id to token ranges.
    public record struct TokenRange
    {
        /// Index of the last token in the range in the tokens list.
        public int EndOffset;

        /// Index of the first token in the range in the tokens list.
        public int StartIndex;

        public int EndIndex => StartIndex + EndOffset;
        public int TokenCount => EndOffset + 1;
    }
}