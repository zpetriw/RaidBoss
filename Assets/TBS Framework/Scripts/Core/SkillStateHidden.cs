/// <summary>
/// Use this when you want to not display the skill. Cases include:
/// 1. A unit is not selected. 
/// 2. It is not your turn. 
/// 3. The game has just started. 
/// </summary>
internal class SkillStateHidden : SkillState
{
    public SkillStateHidden(Skill skill) : base(skill)
    {
    }

    public override void Apply()
    {
        _skill.Hide();
    }

    public override void MakeTransition(SkillState state)
    {
        state.Apply();
        _skill.SkillState = state;
    }
}