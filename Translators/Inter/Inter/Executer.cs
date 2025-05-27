using System.Numerics;

namespace Inter
{
    public class InstructionExecutor
    {
        private readonly VariableStore variables;
        private readonly ConfigManager config;
        private readonly FileHandler fileHandler;
        private readonly int baseAssign;
        private readonly int baseInput;
        private readonly int baseOutput;

        public InstructionExecutor(VariableStore variables, ConfigManager config, FileHandler fileHandler, int baseAssign, int baseInput, int baseOutput)
        {
            this.variables = variables;
            this.config = config;
            this.fileHandler = fileHandler;
            this.baseAssign = baseAssign;
            this.baseInput = baseInput;
            this.baseOutput = baseOutput;
        }

        public void Execute(string instruction, string outputFile)
        {
            var parts = new InstructionParser(config).Parse(instruction);
            if (parts == null)
            {
                fileHandler.WriteOutput($"Недопустимая инструкция: {instruction}", outputFile);
                return;
            }

            string resultVar = parts.Value.ResultVariable;
            string operation = config.GetCommand(parts.Value.Operation);
            var args = parts.Value.Arguments;

            uint result = 0;
            switch (operation)
            {
                case "not":
                    if (args.Length != 1) throw new ArgumentException("NOT требует один аргумент");
                    result = ~EvaluateArgument(args[0]);
                    break;
                case "input":
                    if (args.Length != 1) throw new ArgumentException("INPUT требует один аргумент");
                    fileHandler.WriteOutput($"Введите значение для {args[0]} (основание {baseInput}): ", outputFile);
                    string input = fileHandler.ReadInput();
                    result = Convert.ToUInt32(input, baseInput);
                    break;
                case "output":
                    if (args.Length != 1) throw new ArgumentException("OUTPUT требует один аргумент");
                    fileHandler.WriteOutput(Convert.ToString(EvaluateArgument(args[0]), baseOutput), outputFile);
                    return;
                case "add":
                    if (args.Length != 2) throw new ArgumentException("ADD требует два аргумента");
                    result = EvaluateArgument(args[0]) + EvaluateArgument(args[1]);
                    break;
                case "mult":
                    if (args.Length != 2) throw new ArgumentException("MULT требует два аргумента");
                    result = EvaluateArgument(args[0]) * EvaluateArgument(args[1]);
                    break;
                case "sub":
                    if (args.Length != 2) throw new ArgumentException("SUB требует два аргумента");
                    result = EvaluateArgument(args[0]) - EvaluateArgument(args[1]);
                    break;
                case "pow":
                    if (args.Length != 2) throw new ArgumentException("POW требует два аргумента");
                    result = ModularPow(EvaluateArgument(args[0]), EvaluateArgument(args[1]));
                    break;
                case "div":
                    if (args.Length != 2) throw new ArgumentException("DIV требует два аргумента");
                    uint divisor = EvaluateArgument(args[1]);
                    if (divisor == 0) throw new DivideByZeroException();
                    result = EvaluateArgument(args[0]) / divisor;
                    break;
                case "rem":
                    if (args.Length != 2) throw new ArgumentException("REM требует два аргумента");
                    uint remDivisor = EvaluateArgument(args[1]);
                    if (remDivisor == 0) throw new DivideByZeroException();
                    result = EvaluateArgument(args[0]) % remDivisor;
                    break;
                case "xor":
                    if (args.Length != 2) throw new ArgumentException("XOR требует два аргумента");
                    result = EvaluateArgument(args[0]) ^ EvaluateArgument(args[1]);
                    break;
                case "and":
                    if (args.Length != 2) throw new ArgumentException("AND требует два аргумента");
                    result = EvaluateArgument(args[0]) & EvaluateArgument(args[1]);
                    break;
                case "or":
                    if (args.Length != 2) throw new ArgumentException("OR требует два аргумента");
                    result = EvaluateArgument(args[0]) | EvaluateArgument(args[1]);
                    break;
                case "=":
                    if (args.Length != 1) throw new ArgumentException("Присваивание требует один аргумент");
                    result = EvaluateArgument(args[0]);
                    break;
                default:
                    throw new InvalidOperationException($"Неизвестная операция: {operation}");
            }

            if (!string.IsNullOrEmpty(resultVar) && operation != "output")
            {
                variables.SetValue(resultVar, result);
                fileHandler.WriteOutput($"Отладка: {resultVar} = {Convert.ToString(result, baseOutput)} (основание {baseOutput})", outputFile);
            }
        }

        private uint EvaluateArgument(string arg)
        {
            arg = arg.Trim();
            if (variables.TryGetValue(arg, out uint value))
                return value;
            try
            {
                return Convert.ToUInt32(arg, baseAssign);
            }
            catch
            {
                throw new ArgumentException($"Недопустимый аргумент или неопределенная переменная: {arg}");
            }
        }

        private uint ModularPow(uint baseValue, uint exponent)
        {
            BigInteger result = 1;
            BigInteger b = baseValue;
            BigInteger mod = 1L << 32;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * b) % mod;
                b = (b * b) % mod;
                exponent >>= 1;
            }
            return (uint)result;
        }
    }
}
