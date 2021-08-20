using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyd;
using Tyd.TydAttributes;

namespace DataSINC
{
	namespace DataTypes
	{
		public class NameGenerator
		{
			public string Location { get; set; }
			public string Title { get; set; }

			public string Content { get; set; }

			public NameGenerator(string fname)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fname);
				Location = fname;
				Title = fi.Name.Replace(fi.Extension, "");

				//Load content
				Content = System.IO.File.ReadAllText(fname);
			}

			private NameGenerator()
			{

			}

			public static NameGenerator Create(string name)
			{
				NameGenerator ng = new NameGenerator()
				{
					Location = System.IO.Path.Combine(Settings.latestmod, "NameGenerators", name + ".txt"),
					Title = name,
					Content = "-start(base,base,base,pre)\r\n" +
								"-pre(base)\r\n" +
								"-base(base2, base2, base2, stop)\r\n" +
								"-base2(ext, stop, stop)\r\n" +
								"-ext(stop)"
				};

				return ng;
			}
		}

		[Serializable]
		public class SoftwareType
		{
			[TydIgnore]
			public string Location;
			[TydIgnore]
			public string Title;

			//DATA
			[TydName("Name")]
			private string name;
			public string Name { get => name; set => name = value; }
			public bool Override; //OPTIONAL
			public string Category;
			[TydName("Categories")]
			private SoftwareTypeCategories[] categories;
			public List<SoftwareTypeCategories> Categories { get => categories.ToList(); set => categories = value.ToArray(); } //OPTIONAL
			public string Description;
			public int Unlock;
			public double Random;
			public int IdealPrice; //OPTIONAL
			public int OptimalDevTime;
			public double Popularity; //OPTIONAL
			public int Retention; //OPTIONAL
			[TydName("Iterative")]
			private double iterative;
			public double Iterative { //OPTIONAL
				get => iterative;
				set { if (value > 1) { iterative = 1; return; } if (value < 0) { iterative = 0; return; } iterative = value; } 
			}
			[TydName("OSSupport")]
			private string[] ossupport;
			public List<string> OSSupport { get => ossupport.ToList(); set => ossupport = value.ToArray(); } //OPTIONAL
			public bool OneClient;
			public bool InHouse;
			public string NameGenerator; //OPTIONAL IF GENERATORS SET IN SUBS
			[TydName("SubmarketNames")]
			private string[] submarketnames;
			public List<string> SubmarketNames { get => submarketnames.ToList(); set => submarketnames = value.ToArray(); }
			[TydName("Features")]
			private SoftwareTypeSpecFeatures[] features;
			public List<SoftwareTypeSpecFeatures> Features { get => features.ToList(); set => features = value.ToArray(); }
			public bool Hardware; //OPTIONAL, only added if SoftwareType is Hardware
			public Manufacturing Manufacturing; //OPTIONAL, only added if SoftwareType is Hardware

			public SoftwareType(string fname)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fname);
				Location = fname;
				Title = fi.Name.Replace(fi.Extension, "");
			}

			private SoftwareType()
			{

			}

			public static SoftwareType Create(string name)
			{
				SoftwareType st = new SoftwareType() {
					Location = System.IO.Path.Combine(Settings.latestmod, "SoftwareTypes", name + ".tyd"),
					Name = name,
					Title = name,
					SubmarketNames = new List<string>() { "0", "0", "0" },
					Categories = new List<SoftwareTypeCategories>(),
					Features = new List<SoftwareTypeSpecFeatures>(),
					OSSupport = new List<string>()
				};

				return st;
			}

			[Obsolete]
			public void Save()
			{
				TydDocument doc = new TydDocument();
				TydTable root = new TydTable("SoftwareType");
				root.AddChildren(
					new TydString("Name", Name),
					new TydString("Category", Category),
					new TydString("Description", Description)
					);
				TydFile file = TydFile.FromDocument(doc);
				file.Save(Location);
			}
		}

		[Serializable]
		public class SoftwareTypeCategories
		{
			//DATA
			public string Name;
			public string Description;
			public int Unlock;
			private double Popularity;
			public double popularity { get => Popularity; set { if (value > 1) { Popularity = 1; return; } if (value < 0) { Popularity = 0; return; } Popularity = value; } }
			public int[] Submarkets;
			private double TimeScale;
			public double timeScale { get => TimeScale; set { if (value > 1) { TimeScale = 1; return; } if (value < 0) { TimeScale = 0; return; } TimeScale = value; } }
			public int Retention;
			public int IdealPrice; //IGNORED IF SET IN SOFTWARE TYPE
			private double Iterative;
			public double iterative { get => Iterative; set { if (value > 1) { Iterative = 1; return; } if (value < 0) { Iterative = 0; return; } Iterative = value; } }
			public string NameGenerator; //OPTIONAL
			public int LagBehind; //OPTIONAL
			public bool Hardware; //OPTIONAL, only added if SoftwareType is Hardware
			public Manufacturing Manufacturing; //OPTIONAL, only added if SoftwareType is Hardware

			public SoftwareTypeCategories() { }
			
			public static SoftwareTypeCategories FromNode(TydNode node)
			{
				SoftwareTypeCategories cat = TydConverter.Deserialize<SoftwareTypeCategories>(node);
				return cat;
			}

			public TydNode ToNode()
			{
				return TydConverter.Serialize("", this);
			}

			public TydTable ToTyd()
			{
				TydTable list = new TydTable("");
				list.AddChildren(
					Utils.TydHelpers.ConvertString(Name, "Name"),
					Utils.TydHelpers.ConvertString(Unlock, "Unlock"),
					Utils.TydHelpers.ConvertString(Popularity, "Popularity")
					);
				//list.AddChildren(Utils.TydHelpers.ConvertList(Submarkets, "Submarkets"));
				list.AddChildren(
					Utils.TydHelpers.ConvertString(TimeScale, "TimeScale"),
					Utils.TydHelpers.ConvertString(Retention, "Retention"),
					Utils.TydHelpers.ConvertString(IdealPrice, "IdealPrice"),
					Utils.TydHelpers.ConvertString(Iterative, "Iterative"),
					Utils.TydHelpers.ConvertString(NameGenerator, "NameGenerator")
					);
				return list;
			}
		}

		[Serializable]
		public class SoftwareTypeSpecFeatures
		{
			//DATA
			public string Name;
			public string Spec;
			public string Description;
			public string[] Dependencies;
			public int Unlock;
			public int DevTime;
			public int[] Submarkets;
			private double CodeArt;
			public double codeart { get => CodeArt; set { if (value > 1) { CodeArt = 1; return; } if (value < 0) { CodeArt = 0; return; } CodeArt = value; } }
			public double Server;
			public bool Optional; //OPTIONAL (Will be OFF by default)
			public string[] SoftwareCategories; //OPTIONAL
			public SoftwareTypeSubFeatures[] Features;

			public SoftwareTypeSpecFeatures() { }

			public static SoftwareTypeSpecFeatures FromNode(TydNode node)
			{
				return TydConverter.Deserialize<SoftwareTypeSpecFeatures>(node);
			}

			public TydNode ToNode()
			{
				return TydConverter.Serialize("", this, true);
			}
		}

		[Serializable]
		public class SoftwareTypeSubFeatures
		{
			//DATA
			public string Name;
			public string Description;
			private int Level;
			public int level { get => Level; set { if (value > 3) { Level = 3; return; } if (value < 1) { Level = 1; return; } Level = value; } }
			public int Unlock;
			public int DevTime;
			public int[] Submarkets;
			private double CodeArt;
			public double codeart { get => CodeArt; set { if (value > 1) { CodeArt = 1; return; } if (value < 0) { CodeArt = 0; return; } CodeArt = value; } }
			public double Server;
			public string[] SoftwareCategories; //OPTIONAL
			public string Script_EndOfDay; //OPTIONAL
			public string Script_AfterSales; //OPTIONAL
			public string Script_OnRelease; //OPTIONAL
			public string Script_NewCopies; //OPTIONAL
			public string Script_WorkItemChange; //OPTIONAL
		}

		[Serializable]
		public class CompanyType
		{
			[NonSerialized]
			public string Location;
			[NonSerialized]
			private string title;
			public string Title { get => title; set => title = value; }

			//DATA
			public string Specialization;
			private double PerYear;
			public double peryear { get => PerYear; set { if (value > 1) { PerYear = 1; } else if (value < 0) { PerYear = 0; return; } else { PerYear = value; } } }
			public uint Min;
			public uint Max;
			public bool Frameworks;
			public CompanyTypeTypes[] Types;
			public string NameGenerator;

			public CompanyType(string fname)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fname);
				Location = fname;
				Title = fi.Name.Replace(fi.Extension, "");
				CompanyType ct = FromFile(fname);
				Specialization = ct.Specialization;
				peryear = ct.peryear;
				Min = ct.Min;
				Max = ct.Max;
				Frameworks = ct.Frameworks;
				Types = ct.Types;
				NameGenerator = ct.NameGenerator;
			}

			private CompanyType()
			{

			}

			public static CompanyType Create(string name)
			{
				List<CompanyTypeTypes> dict = new List<CompanyTypeTypes>();
				CompanyType ct = new CompanyType()
				{
					Location = System.IO.Path.Combine(Settings.latestmod, "CompanyTypes", name + ".tyd"),
					Title = name,
					Frameworks = true,
					Min = 0,
					Max = 1,
					NameGenerator = "NameGenerator",
					PerYear = 1,
					Specialization = "Special",
					Types = dict.ToArray()
				};
				return ct;
			}

			public static CompanyType FromFile(string fname)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fname);
				TydDocument doc = TydFile.FromFile(fname).DocumentNode;
				CompanyType ct = TydConverter.Deserialize<CompanyType>(doc[0]);
				ct.Location = fname;
				ct.Title = fi.Name.Replace(fi.Extension, "");
				return ct;
			}
		}

		[Serializable]
		public class CompanyTypeTypes
		{
			//DATA
			public string Software;
			public double Chance;
		}

		public class Personality
		{
			//DATA
			public string Name { get; set; }
			private double worklearn;
			public double WorkLearn { get => worklearn; set { if (value > 1) { worklearn = 1; return;  } if (value < -1) { worklearn = -1; return; } worklearn = value; } }
			private double social;
			public double Social { get => social; set { if (value > 1) { social = 1; return; } if (value < -1) { social = -1; return; } social = value; } }
			private double lazystress;
			public double LazyStress { get => lazystress; set { if (value > 1) { lazystress = 1; return; } if (value < -1) { lazystress = -1; return; } lazystress = value; } }
			public Utils.ObservableDictionary<string, double> Relationships { get; set; }

			public TydTable ToTyd()
			{
				TydTable returntable = new TydTable("");

				NumberFormatInfo format = new NumberFormatInfo() { NumberDecimalSeparator = "." };

				List<TydString> relationships = new List<TydString>();
				foreach(KeyValuePair<string, double> kvp in Relationships)
				{
					relationships.Add(new TydString(kvp.Key, kvp.Value.ToString(format)));
				}

				List<TydString> content = new List<TydString>()
				{
					new TydString("Name", Name),
					new TydString("WorkLearn", WorkLearn.ToString(format)),
					new TydString("Social", Social.ToString(format)),
					new TydString("LazyStress", LazyStress.ToString(format))
				};
				TydTable relationstable = new TydTable("Relationships", relationships.ToArray());

				returntable.AddChildren(content.ToArray());
				returntable.AddChildren(relationstable);
				return returntable;
			}

			public static void SaveDocument(List<TydTable> nodes, List<TydList> incompatibilitynodes, string fname)
			{

				TydTable root = new TydTable("PersonalityGraph", new TydList("Personalities", nodes.ToArray()), new TydList("Incompatibilities", incompatibilitynodes.ToArray()));
				TydDocument doc = new TydDocument();

				doc.AddChildren(root);
				TydFile file = TydFile.FromDocument(doc);
				file.Save(fname);
			}

			public Personality()
			{

			}

			public static Personality Create(string name)
			{
				Personality perso = new Personality() {
				Name = name,
				LazyStress = 0,
				WorkLearn = 0,
				Social = 0,
				Relationships = new Utils.ObservableDictionary<string, double>()
				};

				return perso;
			}
		}

		public class PersonalityIncompatibility
		{
			public string Key { get; set; }
			public string Value { get; set; }

			public PersonalityIncompatibility(string key, string value)
			{
				Key = key;
				Value = value;
			}

			public TydList ToTyd()
			{
				return new TydList("", Key, Value);
			}
		}

		[Serializable]
		public class Manufacturing
		{
			public ManufacturingComponents[] Components;
			public ManufacturingProcesses[] Processes;
			public int FinalTime;

			public Manufacturing() { }

			public static Manufacturing FromNode(TydNode node)
			{
				Manufacturing manu = TydConverter.Deserialize<Manufacturing>(node);
				return manu;
			}

			public TydNode ToNode()
			{
				TydNode node = TydConverter.Serialize("Manufacturing", this, true);
				return node;
			}
		}

		[Serializable]
		public class ManufacturingComponents
		{
			public string Name;
			public string Thumbnail;
			public string BuiltInThumnail; //OPTIONAL
			public string DependsOn; //OPTIONAL
			public int Price;
			public int Time;

			public ManufacturingComponents() { }
		}

		[Serializable]
		public class ManufacturingProcesses
		{
			public string[] Inputs;
			public string Output;

			public ManufacturingProcesses() { }
		}
	}
}
