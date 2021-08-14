using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSINC
{
	public class ObservableList<T> : ObservableCollection<T>
	{
		public ObservableList()
		{

		}

		public ObservableList(IEnumerable<T> collection) : base(collection)
		{

		}

		public ObservableList(List<T> list) : base(list)
		{

		}

		public int FindIndex(Predicate<T> obj)
		{
			int index = 0;

			foreach(T item in Items)
			{
				if(obj(item))
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		public List<T> FindAll(Predicate<T> obj)
		{
			List<T> lst = new List<T>();
			foreach(T item in Items)
			{
				if(obj(item))
				{
					lst.Add(item);
				}
			}
			return lst;
		}
	}
}
