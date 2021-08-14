using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tyd
    {
    ///<summary>
    /// A TydNode that contains a collection of sub-_nodes.
    ///</summary>
    public abstract class TydCollection : TydNode, IEnumerable<TydNode>
        {
        //Data
        protected List<TydNode> _nodes = new List<TydNode>();
        protected Dictionary<string, string> _attributes;

        //Properties
        public string AttributeClass
            {
            get { return GetAttributeOrNull("class"); }
            set { SetAttribute("class", value); }
            }

        public string AttributeHandle
            {
            get { return GetAttributeOrNull("handle"); }
            set { SetAttribute("handle", value); }
            }

        public string AttributeSource
            {
            get { return GetAttributeOrNull("source"); }
            set { SetAttribute("source", value); }
            }

        public bool AttributeAbstract
            {
            get { return HasAttribute("abstract"); }
            set { UnsetAttribute("abstract", !value); }
            }

        public bool AttributeNoInherit
            {
            get { return HasAttribute("noinherit"); }
            set { UnsetAttribute("noinherit", !value); }
            }

        public int Count
            {
            get { return _nodes.Count; }
            }

        public List<TydNode> Nodes
            {
            get { return _nodes; }
            set { _nodes = value; }
            }

        public TydNode this[int index]
            {
            get { return _nodes[index]; }
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        public IEnumerator<TydNode> GetEnumerator()
            {
            foreach (var n in _nodes)
                {
                yield return n;
                }
            }

        public TydCollection(string name, int docLine = -1) : base(name, docLine)
            {
            }

        public void SetupAttributes(Dictionary<string, string> attributes)
            {
            _attributes = attributes;
            }

        /// <summary>
        /// Return all values in child _nodes converted to type T, non recursively
        /// </summary>
        /// <param name="onlyStrings">Whether to throw an exception if non-strings are encountered</param>
        public IEnumerable<T> GetChildValues<T>(bool onlyStrings = true)
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var str = _nodes[i] as TydString;
                if (str == null)
                    {
                    if (onlyStrings)
                        {
                        throw new Exception(string.Format("Could not convert node in {0} as it is not a string", Name));
                        }
                    else
                        {
                        continue;
                        }
                    }
                yield return str.GetValue<T>(Name);
                }
            }

        /// <summary>
        /// Return all values in child _nodes, non recursively
        /// </summary>
        /// <param name="onlyStrings">Whether to throw an exception if non-strings are encountered</param>
        public IEnumerable<string> GetChildValues(bool onlyStrings = true)
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var str = _nodes[i] as TydString;
                if (str == null)
                    {
                    if (onlyStrings)
                        {
                        throw new Exception(string.Format("Node in {0} is not a string", Name));
                        }
                    else
                        {
                        continue;
                        }
                    }
                yield return str.Value;
                }
            }

        /// <summary>
        /// Returns the value of the child with the specified _name, converted to the type T, if it is a TydString
        /// </summary>
        /// <param name="required">Whether to throw an exception if the node does not exist</param>
        public T GetChildValue<T>(string name, bool required = true, T defaultValue = default(T))
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var child = _nodes[i];

                if (name.Equals(child.Name))
                    {
                    return GetChildValue<T>(i);
                    }
                }
            if (required)
                {
                throw new Exception(string.Format("Missing node {0} in {1}", name, Name));
                }
            return defaultValue;
            }


        /// <summary>
        /// Returns the value of the child with the specified name, if it is a TydString
        /// </summary>
        /// <param name="required">Whether to throw an exception if the node does not exist</param>
        public string GetChildValue(string name, bool required = true)
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var child = _nodes[i];
                if (name.Equals(child.Name))
                    {
                    return GetChildValue(i);
                    }
                }
            if (required)
                {
                throw new Exception(string.Format("Missing node {0} in {1}", name, Name));
                }
            return null;
            }

        /// <summary>
        /// Returns the value of the child at index idx, converted to the type T, if it is a TydString
        /// </summary>
        public T GetChildValue<T>(int idx)
            {
            if (idx < 0 || idx >= _nodes.Count)
                {
                throw new Exception(string.Format("Index is out of bounds for {0}", Name));
                }
            var str = _nodes[idx] as TydString;
            if (str == null)
                {
                throw new Exception(string.Format("Node {0} in {1} is not a string", _nodes[idx].Name, Name));
                }
            return str.GetValue<T>();
            }

        /// <summary>
        /// Returns the value of the child at index idx, if it is a TydString
        /// </summary>
        public string GetChildValue(int idx)
            {
            if (idx < 0 || idx >= _nodes.Count)
                {
                throw new Exception(string.Format("Index is out of bounds for {0}", Name));
                }
            var str = _nodes[idx] as TydString;
            if (str == null)
                {
                throw new Exception(string.Format("Node {0} in {1} is not a string", _nodes[idx].Name, Name));
                }
            return str.Value;
            }

        /// <summary>
        /// Returns the child with the specified _name
        /// </summary>
        /// <param name="required">Whether to throw an exception if the node does not exist</param>
        public TydNode GetChild(string name, bool required = false)
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var child = _nodes[i];
                if (name.Equals(child.Name))
                    {
                    return child;
                    }
                }
            if (required)
                {
                throw new Exception(string.Format("Missing node {0} in {1}", name, Name));
                }
            return null;
            }


        /// <summary>
        /// Returns the child with the specified _name as the specified Tyd Type
        /// </summary>
        /// <typeparam name="T">A valid Tyd node type</typeparam>
        /// <param name="required">Whether to throw an exception if the node does not exist</param>
        public T GetChild<T>(string name, bool required = false) where T : TydNode
            {
            for (var i = 0; i < _nodes.Count; i++)
                {
                var child = _nodes[i];
                if (name.Equals(child.Name))
                    {
                    return (T)child;
                    }
                }
            if (required)
                {
                throw new Exception(string.Format("Missing node {0} in {1}", name, Name));
                }
            return null;
            }

        public TydTable Seek(string key, string value)
            {
            var t = this as TydTable;
            if (t != null)
                {
                var n = GetChildValue(key, false);
                if (value.Equals(n))
                    {
                    return t;
                    }
                }
            for (var i = 0; i < _nodes.Count; i++)
                {
                var node = _nodes[i] as TydCollection;
                if (node != null)
                    {
                    var res = node.Seek(key, value);
                    if (res != null)
                        {
                        return res;
                        }
                    }
                }
            return null;
            }

        public IEnumerable<KeyValuePair<string, string>> GetAttributes()
            {
            if (_attributes != null)
                {
                foreach (var pair in _attributes)
                    {
                    yield return pair;
                    }
                }
            }

        public void SetAttribute(string key, string value)
            {
            if (_attributes == null)
                {
                _attributes = new Dictionary<string, string>();
                }
            _attributes[key] = value;
            }


        /// <summary>
        /// Remove an attribute from the table
        /// </summary>
        /// <param name="unset">Whether to remove or add with no value</param>
        public void UnsetAttribute(string key, bool unset)
            {
            if (_attributes == null)
                {
                if (unset)
                    {
                    return;
                    }
                _attributes = new Dictionary<string, string>();
                }
            if (unset)
                {
                _attributes.Remove(key);
                }
            else
                {
                _attributes[key] = null;
                }
            }

        /// <summary>
        /// Try getting an attribute value
        /// </summary>
        public bool TryGetAttribute(string key, out string value)
            {
            if (_attributes != null)
                {
                return _attributes.TryGetValue(key, out value);
                }
            value = null;
            return false;
            }

        /// <summary>
        /// Whether the table has the attribute defined
        /// </summary>
        public bool HasAttribute(string key)
            {
            return _attributes != null && _attributes.ContainsKey(key);
            }

        /// <summary>
        /// Returns the value of ann attribute or null
        /// </summary>
        public string GetAttributeOrNull(string key, string defaultValue = null)
            {
            if (_attributes != null)
                {
                string value;
                if (_attributes.TryGetValue(key, out value))
                    {
                    return value;
                    }
                }
            return defaultValue;
            }

        ///<summary>
        /// Add a node as a child of this node, and link it as a parent.
        ///</summary>
        public T AddChild<T>(T node) where T : TydNode
            {
            _nodes.Add(node);
            node.Parent = this;
            return node;
            }

        public T InsertChild<T>(T node, int id) where T : TydNode
            {
            _nodes.Insert(id, node);
            node.Parent = this;
            return node;
            }

        /// <summary>
        /// Replaces the node that has the same name as the supplied node directly
        /// If no node has the same name, the supplied node will get appended to the end
        /// </summary>
        /// <returns>The supplied node</returns>
        public T ReplaceChild<T>(T node) where T : TydNode
            {
            node.Parent = this;
            for (var i = 0; i < _nodes.Count; i++)
                {
                var n = _nodes[i];
                if (node.Name.Equals(n.Name))
                    {
                    _nodes[i] = node;
                    return node;
                    }
                }
            _nodes.Add(node);
            return node;
            }

        public void AddChildren<T>(params T[] ns) where T : TydNode
            {
            foreach (var node in ns)
                {
                _nodes.Add(node);
                node.Parent = this;
                }
            }

        protected void CopyDataFrom(TydCollection other)
            {
            other.DocIndexEnd = DocIndexEnd;
            other._attributes = other._attributes == null ? null : other._attributes.ToDictionary(x => x.Key, x => x.Value);
            for (var i = 0; i < _nodes.Count; i++)
                {
                other.AddChild(_nodes[i].DeepClone());
                }
            }
        }
    }