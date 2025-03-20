using System;
using System.Text;

namespace Translators_Home_work_1_Comment
{
    class Program
    {
        public static void Main()
        {
            string str;
            CommentCode commentCode = new CommentCode();
            str= commentCode.Comment_Code("int a = 5;\nint b = 10;", "/*", true);
            Console.WriteLine(str);
        }
    }
}