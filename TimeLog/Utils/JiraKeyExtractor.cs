using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TimeLog.Utils
{
    class JiraKeyExtractor
    {
        private static Regex sKeyExpression = new Regex("([a-zA-z]+-\\d+)");

        public static string ExtractRegExp(string command, ref int keyEndPos)
        {
            Match match = sKeyExpression.Match(command);
            if (match.Groups.Count < 2)
                return string.Empty;

            keyEndPos = match.Groups[1].Index + match.Groups[1].Length;
            return match.Groups[1].Value;
        }

        public static string Extract(string command, ref int keyEndPos)
        {
            StringBuilder key = new StringBuilder();

            int state = 0;
            for (int i = 0; i < command.Length; ++i)
            {
                char c = command[i];
                switch (state)
                {
                    case 0:
                        if (char.IsLetter(c))
                        {
                            key.Append(c);
                            state = 1;
                        }
                        else
                        {
                            return string.Empty;
                        }
                        break;

                    case 1:
                        if (char.IsLetter(c))
                        {
                            key.Append(c);
                        }
                        else if (c == '-')
                        {
                            key.Append(c);
                            state = 2;
                        }
                        else
                        {
                            return string.Empty;
                        }
                        break;

                    case 2:
                        if (char.IsDigit(c))
                        {
                            key.Append(c);
                            state = 3;
                        }
                        else
                            return string.Empty;
                        break;

                    case 3:
                        if (char.IsDigit(c))
                        {
                            key.Append(c);
                        }
                        else if (char.IsWhiteSpace(c))
                        {
                            keyEndPos = i;
                            return key.ToString();
                        }
                        else
                            return string.Empty;
                        break;
                }
            }

            if (state == 3)
            {
                keyEndPos = command.Length;
                return key.ToString();
            }

            return string.Empty;
        }
    }
}
