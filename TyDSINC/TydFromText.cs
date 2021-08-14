using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyd
    {
    public static class TydFromText
        {

        public static List<TydNode> Parse(string doc)
            {
            var line = 1;
            return Parse(doc, 0, ref line, true, new List<TydNode>());
            }

        public static TydNode ParseOne(string doc)
            {
            var line = 1;
            var res = Parse(doc, 0, ref line, true, new List<TydNode>(), true);
            if (res.Count == 0)
                {
                throw new Exception("Couldn't retrieve element from empty TyD file");
                }
            return res[0];
            }

        private static void SetAttributeValue(ref Dictionary<string, string> attributes, string key, string value)
            {
            if (attributes == null)
                {
                attributes = new Dictionary<string, string>();
                }
            attributes[key] = value;
            }

        ///<summary>
        /// Recursively parses the string 'doc' starting at char index 'startIndex' and ending when there is an unmatched closing bracket or EOF.
        /// doc should have any opening bracket already stripped off.
        /// This recursive method is used both for parsing files, as well as for parsing specific entries inside files.
        ///</summary>
        private static List<TydNode> Parse(string doc, int startIndex, ref int currentLine, bool expectNames = true, List<TydNode> res = null, bool breakOnFirst = false)
            {
            var p = startIndex;
            res = res ?? new List<TydNode>();
            //Main loop
            while (true)
                {
                string recordName = null;
                Dictionary<string, string> attributes = null;

                try
                    {
                    var before = currentLine;
                    //Skip insubstantial chars
                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    //We reached EOF, so we're finished
                    if (p == doc.Length)
                        {
                        return res;
                        }

                    //Unhandled semicolon, likely after list or table, ignore it according to spec
                    if (doc[p] == ';')
                        {
                        p++;
                        continue;
                        }

                    //We reached a closing bracket, so we're finished with this record
                    if (doc[p] == Constants.TableEndChar || doc[p] == Constants.ListEndChar)
                        {
                        //To avoid counting lines twice from parent call, we subtract last new lines counted
                        currentLine = before;
                        return res;
                        }

                    //Read the record _name if we're not reading anonymous records
                    if (expectNames)
                        {
                        recordName = ReadSymbol(doc, currentLine, ref p);
                        }

                    //Skip whitespace
                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    //Read _attributes
                    while (doc[p] == Constants.AttributeStartChar)
                        {
                        //Skip past the '*' character
                        p++;

                        //Read the att _name
                        var attName = ReadSymbol(doc, currentLine, ref p);
                        p = NextSubstanceIndex(doc, p, ref currentLine);
                        if (doc[p] == Constants.AttributeStartChar || doc[p] == Constants.CommentChar || doc[p] == Constants.TableStartChar)
                            {
                            //No attribute value defined
                            SetAttributeValue(ref attributes, attName, null);
                            }
                        else
                            {
                            //Read the att value
                            string value;
                            ParseStringValue(doc, ref p, ref currentLine, out value);
                            SetAttributeValue(ref attributes, attName, value);
                            p = NextSubstanceIndex(doc, p, ref currentLine);
                            }

                        }
                    }
                catch (Exception e)
                    {
                    throw new Exception("Exception parsing Tyd headers at " + IndexToLocationString(doc, currentLine, p) + ": " + e.ToString(), e);
                    }

                //Read the record value.
                //After this is complete, p should be pointing at the char after the last char of the record.
                if (doc[p] == Constants.TableStartChar)
                    {
                    //It's a table
                    var newTable = new TydTable(recordName, currentLine);

                    //Skip past the opening bracket
                    p++;

                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    //Recursively parse all of new child's children and add them to it
                    foreach (var subNode in Parse(doc, p, ref currentLine, true, newTable.Nodes))
                        {
                        subNode.Parent = newTable;
                        p = subNode.DocIndexEnd + 1;
                        }

                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    if (doc[p] != Constants.TableEndChar)
                        {
                        throw new FormatException("Expected " + Constants.TableEndChar + " at " + IndexToLocationString(doc, currentLine, p));
                        }

                    newTable.DocIndexEnd = p;
                    newTable.SetupAttributes(attributes);
                    res.Add(newTable);

                    //Move pointer one past the closing bracket
                    p++;
                    }
                else if (doc[p] == Constants.ListStartChar)
                    {
                    //It's a list
                    var pStart = p;
                    var newList = new TydList(recordName, currentLine);

                    //Skip past the opening bracket
                    p++;

                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    //Recursively parse all of new child's children and add them to it
                    foreach (var subNode in Parse(doc, p, ref currentLine, false, newList.Nodes))
                        {
                        subNode.Parent = newList;
                        p = subNode.DocIndexEnd + 1;
                        }
                    p = NextSubstanceIndex(doc, p, ref currentLine);

                    if (p >= doc.Length || doc[p] != Constants.ListEndChar)
                        {
                        throw new FormatException(string.Format("Expected {0} from {1} at {2}",
                            Constants.ListEndChar, IndexToLocationString(doc, newList.DocLine, pStart), IndexToLocationString(doc, currentLine, p)));
                        }

                    newList.DocIndexEnd = p;
                    newList.SetupAttributes(attributes);
                    res.Add(newList);
                    if (breakOnFirst)
                        {
                        return res;
                        }
                    //Move pointer one past the closing bracket
                    p++;
                    }
                else
                    {
                    //It's a string
                    var pStart = p;
                    string val;
                    ParseStringValue(doc, ref p, ref currentLine, out val);

                    var strNode = new TydString(recordName, val, currentLine);
                    strNode.DocIndexEnd = p - 1;
                    res.Add(strNode);
                    if (breakOnFirst)
                        {
                        return res;
                        }
                    }
                }
            }

        //We are at the first char of a string value.
        //This returns the string value, and places p at the first char after it.
        private static void ParseStringValue(string doc, ref int p, ref int currentLine, out string val)
            {
            var quoted = doc[p] == '"';

            //Parse as a quoted string
            if (quoted)
                {
                p++; //Move past the opening quote
                var pStart = p;
                var lineStart = currentLine;
                //Walk forward until we find the end quote
                //We need to ignore any that are escaped
                while (p >= doc.Length || !(doc[p] == '"' && doc[p - 1] != '\\'))
                    {
                    if (p >= doc.Length)
                        {
                        throw new FormatException("Expected \" but reached end of file at " + IndexToLocationString(doc, lineStart, pStart));
                        }
                    if (doc[p] == '\n')
                        {
                        currentLine++;
                        }
                    p++;
                    }

                //Set the return value to the contents of the string
                val = doc.Substring(pStart, p - pStart);

                val = ResolveEscapeChars(val);

                //Move past the end quote so we're pointing just after it
                p++;
                }
            else //Parse as a naked string
                {
                var pStart = p;

                //Walk forward until we're on the first string content-terminating char or char group
                //We need to ignore any that are escaped
                while (p < doc.Length
                    && !IsNewline(doc, p)
                    && !((doc[p] == Constants.RecordEndChar
                        || doc[p] == Constants.TableStartChar
                        || doc[p] == Constants.AttributeStartChar
                        || doc[p] == Constants.CommentChar
                        || doc[p] == Constants.TableEndChar
                        || doc[p] == Constants.ListEndChar)
                            && doc[p - 1] != '\\'))
                    {
                    p++;
                    }

                //We are now pointing at the first char after the string value.
                //However, we now need to remove whitespace after the value.
                //So we make pointer q, and walk it backwards until it's on non-whitespace.
                //This lets us find the last non-whitespace char of the string value.
                var q = p - 1;
                while (char.IsWhiteSpace(doc[q]))
                    {
                    q--;
                    }

                val = q >= pStart ? doc.Substring(pStart, q - pStart + 1) : "";

                //Special case for 'null' naked string.
                val = val == "null" ? null : ResolveEscapeChars(val);
                }

            //Special case for ';': We want to be pointing after it, not on it.
            if (p < doc.Length && doc[p] == ';')
                {
                p++;
                }

            //Take the input string and replace any escape sequences with the final chars they correspond to.
            //This can be opimized
            }

        private static string ResolveEscapeChars(string input)
            {
            for (var k = 0; k < input.Length; k++)
                {
                if (input[k] == '\\')
                    {
                    if (input.Length <= k + 1)
                        {
                        throw new Exception("Tyd string value ends with single backslash: " + input);
                        }

                    var resolvedChar = EscapedCharOf(input[k + 1]);
                    input = input.Substring(0, k) + resolvedChar + input.Substring(k + 2);
                    }
                }
            return input;
            }

        //Returns the character that an escape sequence should resolve to, based on the second char of the escape sequence (after the backslash).
        private static char EscapedCharOf(char inputChar)
            {
            switch (inputChar)
                {
                case '\\': return '\\';
                case '"': return '"';
                case '#': return '#';
                case ';': return ';';
                case ']': return ']';
                case '}': return '}';
                case '\r': return '\u000D';
                case 'n': return '\u000A';
                case 't': return '\u0009';
                default: throw new Exception("Cannot escape char: \\" + inputChar);
                }
            }

        //Reads a symbol and return it. Places p at the first char after the symbol.
        //Symbols include:
        //  -Record names
        //  -Attribute names
        //  -Attribute values
        private static string ReadSymbol(string doc, int line, ref int p)
            {
            var pStart = p;
            while (true)
                {
                var c = doc[p];
                if (char.IsWhiteSpace(doc[p]))
                    {
                    break;
                    }

                if (!IsSymbolChar(c))
                    {
                    break;
                    }

                p++;
                }

            if (p == pStart)
                {
                throw new FormatException("Missing symbol at " + IndexToLocationString(doc, line, p));
                }

            return doc.Substring(pStart, p - pStart);
            }

        public static bool IsSymbolChar(char c)
            {
            //This can be optimized to a range check
            for (var i = 0; i < Constants.SymbolChars.Length; i++)
                {
                if (Constants.SymbolChars[i] == c)
                    {
                    return true;
                    }
                }
            return false;
            }

        private static bool IsNewline(string doc, int p)
            {
            return doc[p] == '\n' || (doc[p] == '\r' && p < doc.Length - 1 && doc[p + 1] == '\n');
            }

        private static string IndexToLocationString(string doc, int line, int index)
            {
            var col = 0;
            while (index >= 0 && index < doc.Length && doc[index] != '\n')
                {
                col++;
                index--;
                }
            return "line " + line + " col " + col;
            }

        ///<summary>
        /// Returns the index of the next char after p that is not whitespace or part of a comment.
        /// If there is no more substance in the doc, this returns an index just after the end of the doc.
        ///</summary>
        private static int NextSubstanceIndex(string doc, int p, ref int currentLine)
            {
            //As long as p keeps hitting comment starts or whitespace, we skip forward
            while (true)
                {
                //Reached end of doc - return an index just after doc end
                if (p >= doc.Length)
                    {
                    return doc.Length;
                    }

                //It's whitespace - skip over it
                if (char.IsWhiteSpace(doc[p]))
                    {
                    if (doc[p] == '\n')
                        {
                        currentLine++;
                        }
                    p++;
                    continue;
                    }

                //It's the comment char - skip to the next line
                if (doc[p] == Constants.CommentChar)
                    {
                    while (p < doc.Length && !IsNewline(doc, p))
                        {
                        p++;
                        }
                    currentLine++;
                    if (p < doc.Length)
                        {
                        //Skip past newline char(s). Since there may be just \n or \r\n, we have to handle both cases.
                        if (doc[p] == '\n')
                            {
                            p++;
                            }
                        else
                            {
                            p += 2; //If it's not \n, we assume it's \r\n and skip two
                            }
                        }
                    continue;
                    }

                //It's not whitespace or the comment char - it's substance
                return p;
                }
            }
        }

    }