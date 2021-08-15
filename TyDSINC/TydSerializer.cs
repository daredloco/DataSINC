using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyd;

namespace Tyd
{
	public static class TydSerializer
	{
		//public static T Deserialize<T>(TydFile file)
		//{

		//}

		//public static T Deserialize<T>(TydDocument doc)
		//{

		//}

		/// <summary>
		/// Deserializes a TydList into a List<T>
		/// Throws an InvalidCastException if can't parse TydList content to TydString or content from TydString to T
		/// </summary>
		/// <typeparam name="T">The type of the variables inside the list</typeparam>
		/// <param name="list">The TydList you want to convert</param>
		/// <returns>A List<T> with the contents from TydList</returns>
		public static List<T> Deserialize<T>(TydList list)
		{
			List<T> lst = new List<T>();
			foreach(TydNode item in list)
			{
				//Check if node can be parsed to TydString
				TydString tydstring = item as TydString;
				if(tydstring == null)
				{
					throw new InvalidCastException("Content inside TydList needs to be parsable to TydString!");
				}

				//Check if content from TydString can be parsed to T
				T value = tydstring.GetValue<T>();
				if(value == null)
				{
					throw new InvalidCastException("Couldn't cast content from TydString to T!");
				}

				lst.Add(value);
			}
			return lst;
		}

		/// <summary>
		/// Deserializes a TydTable into a Dictionary<string, T>.
		/// Throws an InvalidCastException if can't parse TydTable content to TydString
		/// </summary>
		/// <typeparam name="T">The type of the value inside the dictionary</typeparam>
		/// <param name="table">The TydTable you want to convert</param>
		/// <returns>A Dictionary<string, T> from the TydTable</returns>
		public static Dictionary<string, T> Deserialize<T>(TydTable table)
		{
			Dictionary<string, T> dict = new Dictionary<string, T>();
			foreach(TydNode item in table)
			{
				//Check if node can be parsed to TydString
				TydString tydstring = item as TydString;
				if (tydstring == null)
				{
					throw new InvalidCastException("Content inside TydTable needs to be parsable to TydString!");
				}

				string key = tydstring.Name;
				T value = (T)(object)tydstring.Value;

				dict.Add(key, value);
			}
			return dict;
		}

		//public static TydNode Serialize<T>(T obj)
		//{

		//}

		/// <summary>
		/// Deserializes a TydList into a List<T>
		/// Throws an InvalidCastException if can't parse List<T> content to a string
		/// </summary>
		/// <typeparam name="T">The type of content inside the List (Needs to be convertable into a string)</typeparam>
		/// <param name="list">The list containing the content you want to serialize</param>
		/// <param name="name">The name of the TydList</param>
		/// <returns>A TydList containing the serialized data</returns>
		public static TydList Serialize<T>(List<T> list, string name = "")
		{
			TydList tydlist = new TydList(name);
			foreach(T item in list)
			{
				//Check if item can be parsed to string
				if((item as string) == null) { 
					throw new InvalidCastException("Content inside list needs to be parsable to string!");
				}
				tydlist.AddChildren(new TydString("", item as string));
			}
			return tydlist;
		}

		/// <summary>
		/// Deserializes a TydTable into a Dictionary<T1, T2>.
		/// Throws an InvalidCastException if can't parse the key or value inside the Dictionary into a string
		/// </summary>
		/// <typeparam name="T1">The type of the key inside the Dictionary (Needs to be convertable into a string)</typeparam>
		/// <typeparam name="T2">The type of the value inside the Dictionary (Needs to be convertable into a string)</typeparam>
		/// <param name="dict">The dictionary containing the data you want to serialize</param>
		/// <param name="name">The name of the TydTable</param>
		/// <returns>A TydTable with the serialized data</returns>
		public static TydTable Serialize<T1,T2>(Dictionary<T1,T2> dict, string name = "")
		{
			TydTable tydtable = new TydTable(name);
			foreach(KeyValuePair<T1,T2> kvp in dict)
			{
				//Check if key and value can be parsed to string
				if((kvp.Key as string) == null || (kvp.Value as string) == null)
				{
					throw new InvalidCastException("Key and Value inside dictionary needs to be parsable to string!");
				}
				tydtable.AddChildren(new TydString(kvp.Key as string, kvp.Value as string));
			}
			return tydtable;
		}
	}
}
