using UnityEngine;

public class CellGridStateGameOver : CellGridState
{
    /// <summary>
    /// We would have to implement what it means when this state is entered. 
    /// Probably we would head to a victory or defeat screen somewhere. 
    /// </summary>
    /// <param name="cellGrid"></param>
    public CellGridStateGameOver(CellGrid cellGrid) : base(cellGrid)
    {
        Debug.Log("[CellGridStateGameOver] constructor.");
    }

    public override void OnStateEnter()
    {
        Debug.Log("[CellGridStateGameOver] OnStateEnter().");
    }
}