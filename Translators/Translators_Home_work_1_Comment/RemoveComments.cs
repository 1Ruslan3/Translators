using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translators_Home_work_1_Comment
{
    internal class RemoveComments
    {
        public string Remove_Comments(string code, string singleLineStart, string multiLineStart)
        {
            // Вычисляем символы окончания многострочного комментария как обратные к начальным
            string multiLineEnd = new string(multiLineStart.Reverse().ToArray());

            // Разделяем код на строки
            string[] lines = code.Split('\n');

            // Список для хранения строк результата
            List<string> resultLines = new List<string>();

            // Флаг, указывающий, находимся ли мы внутри многострочного комментария
            bool inMultiLine = false;

            // Обрабатываем каждую строку
            foreach (var line in lines)
            {
                var ln = line;
                // Если мы внутри многострочного комментария
                if (inMultiLine)
                {
                    // Ищем символы окончания
                    int endIndex = line.IndexOf(multiLineEnd);
                    if (endIndex != -1)
                    {
                        // Если найдены, берем часть строки после комментария
                        ln = line.Substring(endIndex + multiLineEnd.Length);
                        inMultiLine = false;
                    }
                    else
                    {
                        // Если не найдены, пропускаем строку целиком
                        continue;
                    }
                }

                // Проверяем, является ли строка однострочным комментарием
                string trimmed = line.TrimStart();
                if (trimmed.StartsWith(singleLineStart))
                {
                    continue; // Пропускаем строку
                }

                // Удаляем многострочные комментарии из строки
                while (true)
                {
                    int startIndex = line.IndexOf(multiLineStart);
                    if (startIndex == -1)
                    {
                        break; // Если нет начала комментария, выходим из цикла
                    }

                    int endIndex = line.IndexOf(multiLineEnd, startIndex + multiLineStart.Length);
                    if (endIndex != -1)
                    {
                        // Если комментарий начинается и заканчивается в той же строке, удаляем его
                        ln = line.Substring(0, startIndex) + line.Substring(endIndex + multiLineEnd.Length);
                    }
                    else
                    {
                        // Если комментарий начинается, но не заканчивается, добавляем код до комментария
                        string before = line.Substring(0, startIndex);
                        if (!string.IsNullOrWhiteSpace(before))
                        {
                            resultLines.Add(before);
                        }
                        inMultiLine = true;
                        break;
                    }
                }

                // Если мы не внутри многострочного комментария, добавляем обработанную строку
                if (!inMultiLine)
                {
                    resultLines.Add(ln );
                }
            }

            // Объединяем строки результата с переносами строк
            return string.Join("\n", resultLines);
        }
    }
}
