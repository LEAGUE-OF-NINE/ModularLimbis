grammar ModsaLanguage;

// Parser rules
program : expression EOF ; // A program is a single expression

expression
    : '(' expression ')'                  # ParenExpression
    | expression op=('*'|'/') expression  # MulDivExpression
    | expression op=('+'|'-') expression  # AddSubExpression
    | ID '(' expression ')'               # FunctionExpression
    | NUMBER                              # NumberExpression
    | ID                                  # VariableExpression
    ;

// Lexer rules
ID      : [a-zA-Z]+ ;        // Variable or function names (e.g., "x", "sin")
NUMBER  : [0-9]+ ('.' [0-9]+)? ; // Numbers (e.g., "1", "2.5")
WS      : [ \t\r\n]+ -> skip ; // Ignore whitespace

// Operators
MUL : '*' ;
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;