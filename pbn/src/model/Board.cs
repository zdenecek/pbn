namespace pbn.model;



/// <summary>
/// Represents a bridge board.
/// </summary>
/// <param name="Number">Board number</param>
/// <param name="Vulnerability">Board Vulnerability</param>
/// <param name="Dealer">Dealer</param>
/// <param name="CardString">Cards in the PBN format</param>
public record struct Board(int Number, Vulnerability Vulnerability, Position Dealer, string CardString);