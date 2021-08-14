using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Tyd
    {
    ///<summary>
    /// Represents a file of Tyd data.
    /// To read a file: Use FromFile to create a TydFile from a file on disk, and then read the data you want from the TydFile.
    /// To write to a file: Build your TydDocument, create a TydFile from it, then write the TydFile to disk.
    ///</summary>
    public class TydFile
        {
        //Data
        protected TydDocument _docNode;
        protected string _filePath = null;

        //Properties
        public TydDocument DocumentNode
            {
            get { return _docNode; }
            set { _docNode = value; }
            }

        public string FilePath
            {
            get { return _filePath; }
            }

        public string FileName
            {
            get { return Path.GetFileName(_filePath); }
            }

        private TydFile() { }

        ///<summary>
        /// Create a new TydFile from a TydDocument.
        ///</summary>
        public static TydFile FromDocument(TydDocument doc, string filePath = null)
            {
            doc.Name = Path.GetFileName(filePath);
            var t = new TydFile();
            t._docNode = doc;
            t._filePath = filePath;
            return t;
            }

        public static TydFile FromContent(string content, string filePath)
            {
            try
                {
                var tydNodeList = TydFromText.Parse(content);
                var tydDoc = new TydDocument(tydNodeList);
                return FromDocument(tydDoc, filePath);
                }
            catch (Exception e)
                {
                throw new Exception("Exception loading " + filePath + ": " + e);
                }
            }

        public static List<TydFile> ReadAndResolvePath(string path, params string[] exception)
            {
            var tyds = new List<TydFile>();
            foreach (var file in Directory.GetFiles(path, "*.tyd"))
                {
                if (exception != null && exception.Length > 0 && exception.Contains(Path.GetFileNameWithoutExtension(file).ToLower()))
                    {
                    continue;
                    }
                tyds.Add(FromContent(File.ReadAllText(file), file));
                }
            foreach (var file in tyds)
                {
                Inheritance.RegisterAllFrom(file.DocumentNode);
                }
            Inheritance.ResolveAll();
            Inheritance.Clear();
            return tyds;
            }

        ///<summary>
        /// Create a new TydFile by loading data from a file at the given path.
        ///</summary>
        public static TydFile FromFile(string filePath, bool treatXmlAsOneObject = false)
            {
            try
                {
                if (Path.GetExtension(filePath).ToLowerInvariant() == ".xml")
                    {
                    //File is xml format
                    //Load it and convert the tyd _nodes from it
                    var contents = File.ReadAllText(filePath);
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(contents);
                    var nodes = new List<TydNode>();
                    if (treatXmlAsOneObject)
                        {
                        nodes.Add(TydXml.TydNodeFromXmlDocument(xmlDoc));
                        }
                    else
                        {
                        nodes.AddRange(TydXml.TydNodesFromXmlDocument(xmlDoc));
                        }

                    return FromDocument(new TydDocument(nodes), filePath);
                    }
                else
                    {
                    //If it's any extension besides xml, we assume the file is Tyd format
                    string readContents;
                    using (var streamReader = new StreamReader(filePath))
                        {
                        readContents = streamReader.ReadToEnd();
                        }
                    var tydNodeList = TydFromText.Parse(readContents);
                    var tydDoc = new TydDocument(tydNodeList);
                    return FromDocument(tydDoc, filePath);
                    }
                }
            catch (Exception e)
                {
                throw new Exception("Exception loading " + filePath + ": " + e);
                }
            }

        ///<summary>
        /// Returns all the objects defined in the file, serialized to type T.
        ///</summary>
        /*public IEnumerable<T> GetObjects<T>() where T : new()
        {
            if( _docNode == null )
                throw new Exception("TydFile has no document node: " + FileName);

            foreach (var n in TydHelper.TydToObject.GetObjects<T>(_docNode,_filePath))
            {
                yield return n;
            }
        }*/

        /// <summary>
        /// Returns the single object defined by the file, deserialized as type T.
        /// </summary>
        /*public T GetObject<T>( bool resolveCrossRefs = false ) where T : new()
        {
            if( _docNode == null )
                throw new Exception("TydFile has no document node: " + FileName);

            if( _docNode.Count == 0 )
                throw new Exception("TydFile contains a document node but no data: " + FileName );

            return TydHelper.TydToObject.GetObject<T>(_docNode, resolveCrossRefs, _filePath );
        }*/

        ///<summary>
        /// Write to a file, overwriting any file present.
        /// If a path is provided, the file's path is changed to that new path before saving. Otherwise, the current path is used.
        ///</summary>
        public void Save(string path = null)
            {
            if (path != null)
                {
                _filePath = path;
                }
            else if (_filePath == null)
                {
                throw new InvalidOperationException("Saved TydFile which had null path");
                }

            //Build the text we're going to write
            var tydText = new StringBuilder();
            foreach (var node in _docNode)
                {
                tydText.AppendLine(TydToText.Write(node, true));
                }

            //Write to the file
            File.WriteAllText(_filePath, tydText.ToString().TrimEnd());
            }

        public override string ToString()
            {
            return FileName;
            }
        }

    }
