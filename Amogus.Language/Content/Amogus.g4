grammar Amogus;

program: line* EOF;

line: statement | ifBlock | whileBlock;

statement: (assignment|functionCall) ';';

ifBlock: 'if' expression block ('else' elseIfBlock)?;

elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block ('else' elseIfBlock);

WHILE: 'while' | 'until';

assignment: INDENTIFIER '=' expression;

functionCall: INDENTIFIER '(' (expression (',' expression)*) ')';

expression
	: constant							#constantExpression
	| INDENTIFIER						#indentifierExpression
	| functionCall						#functionCallExpression
	| '(' expression ')'				#parenthesizedExpression
	| '!' expression					#notExpression
	| expression multOp expression		#multiplicationExpression
	| expression addOp expression		#additiveExpression
	| expression compareOp expression	#comparisonExpression
	| expression boolOp expression		#booleanExpression
	;

multOp: '*' | '/' | '%' ;
addOp: '+' | '-' ;
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=' ;
boolOp: BOOL_OPERATOR ;

BOOL_OPERATOR: 'and' | 'or' | 'xor' ;

constant: INTEGER | FLOAT | STRING | BOOL | NULL;

INTEGER: [0-9]+ ;
FLOAT: [0-9]+ '.' [0-9]+ ;
STRING: ('*' ~'"' * '"') | ('\'' ~'\''* '\'') ;
NULL: 'null' ;
BOOL: 'true' | 'false' ;

block: '{' line* '}';

WS: [ \t\r\n] -> skip;
INDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;