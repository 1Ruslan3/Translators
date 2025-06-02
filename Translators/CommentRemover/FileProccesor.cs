namespace CommentRemover
{
    public class FileProcessor
    {
        private readonly string inputPath;
        private readonly string outputPath;
        private readonly CommentCleaner cleaner;

        public FileProcessor(string inputPath, string outputPath, CommentCleaner cleaner)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
            this.cleaner = cleaner;
        }

        public void Process()
        {
            string[] lines = File.ReadAllLines(inputPath);

            if (lines.Length < 3)
            {
                Console.WriteLine("Файл должен содержать минимум 3 строки: символы комментариев и код.");
                return;
            }

            string singleLineSymbol = lines[0].Trim();
            string multiLineStart = lines[1].Trim();
            string multiLineEnd = lines[2].Trim();

            string code = string.Join("\n", lines[3..]);
            string cleaned = cleaner.Clean(code, singleLineSymbol, multiLineStart, multiLineEnd);

            File.WriteAllText(outputPath, cleaned);
            Console.WriteLine("Комментарии удалены. Результат записан в: " + outputPath);
        }
    }
}