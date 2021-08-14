using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tyd;

namespace DataSINC
{
	public static class FileHandler
	{
		public static ObservableList<DataTypes.Personality> LoadPersonalities(string tyd)
		{
			ObservableList<DataTypes.Personality> lst = new ObservableList<DataTypes.Personality>();

			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			TydCollection col = doc[0] as TydCollection;
			TydCollection persos = col.GetChild("Personalities") as TydCollection;
			TydList incompatibilities = col.GetChild("Incompatibilities") as TydList;

			NumberFormatInfo format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
			
			foreach (TydCollection node in persos)
			{
				DataTypes.Personality perso = new DataTypes.Personality()
				{
					Name = node.GetChildValue("Name"),
					LazyStress = Convert.ToDouble(node.GetChildValue("LazyStress"),format),
					WorkLearn = Convert.ToDouble(node.GetChildValue("WorkLearn"), format),
					Social = Convert.ToDouble(node.GetChildValue("Social"), format)
				};

				TydTable relationships = node.GetChild("Relationships") as TydTable;

				perso.Relationships = new Utils.ObservableDictionary<string, double>();

				if(relationships != null)
				{
					foreach (TydNode relationship in relationships)
					{
						perso.Relationships.Add(relationship.Name, Convert.ToDouble(relationship.GetNodeValues().First(), format));
					}
				}
				lst.Add(perso);
			}

			return lst;
		}

		public static ObservableList<DataTypes.PersonalityIncompatibility> LoadIncompatibilities(string tyd)
		{
			ObservableList<DataTypes.PersonalityIncompatibility> lst = new ObservableList<DataTypes.PersonalityIncompatibility>();
			
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			TydCollection col = doc[0] as TydCollection;
			TydList incompatibilities = col.GetChild("Incompatibilities") as TydList;
			foreach (TydList node in incompatibilities)
			{
				lst.Add(new DataTypes.PersonalityIncompatibility(node.GetNodeValues().ToArray()[0],node.GetNodeValues().ToArray()[1]));
			}
			return lst;
		}

		public static DataTypes.SoftwareType LoadSoftwareType(string tyd)
		{
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			return null;
		}

		public static DataTypes.CompanyType LoadCompanyType(string tyd)
		{
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			return null;
		}
	}
}
