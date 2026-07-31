using System;
using System.Collections.Generic;
using System.Linq;
using Regex = System.Text.RegularExpressions.Regex;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

namespace ModularSkillScripts;
internal enum NumArgKind
{
	Literal,
	ValueRef,
	Math,
	Acquire
}
internal enum CondMode
{
	AND,
	OR,
	XOR
}
internal enum BatchElementKind
{
	ContinueIfNot,
	ContinueIf,
	IfNot,
	If,
	ValueAssign,
	Consequence
}
internal class ModularScript
{
	public List<ScriptBatch> Batches = new();
}
internal class ScriptBatch
{
	public List<BatchPart> Parts = new();

	public static ScriptBatch Parse(string batchText)
	{
		var result = new ScriptBatch();
		string[] batchArgs = batchText.Split(':');
		for (int i = 0; i < batchArgs.Length; i++)
		{
			string Part = batchArgs[i];
			if (Part.StartsWith("CONTINUEIFNOT"))
			{
				result.Parts.Add(BatchPart.MakeCond(BatchElementKind.ContinueIfNot, ScriptCache.GetCond(Part)));
			}
			else if (Part.StartsWith("STOPIF") || Part.StartsWith("CONTINUEIF"))
			{
				result.Parts.Add(BatchPart.MakeCond(BatchElementKind.ContinueIf, ScriptCache.GetCond(Part)));
			}
			else if (Part.StartsWith("IFNOT"))
			{
				result.Parts.Add(BatchPart.MakeCond(BatchElementKind.IfNot, ScriptCache.GetCond(Part)));
			}
			else if (Part.StartsWith("IF"))
			{
				result.Parts.Add(BatchPart.MakeCond(BatchElementKind.If, ScriptCache.GetCond(Part)));
			}
			else if (Part.StartsWith("VALUE_"))
			{
				string numChar = Part[6].ToString();
				int.TryParse(numChar, out int valueidx);
				string rhs = batchArgs[i + 1];
				result.Parts.Add(BatchPart.MakeValueAssign(valueidx, ScriptCache.GetValueAssign(rhs)));
				i += 1;
			}
			else
			{
				result.Parts.Add(BatchPart.MakeConsequence(ScriptCache.GetConsequenceCall(Part)));
			}
		}
		return result;
	}
}

internal class BatchPart
{
	public BatchElementKind Kind;

	public CondExpr Cond;               
	public int ValueIndex;               
	public ValueAssignExpr Assign;      
	public CallExpr Call;                

	public static BatchPart MakeCond(BatchElementKind kind, CondExpr cond) => new() { Kind = kind, Cond = cond };
	public static BatchPart MakeValueAssign(int index, ValueAssignExpr assign) => new() { Kind = BatchElementKind.ValueAssign, ValueIndex = index, Assign = assign };
	public static BatchPart MakeConsequence(CallExpr call) => new() { Kind = BatchElementKind.Consequence, Call = call };
}
internal class CallExpr
{
	public string Method;
	public string CircledSection;
	public string[] Circles;
	public string Raw;

	public static CallExpr ParseConsequence(string section)
	{
		string[] sectionArgs = section.Split(ModularSA.parenthesisSeparator);
		string method = sectionArgs[0];
		string circledSection = sectionArgs.Length >= 2 ? sectionArgs[1] : "";
		string[] circles = circledSection.Split(',');
		return new CallExpr { Method = method, CircledSection = circledSection, Circles = circles, Raw = section };
	}

	public static CallExpr ParseAcquirer(string section, string[] sectionArgs)
	{
		string methodology = sectionArgs[0];
		string circledSection = sectionArgs.Length > 1 ? sectionArgs[1] : "";
		string[] circles = circledSection.Length > 0 ? circledSection.Split(',') : Array.Empty<string>();
		return new CallExpr { Method = methodology, CircledSection = circledSection, Circles = circles, Raw = section };
	}
}
internal class ValueAssignExpr
{
	public bool DirectNumeric;
	public NumArg Direct;  
	public CallExpr Call;
	public string Raw;

	public static ValueAssignExpr Parse(string section)
	{
		var expr = new ValueAssignExpr { Raw = section };
		string[] sectionArgs = section.Split(ModularSA.parenthesisSeparator);

		if (char.IsNumber(section.Last()))
		{
			expr.DirectNumeric = true;
			expr.Direct = ScriptCache.GetNumArg(sectionArgs[0]);
			return expr;
		}

		expr.Call = CallExpr.ParseAcquirer(section, sectionArgs);
		return expr;
	}
}
internal class NumArg
{
	public NumArgKind Kind;
	public bool Negative;

	public int Literal;            
	public int ValueIndex;        
	public MathExpr Math;           
	public ValueAssignExpr Acquire; 

	public static NumArg Parse(string param)
	{
		var result = new NumArg();

		bool negative = param[0] == '-';
		if (negative) param = param.Remove(0, 1);
		bool math = param[0] == 'm';
		if (math) param = param.Remove(0, 1);
		bool acquire = param[0] == 'G';
		if (acquire) param = param.Remove(0, 1);
		if (param.Last() == ')') param = param.Remove(param.Length - 1);

		result.Negative = negative;

		if (math)
		{
			result.Kind = NumArgKind.Math;
			result.Math = ScriptCache.GetMath(param);
		}
		else if (param.StartsWith("VALUE_"))
		{
			result.Kind = NumArgKind.ValueRef;
			int.TryParse(param[6].ToString(), out int value_idx);
			result.ValueIndex = value_idx;
		}
		else if (acquire)
		{
			param = Regex.Replace(param, @"{", "(");
			param = Regex.Replace(param, @"}", ")");
			param = Regex.Replace(param, @"-", ",");
			result.Kind = NumArgKind.Acquire;
			result.Acquire = ScriptCache.GetValueAssign(param);
		}
		else
		{
			result.Kind = NumArgKind.Literal;
			int.TryParse(param, out result.Literal);
		}

		return result;
	}
}
internal class MathTerm
{
	public char Op;
	public NumArg Value;
}

internal class MathExpr
{
	public List<MathTerm> Terms = new();

	public static MathExpr Parse(string s)
	{
		var expr = new MathExpr();
		var symbols = MainClass.mathsymbolRegex.Matches(s);
		string[] parameters = s.Split(MainClass.mathSeparator);

		expr.Terms.Add(new MathTerm { Op = '\0', Value = ScriptCache.GetNumArg(parameters[0]) });
		for (int i = 0; i < symbols.Count; i++)
		{
			string param = parameters[i + 1];
			char symbol = symbols[i].Value[0];
			expr.Terms.Add(new MathTerm { Op = symbol, Value = ScriptCache.GetNumArg(param) });
		}

		return expr;
	}
}
internal class CondClause
{
	public NumArg Left;
	public char Op; // '<', '>', '='
	public NumArg Right;
}
internal class CondExpr
{
	public CondMode Mode;
	public List<CondClause> Clauses = new();

	public static CondExpr Parse(string param)
	{
		string[] circles = param.Split(ModularSA.parenthesisSeparator)[1].Split(',');
		var expr = new CondExpr();

		int mode = -1; // AND
		switch (circles[0])
		{
			case "AND": mode = 0; break;
			case "OR": mode = 1; break;
			case "XOR": mode = 2; break;
		}

		int idx = 0;
		if (mode == -1) mode = 0;
		else idx++;
		expr.Mode = (CondMode)mode;

		for (int i = idx; i < circles.Length; i++)
		{
			string circle_string = circles[i];
			var symbols = Regex.Matches(circle_string, "(<|>|=)", RegexOptions.IgnoreCase, TimeSpan.FromMinutes(1));
			string[] parameters = circle_string.Split(ModularSA.CompareSeparator);
			string firstParam = parameters[0];
			string secondParam = parameters[1];
			char symbol = symbols[0].Value[0];

			expr.Clauses.Add(new CondClause
			{
				Left = ScriptCache.GetNumArg(firstParam),
				Op = symbol,
				Right = ScriptCache.GetNumArg(secondParam)
			});
		}

		return expr;
	}
}