/// <summary>
/// This may be slight overkill but if we are using events to set states, then we should do it all the way. 
/// Another way to do this would be to use the Unity UI event handling system, but it's either one or the other here. 
/// The "skills" can potentially be sprites or UI elements this way, I suppose. 
/// </summary>
/// <remarks>
/// This is basically mirroring the system set up for UnitState. 
/// </remarks>
public abstract class SkillState
{
    protected Skill _skill;

    public SkillState(Skill skill)
    {
        _skill = skill;
    }

    public abstract void Apply();
    public abstract void MakeTransition(SkillState state);
}