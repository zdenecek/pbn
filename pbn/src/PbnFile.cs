

global using BoardContextId = System.Int32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using pbn.tokens;


namespace pbn;

/// <summary>
/// Represents a PBN file.
/// </summary>
public class PbnFile
{


    /// <summary>
    /// Returns all boards contexts in the file.
    /// <seealso cref="BoardContext"/>
    /// </summary>
    public IReadOnlyList<BoardContext> Boards => this.boards;

    /// <summary>
    /// Returns true if the file is in export format, i.e. contains the export directive.
    /// </summary>
    public bool IsExportFormat => tokens.Any(t => t is ExportEscapedLine);

    /// <summary>
    /// Returns true if at least one board with the given number is present in the file.
    /// </summary>
    public bool HasBoardWithNumber(int number) => this.boards.Any(context => context.BoardNumber == number);

    /// <summary>
    /// Returns the deal with the given number.
    /// </summary>
    public BoardContext? GetBoard(int number) => boards.FirstOrDefault(context => context.BoardNumber == number);

    /// <summary>
    /// All the tokens of a file
    /// </summary>
    public IReadOnlyList<SemanticPbnToken> Tokens => this.tokens;


    /// <summary>
    /// Returns the token at specified position in file.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the index is out of range</exception>
    public SemanticPbnToken this[int index]
    {
        get
        {
            if (index < 0 || index > this.tokens.Count) throw new ArgumentOutOfRangeException();
            return this.tokens[index];
        }
    }

    /// <summary>
    /// Appends a token to the end of the file.
    /// </summary>
    public void AppendToken(SemanticPbnToken token)
    {
        tokens.Add(token);

        if (!(token is Tag tag) || !Tags.IsBoardScopeTag(tag.Tagname))
        {
            return;
        }

        bool createNewBoardContext = boards.Count == 0 || !boards.Last().AcceptsToken(tag);


        if (createNewBoardContext)
        {
            var newBoardContext = new BoardContext(GetNewContextId, this);
            boards.Add(newBoardContext);
            BoardContextIdToTokenRange[newBoardContext.Id] = new TokenRange { StartIndex = this.tokens.Count - 1, TokenCount = 1 };
            newBoardContext.ApplyTag(tag);
        }
        else
        {
            var bc = boards.Last();
            var current = BoardContextIdToTokenRange[bc.Id];
            BoardContextIdToTokenRange[bc.Id] = current with { TokenCount = current.TokenCount + 1 };
            bc.ApplyTag(tag);
        }
    }

    /// <summary>
    /// Inserts a token at the given index. All following tokens, including the one at the index, are shifted.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void InsertToken(int at, SemanticPbnToken token)
    {
        if (at < 0 || at > this.tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Insert token: Index out of range");

        for (var id = 0; id < BoardContextIdToTokenRange.Count; id++)
        {
            var range = BoardContextIdToTokenRange[id];
            if (range.StartIndex >= at)
            {
                range.StartIndex++;
            }
            else if (range.StartIndex + range.TokenCount > at)
            {
                range.TokenCount++;
                if (token is Tag tag)
                {
                    boards[id].ApplyTag(tag);
                }
            }
        }

        tokens.Insert(at, token);
    }

    /// <summary>
    /// Replace token at a given index with a new token. The replaced token is discarded
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ReplaceToken(int at, SemanticPbnToken with)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Replace token: Index out of range");

        var from = tokens[at];
        var id = FindRange(from);
        if (id.HasValue)
        {
            if (from is Tag tagFrom)
            {
                boards[id.Value].UnapplyTag(tagFrom);
            }
            if (with is Tag tagWith)
            {
                boards[id.Value].ApplyTag(tagWith);
            }
        }

        tokens[at] = with;
    }

    /// <summary>
    /// Replace a given token with a new token. The old token is discarded
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
    /// Delete token at a given index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void DeleteTokenAt(int at)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(at), "Delete token at: Index out of range");

        tokens.RemoveAt(at);
    }

    /// <summary>
    /// Delete a given token
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
    /// Serializes all of the files tokens to a stream writer.
    /// </summary>
    public void Serialize(TextWriter writer)
    {
        foreach (var token in tokens)
        {
            token.Serialize(writer);
            writer.WriteLine();
        }
    }


    /// All tokens in the file.
    private readonly List<SemanticPbnToken> tokens = new();

    /// All board contexts in the file.
    private readonly List<BoardContext> boards = new();

    /// <summary>
    ///  A BoardContext is a range of tags in a PbnFile that are relevant for a single board.
    /// </summary>
    public record BoardContext
    {
        /// <summary>
        /// A numeric id assigned to the context at creation. Relevant only in the file's context and assigned arbitrarily.
        /// </summary>
        public readonly BoardContextId Id;

        public int BoardNumber
        {
            get;
            private set;
        }
        private readonly PbnFile pbnFile;

        public BoardContext(int id, PbnFile pbnFile)
        {
            this.Id = id;
            this.pbnFile = pbnFile;
        }


        /// <summary>
        ///  Apply the given token to this context. Used to validate file state.
        /// </summary>
        public void ApplyTag(Tag token)
        {
            if (token.Tagname == Tags.Board)
            {
                Debug.Assert(this.BoardNumber == 0, "Internal error: Board number is already set.");
                this.BoardNumber = int.Parse(token.Content);
            }
        }

        /// <summary>
        ///  Remove tag from this context. Used to validate file state.
        /// </summary>
        public void UnapplyTag(Tag token)
        {
            if (token.Tagname == Tags.Board && this.BoardNumber != 0)
            {
                throw new InvalidOperationException("Internal error: Board number cannot be changed.");
            }
            if (token.Tagname == Tags.Board)
            {
                this.BoardNumber = 0;
            }
        }

        /// <summary>
        /// Check whether a given token can be applied to the context.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="atIndex">Where would the token be added. Null means after the last token currently in the context.</param>
        public bool AcceptsToken(SemanticPbnToken token, int? atIndex = null)
        {
            atIndex ??= this.Tokens.Count();

            if (token is not Tag(var tagname, var _)) return true;

            if (Tags.IsTagRecognized(tagname))
            {
                return !Tokens.Any(x =>
                {
                    if (x is not Tag(var tagname2, var _)) return false;

                    return tagname2 == tagname;
                });
            }

            return true;
        }

        /// <summary>
        /// Returns the tokens that are part of this context.
        /// Invalidates when a token is added or removed from this context.
        /// </summary>
        public IEnumerable<SemanticPbnToken> Tokens => this.pbnFile.GetContextTokens(this.Id);
    }

    private int contextIdIncrementGenerator = 1;
    private int GetNewContextId => contextIdIncrementGenerator++;

    ///  Maps board context id to token ranges, tuples represent in order: start index, number of tokens
    internal record struct TokenRange
    {
        ///  Index of the first token in the range in the tokens vector.
        public int StartIndex;
        ///  Number of tokens in the range, starting at 1.
        public int TokenCount;
    }

    ///  Maps board context id to token ranges, tuples represent in order: start index, number of tokens
    internal readonly Dictionary<BoardContextId, TokenRange>
        BoardContextIdToTokenRange = new();


    private BoardContextId? FindRange(int tokenIndex)
    {
        foreach ( var  (key, range) in BoardContextIdToTokenRange)
        {
            if (range.StartIndex <= tokenIndex && range.StartIndex + range.TokenCount > tokenIndex)
                return key;
        }
        return null;
    }

    private BoardContextId? FindRange(SemanticPbnToken token)
    {
        var index = tokens.FindIndex(p => p == token);
        if (index < 0)
            throw new ArgumentException("Token not found in token vector", nameof(token));

        return FindRange(index);
    }

    private IEnumerable<SemanticPbnToken> GetContextTokens(BoardContextId id)
    {
        var range = BoardContextIdToTokenRange[id];
        return this.tokens.GetRange(range.StartIndex,  range.TokenCount);
    }

    /// <summary>
    /// Delete given token. Does nothing if token is not in the file.
    /// </summary>
    private void DeleteToken(int at)
    {
        if (at < 0 || at >= tokens.Count)
            throw new ArgumentException("Iterator is outside of token list", nameof(at));


        foreach (var (key, value) in BoardContextIdToTokenRange)
        {
            if (value.StartIndex > at)
            {
                var current = BoardContextIdToTokenRange[key];
                BoardContextIdToTokenRange[key] = current with { StartIndex = current.StartIndex - 1 };
            }
            else if (value.StartIndex <= at && value.StartIndex + value.TokenCount > at)
            {
                var current = BoardContextIdToTokenRange[key];
                BoardContextIdToTokenRange[key] = current with { TokenCount = current.TokenCount - 1 };
                if (tokens[at] is Tag tag)
                {
                    boards[key].UnapplyTag(tag);
                }
            }
        }

        tokens.RemoveAt(at);

        // TODO validate state
    }
}