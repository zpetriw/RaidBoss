using System.Linq;

/// <summary>
/// Is this the actual state of the entire cell grid? 
/// We could potentially add a state like "WaitingForSkillSelection" 
/// </summary>
public abstract class CellGridState
{
    protected CellGrid _cellGrid;
    
    protected CellGridState(CellGrid cellGrid)
    {
        _cellGrid = cellGrid;
    }

    public virtual void OnUnitClicked(Unit unit)
    { }
    
    public virtual void OnCellDeselected(Cell cell)
    {
        cell.UnMark();
    }
    public virtual void OnCellSelected(Cell cell)
    {
        cell.MarkAsHighlighted();
    }
    public virtual void OnCellClicked(Cell cell)
    { }

    public virtual void OnStateEnter()
    {
        // I think what this does is that if the cell is ever entered and the match is down to one player,
        // the game immediately ends, since no one can take any further action. 
        if (_cellGrid.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
        {
            // This doesn't seem to be fully implemented. We would probably have to define what this does. 
            _cellGrid.CellGridState = new CellGridStateGameOver(_cellGrid);
        }
    }
    public virtual void OnStateExit()
    {
    }

    public virtual void OnSkillClicked(Skill skill)
    { }

    public virtual void OnSkillSelected(Skill skill)
    { }
}