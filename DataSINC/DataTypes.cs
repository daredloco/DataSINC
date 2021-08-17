using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyd;

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
					Content = "-start(base,base,base,pre)\n" +
								"-pre(base)\n" +
								"-base(base2, base2, base2, stop)\n" +
								"-base2(ext, stop, stop)\n" +
								"-ext(stop)"
				};

				return ng;
			}
		}

		public class SoftwareType
		{
			public string Location { get; set; }
			public string Title { get; set; }

			//DATA
			public string Name { get; set; }
			public bool Override { get; set; } //OPTIONAL
			public string Category { get; set; }
			public List<SoftwareTypeCategories> Categories { get; set; } //OPTIONAL
			public string Description { get; set; }
			public int Unlock { get; set; }
			public double Random { get; set; }
			public int IdealPrice { get; set; } //OPTIONAL
			public int OptimalDevTime { get; set; }
			public double Popularity { get; set; } //OPTIONAL
			public int Retention { get; set; } //OPTIONAL
			private double iterative;
			public double Iterative { //OPTIONAL
				get => iterative;
				set { if (value > 1) { iterative = 1; return; } if (value < 0) { iterative = 0; return; } iterative = value; } 
			}
			public List<string> OSSupport { get; set; } //OPTIONAL
			public bool OneClient { get; set; }
			public bool InHouse { get; set; }
			public string NameGenerator { get; set; } //OPTIONAL IF GENERATORS SET IN SUBS
			public List<string> SubmarketNames { get; set; }
			public List<SoftwareTypeSpecFeatures> Features { get; set; }

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
					Features = new List<SoftwareTypeSpecFeatures>()
				};

				return st;
			}

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

		public class SoftwareTypeCategories
		{
			//DATA
			public string Name { get; set; }
			public string Description { get; set; }
			public int Unlock { get; set; }
			private double popularity;
			public double Popularity { get => popularity; set { if (value > 1) { popularity = 1; return; } if (value < 0) { popularity = 0; return; } popularity = value; } }
			public List<int> Submarkets { get; set; }
			private double timescale;
			public double TimeScale { get => timescale; set { if (value > 1) { timescale = 1; return; } if (value < 0) { timescale = 0; return; } timescale = value; } }
			public int Retention { get; set; }
			public int IdealPrice { get; set; } //IGNORED IF SET IN SOFTWARE TYPE
			private double iterative;
			public double Iterative { get => iterative; set { if (value > 1) { iterative = 1; return; } if (value < 0) { iterative = 0; return; } iterative = value; } }
			public string NameGenerator { get; set; } //OPTIONAL
			public int LagBehind { get; set; } //OPTIONAL

			public TydTable ToTyd()
			{
				TydTable list = new TydTable("");
				list.AddChildren(
					Utils.TydHelpers.ConvertString(Name, "Name"),
					Utils.TydHelpers.ConvertString(Unlock, "Unlock"),
					Utils.TydHelpers.ConvertString(Popularity, "Popularity")
					);
				list.AddChildren(Utils.TydHelpers.ConvertList(Submarkets, "Submarkets"));
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

		public class SoftwareTypeSpecFeatures
		{
			//DATA
			public string Name { get; set; }
			public string Spec { get; set; }
			public string Description { get; set; }
			public List<string> Dependencies { get; set; }
			public int Unlock { get; set; }
			public int DevTime { get; set; }
			public List<int> Submarkets { get; set; }
			private double codeart { get; set; }
			public double CodeArt { get => codeart; set { if (value > 1) { codeart = 1; return; } if (value < 0) { codeart = 0; return; } codeart = value; } }
			private double server { get; set; }
			public double Server { get => server; set { if (value > 1) { server = 1; return; } if (value < 0) { server = 0; return; } server = value; } }
			public bool Optional { get; set; } //OPTIONAL (Will be OFF by default)
			public List<string> SoftwareCategories { get; set; } //OPTIONAL
			public List<SoftwareTypeSubFeatures> Features { get; set; }

			
		}

		public class SoftwareTypeSubFeatures
		{
			//DATA
			public string Name { get; set; }
			public string Description { get; set; }
			private int level;
			public int Level { get => level; set { if (value > 1) { level = 1; return; } if (value < 0) { level = 0; return; } level = value; } }
			public int Unlock { get; set; }
			public int DevTime { get; set; }
			public List<int> Submarkets { get; set; }
			private double codeart { get; set; }
			public double CodeArt { get => codeart; set { if (value > 1) { codeart = 1; return; } if (value < 0) { codeart = 0; return; } codeart = value; } }
			private double server { get; set; }
			public double Server { get => server; set { if (value > 1) { server = 1; return; } if (value < 0) { server = 0; return; } server = value; } }
			public List<string> SoftwareCategories { get; set; } //OPTIONAL
			public string Script_EntryPoint { get; set; } //OPTIONAL
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

		public class HardwareCategory
		{

		}
	}
}
