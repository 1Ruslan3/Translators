namespace CommentRemover
{
    public class ArgumentValidator
    {
        public bool Validate(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Использование: Program.exe <входной_файл> <выходной_файл>");
                return false;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Файл не найден: " + args[0]);
                return false;
            }

            return true;
        }
    }
}