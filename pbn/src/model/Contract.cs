namespace pbn.model;

public record struct Contract(int Level, Suit Suit, Position Declarer,
    ContractDoubleState DoubleState = ContractDoubleState.NotDoubled);

public enum ContractDoubleState
{
    NotDoubled,
    Doubled,
    Redoubled
}