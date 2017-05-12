using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
/// <remarks>
/// This will handle cell states similarly to how a Unit will handle Unit states, 
/// or the (custom) Skill class will handle Skill states. 
/// The difference is that this class will also have access to all units and skills as well. 
/// </remarks>
public class CellGrid : MonoBehaviour
{
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;
    
    private CellGridState _cellGridState;//The grid delegates some of its behaviours to cellGridState object.
    public CellGridState CellGridState
    {
        private get
        {
            return _cellGridState;
        }
        set
        {
            if(_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public Player CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }

    /// <summary>
    /// This is the game object whose children each contain a "Player" script. 
    /// This will define how many sides are in the game. 
    /// </summary>
    public Transform PlayersParent;
    /// <summary>
    /// This is the parent object whose children must each contain a "Skill" script. 
    /// These children may be UI object or sprites, so long as they contain the script and can process mouse-enter actions. 
    /// </summary>
    public Transform SkillsParent;

    /// <summary>
    /// Reference to all Player scripts in the game, which controls how many sides can participate in the match. 
    /// </summary>
    public List<Player> Players { get; private set; }
    /// <summary>
    /// Reference to all cells in the game. 
    /// </summary>
    public List<Cell> Cells { get; private set; }
    /// <summary>
    /// Reference to all units currently on the map. 
    /// </summary>
    public List<Unit> Units { get; private set; }
    /// <summary>
    /// These are the Skill scripts in the game. 
    /// They can be attached to sprites or UI elements - however we want to display them (probably). 
    /// </summary>
    public List<Skill> Skills { get; private set; }

    void Start()
    {
        Players = new List<Player>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);

        Cells = new List<Cell>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
                Cells.Add(cell);
            else
                Debug.LogError("Invalid object in cells parent game object");
        }
      
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }
             
        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(Cells);
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");

        // Adding code here to keep track of all skills in the current game. 
        // These are basically just the empty buttons but we need to make sure 
        // their event handlers are hooked up properly to this central CellGrid manager. 
        Skills = new List<Skill>();
        for (int i = 0; i < SkillsParent.childCount; i++)
        {
            Skill skill = SkillsParent.GetChild(i).GetComponent<Skill>();
            if (skill != null)
                Skills.Add(skill);
            else
                Debug.LogError("Invalid object in Skills Parent game object!");
        }

        StartGame();
    }

    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as Cell);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());
            // Can I just this without Invoke?: 
            // (Yes, but it will prompt you to simplify to a null operator that only works on .NET 4.0.)
            // GameEnded(this, new EventArgs());
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        if(GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        // Added Skill event so the skills can turn themselves off when a turn starts. 
        Skills.ForEach(u => { u.OnTurnStart(); });  

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }
    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        CellGridState = new CellGridStateTurnChanging(this);

        // I don't really expect this to do anything but who knows. 
        Skills.ForEach(u => { u.OnTurnEnd(); });

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        }//Skipping players that are defeated.

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());

        // Added Skill event so the skills can turn themselves off when a turn starts. 
        Skills.ForEach(u => { u.OnTurnStart(); });

        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);     
    }
}
