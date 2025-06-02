namespace MiniInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к файлу инструкций.");
                return;
            }

            string inputPath = args[0];
            string outputPath = args.Length > 1 ? args[1] : "output.txt";

            try
            {
                var interpreter = new Interpreter();
                interpreter.ExecuteFile(inputPath, outputPath);
                Console.WriteLine($"Вывод записан в файл: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
