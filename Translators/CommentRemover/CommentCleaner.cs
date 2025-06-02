using System.Text;

namespace CommentRemover
{
    public class CommentCleaner
    {
        public string Clean(string code, string singleLineComment, string multiLineStart, string multiLineEnd)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            bool inString = false;
            bool inMultiLineComment = false;
            StringBuilder currentLine = new StringBuilder();

            while (i < code.Length)
            {
                char c = code[i];

                if (!inString && !inMultiLineComment && code.Substring(i).StartsWith(singleLineComment))
                {
                    i += singleLineComment.Length;
                    while (i < code.Length && code[i] != '\n') i++;

                    if (currentLine.ToString().Trim().Length > 0)
                        result.AppendLine(currentLine.ToString());

                    currentLine.Clear();

                    if (i < code.Length && code[i] == '\n') i++;
                    continue;
                }

                if (!inString && !inMultiLineComment && code.Substring(i).StartsWith(multiLineStart))
                {
                    inMultiLineComment = true;
                    i += multiLineStart.Length;
                    continue;
                }

                if (inMultiLineComment)
                {
                    if (code.Substring(i).StartsWith(multiLineEnd))
                    {
                        inMultiLineComment = false;
                        i += multiLineEnd.Length;
                    }
                    else
                    {
                        i++;
                    }
                    continue;
                }

                if (c == '"' && !inString)
                {
                    inString = true;
                }
                else if (c == '"' && inString)
                {
                    int backslashCount = 0;
                    int j = i - 1;
                    while (j >= 0 && code[j--] == '\\') backslashCount++;
                    if (backslashCount % 2 == 0) inString = false;
                }

                if (c == '\n')
                {
                    result.AppendLine(currentLine.ToString());
                    currentLine.Clear();
                }
                else
                {
                    currentLine.Append(c);
                }

                i++;
            }

            if (currentLine.Length > 0)
            {
                result.AppendLine(currentLine.ToString());
            }

            return result.ToString();
        }
    }
}