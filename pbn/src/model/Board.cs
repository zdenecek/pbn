namespace pbn.model;

public record struct Board(int Number, Vulnerability Vulnerability, Position Dealer, string CardString)
{
}