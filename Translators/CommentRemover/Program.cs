namespace CommentRemover
{
    class Program
    {
        static void Main(string[] args)
        {
            var validator = new ArgumentValidator();

            if (!validator.Validate(args)) return;

            string inputPath = args[0];
            string outputPath = args[1];

            var cleaner = new CommentCleaner();
            var processor = new FileProcessor(inputPath, outputPath, cleaner);
            processor.Process();
        }
    }
}