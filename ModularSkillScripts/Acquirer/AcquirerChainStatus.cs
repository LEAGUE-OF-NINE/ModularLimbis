using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularSkillScripts.Acquirer
{
	public class AcquirerChainStatus : IModularAcquirer
	{
		public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
		{
			BattleUnitModel targetModel = modular.GetTargetModel(circledSection);
			if (targetModel == null) return -1;

			bool is_attacking = false;
			bool is_defending = false;
			var sinActionList = targetModel.GetSinActionList();
			foreach (SinActionModel sinAction in sinActionList)
			{
				UnitSinModel sinModel = sinAction.currentSelectSin;
				if (sinModel == null) continue;
				SkillModel skill = sinModel.GetSkill();
				if (skill == null) continue;

				DEFENSE_TYPE deftype = skill.GetDefenseType();
				if (deftype is DEFENSE_TYPE.ATTACK or DEFENSE_TYPE.COUNTER) is_attacking = true;
				if (deftype is DEFENSE_TYPE.GUARD or DEFENSE_TYPE.EVADE) is_defending = true;
			}
			if (is_attacking && is_defending) return 3;
			if (is_attacking) return 1;
			if (is_defending) return 2;
			return 0;
		}
	}
}
