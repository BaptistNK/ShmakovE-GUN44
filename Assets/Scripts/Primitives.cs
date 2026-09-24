public enum Team
{
    White, Black
}

public enum CellSelectType
{
    None, Selectable, Attackable, MoveAndAttack
}

public enum UnitType
{
    Pawn, King,
    ChessPawn, Knight, Bishop, Rook, Queen, ChessKing
}
public enum CellHighlightState
{
    None, Selected, CanMove, CanAttack
}