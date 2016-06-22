using System.IO;
using System.Linq;
using System.Text;

namespace BeeSchema {
	class Lexer {
		readonly TextReader reader;

		int line = 1;
		int column = 1;

		static readonly char[] delimiters = {
			'{', '}',
			'[', ']',
			'(', ')',
			':', ';',
			'*', '/', '+', '-',
			'<', '>', '=', '!',
			'|', '&',
			'#',
			',',
			' ', '\t', '\n', '\r'
		};

		public Lexer(TextReader reader) {
			this.reader = reader;
		}

		~Lexer() {
			if (reader != null)
				reader.Dispose();
		}

		public Token NextToken() {
			int ch;
			char c;

			do {
				ch = reader.Read();

				if (ch == -1)
					return null;

				c = (char)ch;

				if (c == '\t')
					column += 4;
				else
					column++;

				if (c == '\n') {
					line++;
					column = 1;
				}
			} while (char.IsWhiteSpace(c));

			switch (c) {
				case '{':
					return new Token(TokenType.OpenBrace, line, column - 1, null);
				case '}':
					return new Token(TokenType.CloseBrace, line, column - 1, null);
				case '[':
					return new Token(TokenType.OpenBracket, line, column - 1, null);
				case ']':
					return new Token(TokenType.CloseBracket, line, column - 1, null);
				case '(':
					return new Token(TokenType.OpenParen, line, column - 1, null);
				case ')':
					return new Token(TokenType.CloseParen, line, column - 1, null);
				case ':':
					return new Token(TokenType.Colon, line, column - 1, null);
				case ';':
					return new Token(TokenType.Semicolon, line, column - 1, null);
				case '*':
					return new Token(TokenType.Asterisk, line, column - 1, null);
				case '/':
					return new Token(TokenType.Divide, line, column - 1, null);
				case '+':
					return new Token(TokenType.Plus, line, column - 1, null);
				case '-':
					return new Token(TokenType.Minus, line, column - 1, null);
				case ',':
					return new Token(TokenType.Comma, line, column - 1, null);
				case '=':
					if (reader.Peek() == '=') {
						reader.Read();
						column++;
						return new Token(TokenType.Equal, line, column - 2, null);
					}

					return new Token(TokenType.Assignment, line, column - 1, null);
				case '!':
					if (reader.Peek() == '=') {
						reader.Read();
						column++;
						return new Token(TokenType.NotEqual, line, column - 2, null);
					}

					return new Token(TokenType.Not, line, column - 1, null);
				case '<':
					if (reader.Peek() == '=') {
						reader.Read();
						column++;
						return new Token(TokenType.LessOrEqual, line, column - 2, null);
					}

					return new Token(TokenType.Less, line, column - 1, null);
				case '>':
					if (reader.Peek() == '=') {
						reader.Read();
						column++;
						return new Token(TokenType.GreaterOrEqual, line, column - 2, null);
					}

					return new Token(TokenType.Greater, line, column - 1, null);
				case '|':
					if (reader.Peek() != '|')
						throw new System.Exception($"Invalid token!  Expected '|'.  Got '{reader.Peek()}'");

					reader.Read();
					column++;
					return new Token(TokenType.Or, line, column - 2, null);
				case '&':
					if (reader.Peek() != '&')
						throw new System.Exception($"Invalid token!  Expected '&'.  Got '{reader.Peek()}'");

					reader.Read();
					column++;
					return new Token(TokenType.And, line, column - 2, null);
			}

			var sb = new StringBuilder();
			Token r;

			if (c == '#') {
				if (reader.Peek() == '#') {
					var l = line;
					var col = column - 1;

					reader.Read();
					column++;

					while (true) {
						ch = reader.Read();
						column++;

						if (ch == -1 || (ch == '#' && reader.Peek() == '#')) {
							reader.Read();
							column += 2;
							break;
						}

						if (ch == '\n') {
							line++;
							column = 1;
						}

						c = (char)ch;

						sb.Append(c);
					}

					r = new Token(TokenType.BlockComment, l, col, sb.ToString());

					return r;
				}
				else {
					while (true) {
						ch = reader.Read();

						if (ch == -1 || ch == '\r' || ch == '\n')
							break;

						c = (char)ch;

						sb.Append(c);
					}

					r = new Token(TokenType.LineComment, line++, column - 1, sb.ToString());
					column = 1;

					return r;
				}
			}

			sb.Append(c);

			if (char.IsNumber(c)) {
				ch = reader.Peek();

				while (	ch != -1
						&& !delimiters.Contains((char)ch)
						&& (char.IsDigit((char)ch) || ch == '.') || ch == 'x') {
					reader.Read();
					c = (char)ch;
					sb.Append(c);
					ch = reader.Peek();
				}

				r = new Token(TokenType.Number, line, --column, sb.ToString());
				column += sb.Length;

				return r;
			}

			ch = reader.Peek();

			while (	ch != -1 && !delimiters.Contains((char)ch)) {
				reader.Read();
				c = (char)ch;
				sb.Append(c);
				ch = reader.Peek();
			}

			r = new Token(TokenType.Word, line, --column, sb.ToString());
			column += sb.Length;

			return r;
		}
	}
}