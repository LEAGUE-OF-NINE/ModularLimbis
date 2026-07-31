using System.Collections.Concurrent;

namespace ModularSkillScripts;

internal static class ScriptCache
{
	private static readonly ConcurrentDictionary<string, ScriptBatch> BatchCache = new();
	private static readonly ConcurrentDictionary<string, CallExpr> ConsequenceCallCache = new();
	private static readonly ConcurrentDictionary<string, ValueAssignExpr> ValueAssignCache = new();
	private static readonly ConcurrentDictionary<string, NumArg> NumArgCache = new();
	private static readonly ConcurrentDictionary<string, MathExpr> MathCache = new();
	private static readonly ConcurrentDictionary<string, CondExpr> CondCache = new();

	public static ScriptBatch GetBatch(string batchText) => BatchCache.GetOrAdd(batchText, ScriptBatch.Parse);
	public static CallExpr GetConsequenceCall(string section) => ConsequenceCallCache.GetOrAdd(section, CallExpr.ParseConsequence);
	public static ValueAssignExpr GetValueAssign(string section) => ValueAssignCache.GetOrAdd(section, ValueAssignExpr.Parse);
	public static NumArg GetNumArg(string param) => NumArgCache.GetOrAdd(param, NumArg.Parse);
	public static MathExpr GetMath(string s) => MathCache.GetOrAdd(s, MathExpr.Parse);
	public static CondExpr GetCond(string param) => CondCache.GetOrAdd(param, CondExpr.Parse);
}