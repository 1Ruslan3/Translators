namespace Inter
{
    class Program
    {
        static void Main(string[] args)
        {
            var validDebugFlags = new[] { "--debug", "-d", "/debug" };
            bool isDebugMode = args.Any(arg => validDebugFlags.Contains(arg.ToLower()));
            var filteredArgs = args.Where(arg => !validDebugFlags.Contains(arg.ToLower())).ToArray();

            if (filteredArgs.Length < 1)
            {
                Console.WriteLine("Использование: Interpreter <входной_файл> [<файл_конфигурации> [<base_assign> [<base_input> [<base_output> [<выходной_файл> [<файл_ввода>]]]]]] [--debug|-d|/debug]");
                return;
            }

            string inputFile = filteredArgs[0];
            string configFile = filteredArgs.Length > 1 ? filteredArgs[1] : "config.txt";
            string lastConfigPath = "last_config.txt";
            string outputFile = filteredArgs.Length > 5 ? filteredArgs[5] : null;
            string inputFilePath = filteredArgs.Length > 6 ? filteredArgs[6] : null;

            int baseAssign = 10, baseInput = 10, baseOutput = 10;
            if (filteredArgs.Length > 2) int.TryParse(filteredArgs[2], out baseAssign);
            if (filteredArgs.Length > 3) int.TryParse(filteredArgs[3], out baseInput);
            if (filteredArgs.Length > 4) int.TryParse(filteredArgs[4], out baseOutput);

            var interpreter = new Interpreter(configFile, lastConfigPath, baseAssign, baseInput, baseOutput, inputFilePath, isDebugMode);

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Ошибка: Входной файл {inputFile} не существует.");
                return;
            }
            if (!File.Exists(configFile) && !File.Exists(lastConfigPath))
            {
                Console.WriteLine($"Ошибка: Файл конфигурации {configFile} не существует, и предыдущая конфигурация не найдена.");
                return;
            }

            if (interpreter.Execute(inputFile, outputFile))
            {
                var aliases = interpreter.GetCommandAliases(configFile);
                if (aliases.Count > 0)
                {
                    Console.WriteLine("Псевдонимы команд:");
                    foreach (var alias in aliases)
                    {
                        Console.WriteLine($"Операция: {alias.Key}, Псевдоним: {alias.Value}");
                    }
                }
            }
        }
    }
}