using System;

namespace ModularSkillScripts.Acquirer;

public class AcquirerTimeGet : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		var balls = circles[0];
		int year = 1984;
		if (circles.Length >= 2)
		{
			year = modular.GetNumFromParamString(circles[1]);
		}

		return balls switch
		{
			"dayofweek" => (int)DateTime.Now.DayOfWeek,
			"dayofmonth" => DateTime.Now.Day,
			"dayofyear" => DateTime.Now.DayOfYear,
			"hours" => DateTime.Now.Hour,
			"minutes" => DateTime.Now.Minute,
			"seconds" => DateTime.Now.Second,
			"milliseconds" => DateTime.Now.Millisecond,
			"ticks" => (int)DateTime.Now.Ticks,
			"month" => DateTime.Now.Month,
			"year" => DateTime.Now.Year,
			"isleapyear" => DateTime.IsLeapYear(year) ? 0 : 1,
			_ => -1
		};
	}
}