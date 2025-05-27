using System.Text.RegularExpressions;

namespace Inter
{
    public class DebugManager
    {
        private readonly VariableStore variables;
        private readonly FileHandler fileHandler;
        private readonly int baseOutput;

        public DebugManager(VariableStore variables, FileHandler fileHandler, int baseOutput)
        {
            this.variables = variables;
            this.fileHandler = fileHandler;
            this.baseOutput = baseOutput;
        }

        public void RunInteractiveMode(string outputFile)
        {
            while (true)
            {
                Console.WriteLine("\nИнтерактивный режим отладки:");
                Console.WriteLine("1. Вывести значение переменной (hex) и дамп памяти (bin)");
                Console.WriteLine("2. Вывести все переменные и их значения");
                Console.WriteLine("3. Изменить значение переменной (hex)");
                Console.WriteLine("4. Объявить новую переменную (цекендорф/римские)");
                Console.WriteLine("5. Удалить переменную");
                Console.WriteLine("6. Продолжить выполнение программы");
                Console.WriteLine("7. Завершить работу интерпретатора");
                Console.Write("Выберите действие (1-7): ");

                string choice = Console.ReadLine()?.Trim();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Введите имя переменной: ");
                            string varName1 = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(varName1) || !variables.Contains(varName1))
                            {
                                Console.WriteLine($"Ошибка отладчика: Переменная '{varName1}' не существует.");
                                continue;
                            }
                            variables.TryGetValue(varName1, out uint value1);
                            string hexValue = Convert.ToString(value1, 16).ToUpper();
                            string memoryDump = GetMemoryDump(value1);
                            Console.WriteLine($"Значение {varName1}: 0x{hexValue}");
                            Console.WriteLine($"Дамп памяти: {memoryDump}");
                            break;

                        case "2":
                            var allVars = variables.GetAllVariables();
                            if (allVars.Count == 0)
                            {
                                Console.WriteLine("Нет известных переменных.");
                            }
                            else
                            {
                                Console.WriteLine("Известные переменные:");
                                foreach (var kvp in allVars)
                                {
                                    Console.WriteLine($"{kvp.Key} = {kvp.Value} (основание {baseOutput})");
                                }
                            }
                            break;

                        case "3":
                            Console.Write("Введите имя переменной: ");
                            string varName3 = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(varName3) || !variables.Contains(varName3))
                            {
                                Console.WriteLine($"Ошибка отладчика: Переменная '{varName3}' не существует.");
                                continue;
                            }
                            Console.Write("Введите новое значение (hex): ");
                            string hexInput = Console.ReadLine()?.Trim();
                            if (!uint.TryParse(hexInput, System.Globalization.NumberStyles.HexNumber, null, out uint newValue))
                            {
                                Console.WriteLine($"Ошибка отладчика: Неверный формат числа в шестнадцатеричной системе.");
                                continue;
                            }
                            variables.SetValue(varName3, newValue);
                            Console.WriteLine($"Переменная {varName3} обновлена: {newValue} (основание {baseOutput})");
                            break;

                        case "4":
                            Console.Write("Введите имя новой переменной: ");
                            string varName4 = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(varName4) || !Regex.IsMatch(varName4, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                            {
                                Console.WriteLine($"Ошибка отладчика: Неверное имя переменной '{varName4}'.");
                                continue;
                            }
                            if (variables.Contains(varName4))
                            {
                                Console.WriteLine($"Ошибка отладчика: Переменная '{varName4}' уже существует.");
                                continue;
                            }
                            Console.WriteLine("Выберите формат ввода:");
                            Console.WriteLine("1. Цекендорфово представление");
                            Console.WriteLine("2. Римские цифры");
                            Console.Write("Выбор (1-2): ");
                            string formatChoice = Console.ReadLine()?.Trim();
                            uint newVarValue;
                            if (formatChoice == "1")
                            {
                                Console.Write("Введите число в цекендорфовом представлении: ");
                                string zeckInput = Console.ReadLine()?.Trim();
                                try
                                {
                                    newVarValue = ParseZeckendorf(zeckInput);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка отладчика: Неверный формат цекендорфова представления: {ex.Message}");
                                    continue;
                                }
                            }
                            else if (formatChoice == "2")
                            {
                                Console.Write("Введите число римскими цифрами: ");
                                string romanInput = Console.ReadLine()?.Trim();
                                try
                                {
                                    newVarValue = ParseRoman(romanInput);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка отладчика: Неверный формат римских цифр: {ex.Message}");
                                    continue;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Ошибка отладчика: Неверный выбор формата.");
                                continue;
                            }
                            variables.SetValue(varName4, newVarValue);
                            Console.WriteLine($"Переменная {varName4} объявлена: {newVarValue} (основание {baseOutput})");
                            break;

                        case "5":
                            Console.Write("Введите имя переменной для удаления: ");
                            string varName5 = Console.ReadLine()?.Trim();
                            if (string.IsNullOrEmpty(varName5) || !variables.Contains(varName5))
                            {
                                Console.WriteLine($"Ошибка отладчика: Переменная '{varName5}' не существует.");
                                continue;
                            }
                            variables.Remove(varName5);
                            Console.WriteLine($"Переменная {varName5} удалена.");
                            break;

                        case "6":
                            Console.WriteLine("Продолжаем выполнение программы...");
                            return;

                        case "7":
                            Console.WriteLine("Завершение работы интерпретатора...");
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine($"Ошибка отладчика: Неверный выбор действия '{choice}'.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отладчика: {ex.Message}");
                }
            }
        }

        private string GetMemoryDump(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return string.Join(" ", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        private uint ParseZeckendorf(string input)
        {
            if (string.IsNullOrEmpty(input) || !Regex.IsMatch(input, @"^[01]+$"))
                throw new ArgumentException("Цекендорфово представление должно содержать только 0 и 1.");

            List<uint> fib = new List<uint> { 1, 2 };
            while (fib.Last() < uint.MaxValue / 2)
            {
                fib.Add(fib[fib.Count - 1] + fib[fib.Count - 2]);
            }

            uint result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '1')
                {
                    int fibIndex = input.Length - 1 - i;
                    if (fibIndex >= fib.Count)
                        throw new ArgumentException("Число выходит за пределы допустимых значений.");
                    result += fib[fibIndex];
                }
            }

            if (input.Contains("11"))
                throw new ArgumentException("Цекендорфово представление не должно содержать двух последовательных единиц.");

            return result;
        }

        private uint ParseRoman(string input)
        {
            if (string.IsNullOrEmpty(input) || !Regex.IsMatch(input, @"^[IVXLCDM]+$"))
                throw new ArgumentException("Римские цифры должны содержать только символы I, V, X, L, C, D, M.");

            Dictionary<char, uint> romanValues = new Dictionary<char, uint>
        {
            {'I', 1}, {'V', 5}, {'X', 10}, {'L', 50},
            {'C', 100}, {'D', 500}, {'M', 1000}
        };

            uint result = 0;
            uint prevValue = 0;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                uint currentValue = romanValues[input[i]];
                if (currentValue >= prevValue)
                {
                    result += currentValue;
                }
                else
                {
                    result -= currentValue;
                }
                prevValue = currentValue;
            }

            if (result == 0)
                throw new ArgumentException("Римские цифры не образуют допустимое число.");

            return result;
        }
    }
}
