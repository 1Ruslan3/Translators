using System.Text.RegularExpressions;

namespace Inter
{
    public class VariableStore
    {
        private readonly Dictionary<string, uint> variables = new Dictionary<string, uint>();

        public bool TryGetValue(string name, out uint value)
        {
            return variables.TryGetValue(name, out value);
        }

        public void SetValue(string name, uint value)
        {
            if (!IsValidVariableName(name))
                throw new ArgumentException($"Неверное имя переменной: {name}");
            variables[name] = value;
        }

        public bool Remove(string name)
        {
            return variables.Remove(name);
        }

        public bool Contains(string name)
        {
            return variables.ContainsKey(name);
        }

        public Dictionary<string, uint> GetAllVariables()
        {
            return new Dictionary<string, uint>(variables);
        }

        private bool IsValidVariableName(string name)
        {
            return !string.IsNullOrEmpty(name) && Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }
    }
}
