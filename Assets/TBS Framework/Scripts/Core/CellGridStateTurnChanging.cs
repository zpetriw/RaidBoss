using UnityEngine;

/// <summary>
/// This is an amazing function that is called when the turn changes. Wow. 
/// </summary>
class CellGridStateTurnChanging : CellGridState
{
    public CellGridStateTurnChanging(CellGrid cellGrid) : base(cellGrid)
    {
        Debug.Log("Turn over for player " + cellGrid.CurrentPlayerNumber);
    }
}

