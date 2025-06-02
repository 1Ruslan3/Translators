using System.Text.RegularExpressions;

namespace MiniInterpreter
{
    public sealed class Interpreter
    {
        private readonly Dictionary<string, double> _variables = new();
        private readonly Dictionary<string, Function> _functions = new();
        private StreamWriter _output;

        public void ExecuteFile(string filePath, string outputPath)
        {
            var lines = File.ReadAllLines(filePath);
            using (_output = new StreamWriter(outputPath, false))
            {
                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    try
                    {
                        ExecuteLine(trimmed);
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Ошибка в строке \"{trimmed}\": {ex.Message}");
                    }
                }
            }

            _variables.Clear();
            _functions.Clear();
        }

        private void ExecuteLine(string line)
        {
            if (line.StartsWith("print"))
                HandlePrint(line);
            else if (line.Contains(':'))
                DefineFunction(line);
            else if (line.Contains('='))
                AssignVariable(line);
            else
                throw new Exception("Неизвестная инструкция");
        }

        private void HandlePrint(string line)
        {
            string[] parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                foreach (var kvp in _variables)
                    _output.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
            else
            {
                string varName = parts[1].Trim().TrimEnd(';');
                if (_variables.TryGetValue(varName, out double val))
                    _output.WriteLine($"{varName} = {val}");
                else
                    throw new Exception($"Переменная '{varName}' не найдена.");
            }
        }

        private void DefineFunction(string line)
        {
            string[] parts = line.Split(':', 2);
            string header = parts[0].Trim();
            string body = parts[1].Trim().TrimEnd(';');

            Match match = Regex.Match(header, @"^(\w+)\s*\(([^)]*)\)$");
            if (!match.Success)
                throw new Exception("Неверный синтаксис функции.");

            string name = match.Groups[1].Value;
            var parameters = new List<string>(match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            _functions[name] = new Function(parameters, body);
        }

        private void AssignVariable(string line)
        {
            string[] parts = line.Split('=', 2);
            string left = parts[0].Trim();
            string right = parts[1].Trim().TrimEnd(';');

            Match match = Regex.Match(left, @"^(\w+)(\([if]\))?$");
            if (!match.Success)
                throw new Exception("Неверный синтаксис переменной.");

            string varName = match.Groups[1].Value;
            var parser = new ExpressionParser(right, _variables, _functions);
            double value = parser.Parse().Evaluate();
            _variables[varName] = value;
        }
    }
}
