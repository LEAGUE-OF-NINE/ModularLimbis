namespace ModularSkillScripts.Consequence;

public class ConsequenceChangeMap : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		string mapname = circles[0];
		float mapsize = modular.GetNumFromParamString(circles[1]);
		BattleMapManager.Instance.LoadAndAddMap(mapname, mapsize);
		BattleMapManager.Instance.ChangeMap(mapname, mapsize);
	}
}