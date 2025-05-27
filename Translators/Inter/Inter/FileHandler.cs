namespace Inter
{
    public class FileHandler
    {
        private readonly string inputFilePath;

        public FileHandler(string inputFilePath = null)
        {
            this.inputFilePath = inputFilePath;
        }

        public string ReadInputFile(string inputFile)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Входной файл {inputFile} не существует.");
            return File.ReadAllText(inputFile);
        }

        public void ClearOutputFile(string outputFile)
        {
            if (!string.IsNullOrEmpty(outputFile))
            {
                try
                {
                    File.WriteAllText(outputFile, string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при очистке выходного файла {outputFile}: {ex.Message}");
                }
            }
        }

        public void WriteOutput(string message, string outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(message);
            }
            else
            {
                try
                {
                    using (var writer = new StreamWriter(outputFile, true))
                    {
                        writer.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка записи в выходной файл {outputFile}: {ex.Message}");
                }
            }
        }

        public string ReadInput()
        {
            if (!string.IsNullOrEmpty(inputFilePath) && File.Exists(inputFilePath))
            {
                try
                {
                    string input = File.ReadLines(inputFilePath).FirstOrDefault();
                    if (input == null)
                        throw new IOException("Файл ввода пуст.");
                    return input;
                }
                catch (Exception ex)
                {
                    throw new IOException($"Ошибка чтения файла ввода {inputFilePath}: {ex.Message}");
                }
            }
            return Console.ReadLine();
        }
    }
}
