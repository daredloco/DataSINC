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
		
			foreach(T item in base.Items)
			{
				if(item.Equals((T)obj.Clone()))
				{
					return index;
				}
				index++;
			}
			return -1;
		}
	}
}
