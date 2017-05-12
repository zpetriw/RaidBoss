using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We will use this class to do the mechanics for a square that sits in the front row of the player's party.
/// This way, any melee class in the front row can hit the boss squares. 
/// </summary>
public abstract class MeleeSquare : Cell {

    /// <summary>
    /// We will limit this so that the only direction the melee party can attack is "upwards"
    /// towards the boss and any potential minions. 
    /// </summary>
    protected static readonly Vector2[] _directions =
    {
        new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
        //new Vector2(0, 1)
    };

    /// <summary>
    /// For calculating distance we will only consider the vertical distance between characters for now. 
    /// I believe that this is mostly used for calculating attack range.
    /// It may need to be adjusted if it is used for other things. 
    /// </summary>
    /// <param name="other">The other cell to which we are comparing distance. </param>
    /// <returns></returns>
    public override int GetDistance(Cell other)
    {
        //return (int)(Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y));
        return (int)Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y);
    }

    /// <summary>
    /// We can probably leave this as is. 
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    public override List<Cell> GetNeighbours(List<Cell> cells)
    {
        List<Cell> ret = new List<Cell>();
        foreach (var direction in _directions)
        {
            var neighbour = cells.Find(c => c.OffsetCoord == OffsetCoord + direction);
            if (neighbour == null) continue;

            ret.Add(neighbour);
        }
        return ret;
    }
    //Each square cell has four neighbors, which positions on grid relative to the cell are stored in _directions constant.
    //It is totally possible to implement squares that have eight neighbours, it would require modification of GetDistance function though.
}
