using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tyd;

namespace DataSINC
{
	namespace Converter
	{
		public class SoftwareTypes
		{
			public static List<DataTypes.SoftwareType> FromTYD(string fname)
			{
				TydFile file = TydFile.FromFile(fname);
				TydDocument doc = file.DocumentNode;
				TydCollection rootCol = doc[0] as TydCollection;

				
				//string name = rootCol.GetChildValue("Name");
				//string category = rootCol.GetChildValue("Category");
				//string desc = rootCol.GetChildValue("Description");
				//TydCollection categories = rootCol.GetChild("Categories") as TydCollection;
				//string rand = rootCol.GetChildValue("Random");
				//string optdev = rootCol.GetChildValue("OptimalDevTime");
				//List<string> submarkets = rootCol.GetChild<TydCollection>("SubmarketNames").GetChildValues().ToList();
				//string namegenerator = rootCol.GetChildValue("NameGenerator");
				//TydCollection features = rootCol.GetChild("Features") as TydCollection;

				//MessageBox.Show(
				//	"Name: " + name + "\n" + 
				//	"Category: " + category + "\n" + 
				//	"Description: " + desc + "\n" + 
				//	"Random: " + rand + "\n" + 
				//	"Optimal Development Time: " + optdev + "\n" +
				//	"Submarkets0: "+ submarkets[0] + "\n" + 
				//	"Namegenerator: " + namegenerator);
				return null;
			}
		}

		public class CompanyTypes
		{

		}

		public class Personalities
		{

		}

		public class NameGenerators
		{

		}
	}
}
