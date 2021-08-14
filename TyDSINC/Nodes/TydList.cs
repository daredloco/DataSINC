namespace Tyd
    {
    ///<summary>
    /// Contains an ordered collection of anonymous TydNodes.
    /// Generally used to represent lists of items.
    ///</summary>
    public class TydList : TydCollection
        {
        public TydList(string name, int docLine = -1) : base(name, docLine)
            {
            }

        public TydList(string name, params TydNode[] children) : base(name)
            {
            AddChildren(children);
            }

        public TydList(string name, params string[] children) : base(name)
            {
            for (var i = 0; i < children.Length; i++)
                {
                AddChild(new TydString(null, children[i]));
                }
            }

        public override TydNode DeepClone()
            {
            var c = new TydList(_name, DocLine);
            CopyDataFrom(c);
            return c;
            }

        public override string ToString()
            {
            return string.Format("{0}({1}, {2})", Name, "TydList", Count);
            }
        }
    }