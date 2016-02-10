
namespace BeeSchema {
	class Token {
		public readonly TokenType Type;
		public readonly int Line;
		public readonly int Column;
		public readonly string Value;

		public Token(TokenType type, int line, int column, string value) {
			Type = type;
			Line = line;
			Column = column;
			Value = value;
		}
	}
}