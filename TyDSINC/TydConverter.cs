using System;
using System.ComponentModel;
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
                var result = Array.CreateInstance(subType, n.Count);
                for (var i = 0; i < n.Count; i++)
                    {
                    result.SetValue(Deserialize(n[i], subType), i);
                    }
                return result;
                }
            var members = FormatterServices.GetSerializableMembers(t);
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
                var child = table.GetChild(info.Name);
                if (child == null)
                    {
                    continue;
                    }
                objects[i] = Deserialize(child, info.FieldType);
                }
            FormatterServices.PopulateObjectMembers(res, members, objects);
            return res;
            }

        public static TydNode Serialize(string name, object obj)
            {
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
                res.AddChild(Serialize(info.Name, info.GetValue(obj)));
                }
            return res;
            }
        }
    }