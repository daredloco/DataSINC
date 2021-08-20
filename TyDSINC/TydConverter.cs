using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Tyd
{
    public static class TydConverter
    {
        public static T Deserialize<T>(TydNode node, bool useEmptyConstructor = false)
        {
            return (T)Deserialize(node, typeof(T), useEmptyConstructor);
        }

        public static object Deserialize(TydNode node, Type t, bool useEmptyConstructor = false)
        {
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
            }
            var str = node as TydString;
            if (t == typeof(string))
            {
                return str.Value;
            }
            if (str != null && str.Value == null)
            {
                return null;
            }
            if (t.IsPrimitive)
            {
                return TypeDescriptor.GetConverter(t).ConvertFrom(str.Value);
            }
            else if (t.IsArray)
            {
                var n = node as TydList;
                var subType = t.GetElementType();
                if(n == null)
				{
                    var ns = node as TydString;
                    n = new TydList(node.Name, ns);
				}
                var result = Array.CreateInstance(subType, n.Count);
                for (var i = 0; i < n.Count; i++)
                {
                    result.SetValue(Deserialize(n[i], subType), i);
                }
                return result;
            }
            var members = FormatterServices.GetSerializableMembers(t);
            System.Collections.Generic.List<MemberInfo> tmpmembers = new System.Collections.Generic.List<MemberInfo>();
            foreach (var member in members)
            {
                if (!Attribute.IsDefined(member, typeof(TydAttributes.TydIgnore)))
                {
                    tmpmembers.Add(member);
                }
            }
            members = tmpmembers.ToArray();
            var objects = new object[members.Length];
            var res = useEmptyConstructor ? Activator.CreateInstance(t) : FormatterServices.GetUninitializedObject(t);
            var table = node as TydTable;
            for (var i = 0; i < members.Length; i++)
            {
                var info = members[i] as FieldInfo;
                if (Attribute.IsDefined(info, typeof(NonSerializedAttribute)))
                {
                    continue;
                }
                if (Attribute.IsDefined(info, typeof(TydAttributes.TydIgnore)))
                {
                    continue;
                }
                var child = Attribute.IsDefined(info, typeof(TydAttributes.TydName)) ? table.GetChild(((TydAttributes.TydName)Attribute.GetCustomAttribute(info, typeof(TydAttributes.TydName))).GetName()) : table.GetChild(info.Name);
                if (child == null)
                {
                    continue;
                }
                objects[i] = Deserialize(child, info.FieldType);
            }
            FormatterServices.PopulateObjectMembers(res, members, objects);
            return res;
        }

        public static TydNode Serialize(string name, object obj, bool ignorenullvalues = false)
        {
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
            }
            if (obj == null)
            {
                return new TydString(name, null);
            }
            var t = obj.GetType();
            if (t.IsPrimitive || t == typeof(string))
            {
                return new TydString(name, obj.ToString());
            }
            else if (t.IsArray)
            {
                var result = new TydList(name);
                var arr = obj as Array;
                for (var i = 0; i < arr.Length; i++)
                {
                    result.AddChild(Serialize(null, arr.GetValue(i)));
                }
                return result;
            }
            var res = new TydTable(name);
            var members = FormatterServices.GetSerializableMembers(t);
            for (var i = 0; i < members.Length; i++)
            {
                var info = members[i] as FieldInfo;
                if (Attribute.IsDefined(info, typeof(NonSerializedAttribute)))
                {
                    continue;
                }
                if (Attribute.IsDefined(info, typeof(TydAttributes.TydIgnore)))
                {
                    continue;
                }
                if (ignorenullvalues && info.GetValue(obj) == null)
				{
                    continue;
                }
                string tydname = Attribute.IsDefined(info, typeof(TydAttributes.TydName)) ? ((TydAttributes.TydName)Attribute.GetCustomAttribute(info, typeof(TydAttributes.TydName))).GetName() : info.Name;
                res.AddChild(Serialize(tydname, info.GetValue(obj)));
            }
            return res;
        }
    }
}