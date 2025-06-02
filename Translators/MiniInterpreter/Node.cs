namespace MiniInterpreter
{
    public interface INode
    {
        double Evaluate();
    }

    public sealed class ConstantNode : INode
    {
        private readonly double _value;
        public ConstantNode(double value) => _value = value;
        public double Evaluate() => _value;
    }

    public sealed class VariableNode : INode
    {
        private readonly string _name;
        private readonly Dictionary<string, double> _vars;
        public VariableNode(string name, Dictionary<string, double> vars)
        {
            _name = name;
            _vars = vars;
        }
        public double Evaluate()
        {
            if (!_vars.TryGetValue(_name, out var value))
                throw new Exception($"Переменная '{_name}' не найдена.");
            return value;
        }
    }

    public sealed class BinaryNode : INode
    {
        private readonly char _op;
        private readonly INode _left, _right;
        public BinaryNode(char op, INode left, INode right)
        {
            _op = op;
            _left = left;
            _right = right;
        }
        public double Evaluate()
        {
            double l = _left.Evaluate();
            double r = _right.Evaluate();
            return _op switch
            {
                '+' => l + r,
                '-' => l - r,
                '*' => l * r,
                '/' => l / r,
                _ => throw new Exception($"Неизвестный оператор {_op}")
            };
        }
    }

    public sealed class FunctionNode : INode
    {
        private readonly string _name;
        private readonly List<INode> _arguments;
        private readonly Dictionary<string, double> _parentScope;
        private readonly Dictionary<string, Function> _functions;

        public FunctionNode(string name, List<INode> arguments, Dictionary<string, double> scope, Dictionary<string, Function> functions)
        {
            _name = name;
            _arguments = arguments;
            _parentScope = scope;
            _functions = functions;
        }

        public double Evaluate()
        {
            if (!_functions.ContainsKey(_name))
                throw new Exception($"Функция '{_name}' не определена");

            var func = _functions[_name];
            if (func.Parameters.Count != _arguments.Count)
                throw new Exception($"Функция '{_name}' требует {func.Parameters.Count} аргументов");

            var localVars = new Dictionary<string, double>(_parentScope);
            for (int i = 0; i < func.Parameters.Count; i++)
                localVars[func.Parameters[i]] = _arguments[i].Evaluate();

            var parser = new ExpressionParser(func.Expression, localVars, _functions);
            return parser.Parse().Evaluate();
        }
    }
}
