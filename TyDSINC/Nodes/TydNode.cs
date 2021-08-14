using System.Collections.Generic;

namespace Tyd
    {
    ///<summary>
    /// Root class of all Tyd _nodes.
    ///</summary>
    public abstract class TydNode
        {
        //Data
        protected string _name;          //Can be null for anonymous _nodes

        //Data for error messages
        public int DocLine = -1;        //Line in the doc where this node starts
        public int DocIndexEnd = -1;    //Index in the doc where this node ends

        //Access
        public TydNode Parent { get; set; }

        public string Name
            {
            get { return _name; }
            set { _name = value; }
            }

        public int LineNumber
            {
            get { return DocLine; }
            }

        public string FullTyd
            {
            get { return TydToText.Write(this, true); }
            }

        //Construction
        public TydNode(string name, int docLine = -1)
            {
            _name = name;
            DocLine = docLine;
            }

        public IEnumerable<string> GetNodeValues()
            {
            var s = this as TydString;
            if (s != null)
                {
                yield return s.Value;
                }
            else
                {
                var coll = this as TydCollection;
                if (coll != null)
                    {
                    foreach (var value in coll.GetChildValues())
                        {
                        yield return value;
                        }
                    }
                }
            }

        public abstract TydNode DeepClone();
        }
    }