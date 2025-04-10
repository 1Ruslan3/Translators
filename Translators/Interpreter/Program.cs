using System;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            try
            {
                var interpreter = new InterpreterStart();
                int baseA = args.Length > 2 ? int.Parse(args[2]) : 10;
                int baseI = args.Length > 3 ? int.Parse(args[3]) : 10;
                int baseO = args.Length > 4 ? int.Parse(args[4]) : 10;

                interpreter.Start(args[0], args[1], baseA, baseI, baseO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            InterpreterConfig congig = new InterpreterConfig();
            //Console.WriteLine(InterpreterExecute.VarName.Count);
            for (int i = 0; i < InterpreterExecute.VarName.Count; i++)
            {
                Console.WriteLine(InterpreterConfig.commandAliases["output"]);
                Console.WriteLine(InterpreterConfig.commandAliases["add"]);
                Console.WriteLine(InterpreterExecute.VarName[i]);
                Console.WriteLine(InterpreterConfig.variables[InterpreterExecute.VarName[i]].ToString());
            }
        }
    }
}