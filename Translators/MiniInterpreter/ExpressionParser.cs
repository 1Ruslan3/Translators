using System.Globalization;

namespace MiniInterpreter
{
    public sealed class ExpressionParser
    {
        private readonly string _input;
        private readonly Dictionary<string, double> _vars;
        private readonly Dictionary<string, Function> _funcs;
        private int _pos;

        public ExpressionParser(string input, Dictionary<string, double> vars, Dictionary<string, Function> funcs)
        {
            _input = input;
            _vars = vars;
            _funcs = funcs;
            _pos = 0;
        }

        public INode Parse()
        {
            var result = ParseExpression();
            SkipWhitespace();
            if (_pos < _input.Length)
                throw new Exception("Лишние символы в выражении");
            return result;
        }

        private INode ParseExpression()
        {
            var left = ParseTerm();
            while (true)
            {
                SkipWhitespace();
                if (Match('+'))
                    left = new BinaryNode('+', left, ParseTerm());
                else if (Match('-'))
                    left = new BinaryNode('-', left, ParseTerm());
                else
                    break;
            }
            return left;
        }

        private INode ParseTerm()
        {
            var left = ParseFactor();
            while (true)
            {
                SkipWhitespace();
                if (Match('*'))
                    left = new BinaryNode('*', left, ParseFactor());
                else if (Match('/'))
                    left = new BinaryNode('/', left, ParseFactor());
                else
                    break;
            }
            return left;
        }

        private INode ParseFactor()
        {
            SkipWhitespace();
            if (Match('('))
            {
                var expr = ParseExpression();
                Expect(')');
                return expr;
            }

            if (char.IsLetter(Current()))
                return ParseIdentifierOrFunction();

            return ParseNumber();
        }

        private INode ParseIdentifierOrFunction()
        {
            string name = ParseIdentifier();
            SkipWhitespace();
            if (Match('('))
            {
                var args = new List<INode>();
                if (!Match(')'))
                {
                    do
                    {
                        args.Add(ParseExpression());
                    } while (Match(','));
                    Expect(')');
                }

                if (!_funcs.ContainsKey(name))
                    throw new Exception($"Функция '{name}' не определена.");

                return new FunctionNode(name, args, _vars, _funcs);
            }

            if (!_vars.ContainsKey(name))
                throw new Exception($"Неизвестный идентификатор: '{name}'");

            return new VariableNode(name, _vars);
        }

        private INode ParseNumber()
        {
            int start = _pos;
            while (_pos < _input.Length && (char.IsDigit(_input[_pos]) || _input[_pos] == '.'))
                _pos++;

            string token = _input.Substring(start, _pos - start);
            if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                throw new Exception("Ошибка разбора числа");

            return new ConstantNode(result);
        }

        private string ParseIdentifier()
        {
            int start = _pos;
            while (_pos < _input.Length && char.IsLetterOrDigit(_input[_pos]))
                _pos++;
            return _input.Substring(start, _pos - start);
        }

        private void SkipWhitespace()
        {
            while (_pos < _input.Length && char.IsWhiteSpace(_input[_pos])) _pos++;
        }

        private bool Match(char expected)
        {
            if (_pos < _input.Length && _input[_pos] == expected)
            {
                _pos++;
                return true;
            }
            return false;
        }

        private void Expect(char expected)
        {
            if (!Match(expected))
                throw new Exception($"Ожидался символ '{expected}'");
        }

        private char Current() => _pos < _input.Length ? _input[_pos] : '\0';
    }
}
