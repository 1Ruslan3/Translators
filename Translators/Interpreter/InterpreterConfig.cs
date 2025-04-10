using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    public class InterpreterConfig
    {
        public static Dictionary<string, string> commandAliases = new();
        public static Dictionary<string, uint> variables = new();
        public static string assignmentDirection = "left="; 
        public static string unarySyntax = "op()"; 
        public static string binarySyntax = "op()"; 
        public static int baseAssign = 10;
        public static int baseInput = 10;
        public static int baseOutput = 10;
        public const string LAST_CONFIG_PATH = "last_config.txt";

        public static void LoadConfig(string configFile)
        {
            if (!File.Exists(configFile) && File.Exists(LAST_CONFIG_PATH))
            {
                configFile = File.ReadAllText(LAST_CONFIG_PATH).Trim();
            }

            if (!File.Exists(configFile))
                throw new Exception("Config file not found");

            string[] lines = File.ReadAllLines(configFile);
            foreach (string line in lines)
            {
                string trimmed = Regex.Replace(line.Split('#')[0], @"\s+", " ").Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed == "left=") assignmentDirection = "left=";
                else if (trimmed == "right=") assignmentDirection = "right=";
                else if (trimmed == "op()") { unarySyntax = "op()"; binarySyntax = "op()"; }
                else if (trimmed == "(op)") binarySyntax = "(op)";
                else if (trimmed == "()op") { unarySyntax = "()op"; binarySyntax = "()op"; }
                else
                {
                    string[] parts = trimmed.Split(' ');
                    if (parts.Length == 2)
                        commandAliases[parts[0]] = parts[1];
                }
            }
        }

        public static void SaveLastConfig(string configFile)
        {
            File.WriteAllText(LAST_CONFIG_PATH, configFile);
        }

    }
}
