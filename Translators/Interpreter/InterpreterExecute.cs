using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    public class InterpreterExecute
    {
        public static List<string> VarName = new List<string>();

        public static string RemoveComments(string content)
        {
            while (content.Contains("["))
            {
                int start = content.IndexOf("[");
                int end = content.IndexOf("]", start);
                if (end == -1) break;
                content.Remove(start, end - start + 1);
            }

            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                int commentIndex = lines[i].IndexOf('#');
                if (commentIndex != -1)
                {
                    lines[i] = lines[i].Substring(0, commentIndex);
                }
            }

            return string.Join('\n', lines);
        }

        public static void ExecuteProgram(string programFile)
        {
            if (!File.Exists(programFile))
                throw new Exception("Program file not found");

            string content = RemoveComments(File.ReadAllText(programFile));
            string[] instructions = content.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (string instruction in instructions)
            {
                ExecuteInstruction(instruction.Trim());
            }
        }

        public static void ExecuteInstruction(string instruction)
        {
            instruction = Regex.Replace(instruction.ToLower(), @"\s+", " ");
            string pattern;
            if (InterpreterConfig.assignmentDirection == "left=") 
            {
                pattern = @"^([a-z_][a-z0-9_]*) *-> *(.*)$";
            }
            else
            {
                pattern = @"^(.*) *-> *([a-z_][a-z0-9_]*)$";
            }
            
            var match = Regex.Match(instruction, pattern);
            if (!match.Success) return;

            string varName = match.Groups[InterpreterConfig.assignmentDirection == "left=" ? 1 : 2].Value;
            string expression = match.Groups[InterpreterConfig.assignmentDirection == "left=" ? 2 : 1].Value;

            VarName.Add(varName);

            uint result = EvaluateExpression(expression);
            InterpreterConfig.variables[varName] = result;
        }

        public static uint EvaluateExpression(string expression)
        {
            expression = expression.Trim();

            if (uint.TryParse(expression, System.Globalization.NumberStyles.AllowHexSpecifier, null, out uint constant))
            {
                return Convert.ToUInt32(expression, InterpreterConfig.baseAssign);
            }

            string[] unaryOps = { "not", "input", "output" };
            foreach (var op in unaryOps)
            {
                string alias = InterpreterConfig.commandAliases.ContainsKey(op) ? InterpreterConfig.commandAliases[op] : op;
                string pattern = InterpreterConfig.unarySyntax == "op()" 
                    ? $@"^{alias}\((.*?)\)$"
                    : $@"^\((.*?)\){alias}$";
               
                var match = Regex.Match(expression, pattern);
                if (match.Success )
                {
                    string arg = match.Groups[1].Value;
                    return ExecuteUnary(op, arg);
                }
            }

            string[] binaryOps = { "add", "mult", "sub", "pow", "div", "rem", "xor", "and", "or" };
            foreach (var op in binaryOps)
            {
                string alias = InterpreterConfig.commandAliases.ContainsKey(op) ? InterpreterConfig.commandAliases[op] : op;
                string pattern = InterpreterConfig.binarySyntax switch
                {
                    "op()" => $@"^{alias}\((.*?),(.*?)\)$",
                    "(op)" => $@"^(.*?)\s+{alias}\s+(.*?)$",
                    "()op" => $@"^\((.*?),(.*?)\){alias}$",
                    _ => ""
                };

                var match = Regex.Match(expression, pattern);
                if (match.Success)
                {
                    string arg1 = match.Groups[1].Value;
                    string arg2 = match.Groups[2].Value;
                    return ExecuteBinary(op, arg1, arg2);
                }
            }

            if (InterpreterConfig.variables.ContainsKey(expression))
                return InterpreterConfig.variables[expression]; 

            throw new Exception($"Invalid expression: {expression}");
        }

        public static uint ExecuteUnary(string operation, string arg)
        {
            uint value = InterpreterConfig.variables.ContainsKey(arg) ? InterpreterConfig.variables[arg] : Convert.ToUInt32(arg, InterpreterConfig.baseAssign);

            switch(operation)
            {
                case "not":
                    return ~value;
                case "input":
                    Console.Write($"Enter value for {arg} (base {InterpreterConfig.baseInput}): ");
                    return Convert.ToUInt32(Console.ReadLine(), InterpreterConfig.baseInput);
                case "output":
                    Console.WriteLine(Convert.ToString(value, InterpreterConfig.baseOutput));
                    return value;
                default: throw new Exception($"Unknown unary operation: {operation}");
            }
        }

        public static uint ExecuteBinary(string operation, string arg1, string arg2)
        {
            uint val1 = InterpreterConfig.variables.ContainsKey(arg1) ? InterpreterConfig.variables[arg1] : Convert.ToUInt32(arg1, InterpreterConfig.baseAssign);
            uint val2 = InterpreterConfig.variables.ContainsKey(arg2) ? InterpreterConfig.variables[arg2] : Convert.ToUInt32(arg2, InterpreterConfig.baseAssign);

            switch(operation)
            {
                case "add": return val1 + val2;
                case "mult": return val1 * val2;
                case "sub": return val1 - val2;
                case "pow": return Power(val1, val2);
                case "div": return val2 != 0 ? val1 / val2 : throw new Exception("Division by zero");
                case "rem": return val2 != 0 ? val1 % val2 : throw new Exception("Division by zero");
                case "xor": return val1 ^ val2;
                case "and": return val1 & val2;
                case "or": return val1 | val2;
                default: throw new Exception($"Unknown binary operation: {operation}");
            }
        }

        private static uint Power(uint baseNum, uint exp)
        {
            uint result = 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                    result *= baseNum;
                baseNum *= baseNum;
                exp >>= 1;
            }
            return result;
        }

    }
}
