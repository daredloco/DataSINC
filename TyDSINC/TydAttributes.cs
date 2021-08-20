using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyd
{
	namespace TydAttributes
	{
		[AttributeUsage(AttributeTargets.Field)]
		public sealed class TydIgnore : Attribute
		{
			public TydIgnore()
			{ }
		}

		[AttributeUsage(AttributeTargets.Field)]
		public sealed class TydName : Attribute
		{
			private string Name;

			public TydName(string name)
			{
				Name = name;
			}

			public string GetName()
			{
				return Name;
			}
		}
	}
}
