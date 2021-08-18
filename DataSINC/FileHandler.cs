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

	//	public static DataTypes.SoftwareType LoadSoftwareType(string fname)
	//	{
	//		NumberFormatInfo format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
	//		string tyd = System.IO.File.ReadAllText(fname);
	//		TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
	//		TydCollection col = doc[0] as TydCollection;

	//		string name = col.GetChildValue("Name");

	//		TydNode ossupportnode = col.GetChild("OSSupport", false);
	//		string ossupportstr = null;
	//		if(ossupportnode != null)
	//		{
	//			if(ossupportnode.GetType() == typeof(TydList))
	//			{
	//				foreach(string str in ((TydList)ossupportnode).GetChildValues())
	//				{
	//					if(ossupportstr == null)
	//					{
	//						ossupportstr = str;
	//					}
	//					else
	//					{
	//						ossupportstr += ";" + str;
	//					}
	//				}
	//			}
	//			else
	//			{
	//				ossupportstr = ((TydString)ossupportnode).GetNodeValues().ToArray()[0];
	//			}
	//		}

	//		bool overwrite = col.GetChildValue<bool>("Override", false);
	//		string category = col.GetChildValue("Category");
	//		TydList categories = col.GetChild<TydList>("Categories");
	//		string description = col.GetChildValue("Description",false);
	//		int unlock = col.GetChildValue<int>("Unlock", false);
	//		double random = double.Parse(col.GetChildValue("Random"),format);
	//		int idealprice = col.GetChildValue<int>("IdealPrice",false);
	//		int optimaldevtime = col.GetChildValue<int>("OptimalDevTime");
	//		double popularity = double.Parse(col.GetChildValue("Popularity",false,"0"),format);
	//		int retention = col.GetChildValue<int>("Retention",false);
	//		double iterative = double.Parse(col.GetChildValue("Iterative",false,"0"),format);
	//		string ossupport = ossupportstr;
	//		bool oneclient = col.GetChildValue<bool>("OneClient",false);
	//		bool inhouse = col.GetChildValue<bool>("InHouse",false);
	//		string namegen = col.GetChildValue<string>("NameGenerator",false);

	//		List<string> smarketnames = new List<string>();
	//		TydNode submarketnames = col.GetChild("SubmarketNames");
	//		if (submarketnames.GetType() == typeof(TydString))
	//		{
	//			smarketnames.Add(submarketnames.GetNodeValues().First());
	//		}
	//		else
	//		{
	//			smarketnames.AddRange(((TydList)submarketnames).GetChildValues().ToList());
	//		}
	//		TydList features = col.GetChild<TydList>("Features");

	//		List<DataTypes.SoftwareTypeCategories> cats = new List<DataTypes.SoftwareTypeCategories>();
	//		if(categories != null)
	//		{
	//			foreach (TydCollection catcol in categories)
	//			{
	//				List<int> smarkets = new List<int>();
	//				TydNode submarketsnode = catcol.GetChild("Submarkets");
	//				if(submarketsnode.GetType() == typeof(TydString))
	//				{
	//					smarkets.Add(int.Parse(submarketsnode.GetNodeValues().First()));
	//				}
	//				else
	//				{
	//					smarkets.AddRange(((TydList)submarketsnode).GetChildValues<int>().ToList());
	//				}

	//				DataTypes.SoftwareTypeCategories stc = new DataTypes.SoftwareTypeCategories()
	//				{
	//					Name = catcol.GetChildValue("Name"),
	//					Description = catcol.GetChildValue("Description",false),
	//					Popularity = double.Parse(catcol.GetChildValue("Popularity"), format),
	//					Retention = catcol.GetChildValue<int>("Retention"),
	//					TimeScale = double.Parse(catcol.GetChildValue("TimeScale"), format),
	//					Iterative = double.Parse(catcol.GetChildValue("Iterative"), format),
	//					IdealPrice = catcol.GetChildValue<int>("IdealPrice",false),
	//					Submarkets = smarkets,
	//					NameGenerator = catcol.GetChildValue("NameGenerator", false),
	//					Unlock = catcol.GetChildValue<int>("Unlock", false),
	//				};
	//				cats.Add(stc);
	//			}
	//		}
	//		else
	//		{
	//			Debug.Info(name + " doesnt have categories!");
	//		}


	//		List<DataTypes.SoftwareTypeSpecFeatures> feats = new List<DataTypes.SoftwareTypeSpecFeatures>();
			

	//		foreach(TydCollection featcol in features)
	//		{

	//			TydList deplst = new TydList("");
	//			TydList catlst = featcol.GetChild<TydList>("SoftwareCategories");
	//			TydNode subnode = featcol.GetChild("Submarkets");

	//			TydNode depnode = featcol.GetChild("Dependencies", false);
	//			if (depnode != null && depnode.GetType() == typeof(TydString))
	//			{
	//				deplst.AddChildren(depnode);
	//			}
	//			else
	//			{
	//				deplst = depnode as TydList;
	//			}

	//			TydList subfeatures = featcol.GetChild<TydList>("Features", false);
	//			List<DataTypes.SoftwareTypeSubFeatures> sublist = new List<DataTypes.SoftwareTypeSubFeatures>();
	//			if(subfeatures != null)
	//			{
	//				foreach (TydCollection subcol in subfeatures)
	//				{
	//					string sfname = subcol.GetChildValue("Name");
	//					double sfcodeart = double.Parse(subcol.GetChildValue("CodeArt"), format);
	//					string sfdescription = subcol.GetChildValue("Description", false);
	//					int sfdevtime = subcol.GetChildValue<int>("DevTime");
	//					int sflevel = subcol.GetChildValue<int>("Level");
	//					string sfscript = subcol.GetChildValue("Script_EntryPoint", false);
	//					double sfserver = double.Parse(subcol.GetChildValue("Server", false, "0"), format);
	//					int sfunlock = subcol.GetChildValue<int>("Unlock", false);

	//					TydList softcat = subcol.GetChild<TydList>("SoftwareCategories", false);


	//					//IF LEVEL IS 3 SET SuBMARKETS TO 0
	//					TydList submark = null;
	//					TydNode submarknode = subcol.GetChild("Submarkets", false);
	//					if (sflevel != 3 && submarknode.GetType() == typeof(TydList))
	//					{
	//						submark = subcol.GetChild<TydList>("Submarkets", false);
	//					}

	//					List<string> softcatlist = new List<string>();
	//					if (softcat != null)
	//					{
	//						softcatlist = softcat.GetChildValues().ToList();
	//					}
	//					List<int> submarklist = new List<int>() { 0, 0, 0 };
	//					if (submark != null && submarknode.GetType() == typeof(TydList))
	//					{
	//						submarklist = submark.GetChildValues<int>().ToList();
	//					}
	//					else if (submark == null & submarknode.GetType() == typeof(TydString))
	//					{
	//						submarklist = new List<int>() { ((TydString)submarknode).GetValue<int>() };
	//					}

	//					DataTypes.SoftwareTypeSubFeatures subfeature = new DataTypes.SoftwareTypeSubFeatures()
	//					{
	//						Name = sfname,
	//						CodeArt = sfcodeart,
	//						Description = sfdescription,
	//						DevTime = sfdevtime,
	//						Level = sflevel,
	//						Script_EntryPoint = sfscript,
	//						Server = sfserver,
	//						SoftwareCategories = softcatlist,
	//						Submarkets = submarklist,
	//						Unlock = sfunlock
	//					};
	//					sublist.Add(subfeature);
	//				}

	//			}

	//			//LOAD SOFTWARETYPESPECFEATURES
	//			List<string> depfeatlist = new List<string>();
	//			if (deplst != null)
	//			{
	//				depfeatlist = deplst.GetChildValues().ToList();
	//			}
	//			List<string> catfeatlist = new List<string>();
	//			if (catlst != null)
	//			{
	//				catfeatlist = catlst.GetChildValues().ToList();
	//			}
	//			List<int> subfeatlist = new List<int>();
	//			if (subnode != null && subnode.GetType() == typeof(TydList))
	//			{
	//				subfeatlist = ((TydList)subnode).GetChildValues<int>().ToList();
	//			}
	//			else if(subnode != null && subnode.GetType() == typeof(TydString))
	//			{
	//				subfeatlist = new List<int>() { ((TydString)subnode).GetValue<int>() };
	//			}

	//			DataTypes.SoftwareTypeSpecFeatures stf = new DataTypes.SoftwareTypeSpecFeatures()
	//			{
	//				Name = featcol.GetChildValue("Name"),
	//				CodeArt = double.Parse(featcol.GetChildValue("CodeArt"),format),
	//				Dependencies = depfeatlist,
	//				Description = featcol.GetChildValue("Description",false),
	//				DevTime = featcol.GetChildValue<int>("DevTime"),
	//				Features = sublist,
	//				Optional = featcol.GetChildValue<bool>("Optional",false),
	//				Server = double.Parse(featcol.GetChildValue("Server",false,"0"), format),
	//				SoftwareCategories = catfeatlist,
	//				Spec = featcol.GetChildValue("Spec"),
	//				Submarkets = subfeatlist,
	//				Unlock = featcol.GetChildValue<int>("Unlock",false)
	//			};
	//			feats.Add(stf);
	//		}

	//		DataTypes.SoftwareType st = new DataTypes.SoftwareType(fname)
	//		{
	//			Name = name,
	//			Override = overwrite,
	//			Category = category,
	//			Categories = cats,
	//			Description = description,
	//			Unlock = unlock,
	//			Random = random,
	//			IdealPrice = idealprice,
	//			OptimalDevTime = optimaldevtime,
	//			Popularity = popularity,
	//			Retention = retention,
	//			Iterative = iterative,
	//			OSSupport = ossupport,
	//			OneClient = oneclient,
	//			InHouse = inhouse,
	//			NameGenerator = namegen,
	//			SubmarketNames = smarketnames,
	//			Features = feats
	//		};
	//		return st;
	//}

		public static DataTypes.SoftwareType LoadSoftwareType(string fname)
		{
			Debug.Info("Loading Softwaretype at " + fname);
			string tyd = System.IO.File.ReadAllText(fname);
			TydDocument doc = new TydDocument(TydFromText.Parse(tyd));
			TydCollection root = doc[0] as TydCollection;

			bool isHardware = false;

			//All variables from the SoftwareType datatype
			string name = "";
			List<string> ossupport = new List<string>();
			bool overwrite = false;
			string category = "";
			List<DataTypes.SoftwareTypeCategories> categories = new List<DataTypes.SoftwareTypeCategories>();
			string description = "";
			int unlock = 0;
			double random = 0;
			int idealprice = 0;
			int optimaldevtime = 0;
			double popularity = 0;
			int retention = 0;
			double iterative = 0;
			bool oneclient = false;
			bool inhouse = false;
			string namegen = "";
			List<string> submarketnames = new List<string>();
			List<DataTypes.SoftwareTypeSpecFeatures> features = new List<DataTypes.SoftwareTypeSpecFeatures>();
			DataTypes.Manufacturing manufacturing = null;

			//Read all nodes inside the root
			foreach (TydNode node in root)
			{
				switch (node.Name)
				{
					//The Name node (Must be a string!)
					case "Name":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Name' isn't a TydString!"));
							return null;
						}
						name = (node as TydString).Value;
						break;
					//The OSSupport node (Can be String or List)
					case "OSSupport":
						if ((node as TydString) != null)
						{
							ossupport.Add((node as TydString).Value);
						} else if ((node as TydList) != null)
						{
							ossupport = (node as TydList).GetChildValues().ToList();
						}
						else
						{
							Debug.Exception(new InvalidCastException("The node from OSSupport is neither TydString nor TydList!"));
							return null;
						}
						break;
					//The Override node (Must be boolean)
					case "Override":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Override' isn't a TydString!"));
							return null;
						}
						overwrite = (node as TydString).GetValue<bool>();
						break;
					//The Category node (Must be a string!)
					case "Category":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Category' isn't a TydString!"));
							return null;
						}
						category = (node as TydString).Value;
						break;
					//The Categories node (Is a TydList!)
					case "Categories":
						if ((node as TydList) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Categories' isn't a TydList!"));
							return null;
						}
						foreach (TydNode catnode in node as TydList)
						{
							categories.Add(DataTypes.SoftwareTypeCategories.FromNode(catnode));
						}
						break;
					//The Description node (Must be a string!)
					case "Description":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Description' isn't a TydString!"));
							return null;
						}
						description = (node as TydString).Value;
						break;
					//The Unlock node (Must be a boolean)
					case "Unlock":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Unlock' isn't a TydString!"));
							return null;
						}
						unlock = (node as TydString).GetValue<int>();
						break;
					//The Random node (Must be a double)
					case "Random":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Random' isn't a TydString!"));
							return null;
						}
						random = Utils.Helpers.StringToDouble((node as TydString).Value);
						break;
					//The IdealPrice node (Must be an integer)
					case "IdealPrice":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'IdealPrice' isn't a TydString!"));
							return null;
						}
						idealprice = (node as TydString).GetValue<int>();
						break;
					//The OptimalDevTime node (Must be an integer)
					case "OptimalDevTime":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'OptimalDevTime' isn't a TydString!"));
							return null;
						}
						optimaldevtime = (node as TydString).GetValue<int>();
						break;
					//The Popularity node (Must be a double)
					case "Popularity":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Popularity' isn't a TydString!"));
							return null;
						}
						popularity = Utils.Helpers.StringToDouble((node as TydString).Value);
						break;
					//The Retention node (Must be an integer)
					case "Retention":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Retention' isn't a TydString!"));
							return null;
						}
						retention = (node as TydString).GetValue<int>();
						break;
					//The Iterative node (Must be a double)
					case "Iterative":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Iterative' isn't a TydString!"));
							return null;
						}
						iterative = Utils.Helpers.StringToDouble((node as TydString).Value);
						break;
					//The OneClient node (Must be boolean)
					case "OneClient":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'OneClient' isn't a TydString!"));
							return null;
						}
						oneclient = (node as TydString).GetValue<bool>();
						break;
					//The InHouse node (Must be a boolean)
					case "InHouse":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'InHouse' isn't a TydString!"));
							return null;
						}
						inhouse = (node as TydString).GetValue<bool>();
						break;
					//The NameGenerator node (Must be a string)
					case "NameGenerator":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'NameGenerator' isn't a TydString!"));
							return null;
						}
						namegen = (node as TydString).Value;
						break;
					//The SubmarketNames node (Must be an array of strings)
					case "SubmarketNames":
						if ((node as TydList) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'SubmarketNames' isn't a TydList!"));
							return null;
						}
						foreach (TydString item in node as TydList)
						{
							submarketnames.Add(item.Value);
						}
						break;
					//The Features node (Is a TydCollection!)
					case "Features":
						if ((node as TydList) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Features' isn't a TydList!"));
							return null;
						}
						foreach (TydNode featnode in node as TydList)
						{
							features.Add(DataTypes.SoftwareTypeSpecFeatures.FromNode(featnode));
						}
						break;
					//The Hardware node (Is a TydString and needs to be boolean!)
					case "Hardware":
						if ((node as TydString) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Hardware' isn't a TydString!"));
							return null;
						}
						else if (!bool.TryParse((node as TydString).Value, out _))
						{
							Debug.Exception(new InvalidCastException("The node 'Hardware' must be a valid boolean!"));
							return null;
						}
						//TODO: Handle the Hardware here, it would be best to load the Manufacturing here as well
						isHardware = bool.Parse((node as TydString).Value);
						break;
					//The Manufacturing node (Needs to be a TydTable!)
					case "Manufacturing":
						if((node as TydTable) == null)
						{
							Debug.Exception(new InvalidCastException("The node 'Manufacturing' isn't a TydTable!"));
							return null;
						}
						throw new Exception(node.FullTyd);
						manufacturing = DataTypes.Manufacturing.FromNode(node);
						break;
					default:
						Debug.Exception(new Exception("Node " + node.Name + " is unknown and will be ignored! If this shouldn't be the case, open an Issue on Github.") ,false); 
						break;
				}
			}

			DataTypes.SoftwareType softwaretype = new DataTypes.SoftwareType(fname)
			{
				Name = name,
				OSSupport = ossupport,
				Override = overwrite,
				Category = category,
				Categories = categories,
				Description = description,
				Unlock = unlock,
				Random = random,
				IdealPrice = idealprice,
				OptimalDevTime = optimaldevtime,
				Popularity = popularity,
				Retention = retention,
				Iterative = iterative,
				OneClient = oneclient,
				InHouse = inhouse,
				NameGenerator = namegen,
				SubmarketNames = submarketnames,
				Features = features,
			};
			if(isHardware)
			{
				softwaretype.Hardware = true;
				softwaretype.Manufacturing = manufacturing;
			}
			return softwaretype;
		}

		public static DataTypes.CompanyType LoadCompanyType(string fname)
		{
			return new DataTypes.CompanyType(fname);
		}
	}
}
