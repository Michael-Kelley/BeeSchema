
namespace BeeSchema {
	enum TokenType {
		None,

		OpenBrace, CloseBrace,
		OpenBracket, CloseBracket,
		OpenParen, CloseParen,
		Colon, Semicolon,
		Comma,

		Assignment,
		Asterisk, Divide, Plus, Minus,
		Less, Greater, Equal, NotEqual, LessOrEqual, GreaterOrEqual,
		Not, Or, And,

		LineComment, BlockComment,

		Number, Word
	}
}