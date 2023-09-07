namespace pbn.model;

/// <summary>
///     Represents a bridge contract without a result.
/// </summary>
public record struct Contract(int Level, Suit Suit, Position Declarer,
    ContractDoubleState DoubleState = ContractDoubleState.NotDoubled);

/// <summary>
///     Represents a contract double state, whether it is not doubled, doubled or redoubled.
/// </summary>
public enum ContractDoubleState
{
    NotDoubled,
    Doubled,
    Redoubled
}