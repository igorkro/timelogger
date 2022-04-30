// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

using System.Text;
using System.Text.RegularExpressions;

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
