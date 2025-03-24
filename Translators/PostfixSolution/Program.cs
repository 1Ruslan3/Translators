using System;
using System.Text;
using System.Linq;

namespace PostfixSolution
{
    class Program
    {

        public static void Main()
        {
            string str = "9 / (2 + (5 - 4)) + 6 * ((5 + 3) / 4 + 2)";

            Postfix a = new Postfix();

            var newStr = a.InfixToPostfix(str);
            
            var result = a.CalculatePostfix(newStr);

            for (int i = 0; i < newStr.Length; i++)
            {
                Console.WriteLine(newStr[i]);
            }

            Console.WriteLine(result);
            
        }
    }
}