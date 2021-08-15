using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tyd;

namespace DataSINC
{
	namespace Utils
	{
		public class ObservableDictionary<TKey, TValue> :
		ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
		INotifyCollectionChanged, INotifyPropertyChanged
		{
			readonly IDictionary<TKey, TValue> dictionary;

			/// <summary>Event raised when the collection changes.</summary>
			public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

			/// <summary>Event raised when a property on the collection changes.</summary>
			public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

			/// <summary>
			/// Initializes an instance of the class.
			/// </summary>
			public ObservableDictionary()
				: this(new Dictionary<TKey, TValue>())
			{
			}

			/// <summary>
			/// Initializes an instance of the class using another dictionary as 
			/// the key/value store.
			/// </summary>
			public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
			}

			void AddWithNotification(KeyValuePair<TKey, TValue> item)
			{
				AddWithNotification(item.Key, item.Value);
			}

			void AddWithNotification(TKey key, TValue value)
			{
				dictionary.Add(key, value);

				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
					new KeyValuePair<TKey, TValue>(key, value)));
				PropertyChanged(this, new PropertyChangedEventArgs("Count"));
				PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
				PropertyChanged(this, new PropertyChangedEventArgs("Values"));
			}

			bool RemoveWithNotification(TKey key)
			{
				TValue value;
				if (dictionary.TryGetValue(key, out value) && dictionary.Remove(key))
				{
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
						new KeyValuePair<TKey, TValue>(key, value)));
					PropertyChanged(this, new PropertyChangedEventArgs("Count"));
					PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
					PropertyChanged(this, new PropertyChangedEventArgs("Values"));

					return true;
				}

				return false;
			}

			void UpdateWithNotification(TKey key, TValue value)
			{
				TValue existing;
				if (dictionary.TryGetValue(key, out existing))
				{
					dictionary[key] = value;

					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
						new KeyValuePair<TKey, TValue>(key, value),
						new KeyValuePair<TKey, TValue>(key, existing)));
					PropertyChanged(this, new PropertyChangedEventArgs("Values"));
				}
				else
				{
					AddWithNotification(key, value);
				}
			}

			/// <summary>
			/// Allows derived classes to raise custom property changed events.
			/// </summary>
			protected void RaisePropertyChanged(PropertyChangedEventArgs args)
			{
				PropertyChanged(this, args);
			}

			#region IDictionary<TKey,TValue> Members

			/// <summary>
			/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <param name="key">The object to use as the key of the element to add.</param>
			/// <param name="value">The object to use as the value of the element to add.</param>
			public void Add(TKey key, TValue value)
			{
				AddWithNotification(key, value);
			}

			/// <summary>
			/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
			/// </summary>
			/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
			/// <returns>
			/// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
			/// </returns>
			public bool ContainsKey(TKey key)
			{
				return dictionary.ContainsKey(key);
			}

			/// <summary>
			/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
			public ICollection<TKey> Keys
			{
				get { return dictionary.Keys; }
			}

			/// <summary>
			/// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <param name="key">The key of the element to remove.</param>
			/// <returns>
			/// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </returns>
			public bool Remove(TKey key)
			{
				return RemoveWithNotification(key);
			}

			/// <summary>
			/// Gets the value associated with the specified key.
			/// </summary>
			/// <param name="key">The key whose value to get.</param>
			/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
			/// <returns>
			/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
			/// </returns>
			public bool TryGetValue(TKey key, out TValue value)
			{
				return dictionary.TryGetValue(key, out value);
			}

			/// <summary>
			/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
			public ICollection<TValue> Values
			{
				get { return dictionary.Values; }
			}

			/// <summary>
			/// Gets or sets the element with the specified key.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <returns></returns>
			public TValue this[TKey key]
			{
				get { return dictionary[key]; }
				set { UpdateWithNotification(key, value); }
			}

			#endregion

			#region ICollection<KeyValuePair<TKey,TValue>> Members

			void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
			{
				AddWithNotification(item);
			}

			void ICollection<KeyValuePair<TKey, TValue>>.Clear()
			{
				((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();

				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				PropertyChanged(this, new PropertyChangedEventArgs("Count"));
				PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
				PropertyChanged(this, new PropertyChangedEventArgs("Values"));
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
			}

			void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
			}

			int ICollection<KeyValuePair<TKey, TValue>>.Count
			{
				get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count; }
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
			{
				get { return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly; }
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
			{
				return RemoveWithNotification(item.Key);
			}

			#endregion

			#region IEnumerable<KeyValuePair<TKey,TValue>> Members

			IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
			}

			#endregion
		}

		public static class TydHelpers
		{
			public static TydList ConvertList<T>(List<T> list, string name = "")
			{
				TydList tydlist = new TydList(name);
				foreach(T content in list)
				{
					tydlist.AddChildren(new TydString("", content as string));
				}
				return tydlist;
			}

			public static TydTable ConvertTable<T>(Dictionary<string,T> dictionary, string name = "")
			{
				TydTable tydtable = new TydTable(name);
				foreach(KeyValuePair<string,T> kvp in dictionary)
				{
					tydtable.AddChildren(new TydString(kvp.Key, kvp.Value as string));
				}
				return tydtable;
			}

			public static TydString ConvertString<T>(T value, string name = "")
			{
				double doubleval;
				if(double.TryParse(value as string, out doubleval))
				{
					return new TydString(name, Helpers.DoubleToString(doubleval));
				}
				return new TydString(name, value as string);
			}
		}

		public static class Helpers
		{
			public static string DoubleToString(double number)
			{
				NumberFormatInfo format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
				return number.ToString(format);
			}

			public static bool IsDouble<T>(T variable)
			{
				if(variable.GetType() == typeof(double))
				{
					//Check if variable is of type double
					return true;
				}else if(variable.GetType() == typeof(decimal))
				{
					//Check if variable is of type decimal
					return true;
				}else if(variable.GetType() == typeof(float))
				{
					//Check if variable is of type float
					return true;
				}

				if(variable.GetType() != typeof(string))
				{
					//Check if variable is of type string
					return false;
				}

				//Check if variable can be parsed to double
				if (double.TryParse(variable as string, out _))
				{
					return true;
				}

				//Check if variable can be parsed to double 
				string variant1 = variable as string;
				variant1 = variant1.Replace(",", ".");
				if (double.TryParse(variant1, out _))
				{
					return true;
				}

				return false;
			}
		}
	}
}
