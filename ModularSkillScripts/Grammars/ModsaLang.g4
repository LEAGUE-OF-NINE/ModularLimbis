grammar ModsaLang;

// Parser rules
program : expression EOF ; // A program is a single expression

expression
    : '(' expression ')'                                            # CircleEx
    | expression op=(ADD|SUB|MUL|DIV|MAX|MIN) expression            # MathEx
    | MODVAL '(' expression ')'                                     # SetModvalEx
    | ID '(' expression ')'                                         # FunctionEx
    | NUMBER                                                        # NumberEx
    | MODVAL                                                        # ModvalEx
    | ID                                                            # VariableEx
    ;

// Lexer rules
MODVAL  : 'VALUE_' [0-9] ; // Modular VALUE
ID      : [a-zA-Z]+ ;        // Variable or function names (e.g., "x", "sin")
NUMBER  : [0-9]+ ('.' [0-9]+)? ; // Numbers (e.g., "1", "2.5")
WS      : [ \t\r\n]+ -> skip ; // Ignore whitespace

// Operators
ADD : '+' ;
SUB : '-' ;
MUL : '*' ;
DIV : '/' ;
MAX : '¡' ;
MIN : '!' ;