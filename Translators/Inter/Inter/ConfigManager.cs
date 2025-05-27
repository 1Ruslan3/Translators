namespace Inter
{
    public class ConfigManager
    {
        private readonly string configFilePath;
        private readonly string lastConfigPath;
        public Dictionary<string, string> CommandAliases { get; } = new Dictionary<string, string>();
        public string AssignmentDirection { get; private set; } = "left=";
        public string UnarySyntax { get; private set; } = "op()";
        public string BinarySyntax { get; private set; } = "(op)";

        public ConfigManager(string configFile, string lastConfigPath)
        {
            this.configFilePath = configFile;
            this.lastConfigPath = lastConfigPath;
            LoadConfig();
        }

        private void LoadConfig()
        {
            string path = File.Exists(configFilePath) ? configFilePath : lastConfigPath;
            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    if (parts[0] == "left=" || parts[0] == "right=")
                        AssignmentDirection = parts[0];
                    else if (parts[0] == "op()" || parts[0] == "()op")
                        UnarySyntax = parts[0];
                    else if (parts[0] == "op()" || parts[0] == "(op)" || parts[0] == "()op")
                        BinarySyntax = parts[0];
                }
                else if (parts.Length == 2)
                {
                    if (CommandAliases.ContainsValue(parts[1]))
                    {
                        Console.WriteLine($"Предупреждение: Псевдоним '{parts[1]}' уже используется.");
                    }
                    CommandAliases[parts[0]] = parts[1];
                }
            }

            File.WriteAllText(lastConfigPath, path);
        }

        public Dictionary<string, string> GetCommandAliases(string configFile)
        {
            var aliases = new Dictionary<string, string>();
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"Ошибка: Файл конфигурации {configFile} не существует.");
                return aliases;
            }

            var lines = File.ReadAllLines(configFile);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    aliases[parts[0]] = parts[1];
                }
            }

            return aliases;
        }

        public string GetCommand(string alias)
        {
            return CommandAliases.FirstOrDefault(x => x.Value == alias).Key ?? alias;
        }
    }
}