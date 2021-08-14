using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace DataSINC
{
	public class Database
	{
		public static Database Instance;

		public string rootfolder;
		public List<DataTypes.SoftwareType> SoftwareTypes = new List<DataTypes.SoftwareType>();
		public List<DataTypes.NameGenerator> NameGenerators = new List<DataTypes.NameGenerator>();
		public List<DataTypes.CompanyType> CompanyTypes = new List<DataTypes.CompanyType>();
		public List<DataTypes.Personality> Personalities = new List<DataTypes.Personality>();

		public Database(string modfolder)
		{
			rootfolder = modfolder;

			//Load NameGenerators
			string namegenpath = Path.Combine(modfolder, "NameGenerators");
			if(Directory.Exists(namegenpath))
			{
				foreach(string fname in Directory.GetFiles(namegenpath))
				{
					DataTypes.NameGenerator ng = new DataTypes.NameGenerator(fname);
					NameGenerators.Add(ng);
				}
			}

			//Load Software Types
			string softwarepath = Path.Combine(modfolder, "SoftwareTypes");
			if(Directory.Exists(softwarepath))
			{
				foreach(string fname in Directory.GetFiles(softwarepath))
				{
					DataTypes.SoftwareType st = new DataTypes.SoftwareType(fname);
					SoftwareTypes.Add(st);
				}
			}

			//Load Company Types
			string companypath = Path.Combine(modfolder, "SoftwareTypes");
			if(Directory.Exists(companypath))
			{
				foreach(string fname in Directory.GetFiles(companypath))
				{
					DataTypes.CompanyType ct = new DataTypes.CompanyType(fname);
					CompanyTypes.Add(ct);
				}
			}

			//Load Personalities
			string personalitypath = Path.Combine(modfolder, "Personalities.tyd");
			if(File.Exists(personalitypath))
			{
				 Personalities = FileHandler.LoadPersonalities(File.ReadAllText(personalitypath));
			}

			Instance = this;
		}

		public void Save()
		{
			foreach(DataTypes.NameGenerator ngen in NameGenerators)
			{
				if (File.Exists(ngen.Location)) File.Delete(ngen.Location);
				File.WriteAllText(ngen.Location, ngen.Content);
			}
			string personalitypath = Path.Combine(rootfolder, "Personalities.tyd");
			if (File.Exists(personalitypath)) { File.Delete(personalitypath); }
			
			MessageBox.Show("Mod saved!");
		}
	}
}
