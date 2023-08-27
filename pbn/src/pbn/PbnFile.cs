/// @brief Represent type of internal type id. Id is used to identify a board context at runtime and is never persisted.
global using BoardContextId = System.Int32;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using pbn.tokens;


/// @brief Represents a .pbn file
public class PbnFile
{
    private IEnumerable<int> contextIdGenerator()
    {
        int i = 0;
        while (true) yield return ++i;
    }

    /**
     * @brief Returns all boards contexts in the file.
     *
     * @see BoardContext
     */
    public IReadOnlyList<BoardContext> BoardContexts
    {
        get => this.boardContexts;
    }

    /// @brief return true if the file is in EXPORT format and contains the export directive.
    public bool isExportFormat { get; private set; }

    /// @brief Returns true if at least one board with the given number is present in the file.
    public bool hasBoardWithNumber(BoardNumber number)
    {
        return this.boardContexts.Any(context => context.BoardNumber == number);
    }

    /**
     * @brief Returns the deal with the given number.
     */
    public BoardContext? getBoard(BoardNumber number)
    {
        var board = this.boardContexts.FirstOrDefault(context => context.BoardNumber == number);

        return board;
    }

    /**
     * @brief All the tokens of a file
     */
    public IReadOnlyList<SemanticPbnToken> Tokens() => this.tokens;

    /**
     * @brief Returns observer ptr of the token at specified at.
     * @param index Index of the token.
     * @throws std::out_of_range_error if the index is out of range.
     */
    public SemanticPbnToken this[int index]
    {
        get
        {
            return this.tokens[index];
        }
    }

    /**
     * @brief Appends a token to the end of the file.
     *
     * @param token The token to append.
     */
    public void AppendToken(SemanticPbnToken token)
    {
        tokens.Add(token);

        if (!(token is Tag tag) || !Tags.IsBoardScopeTag(tag.Tagname) )
        {
            return;
        }

        bool createNewBoardContext = boardContexts.Count == 0 || !boardContexts.Last().AcceptsToken(tag);


        if (createNewBoardContext)
        {
            var newBoardContext = new BoardContext(getNewBoardContextId(), this);
            boardContexts.Add(newBoardContext);
            BoardContextIdToTokenRange[newBoardContext.Id] = new TokenRange { StartIndex = this.tokens.Count - 1, TokenCount = 1 };
            newBoardContext.ApplyTag(tag);
        }
        else
        {
            var bc = boardContexts.Last();
            var current = BoardContextIdToTokenRange[bc.Id];
            BoardContextIdToTokenRange[bc.Id] = current with { TokenCount = current.TokenCount + 1 };
            bc.ApplyTag(tag);
        }
    }

    /**
     * @brief Inserts a token at the given index.
     *
     * @param at
     * @param token
     */
    public void InsertToken(int at, SemanticPbnToken token)
    {
        if (at < 0 || at > this.tokens.Count)
        {
            throw new IndexOutOfRangeException("Insert token: Index out of range");
        }

        for (int id = 0; id < BoardContextIdToTokenRange.Count; id++)
        {
            var range = BoardContextIdToTokenRange[(BoardContextId)id];
            if (range.StartIndex >= at)
            {
                range.StartIndex++;
            }
            else if (range.StartIndex + range.TokenCount > at)
            {
                range.TokenCount++;
                if (token is Tag)
                {
                    boardContexts[id].ApplyTag(token as Tag);
                }
            }
        }

        tokens.Insert((int)at, token);
    }

    public void ReplaceToken(int at, SemanticPbnToken with)
    {
        if (at >= tokens.Count)
            throw new ArgumentOutOfRangeException("at", "Index out of range");

        var from = tokens[(int)at];
        var id = FindRange(from);
        if (id.HasValue)
        {
            if (from is Tag tag)
            {
                boardContexts[id.Value].UnapplyTag(tag);
            }
            if (with is Tag tag1)
            {
                boardContexts[id.Value].ApplyTag(tag1);
            }
        }

        tokens[(int)at] = with;
    }

    public void ReplaceToken(SemanticPbnToken from, SemanticPbnToken to)
    {
        var it = tokens.FindIndex(p => p == from);
        if (it < 0)
            throw new ArgumentException("Token not found in token vector", nameof(from));

        ReplaceToken((int)it, to);
    }

    public void DeleteTokenAt(int at)
    {
        if (at >= tokens.Count)
            throw new ArgumentOutOfRangeException("at", "Index out of range");

        tokens.RemoveAt((int)at);
    }

    public void DeleteToken(SemanticPbnToken token)
    {
        var it = tokens.FindIndex(p => p == token);
        if (it < 0)
            return;

        DeleteToken((int)it);
    }

    public void Serialize(TextWriter writer)
    {
        foreach (var token in tokens)
        {
            token.Serialize(writer);
            writer.WriteLine();
        }
    }


    /// @brief All tokens in the file.
    private List<SemanticPbnToken> tokens = new();

    /// @brief All board contexts in the file.
    private List<BoardContext> boardContexts = new();


    /// @brief A BoardContext is a collection of tags in a PbnFile that are relevant for a single board.
    public record BoardContext
    {

        public readonly BoardContextId Id;
        public BoardNumber BoardNumber
        {
            /// @brief Returns the board number of this context.
            get;
            private set;
        }
        private PbnFile pbnFile;

        public BoardContext(int id, PbnFile pbnFile) 
        {
            this.Id = id;
            this.pbnFile = pbnFile;
        }



        /// @brief Apply the given token to this context. Used to validate file state.
        public void ApplyTag(Tag token)
        {
            if (token.Tagname == Tags.Board)
            {
                Debug.Assert(this.BoardNumber == 0, "Internal error: Board number is already set.");
                this.BoardNumber = int.Parse(token.Content);
            }
        }

        /// @brief Remove tag from this context. Used to validate file state.
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

        public bool AcceptsToken(SemanticPbnToken token, int? atIndex = null)
        {
            if (atIndex == null)
            {
                atIndex = this.Tokens.Count;
            }

            if (!(token is Tag tag)) return true;

            if (Tags.IsTagRecognized(tag.Tagname))
            {
                return !Tokens.Any(x =>
                {
                    if (!(x is Tag tag2)) return false;

                    return tag2.Tagname == tag.Tagname;
                });
            }

            return true;
        }

        /**
         * @brief Returns the tokens that are part of this context.
         * Invalidates when a token is added or removed from this context.
         * @return A span of tokens.
         */
        public IReadOnlyList<SemanticPbnToken> Tokens
        {
            get
            {

                var range = this.pbnFile.BoardContextIdToTokenRange[Id];

                var start = this.pbnFile.tokens.GetRange(range.StartIndex, range.TokenCount);
                return start;
            }
        }
    }




    private BoardContextId nextBoardContextId = 0;

    private BoardContextId getNewBoardContextId()
    {
        return nextBoardContextId++;
    }

    /// @brief Maps board context id to token ranges, tuples represent in order: start index, number of tokens
    internal record struct TokenRange
    {
        /// @brief Index of the first token in the range in the tokens vector.
        public int StartIndex;
        /// @brief Number of tokens in the range, starting at 1.
        public int TokenCount;
    }

    /// @brief Maps board context id to token ranges, tuples represent in order: start index, number of tokens
    internal Dictionary<BoardContextId, TokenRange>
        BoardContextIdToTokenRange = new();


    private BoardContextId? FindRange(int tokenIndex)
    {
        foreach (var pair in BoardContextIdToTokenRange)
        {
            var range = pair.Value;
            if (range.StartIndex <= tokenIndex && range.StartIndex + range.TokenCount > tokenIndex)
                return pair.Key;
        }
        return null;
    }

    private BoardContextId? FindRange(SemanticPbnToken token)
    {
        var it = tokens.FindIndex(p => p == token);
        if (it < 0)
            throw new ArgumentException("Token not found in token vector", nameof(token));

        return FindRange((int)it);
    }

    IEnumerable<SemanticPbnToken> rangeTokens(BoardContextId id)
    {
        var range = BoardContextIdToTokenRange[id];
        return this.tokens.GetRange((int)range.StartIndex, (int)(range.StartIndex + range.TokenCount));
    }

    /**
     * @brief Delete given token
     * Does nothing if token is not in the file.
     */
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
                    boardContexts[key].UnapplyTag(tag);
                }
            }
        }

        tokens.RemoveAt(at);

        // TODO validate state
    }
}

