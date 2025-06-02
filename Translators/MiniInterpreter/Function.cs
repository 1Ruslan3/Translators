namespace MiniInterpreter
{
    public sealed class Function
    {
        public List<string> Parameters { get; }
        public string Expression { get; }

        public Function(List<string> parameters, string expression)
        {
            Parameters = parameters;
            Expression = expression;
        }
    }
}
