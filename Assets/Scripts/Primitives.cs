using System;
[Flags]
public enum NeighbourType
{
    None =0,
    Left = 1<<0,
    Right = 1<<1,
    Top = 1<<2,
    Bottom = 1<<3
}

public enum Team
{
    Player1,
    Player2
}