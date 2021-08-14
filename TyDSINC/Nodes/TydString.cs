using System;
using System.ComponentModel;

namespace Tyd
    {
    ///<summary>
    /// Represents a record of a single string. Also used to represent records with null values.
    ///</summary>
    public class TydString : TydNode
        {
        //Properties
        public string Value { get; set; }

        public TydString(string name, string val, int docLine = -1) : base(name, docLine)
            {
            Value = val;
            }

        public override TydNode DeepClone()
            {
            var c = new TydString(_name, Value, DocLine);
            c.DocIndexEnd = DocIndexEnd;
            return c;
            }

        public override string ToString()
            {
            return string.Format("{0}=\"{1}\"", Name ?? "NullName", Value);
            }

        /// <summary>
        /// Converts the string to a value of type T
        /// </summary>
        /// <param name="name">If this is a nameless record from list, you can supply the list _name here for better exception messages</param>
        public T GetValue<T>(string name = null)
            {
            var t = typeof(T);
            T val;
            try
                {
                val = (T)TypeDescriptor.GetConverter(t).ConvertFrom(Value);
                }
            catch (Exception)
                {
                throw new Exception(string.Format("Could not convert node {1} = \"{2}\" to {0}", t.Name, name ?? Name, Value));
                }

            if (val != null)
                {
                return val;
                }
            throw new Exception(string.Format("Could not convert node {1} = \"{2}\" to {0}", t.Name, name ?? Name, Value));
            }
        }
    }