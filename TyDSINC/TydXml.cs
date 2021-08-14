using System.Collections.Generic;
using System.Xml;

namespace Tyd
    {
    public static class TydXml
        {
        ///<summary>
        /// Read an XML document and convert it to a single Tyd node.
        /// This treats the XML root as the root of a single table.
        ///</summary>
        public static TydNode TydNodeFromXmlDocument(XmlDocument xmlDocument)
            {
            return TydNodeFromXmlNode(xmlDocument.DocumentElement);
            }

        ///<summary>
        /// Read an XML document and convert it to a sequence of Tyd _nodes.
        /// This ignores the XML root and treats each of the root's children as a separate Tyd table.
        ///</summary>
        public static IEnumerable<TydNode> TydNodesFromXmlDocument(XmlDocument xmlDocument)
            {
            foreach (XmlNode xmlChild in xmlDocument.DocumentElement.ChildNodes)
                {
                var newNode = TydNodeFromXmlNode(xmlChild);
                if (newNode != null)
                    {
                    yield return newNode;
                    }
                }
            }

        ///<summary>
        /// Convert a single XML tree into a Tyd tree.
        /// If expectName is false, it'll be parsed as a list item.
        ///</summary>
        public static TydNode TydNodeFromXmlNode(XmlNode xmlRoot)
            {
            if (xmlRoot is XmlComment)
                {
                return null;
                }

            var newTydName = xmlRoot.Name != "li"
            ? xmlRoot.Name
            : null;

            //Record _attributes here so we can use them later
            var attributes = new Dictionary<string, string>();

            var xmlAttributes = xmlRoot.Attributes;
            if (xmlAttributes != null)
                {
                foreach (XmlAttribute a in xmlAttributes)
                    {
                    attributes[a.Name] = a.Value;
                    }
                }

            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild is XmlText)
                {
                //It's a string
                return new TydString(newTydName, xmlRoot.FirstChild.InnerText);
                }
            else if (xmlRoot.HasChildNodes && xmlRoot.FirstChild.Name == "li")
                {
                //Children are named 'li'
                //It's a list

                var tydRoot = new TydList(newTydName);
                tydRoot.SetupAttributes(attributes);
                foreach (XmlNode xmlChild in xmlRoot.ChildNodes)
                    {
                    tydRoot.AddChild(TydNodeFromXmlNode(xmlChild));
                    }
                return tydRoot;
                }
            else
                {
                //This case catches _nodes with no children.
                //Note that the case of no children is ambiguous between list and table; we choose list arbitrarily.

                //It's a table
                var tydRoot = new TydTable(newTydName);
                foreach (XmlNode xmlChild in xmlRoot.ChildNodes)
                    {
                    tydRoot.AddChild(TydNodeFromXmlNode(xmlChild));
                    }
                return tydRoot;
                }
            }
        }
    }