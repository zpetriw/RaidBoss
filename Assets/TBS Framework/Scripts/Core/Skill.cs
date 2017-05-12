using System;
using UnityEngine;
/// <summary>
/// This is a clickable skill whose information needs to be passed to the cell grid.
/// It will be similar to a Unit in the sense that a Skill needs to be selected (for a human)
/// before any action can be taken with that unit. 
/// </summary>
public abstract class Skill : MonoBehaviour
{
    /// <summary>
    /// SkillClicked event is invoked when user clicks the Skill. It requires a collider on the Skill game object to work.
    /// </summary>
    public event EventHandler SkillClicked;
    /// <summary>
    /// SkillSelected event is invoked when user clicks on Skill that belongs to him. It requires a collider on the Skill game object to work.
    /// </summary>
    public event EventHandler SkillSelected;
    /// <summary>
    /// This should handle events when the skill is deselected.
    /// </summary>
    public event EventHandler SkillDeselected;
    /// <summary>
    /// SkillHighlighted event is invoked when user moves cursor over the Skill. It requires a collider on the Skill game object to work.
    /// </summary>
    public event EventHandler SkillHighlighted;
    /// <summary>
    /// Handle events when the skill is dehighlighted.
    /// </summary>
    public event EventHandler SkillDehighlighted;


    public event EventHandler<SkillEventArgs> UnitAttacked;
    public event EventHandler<SkillEventArgs> UnitDestroyed;
    //public event EventHandler<MovementEventArgs> UnitMoved;   // Let's not have our skills move our units... yet. 

    /// <summary>
    /// Not 100% when this is used, but it should mirror UnitState in action somehow.
    /// </summary>
    public SkillState SkillState { get; set; }
    /// <summary>
    /// This will call the override method of "MakeTransition". 
    /// It can call it for any child of SkillState, but can never be called on SkillState because this is an abstract class. 
    /// </summary>
    /// <param name="state">This is the state to which we wish to transition this skill. It must be a child class of SkillState, 
    /// with actual implementations of the base class' abstract values. </param>
    public void SetState(SkillState state)
    {
        SkillState.MakeTransition(state);
    }

    /// <summary>
    /// This is the unit that owns this skill. 
    /// When choosing a target for the skill, we will look to the owner's orientation (player number) 
    /// to decide what it means to target an enemy, an ally, or self. 
    /// </summary>
    public Unit skillOwner { get; private set; }

    /// <summary>
    /// Note that we don't exactly want a list of buffs that this skill contains, but rather buffs that will be applied when the skill is applied. 
    /// This can be added later. 
    /// </summary>
    //public List<Buff> Buffs { get; private set; }

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// Is this necessary for a Skill? We can probably just draw it on a UI and not care what box its in. 
    /// It can send its own information through some method. Perhaps it occupies a number and then that corresponds to a skill owned by the unit. 
    /// </summary>
    //public Cell Cell { get; set; }

    /// <summary>
    /// Indicates the player that the unit belongs to. Should correspoond with PlayerNumber variable on Player script.
    /// Should this be changed to correspond to a Unit that the skill belongs to? Or just a skill number of a unit. 
    /// </summary>
    //public int PlayerNumber;

    /// <summary>
    /// Indicates if movement animation is playing.
    /// Should this be changed to "IsBeingCast" or something when the skill is being cast? Is this necessary? 
    /// Ideally you would just cast a skill and then ... deselect the unit if he has no action points, 
    /// or show the skill bar again if he does have action points. 
    /// </summary>
    //public bool isMoving { get; set; }

    /// <summary>
    /// Method called after object instantiation to initialize fields etc. 
    /// </summary>
    public virtual void Initialize()
    {
        //Buffs = new List<Buff>();

        //UnitState = new UnitStateNormal(this);

        // Do we want to initialize to a normal state, or to a hidden state maybe? 
        // I would argue (against whom?) that we should perhaps initialize a skill to a hidden state 
        // and then find a way to turn it on when necessary. Perhaps, when a unit is clicked. 
        SkillState = new SkillStateNormal(this);


        //TotalHitPoints = HitPoints;
        //TotalMovementPoints = MovementPoints;
        //TotalActionPoints = ActionPoints;
    }

    /// <summary>
    /// We run this when a skill is clicked. 
    /// Ideally this would select the skill and start figuring out available targets. 
    /// </summary>
    /// <remarks>
    /// Does a UI button have this event defined the same way as a sprite with a collider does?
    /// </remarks>
    protected virtual void OnMouseDown()
    {
        //SkillClicked?.Invoke(this, new EventArgs());  // Wah... we need .NET 4.0 for this awesome feature. 
        if (SkillClicked != null)
            SkillClicked.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Used largely only in GUIs. 
    /// When we just pan over this skill, we can highlight it. 
    /// We can use this to display info about the skill without selecting it. 
    /// We can also use this to display available targets if we get lazy and don't want to lift our fingers. 
    /// Not sure if we can fire from here because that would require hovering over a target, and thus we are no longer hovering over this skill. 
    /// </summary>
    /// <remarks>
    /// Does a UI button have this event defined the same way as a sprite with a collider does?
    /// Note that these Event Handlers are typically only used by GUIs.
    /// I.e. no action on the game board happens when something is highlighted. 
    /// It is an innocuous action and it is entirely possible that this function goes unused. 
    /// </remarks>
    protected virtual void OnMouseEnter()
    {
        if (SkillHighlighted != null)
            SkillHighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Used largely only in GUIs. 
    /// </summary>
    /// <remarks>
    /// Does a UI button have this event defined the same way as a sprite with a collider does?
    /// No game-related action happens when the skill is unhighlighted. 
    /// It is entirely possible that this can go unused. 
    /// </remarks>
    protected virtual void OnMouseExit()
    {
        if (SkillDehighlighted != null)
            SkillDehighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
        //MovementPoints = TotalMovementPoints;
        //ActionPoints = TotalActionPoints;

        // Set state to ... hidden? 
        SetState(new SkillStateHidden(this));

        // Note: when we start the game, every unit starts as normal. 
        // Only when your turn starts do you mark units as friendly. 
        // This is a small distinction and probably not critical to anything except appearances. 
        //SetState(new UnitStateMarkedAsFriendly(this));
    }
    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        // Hide the skill when the turn ends. 
        // This is overkill because it will be called when the next player starts his turn. 
        // Maybe we can add something else here, but a state change to the same state is not necessary. 
        //SetState(new SkillStateHidden(this));

        //Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        //Buffs.RemoveAll(b => b.Duration == 0);
        //Buffs.ForEach(b => { b.Duration--; });

        //SetState(new UnitStateNormal(this));
    }
    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    /// <remarks>
    /// Note that skills are part of the UI and are thus never destroyed, only hidden. 
    /// </remarks>
    //protected virtual void OnDestroyed()
    //{
    //    Cell.IsTaken = false;
    //    MarkAsDestroyed();
    //    Destroy(gameObject);
    //}

    /// <summary>
    /// Method is called when the skill is selected.
    /// </summary>
    /// <remarks>
    /// The state that we transition to will be pretty eim
    /// </remarks>
    public virtual void OnSkillSelected()
    {
        SetState(new SkillStateMarkedAsSelected(this));
        if (SkillSelected != null)
            SkillSelected.Invoke(this, new EventArgs());
    }
    /// <summary>
    /// Method is called when the skill is deselected.
    /// </summary>
    public virtual void OnSkillDeselected()
    {
        SetState(new SkillStateNormal(this));
        if (SkillDeselected != null)
            SkillDeselected.Invoke(this, new EventArgs());

        //SetState(new UnitStateMarkedAsFriendly(this));
        //if (UnitDeselected != null)
        //    UnitDeselected.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method indicates if it is possible to attack unit given as parameter, from cell given as second parameter.
    /// </summary>
    /// <remarks>
    /// This will be a very important method for us. 
    /// We will need to determine if the unit is targetable, and this information will come from both the skill 
    /// and the orientation of its owner. 
    /// </remarks>
    public virtual bool IsUnitAttackable(Unit other, Cell sourceCell)
    {
        if (sourceCell.GetDistance(other.Cell) <= AttackRange)
            return true;

        return false;
    }
    /// <summary>
    /// Method deals damage to unit given as parameter.
    /// This could probably be implemented on each unit specifically if needed. 
    /// </summary>
    public virtual void DealDamage(Unit other)
    {
        if (isMoving)
            return;
        if (ActionPoints == 0)
            return;
        if (!IsUnitAttackable(other, Cell))
            return;

        MarkAsAttacking(other);
        ActionPoints--;
        other.Defend(this, AttackFactor);

        if (ActionPoints == 0)
        {
            SetState(new UnitStateMarkedAsFinished(this));
            MovementPoints = 0;
        }
    }

    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    //public abstract void MarkAsDefending(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    //public abstract void MarkAsAttacking(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    //public abstract void MarkAsDestroyed();

    /// <summary>
    /// Method marks unit as current players unit.
    /// </summary>
    //public abstract void MarkAsFriendly();
    /// <summary>
    /// Method mark units to indicate user that the unit is in range and can be attacked.
    /// </summary>
    //public abstract void MarkAsReachableEnemy();
    /// <summary>
    /// Method marks skill as currently selected, to distinguish it from other skills.
    /// </summary>
    public abstract void MarkAsSelected();
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    //public abstract void MarkAsFinished();
    /// <summary>
    /// Method returns the unit to its base appearance.
    /// Note that this was previously called "UnMark" but "Show" seems like a better name. 
    /// </summary>
    public abstract void Show();
    /// <summary>
    /// This method will hide the skill. Use cases include: 
    /// 1. It is not the player's turn. 
    /// 2. The turn or game has just started. 
    /// 3. No unit is selected. 
    /// 4. Skill is attacking (or do we leave it up?)
    /// </summary>
    public abstract void Hide();

    /// <summary>
    /// This class will contain skill information that will then be afflicted onto whatever target we're selecting. 
    /// </summary>
    public class SkillEventArgs : EventArgs
    {
        public Unit Attacker;
        public Unit Defender;

        // You can modify this to be its own class of fire, cold, physical, arcane, etc. damage. 
        public int Damage;
        
        // Add some status effects to fly their way also. 
        // This include their effect, as well as their application chance. i.e. 90% or 140%. 

        /// <summary>
        /// This will have to be beefed up to also inflict StatusEffects onto the target. 
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public SkillEventArgs(Unit attacker, Unit defender, int damage)
        {
            Attacker = attacker;
            Defender = defender;

            Damage = damage;
        }
    }

}