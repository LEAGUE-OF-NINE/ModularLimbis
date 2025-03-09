// Generated from /media/glitch/ExtraWorld/repos/ModularSkillScripts repos/ModularSkillScripts/Grammars/ModsaLanguage.g4 by ANTLR 4.13.2
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link ModsaLanguageParser}.
 */
public interface ModsaLanguageListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link ModsaLanguageParser#program}.
	 * @param ctx the parse tree
	 */
	void enterProgram(ModsaLanguageParser.ProgramContext ctx);
	/**
	 * Exit a parse tree produced by {@link ModsaLanguageParser#program}.
	 * @param ctx the parse tree
	 */
	void exitProgram(ModsaLanguageParser.ProgramContext ctx);
	/**
	 * Enter a parse tree produced by the {@code AddSubExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterAddSubExpression(ModsaLanguageParser.AddSubExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code AddSubExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitAddSubExpression(ModsaLanguageParser.AddSubExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code FunctionExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterFunctionExpression(ModsaLanguageParser.FunctionExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code FunctionExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitFunctionExpression(ModsaLanguageParser.FunctionExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ParenExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterParenExpression(ModsaLanguageParser.ParenExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ParenExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitParenExpression(ModsaLanguageParser.ParenExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code NumberExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterNumberExpression(ModsaLanguageParser.NumberExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code NumberExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitNumberExpression(ModsaLanguageParser.NumberExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code VariableExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterVariableExpression(ModsaLanguageParser.VariableExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code VariableExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitVariableExpression(ModsaLanguageParser.VariableExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code MulDivExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMulDivExpression(ModsaLanguageParser.MulDivExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code MulDivExpression}
	 * labeled alternative in {@link ModsaLanguageParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMulDivExpression(ModsaLanguageParser.MulDivExpressionContext ctx);
}