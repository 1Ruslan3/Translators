using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translators_Home_work_1_Comment
{
    internal class CommentCode
    {
        public string Comment_Code(string code, string startComment, bool isMultiline)
        {
            
            var lines = code.Split('\n');

            if (isMultiline)
            {
                
                if (lines.Length > 0)
                {
                    
                    lines[0] = startComment + lines[0];

                    // Вычисляем символы конца как обратную последовательность startComment
                    var endComment = new string(startComment.Reverse().ToArray());

                    if (lines.Length == 1)
                    {
                        // Если строка одна, добавляем конец к ней же
                        lines[0] = lines[0] + endComment;
                    }
                    else
                    {
                        // Если строк несколько, добавляем конец к последней строке
                        lines[lines.Length - 1] = lines[lines.Length - 1] + endComment;
                    }
                }
            }
            else
            {
                // Однострочный комментарий
                for (int i = 0; i < lines.Length; i++)
                {
                    // Добавляем символы начала к каждой строке
                    lines[i] = startComment + lines[i];
                }
            }

            // Объединяем строки обратно с переносом строк
            return string.Join("\n", lines);
        }
    }
}
