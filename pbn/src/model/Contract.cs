namespace pbn.model;

/// Represents a bridge contract without a result.
public record struct Contract(int Level, Suit Suit, Position Declarer,
    ContractDoubleState DoubleState = ContractDoubleState.NotDoubled);

/// Represents a contract double state, whether it is not doubled, doubled or redoubled.
public enum ContractDoubleState
{
    NotDoubled,
    Doubled,
    Redoubled
}