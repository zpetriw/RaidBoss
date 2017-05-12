/// <summary>
/// This is a state that the skill has when it is selectable, but not yet selected. 
/// Note that we will need to have a state where the skill is "dead" or hidden from view. 
/// </summary>
internal class SkillStateNormal : SkillState
{
    public SkillStateNormal(Skill skill) : base(skill)
    {
    }

    public override void Apply()
    {
        _skill.Show();
    }

    public override void MakeTransition(SkillState state)
    {
        state.Apply();
        _skill.SkillState = state;
    }
}