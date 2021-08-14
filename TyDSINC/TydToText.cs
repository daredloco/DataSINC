using System;
using System.Linq;
using System.Text;

namespace Tyd
    {
    public static class TydToText
        {
        /*
            Possible future features:
                - Ability to align string values into a single column
                - Ability to write lists/tables with 0 or 1 children on a single line
                - Some way to better control which strings get quotes and which don't
         */

        ///<summary>
        /// Writes a given TydNode, along with all its descendants, as a string, at a given indent level.
        /// This method is recursive.
        ///</summary>
        public static string Write(TydNode node, bool whitesmiths, int indent = 0, int longestName = 0, bool forceQuotes = false, bool noInlineTables = false)
            {
            var braceIndent = whitesmiths ? indent + 1 : indent;
            //It's a string
            var str = node as TydString;
            if (str != null)
                {
                if (str.Name != null)
                    {
                    return IndentString(indent) + node.Name + RepeatString(" ", (Math.Max(0, longestName - node.Name.Length) + 1)) + StringContentWriteable(str.Value, forceQuotes);
                    }
                else
                    {
                    return IndentString(indent) + StringContentWriteable(str.Value, forceQuotes);
                    }
                }

            var doc = node as TydDocument;
            if (doc != null)
                {
                var nameLength = doc.Nodes.Max(x => x.Name.Length);
                var sb = new StringBuilder();
                foreach (var subNode in doc)
                    {
                    sb.AppendLine(Write(subNode, whitesmiths, indent, nameLength, forceQuotes, noInlineTables));
                    if (subNode is TydCollection)
                        {
                        sb.AppendLine();
                        }
                    }
                return sb.ToString();
                }

            //It's a table
            var tab = node as TydTable;
            if (tab != null)
                {
                var sb = new StringBuilder();
                var simple = !noInlineTables && tab.Parent != null && !(tab.Parent is TydDocument) && IsSimpleCollection(tab);
                var intro = AppendNodeIntro(tab, sb, indent);
                //Intro line
                if (intro && !simple)
                    {
                    sb.AppendLine();
                    }

                if (simple)
                    {
                    if (!intro)
                        {
                        sb.Append(IndentString(indent) + Constants.TableStartChar);
                        }
                    else
                        {
                        sb.Append(RepeatString(" ", Math.Max(0, longestName - tab.Name.Length) + 1) + Constants.TableStartChar);
                        }
                    for (var i = 0; i < tab.Count; i++)
                        {
                        sb.Append(i == 0 ? " " : "; ");
                        sb.Append(Write(tab[i], whitesmiths, 0, 0, forceQuotes, noInlineTables));
                        }
                    sb.Append(" " + Constants.TableEndChar);
                    }
                else
                    {
                    var nameLength = tab.Nodes.Max(x => x.Name.Length);
                    //Sub-_nodes
                    sb.AppendLine(IndentString(braceIndent) + Constants.TableStartChar);
                    for (var i = 0; i < tab.Count; i++)
                        {
                        sb.AppendLine(Write(tab[i], whitesmiths, indent + 1, nameLength, forceQuotes, noInlineTables));
                        }
                    sb.Append(IndentString(braceIndent) + Constants.TableEndChar);
                    }

                return sb.ToString();
                }

            //It's a list
            var list = node as TydList;
            if (list != null)
                {
                var sb = new StringBuilder();
                var simple = IsSimpleCollection(list);
                var intro = AppendNodeIntro(list, sb, indent);
                //Intro line
                if (intro && !simple)
                    {
                    sb.AppendLine();
                    }

                if (simple)
                    {
                    if (!intro)
                        {
                        sb.Append(IndentString(indent) + Constants.ListStartChar);
                        }
                    else
                        {
                        sb.Append(RepeatString(" ", Math.Max(0, longestName - list.Name.Length) + 1) + Constants.ListStartChar);
                        }
                    for (var i = 0; i < list.Count; i++)
                        {
                        sb.Append(i == 0 ? " " : "; ");
                        sb.Append(Write(list[i], whitesmiths, 0, 0, forceQuotes, noInlineTables));
                        }
                    sb.Append(" " + Constants.ListEndChar);
                    }
                else
                    {
                    //Sub-_nodes
                    sb.AppendLine(IndentString(braceIndent) + Constants.ListStartChar);
                    for (var i = 0; i < list.Count; i++)
                        {
                        sb.AppendLine(Write(list[i], whitesmiths, indent + 1, 0, forceQuotes, noInlineTables));
                        }
                    sb.Append(IndentString(braceIndent) + Constants.ListEndChar);
                    }

                return sb.ToString();
                }

            throw new ArgumentException();
            }

        private static bool IsSimpleCollection(TydCollection l)
            {
            var c = 0;
            var c2 = 0;
            for (var i = 0; i < l.Nodes.Count; i++)
                {
                var node = l.Nodes[i] as TydString;
                if (node != null)
                    {
                    if (node.Value != null)
                        {
                        if (node.Value.Contains("\n"))
                            {
                            return false;
                            }
                        c += node.Value.Length;
                        }
                    if (node.Name != null)
                        {
                        c += node.Name.Length;
                        }
                    c2++;
                    }
                else
                    {
                    return false;
                    }
                }
            return c2 < 2 || c < 64;
            }

        private static string StringContentWriteable(string value, bool forceQuotes)
            {
            if (value == "")
                {
                return "\"\"";
                }

            if (value == null)
                {
                return Constants.NullValueString;
                }

            return forceQuotes || ShouldWriteWithQuotes(value)
            ? "\"" + EscapeCharsEscapedForQuotedString(value) + "\""
            : value;



            //Returns string content s with escape chars properly escaped according to Tyd rules.
            }

        //This is a set of heuristics to try to determine if we should write a string quoted or naked.
        public static bool ShouldWriteWithQuotes(string value)
            {
            var len = 0;
            //Check the string character-by-character
            for (var i = 0; i < value.Length; i++)
                {
                var c = value[i];

                if (!TydFromText.IsSymbolChar(c) && c != '.')
                    {
                    return true;
                    }

                //Chars that imply we should use quotes
                //Some of these are heuristics, like space.
                //Some absolutely require quotes, like the double-quote itself. They'll break naked strings if unescaped (and naked strings are always written unescaped).
                //Note that period is not on this list; it commonly appears as a decimal in numbers.
                if (c == ' '
                    || c == '\n'
                    || c == '\t'
                    || c == '"'
                    || c == Constants.CommentChar
                    || c == Constants.RecordEndChar
                    || c == Constants.AttributeStartChar
                    || c == Constants.TableStartChar
                    || c == Constants.TableEndChar
                    || c == Constants.ListStartChar
                    || c == Constants.ListEndChar
                    )
                    {
                    return true;
                    }
                if (!char.IsWhiteSpace(c))
                    {
                    len++;
                    }
                }

            return len == 0;
            }

        private static string EscapeCharsEscapedForQuotedString(string s)
            {
            return s.Replace("\"", "\\\"")
                .Replace("#", "\\#");
            }

        private static bool AppendNodeIntro(TydCollection node, StringBuilder sb, int indent)
            {
            var appendedSomething = false;

            if (node.Name != null)
                {
                AppendWithWhitespace(node.Name, sb, ref appendedSomething, indent);
                }

            foreach (var attribute in node.GetAttributes())
                {
                if (attribute.Value != null)
                    {
                    AppendWithWhitespace(Constants.AttributeStartChar + attribute.Key + " " + StringContentWriteable(attribute.Value, false), sb, ref appendedSomething, indent);
                    }
                else
                    {
                    AppendWithWhitespace(Constants.AttributeStartChar + attribute.Key, sb, ref appendedSomething, indent);
                    }
                }

            return appendedSomething;

            }

        private static void AppendWithWhitespace(string s, StringBuilder sb, ref bool appendedSomething, int indent)
            {
            sb.Append((appendedSomething ? " " : IndentString(indent)) + s);
            appendedSomething = true;
            }

        private static string RepeatString(string s, int repeat)
            {
            if (repeat == 0)
                {
                return "";
                }
            else if (repeat == 1)
                {
                return s;
                }
            var sb = new StringBuilder(repeat);
            for (var i = 0; i < repeat; i++)
                {
                sb.Append(s);
                }
            return sb.ToString();
            }

        public static string IndentString(int indent)
            {
            var s = "";
            for (var i = 0; i < indent; i++)
                {
                s += "    ";
                }
            return s;
            }
        }

    }