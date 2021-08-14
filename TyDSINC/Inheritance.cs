using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyd
    {

    /// <summary>
    /// Handles inheritance between TydNodes via handle and source _attributes.
    ///
    /// To use Inheritance:
    /// 1. Call Initialize().
    /// 2. Register all the _nodes you want to interact with each other.
    /// 3. Call ResolveAll. This will modify the registered _nodes in-place with any inheritance data.
    /// 4. Call Complete().
    ///
    /// It's recommended you use try/catch to ensure that Complete is always called.
    /// </summary>
    public static class Inheritance
        {
        private class InheritanceNode
            {
            public TydCollection TydNode;
            public bool Resolved;
            public InheritanceNode Source;        // Node from which I inherit.
            private List<InheritanceNode> _heirs = null;  // Nodes which inherit from me.

            public InheritanceNode(TydCollection tydNode)
                {
                this.TydNode = tydNode;
                }

            public int HeirCount
                {
                get { return _heirs != null ? _heirs.Count : 0; }
                }

            public InheritanceNode GetHeir(int index)
                {
                return _heirs[index];
                }

            public void AddHeir(InheritanceNode n)
                {
                if (_heirs == null)
                    {
                    _heirs = new List<InheritanceNode>();
                    }

                _heirs.Add(n);
                }

            public override string ToString()
                {
                return TydNode.ToString();
                }
            }

        //Working vars
        private static List<InheritanceNode> nodesUnresolved = new List<InheritanceNode>();
        private static Dictionary<TydNode, InheritanceNode> nodesResolved = new Dictionary<TydNode, InheritanceNode>();
        private static Dictionary<string, InheritanceNode> nodesByHandle = new Dictionary<string, InheritanceNode>();

        public static void Clear()
            {
            nodesResolved.Clear();
            nodesUnresolved.Clear();
            nodesByHandle.Clear();
            }

        ///<summary>
        /// Registers a single node.
        /// When we resolve later, we'll be able to use this node as a source.
        ///</summary>
        public static void Register(TydCollection colNode)
            {

            //If the node has no handle, and no source, we can ignore it since it's not connected to inheritance at all.
            var nodeHandle = colNode.AttributeHandle;
            var nodeSource = colNode.AttributeSource;
            if (nodeHandle == null && nodeSource == null)
                {
                return;
                }

            //Ensure we're don't have two _nodes of the same handle
            if (nodeHandle != null && nodesByHandle.ContainsKey(nodeHandle))
                {
                throw new Exception(string.Format("Tyd error: Multiple Tyd _nodes with the same handle {0}.", nodeHandle));
                }

            //Make an inheritance node for the Tyd node
            var newNode = new InheritanceNode(colNode);
            nodesUnresolved.Add(newNode);
            if (nodeHandle != null)
                {
                nodesByHandle.Add(nodeHandle, newNode);
                }
            }

        ///<summary>
        /// Registers all _nodes from doc.
        /// When we resolve later, we'll be able to use the _nodes in this document as a sources.
        ///</summary>
        public static void RegisterAllFrom(TydDocument doc)
            {

            for (var i = 0; i < doc.Count; i++)
                {
                var tydCol = doc[i] as TydCollection;
                if (tydCol != null)
                    {
                    Register(tydCol);
                    }
                }
            }

        ///<summary>
        /// Resolves all registered _nodes.
        ///</summary>
        public static void ResolveAll()
            {

            LinkAllInheritanceNodes();
            ResolveAllUnresolvedInheritanceNodes();
            }

        // Merge all unresolved _nodes with their source _nodes.
        private static void ResolveAllUnresolvedInheritanceNodes()
            {
            // find roots from which we'll start resolving _nodes,
            // a node is a root node if it has null source or its source has been already resolved,
            // this method works only for single inheritance!
            var roots = nodesUnresolved.Where(x => x.Source == null || x.Source.Resolved).ToList(); // important to make a copy

            for (var i = 0; i < roots.Count; i++)
                {
                ResolveInheritanceNodeAndHeirs(roots[i]);
                }

            // check if there are any unresolved _nodes (if there are, then it means that there is a cycle),
            // and move _nodes to resolved _nodes collection
            for (var i = 0; i < nodesUnresolved.Count; i++)
                {
                if (!nodesUnresolved[i].Resolved)
                    {
                    throw new FormatException("Tyd error: Cyclic inheritance detected for node:\n" + nodesUnresolved[i].TydNode.FullTyd);
                    //continue;
                    }
                nodesResolved.Add(nodesUnresolved[i].TydNode, nodesUnresolved[i]);
                }

            nodesUnresolved.Clear();
            }

        // Link all unresolved _nodes to their sources and heirs.
        private static void LinkAllInheritanceNodes()
            {
            for (var i = 0; i < nodesUnresolved.Count; i++)
                {
                var urn = nodesUnresolved[i];

                var attSource = urn.TydNode.AttributeSource;
                if (attSource == null)
                    {
                    continue;
                    }

                if (!nodesByHandle.TryGetValue(attSource, out urn.Source))
                    {
                    throw new Exception(string.Format("Could not find source node named '{0}' for Tyd node: {1}", attSource, urn.TydNode.FullTyd));
                    }

                if (urn.Source != null)
                    {
                    urn.Source.AddHeir(urn);
                    }
                }
            }

        ///<summary>
        /// Resolves given node and then all its heir _nodes recursively using DFS.
        ///</summary>
        private static void ResolveInheritanceNodeAndHeirs(InheritanceNode node)
            {
            //Error check
            // if we've reached a resolved node by traversing the tree, then it means
            // that there's a cycle, note that we're not reporting the full cycle in
            // the error message here, but only the last node which created a cycle
            if (node.Resolved)
                {
                throw new Exception(string.Format("Cyclic inheritance detected for Tyd node:\n{0}", node.TydNode.FullTyd));
                }

            //Resolve this node
                {
                if (node.Source == null)
                    {
                    // No source - Just use the original node
                    node.Resolved = true;
                    }
                else
                    {
                    //Source exists - We now inherit from it
                    //We must use source's RESOLVED node here because our source can have its own source.
                    if (!node.Source.Resolved)
                        {
                        throw new Exception(string.Format(
                        "Tried to resolve Tyd inheritance node {0} whose source has not been resolved yet. This means that this method was called in incorrect order.",
                        node));
                        }

                    CheckForDuplicateNodes(node.TydNode);

                    node.Resolved = true;

                    //Write resolved node's class attribute
                    //Original takes precedence over source; source takes precedence over default
                    var attClass = node.TydNode.AttributeClass ?? node.Source.TydNode.AttributeClass;
                    node.TydNode.SetAttribute("class", attClass);

                    //Apply inheritance from source to node
                    ApplyInheritance(node.Source.TydNode, node.TydNode);
                    }
                }

            //Recur to the heirs and resolve them too
            for (var i = 0; i < node.HeirCount; i++)
                {
                ResolveInheritanceNodeAndHeirs(node.GetHeir(i));
                }
            }

        ///<summary>
        /// Copies all child _nodes from source into heir, recursively.
        /// -If a node appears only in source or only in heir, it is included.
        /// -If a list appears in both source and heir, source's entries are appended to heir's entries.
        /// -If a non-list node appears in both source and heir, heir's node is overwritten.
        ///</summary>
        private static void ApplyInheritance(TydNode source, TydNode heir)
            {
            try
                {
                //They're either strings or nulls: We just keep the existing heir's value
                if (source is TydString)
                    {
                    return;
                    }

                //Heir has noinherit attribute: Skip this inheritance
                    {
                    var heirCol = heir as TydCollection;
                    if (heirCol != null && heirCol.AttributeNoInherit)
                        {
                        return;
                        }
                    }

                //They're tables: Combine all children of source and heir. Unique-name source nodes are prepended
                    {
                    var sourceObj = source as TydTable;
                    if (sourceObj != null)
                        {
                        var heirTable = (TydTable)heir;
                        for (var i = 0; i < sourceObj.Count; i++)
                            {
                            var sourceChild = sourceObj[i];
                            var heirMatchingChild = heirTable[sourceChild.Name];

                            if (heirMatchingChild != null)
                                {
                                ApplyInheritance(sourceChild, heirMatchingChild);
                                }
                            else
                                {
                                heirTable.InsertChild(sourceChild.DeepClone(), 0); 
                                }
                            }
                        return;
                        }
                    }

                //They're lists: Prepend source's children before heir's children
                    {
                    var sourceList = source as TydList;
                    if (sourceList != null)
                        {
                        var heirList = (TydList)heir;
                        for (var i = 0; i < sourceList.Count; i++)
                            {
                            //Insert at i so the nodes stay in the same order from source to heir
                            heirList.InsertChild(sourceList[i].DeepClone(), i); 
                            }
                        return;
                        }
                    }
                }
            catch (Exception e)
                {
                throw new Exception("ApplyInheritance exception: " + e + ".\nsource: (" + source + ")\n" + TydToText.Write(source, true) + "\ntarget: (" + heir + ")\n" + TydToText.Write(heir, true));
                }
            }

        private static HashSet<string> tempUsedNodeNames = new HashSet<string>();
        private static void CheckForDuplicateNodes(TydCollection originalNode)
            {
            //This is needed despite another check elsewhere
            //Because the source-data-combination process wipes out duplicate Tyd data

            tempUsedNodeNames.Clear();

            for (var i = 0; i < originalNode.Count; i++)
                {
                var node = originalNode[i];

                if (node.Name == null)
                    {
                    continue;
                    }

                if (tempUsedNodeNames.Contains(node.Name))
                    {
                    throw new FormatException("Tyd error: Duplicate Tyd node _name " + node.Name + " in this Tyd block: " + originalNode);
                    }
                else
                    {
                    tempUsedNodeNames.Add(node.Name);
                    }
                }

            tempUsedNodeNames.Clear();
            }
        }

    }