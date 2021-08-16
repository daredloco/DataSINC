using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSINC
{
	namespace TextboxValues
	{
		public class CompanyTypes
		{

		}

		public class SoftwareTypes
		{
			private string name;
			public string Name
			{
				get => name;
				set
				{
					name = value;
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new ApplicationException("Name is mandatory!");
					}
				}
			}

			private string category;
			public string Category
			{
				get => category;
				set
				{
					category = value;
					if (string.IsNullOrWhiteSpace(value))
					{
						throw new ApplicationException("Category is mandatory!");
					}
				}
			}
			public uint Unlock { get; set; }
			public double Random { get; set; }
			public int IdealPrice { get; set; }
			public int OptimalDevTime { get; set; }
			public double Popularity { get; set; }
			public int Retention { get; set; }
			public double Iterative { get; set; }
		}

		public class Personalities
		{

		}
	}
}
