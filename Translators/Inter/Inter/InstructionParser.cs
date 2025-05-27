using System.Text.RegularExpressions;

namespace Inter
{
    public sealed class InstructionParser
    {
        private readonly ConfigManager config;

        public InstructionParser(ConfigManager config)
        {
            this.config = config;
        }

        public (string ResultVariable, string Operation, string[] Arguments)? Parse(string instruction)
        {
            instruction = Regex.Replace(instruction, @"\s+", " ").Trim().ToLower();
            string assignOp = config.CommandAliases.ContainsKey("=") ? config.CommandAliases["="] : "=";

            string resultVar = null;
            string expr = instruction;

            if (config.AssignmentDirection == "right=")
            {
                var match = Regex.Match(instruction, $@"^(.+?)\s*{Regex.Escape(assignOp)}\s*([a-zA-Z_][a-zA-Z0-9_]*)$");
                if (match.Success)
                {
                    expr = match.Groups[1].Value.Trim();
                    resultVar = match.Groups[2].Value;
                }
            }
            else
            {
                var match = Regex.Match(instruction, $@"^([a-zA-Z_][a-zA-Z0-9_]*)\s*{Regex.Escape(assignOp)}\s*(.+)$");
                if (match.Success)
                {
                    resultVar = match.Groups[1].Value;
                    expr = match.Groups[2].Value.Trim();
                }
            }

            if (resultVar != null && Regex.IsMatch(expr, @"^[a-zA-Z_][a-zA-Z0-9_]*$|^[0-9]+$"))
            {
                return (resultVar, "=", new[] { expr });
            }

            if (config.UnarySyntax == "op()")
            {
                var match = Regex.Match(instruction, @"^([^\s(]+)\s*\(\s*([a-zA-Z0-9_]+)\s*\)$");
                if (match.Success)
                {
                    var op = match.Groups[1].Value;
                    var arg = match.Groups[2].Value;
                    return (null, op, new[] { arg });
                }
            }
            else if (config.UnarySyntax == "()op")
            {
                var match = Regex.Match(instruction, @"^\(\s*([a-zA-Z0-9_]+)\s*\)\s*([^\s)]+)$");
                if (match.Success)
                {
                    var op = match.Groups[2].Value;
                    var arg = match.Groups[1].Value;
                    return (null, op, new[] { arg });
                }
            }

            if (config.BinarySyntax == "(op)" && resultVar != null)
            {
                var match = Regex.Match(expr, @"^([a-zA-Z0-9_]+)\s+([^\s(]+)\s+([a-zA-Z0-9_]+)$");
                if (match.Success)
                {
                    var op = match.Groups[2].Value;
                    var arg1 = match.Groups[1].Value;
                    var arg2 = match.Groups[3].Value;
                    return (resultVar, op, new[] { arg1, arg2 });
                }

                match = Regex.Match(expr, @"^([^\s(]+)\s*\(\s*([a-zA-Z0-9_]+)\s*,\s*([a-zA-Z0-9_]+)\s*\)$");
                if (match.Success)
                {
                    var op = match.Groups[1].Value;
                    var arg1 = match.Groups[2].Value;
                    var arg2 = match.Groups[3].Value;
                    return (resultVar, op, new[] { arg1, arg2 });
                }
            }
            else if (config.BinarySyntax == "op()")
            {
                var match = Regex.Match(expr, @"^([^\s(]+)\s*\(\s*([a-zA-Z0-9_]+)\s*,\s*([a-zA-Z0-9_]+)\s*\)$");
                if (match.Success)
                {
                    var op = match.Groups[1].Value;
                    var arg1 = match.Groups[2].Value;
                    var arg2 = match.Groups[3].Value;
                    return (resultVar, op, new[] { arg1, arg2 });
                }
            }
            else if (config.BinarySyntax == "()op")
            {
                var match = Regex.Match(expr, @"^\(\s*([a-zA-Z0-9_]+)\s*,\s*([a-zA-Z0-9_]+)\s*\)\s*([^\s)]+)$");
                if (match.Success)
                {
                    var op = match.Groups[3].Value;
                    var arg1 = match.Groups[1].Value;
                    var arg2 = match.Groups[2].Value;
                    return (resultVar, op, new[] { arg1, arg2 });
                }
            }

            return null;
        }
    }
}
