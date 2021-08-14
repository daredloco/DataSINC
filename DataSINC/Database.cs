﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace DataSINC
{
	public class Database
	{
		public static Database Instance;

		public string rootfolder;
		public ObservableList<DataTypes.SoftwareType> SoftwareTypes = new ObservableList<DataTypes.SoftwareType>();
		public ObservableList<DataTypes.NameGenerator> NameGenerators = new ObservableList<DataTypes.NameGenerator>();
		public ObservableList<DataTypes.CompanyType> CompanyTypes = new ObservableList<DataTypes.CompanyType>();
		public ObservableList<DataTypes.Personality> Personalities = new ObservableList<DataTypes.Personality>();
		public ObservableList<DataTypes.PersonalityIncompatibility> Incompatibilities = new ObservableList<DataTypes.PersonalityIncompatibility>();

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
				Incompatibilities = FileHandler.LoadIncompatibilities(File.ReadAllText(personalitypath));
			}
			Instance = this;
		}

		public void Save()
		{
			Directory.CreateDirectory(Path.Combine(rootfolder, "SoftwareTypes"));
			Directory.CreateDirectory(Path.Combine(rootfolder, "CompanyTypes"));
			Directory.CreateDirectory(Path.Combine(rootfolder, "NameGenerators"));

			foreach (DataTypes.NameGenerator ngen in NameGenerators)
			{
				if (File.Exists(ngen.Location)) File.Delete(ngen.Location);
				File.WriteAllText(ngen.Location, ngen.Content);
			}
			string personalitypath = Path.Combine(rootfolder, "Personalities.tyd");
			List<Tyd.TydTable> persotyd = new List<Tyd.TydTable>();
			foreach(DataTypes.Personality perso in Personalities)
			{
				persotyd.Add(perso.ToTyd());
			}
			List<Tyd.TydList> incotyd = new List<Tyd.TydList>();
			foreach(DataTypes.PersonalityIncompatibility inco in Incompatibilities)
			{
				incotyd.Add(inco.ToTyd());
			}
			DataTypes.Personality.SaveDocument(persotyd, incotyd, personalitypath);
			MessageBox.Show("Mod saved!");
		}
	}
}
