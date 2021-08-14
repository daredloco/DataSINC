using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataSINC
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Settings.Load();
			menu_about.Click += MenuAboutClick;
			menu_new.Click += NewMod;
			menu_load.Click += LoadMod;
			menu_save.Click += SaveMod;
			menu_exit.Click += ExitApp;

			//Add Update * Button click functionality
			ngbt_save.Click += SaveNameGen;
			pebt_save.Click += SavePersonality;

			//Add new item button functionality
			ngbt_new.Click += NewNameGen;
			pebt_new.Click += NewPersonality;

			lb_namegens.SelectionMode = SelectionMode.Single;
			lb_namegens.SelectionChanged += Lb_namegens_SelectionChanged;

			lb_softwaretypes.SelectionMode = SelectionMode.Single;
			lb_softwaretypes.SelectionChanged += Lb_softwaretypes_SelectionChanged;

			lb_companytypes.SelectionMode = SelectionMode.Single;
			lb_companytypes.SelectionChanged += Lb_companytypes_SelectionChanged;

			lb_persos.SelectionMode = SelectionMode.Single;
			lb_persos.SelectionChanged += Lb_persos_SelectionChanged;

			//Change Slider title if slider changes
			//PERSONALITIES
			pes_lazystress.ValueChanged += Pes_lazystress_ValueChanged;
			pes_social.ValueChanged += Pes_social_ValueChanged;
			pes_worklearn.ValueChanged += Pes_worklearn_ValueChanged;

			//Handle contentmenu clicks
			pecm_new.Click += PeNewRelation;
			pecm_edit.Click += PeEditRelation;
			pecm_remove.Click += PeRemoveRelation;
		}

		private void NewPersonality(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.Personality);
			if(popup.ShowDialog() == true)
			{
				GeneratePersonalityList();
			}
		}

		private void NewNameGen(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.NameGenerator);
			if(popup.ShowDialog() == true)
			{
				GenerateNameGenList();
			}
		}

		private void ExitApp(object sender, RoutedEventArgs e)
		{
			Environment.Exit(1);
		}

		private void SavePersonality(object sender, RoutedEventArgs e)
		{
			DataTypes.Personality perso = (DataTypes.Personality)lb_persos.SelectedItem;
			perso.Name = petb_name.Text;
			perso.LazyStress = pes_lazystress.Value;
			perso.WorkLearn = pes_worklearn.Value;
			perso.Social = pes_social.Value;
			//TODO: Handle perso.Relationships

			int index = Database.Instance.Personalities.FindIndex(x => x.Name == perso.Name);
			Database.Instance.Personalities[index] = perso;
			GeneratePersonalityList();

			MessageBox.Show("Personality " + perso.Name + " updated!", "Data updated!", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void PeNewRelation(object sender, EventArgs args)
		{

		}

		private void PeEditRelation(object sender, EventArgs args)
		{

		}

		private void PeRemoveRelation(object sender, EventArgs args)
		{
			pelb_relationships.Items.Remove(pelb_relationships.SelectedItem);
		}

		private void Pes_worklearn_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			pel_worklearn.Content = "Work/Learn (" + pes_worklearn.Value + ")";
		}

		private void Pes_social_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			pel_social.Content = "Social (" + pes_social.Value + ")";
		}

		private void Pes_lazystress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			pel_lazystress.Content = "Lazy/Stress (" + pes_lazystress.Value + ")";
		}

		private void Lb_persos_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count < 1) { return; }
			DataTypes.Personality pers = (DataTypes.Personality)e.AddedItems[0];

			petb_name.Text = pers.Name;
			pes_lazystress.Value = pers.LazyStress;
			pes_social.Value = pers.Social;
			pes_worklearn.Value = pers.WorkLearn;
			pelb_relationships.Items.Clear();
			foreach(KeyValuePair<string, double> kvp in pers.Relationships)
			{
				pelb_relationships.Items.Add(kvp);
			}
		}

		private void Lb_companytypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Count < 1) { return; }
			DataTypes.CompanyType ct = (DataTypes.CompanyType)e.AddedItems[0];
		}

		private void Lb_softwaretypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count < 1) { return; }
			DataTypes.SoftwareType st = (DataTypes.SoftwareType)e.AddedItems[0];
		}

		private void Lb_namegens_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count < 1) { return; }
			DataTypes.NameGenerator ng = (DataTypes.NameGenerator)e.AddedItems[0];

			ngtb_title.Text = ng.Title;
			ngtb_content.Text = ng.Content;
		}

		private void SaveNameGen(object sender, EventArgs args)
		{
			DataTypes.NameGenerator ngen = (DataTypes.NameGenerator)lb_namegens.SelectedItem;
			ngen.Title = ngtb_title.Text;
			ngen.Content = ngtb_content.Text;

			int index = Database.Instance.NameGenerators.FindIndex(x => x.Location == ngen.Location);
			Database.Instance.NameGenerators[index] = ngen;
			GenerateNameGenList();

			MessageBox.Show("Namegenerator " + ngen.Title + " updated!","Data updated!", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void LoadMod(object sender, EventArgs args)
		{
			using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
			{
				fbd.Description = "Select modfolder";
				fbd.ShowNewFolderButton = false;
				fbd.SelectedPath = Settings.latestmod;
				System.Windows.Forms.DialogResult result = fbd.ShowDialog();
				if(result == System.Windows.Forms.DialogResult.OK)
				{
					if(!string.IsNullOrWhiteSpace(fbd.SelectedPath))
					{
						Database.Instance = new Database(fbd.SelectedPath);
						GenerateLists();
						Settings.latestmod = fbd.SelectedPath;
						Settings.Save();
					}
				}
			}
		}

		private void NewMod(object sender, EventArgs args)
		{
			using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
			{
				fbd.Description = "Select modfolder";
				fbd.ShowNewFolderButton = true;
				fbd.SelectedPath = Settings.latestmod;
				System.Windows.Forms.DialogResult result = fbd.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
					{
						//TODO: Create new mod
						if(System.IO.Directory.GetFiles(fbd.SelectedPath).Length > 0)
						{
							MessageBox.Show("You need to select an empty folder for the mod!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						}
						Settings.latestmod = fbd.SelectedPath;
						Settings.Save();
						Database.Instance = new Database(fbd.SelectedPath);
					}
				}
			}
		}

		private void SaveMod(object sender, EventArgs args)
		{
			if (Database.Instance == null)
			{
				MessageBox.Show("Please select a mod first!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			Database.Instance.Save();
		}

		private void GenerateLists()
		{
			GenerateCompanyTypesList();
			GenerateNameGenList();
			GenerateSoftwareTypesList();
			GeneratePersonalityList();
		}

		private void MenuAboutClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Editor for the game Software INC.\nVersion: 1.0\n\nUses TyDSharp from https://github.com/khornel/TyDSharp", "About", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void GenerateNameGenList()
		{
			lb_namegens.ItemsSource = Database.Instance.NameGenerators;
			if (Database.Instance.NameGenerators.Count > 0)
			{
				lb_namegens.SelectedIndex = 0;
			}
		}

		private void GenerateSoftwareTypesList()
		{
			lb_softwaretypes.ItemsSource = Database.Instance.SoftwareTypes;
			if (Database.Instance.SoftwareTypes.Count > 0)
			{
				lb_softwaretypes.SelectedIndex = 0;
			}
		}

		public void GenerateCompanyTypesList()
		{
			lb_companytypes.ItemsSource = Database.Instance.CompanyTypes;
			if (Database.Instance.CompanyTypes.Count > 0)
			{
				lb_companytypes.SelectedIndex = 0;
			}
		}

		public void GeneratePersonalityList()
		{
			lb_persos.ItemsSource = Database.Instance.Personalities;
			if(Database.Instance.Personalities.Count > 0)
			{
				lb_persos.SelectedIndex = 0;
			}
		}
	}
}
