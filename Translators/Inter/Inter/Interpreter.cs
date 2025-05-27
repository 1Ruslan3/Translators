using System.Text.RegularExpressions;

namespace Inter
{
    public class Interpreter
    {
        private readonly ConfigManager config;
        private readonly VariableStore variables;
        private readonly InstructionExecutor executor;
        private readonly DebugManager debugger;
        private readonly FileHandler fileHandler;
        private readonly bool isDebugMode;

        public Interpreter(string configFile, string lastConfigPath, int baseAssign, int baseInput, int baseOutput, string inputFilePath = null, bool isDebugMode = false)
        {
            this.fileHandler = new FileHandler(inputFilePath);
            this.config = new ConfigManager(configFile, lastConfigPath);
            this.variables = new VariableStore();
            this.executor = new InstructionExecutor(variables, config, fileHandler, baseAssign, baseInput, baseOutput);
            this.debugger = new DebugManager(variables, fileHandler, baseOutput);
            this.isDebugMode = isDebugMode;
        }

        public bool Execute(string inputFile, string outputFile = null)
        {
            string content;
            try
            {
                content = fileHandler.ReadInputFile(inputFile);
            }
            catch (Exception ex)
            {
                fileHandler.WriteOutput(ex.Message, outputFile);
                return false;
            }

            var instructions = RemoveComments(content)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            fileHandler.ClearOutputFile(outputFile);

            foreach (var instruction in instructions)
            {
                if (isDebugMode && instruction == "BREAKPOINT")
                {
                    fileHandler.WriteOutput("Достигнута точка останова.", outputFile);
                    debugger.RunInteractiveMode(outputFile);
                    continue;
                }

                try
                {
                    executor.Execute(instruction, outputFile);
                }
                catch (Exception ex)
                {
                    fileHandler.WriteOutput($"Ошибка при выполнении инструкции '{instruction}': {ex.Message}", outputFile);
                }
            }
            return true;
        }

        private string RemoveComments(string content)
        {
            var multiLineCommentPattern = @"\[.*?\]";
            content = Regex.Replace(content, multiLineCommentPattern, "", RegexOptions.Singleline);

            var lines = content.Split('\n');
            var resultLines = new List<string>();
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                if (isDebugMode && trimmedLine.StartsWith("#BREAKPOINT", StringComparison.OrdinalIgnoreCase))
                {
                    if (trimmedLine.Equals("#BREAKPOINT", StringComparison.OrdinalIgnoreCase))
                    {
                        resultLines.Add("BREAKPOINT");
                        continue;
                    }

                    var match = Regex.Match(trimmedLine, @"^#BREAKPOINT\s*(.*)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        resultLines.Add("BREAKPOINT");
                        string remaining = match.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(remaining))
                        {
                            resultLines.Add(remaining);
                        }
                        continue;
                    }
                }

                int commentIndex = line.IndexOf('#');
                if (commentIndex >= 0)
                {
                    trimmedLine = line.Substring(0, commentIndex).Trim();
                }
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    resultLines.Add(trimmedLine);
                }
            }

            return string.Join(";", resultLines);
        }

        public Dictionary<string, string> GetCommandAliases(string configFile)
        {
            return config.GetCommandAliases(configFile);
        }
    }
}
