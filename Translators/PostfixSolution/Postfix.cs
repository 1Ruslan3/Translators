using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostfixSolution
{
    internal class Postfix
    {
        private static Dictionary<char, int> precedence = new Dictionary<char, int>
        {
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 }
        };

        public string InfixToPostfix(string infix)
        {
            List<string> output = new List<string>();
            Stack<char> operators = new Stack<char>();

            string[] tokens = Regex.Split(infix.Replace(" ", ""), @"([+\-*/()])")
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
             
            foreach (var token in tokens)
            {
                if (int.TryParse(token, out _))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push('(');
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        output.Add(operators.Pop().ToString());
                    }
                    operators.Pop();
                }
                else if (precedence.ContainsKey(token[0]))
                {
                    while(operators.Count > 0 && operators.Peek() != '('
                        && precedence[operators.Peek()] >= precedence[token[0]])
                    {
                        output.Add(operators.Pop().ToString());
                    }
                    operators.Push(token[0]);
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop().ToString());
            }

            return string.Join("", output);
        }

        public double CalculatePostfix(string postfix)
        {
            Stack<double> stack = new Stack<double>();
            char[] tokens = postfix.ToArray();
            string token;

            for (int i = 0; i < tokens.Length; i++)
            {
                token = tokens[i].ToString();   
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else
                {
                    double y = stack.Pop();
                    double x = stack.Pop();

                    switch(tokens[i])
                    {
                        case '+':
                            stack.Push(x + y);
                            break;
                        case '-':
                            stack.Push(x - y);
                            break;
                        case '*':
                            stack.Push(x * y);
                            break;
                        case '/':
                            stack.Push(x / y);
                            break;
                    }
                }
            }

            return stack.Pop();
        }
    }
}