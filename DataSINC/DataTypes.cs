using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		}

		public class SoftwareType
		{
			public string Location { get; set; }
			public string Title { get; set; }

			//DATA
			public string Name { get; set; }
			public bool Override { get; set; }
			public string Category { get; set; }
			public List<SoftwareTypeCategories> Categories { get; set; } //OPTIONAL
			public string Description { get; set; }
			public int Unlock { get; set; }
			public double Random { get; set; }
			public int IdealPrice { get; set; } //OPTIONAL
			public int OptimalDevTime { get; set; }
			public int Popularity { get; set; } //OPTIONAL
			public int Retention { get; set; } //OPTIONAL
			private double iterative;
			public double Iterative { //OPTIONAL
				get => iterative;
				set { if (value > 1) { iterative = 1; return; } if (value < 0) { iterative = 0; return; } iterative = value; } 
			}
			public bool OSSupport { get; set; } //OPTIONAL
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

		public class CompanyType
		{
			public string Location { get; set; }
			public string Title { get; set; }

			//DATA
			public string Specialization { get; set; }
			private double peryear;
			public double PerYear { get => peryear; set { if (value > 1) { peryear = 1; return; } if (value < 0) { peryear = 0; return; } peryear = value; } }
			public uint Min { get; set; }
			public uint Max { get; set; }
			public bool Frameworks { get; set; }
			public List<CompanyTypeTypes> Types { get; set; }
			public string NameGen { get; set; }

			public CompanyType(string fname)
			{
				System.IO.FileInfo fi = new System.IO.FileInfo(fname);
				Location = fname;
				Title = fi.Name.Replace(fi.Extension, "");
				Converter.SoftwareTypes.FromTYD(fname);
			}
		}

		public class CompanyTypeTypes
		{
			//DATA
			public string Software { get; set; }
			public int Chance { get; set; }
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

			public TydDocument ToTyd()
			{
				TydDocument doc = new TydDocument();
				
				return doc;
			}
		}

		public class PersonalityIncompatibilities
		{
			Dictionary<string, string> Incompatibilities = new Dictionary<string, string>();
		}
	}
}
