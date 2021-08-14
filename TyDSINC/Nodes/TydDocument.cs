using System.Collections.Generic;

namespace Tyd
    {
    ///<summary>
    /// Represents an entire Tyd document.
    ///</summary>
    public class TydDocument : TydTable
        {
        ///<summary>
        /// Create a new empty TydDocument.
        ///</summary>
        public TydDocument() : base(null)
            {
            _nodes = new List<TydNode>();
            }

        ///<summary>
        /// Create a new TydDocument from a list of TydNodes.
        ///</summary>
        public TydDocument(IEnumerable<TydNode> nodes) : base(null)
            {
            _nodes = new List<TydNode>();
            _nodes.AddRange(nodes);
            }

        public override string ToString()
            {
            return string.Format("{0}({1}, {2})", Name, "TydDocument", Count);
            }
        }
    }