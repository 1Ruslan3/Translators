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

            Stack<char> stack = new Stack<char>();

            StringBuilder newStr = new StringBuilder();

            char[] chars = str.Where(c => c != ' ').ToArray();

            stack.Push('(');


            for (int i = 0;  i < chars.Length; i++) 
            {
                if (Convert.ToInt32(chars[i]) == 40)
                {
                    stack.Push(chars[i]);
                }
                if (Convert.ToInt32(chars[i]) >= 48 && Convert.ToInt32(chars[i]) <= 57)
                {
                    newStr.Append(chars[i]);
                }
                if (Convert.ToInt32(chars[i]) == 43)
                {
                    if (stack.Count > 0)
                    {
                 
                        if (stack.Peek() == 42 || stack.Peek() == 47 || stack.Peek() == 45 || stack.Peek() == 43)
                        {
                            newStr.Append(stack.Pop());
                        }
                        
                    }
                    stack.Push(chars[i]);
                }
                if (Convert.ToInt32(chars[i]) == 41)
                {
                    while (stack.Peek() != '(')
                    {
                        newStr.Append(stack.Pop());
                    }
                    if (stack.Count > 1)
                    {
                        stack.Pop();
                    }
                }
                if (Convert.ToInt32(chars[i]) == 42)
                {
                    if (stack.Peek() == '/')
                    {
                        newStr.Append(stack.Pop());
                    }
                    stack.Push(chars[i]);
                }
                if (Convert.ToInt32(chars[i]) == 45)
                {
                    if (stack.Count > 0)
                    {

                        if (stack.Peek() == 42 || stack.Peek() == 47 || stack.Peek() == 45 || stack.Peek() == 43)
                        {
                            newStr.Append(stack.Pop());
                        }

                    }
                    stack.Push(chars[i]);
                }
                if (Convert.ToInt32(chars[i]) == 47)
                {
                    if (stack.Peek() == '*')
                    {
                        newStr.Append(stack.Pop());
                    }
                    stack.Push(chars[i]);
                }
            }
            while (stack.Count > 1)
            {
                newStr.Append(stack.Pop());
            }

            for (int i = 0; i < newStr.Length; i++)
            {
                Console.WriteLine(newStr[i]);
            }
        }
    }
}