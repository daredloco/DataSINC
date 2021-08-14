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

		public static DataTypes.SoftwareType LoadSoftwareType(string fname)
		{
			NumberFormatInfo format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
			string tyd = System.IO.File.ReadAllText(fname);
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			TydCollection col = doc[0] as TydCollection;
			string name = col.GetChildValue("Name");
			bool overwrite = col.GetChildValue<bool>("Override", false);
			string category = col.GetChildValue("Category");
			TydList categories = col.GetChild<TydList>("Categories");
			string description = col.GetChildValue("Description");
			int unlock = col.GetChildValue<int>("Unlock", false);
			double random = double.Parse(col.GetChildValue("Random"),format);
			int idealprice = col.GetChildValue<int>("IdealPrice",false);
			int optimaldevtime = col.GetChildValue<int>("OptimalDevTime");
			double popularity = double.Parse(col.GetChildValue("Popularity",false,"0"),format);
			int retention = col.GetChildValue<int>("Retention",false);
			double iterative = double.Parse(col.GetChildValue("Iterative",false,"0"),format);
			string ossupport = col.GetChildValue("OSSupport",false);
			bool oneclient = col.GetChildValue<bool>("OneClient",false);
			bool inhouse = col.GetChildValue<bool>("InHouse",false);
			string namegen = col.GetChildValue<string>("NameGenerator",false);
			TydList submarketnames = col.GetChild<TydList>("SubmarketNames");
			TydList features = col.GetChild<TydList>("Features");

			List<DataTypes.SoftwareTypeCategories> cats = new List<DataTypes.SoftwareTypeCategories>();
			if(categories != null)
			{
				foreach (TydCollection catcol in categories)
				{
					TydList catsubmarkets = catcol.GetChild<TydList>("Submarkets");

					DataTypes.SoftwareTypeCategories stc = new DataTypes.SoftwareTypeCategories()
					{
						Name = catcol.GetChildValue("Name"),
						Description = catcol.GetChildValue("Description"),
						Popularity = double.Parse(catcol.GetChildValue("Popularity"), format),
						Retention = catcol.GetChildValue<int>("Retention"),
						TimeScale = double.Parse(catcol.GetChildValue("TimeScale"), format),
						Iterative = double.Parse(catcol.GetChildValue("Iterative"), format),
						IdealPrice = catcol.GetChildValue<int>("IdealPrice"),
						Submarkets = catsubmarkets.GetChildValues<int>().ToList(),
						NameGenerator = catcol.GetChildValue("NameGenerator", false),
						Unlock = catcol.GetChildValue<int>("Unlock", false),
					};
					cats.Add(stc);
				}
			}
			else
			{
				Debug.Info(name + " doesnt have categories!");
			}
			

			List<DataTypes.SoftwareTypeSpecFeatures> feats = new List<DataTypes.SoftwareTypeSpecFeatures>();
			foreach(TydCollection featcol in features)
			{
				TydList deplst = featcol.GetChild<TydList>("Dependencies");
				TydList catlst = featcol.GetChild<TydList>("SoftwareCategories");
				TydList sublst = featcol.GetChild<TydList>("Submarkets");
				

				TydList subfeatures = featcol.GetChild<TydList>("Features");
				List<DataTypes.SoftwareTypeSubFeatures> sublist = new List<DataTypes.SoftwareTypeSubFeatures>();
				foreach(TydCollection subcol in subfeatures)
				{
					string sfname = subcol.GetChildValue("Name");
					double sfcodeart = double.Parse(subcol.GetChildValue("CodeArt"), format);
					string sfdescription = subcol.GetChildValue("Description", false);
					int sfdevtime = subcol.GetChildValue<int>("DevTime");
					int sflevel = subcol.GetChildValue<int>("Level");
					string sfscript = subcol.GetChildValue("Script_EntryPoint", false);
					double sfserver = double.Parse(subcol.GetChildValue("Server", false, "0"), format);
					int sfunlock = subcol.GetChildValue<int>("Unlock", false);

					TydList softcat = subcol.GetChild<TydList>("SoftwareCategories",false);


					//IF LEVEL IS 3 SET SuBMARKETS TO 0
					TydList submark = null;
					if (sflevel != 3)
					{
						submark = subcol.GetChild<TydList>("Submarkets", false);
					}

					List<string> softcatlist = new List<string>();
					if(softcat != null)
					{
						softcatlist = softcat.GetChildValues().ToList();
					}
					List<int> submarklist = new List<int>() { 0, 0, 0 };
					if(submark != null)
					{
						submarklist = submark.GetChildValues<int>().ToList();
					}

					DataTypes.SoftwareTypeSubFeatures subfeature = new DataTypes.SoftwareTypeSubFeatures()
					{
						Name = sfname,
						CodeArt = sfcodeart,
						Description = sfdescription,
						DevTime = sfdevtime,
						Level = sflevel,
						Script_EntryPoint = sfscript,
						Server = sfserver,
						SoftwareCategories = softcatlist,
						Submarkets = submarklist,
						Unlock = sfunlock
					};
					sublist.Add(subfeature);
				}

				//LOAD SOFTWARETYPESPECFEATURES
				List<string> depfeatlist = new List<string>();
				if (deplst != null)
				{
					depfeatlist = deplst.GetChildValues().ToList();
				}
				List<string> catfeatlist = new List<string>();
				if (catlst != null)
				{
					catfeatlist = catlst.GetChildValues().ToList();
				}
				List<int> subfeatlist = new List<int>();
				if (sublst != null)
				{
					subfeatlist = sublst.GetChildValues<int>().ToList();
				}

				Debug.Info("SOFTWARETYPESPECFEATURE: " + name + " => " + featcol.GetChildValue("Name"));

				DataTypes.SoftwareTypeSpecFeatures stf = new DataTypes.SoftwareTypeSpecFeatures()
				{
					Name = featcol.GetChildValue("Name"),
					CodeArt = double.Parse(featcol.GetChildValue("CodeArt"),format),
					Dependencies = depfeatlist,
					Description = featcol.GetChildValue("Description",false),
					DevTime = featcol.GetChildValue<int>("DevTime"),
					Features = sublist,
					Optional = featcol.GetChildValue<bool>("Optional",false),
					Server = double.Parse(featcol.GetChildValue("Server",false,"0"), format),
					SoftwareCategories = catfeatlist,
					Spec = featcol.GetChildValue("Spec"),
					Submarkets = subfeatlist,
					Unlock = featcol.GetChildValue<int>("Unlock",false)
				};
				feats.Add(stf);
			}

			DataTypes.SoftwareType st = new DataTypes.SoftwareType(fname)
			{
				Name = name,
				Override = overwrite,
				Category = category,
				Categories = cats,
				Description = description,
				Unlock = unlock,
				Random = random,
				IdealPrice = idealprice,
				OptimalDevTime = optimaldevtime,
				Popularity = popularity,
				Retention = retention,
				Iterative = iterative,
				OSSupport = ossupport,
				OneClient = oneclient,
				InHouse = inhouse,
				NameGenerator = namegen,
				SubmarketNames = submarketnames.GetChildValues().ToList(),
				Features = feats
			};
			return st;
	}

		public static DataTypes.CompanyType LoadCompanyType(string tyd)
		{
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			return null;
		}
	}
}
