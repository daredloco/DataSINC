using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
		public bool IsSaved = true;

		public MainWindow()
		{
			InitializeComponent();
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
			Debug.Info("Open MainWindow...");
			Settings.Load();
			menu_about.Click += MenuAboutClick;
			menu_help.Click += MenuHelpClick;
			menu_git.Click += MenuGitClick;
			menu_new.Click += NewMod;
			menu_load.Click += LoadMod;
			menu_save.Click += SaveMod;
			menu_exit.Click += ExitApp;

			//Add Update * Button click functionality
			ctbt_save.Click += SaveCompanyType;
			stbt_save.Click += SaveSoftwareType;
			ngbt_save.Click += SaveNameGen;
			pebt_save.Click += SavePersonality;

			//Add new item button functionality
			ctbt_new.Click += NewCompanyType;
			stbt_new.Click += NewSoftwareType;
			ngbt_new.Click += NewNameGen;
			pebt_new.Click += NewPersonality;

			//Remove item functionality
			ctmain_remove.Click += RemoveCompanyType;
			stmain_remove.Click += RemoveSoftwareType;
			ngmain_remove.Click += RemoveNameGenerator;
			pemain_remove.Click += RemovePersonality;

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
			ctcm_new.Click += CtNewType;
			ctcm_edit.Click += CtEditType;
			ctcm_remove.Click += CtRemoveType;
			stcatcm_new.Click += StNewCategory;
			stcatcm_edit.Click += StEditCategory;
			stcatcm_remove.Click += StRemoveCategory;
			stfeatcm_new.Click += StNewFeature;
			stfeatcm_edit.Click += StEditFeature;
			stfeatcm_remove.Click += StRemoveFeature;

			//Manufatoring Button
			stbt_manufacturing.Click += ShowManufacturing;

			Closing += OnShutdown;
			KeyDown += OnControlKeys;
		}

		private void StRemoveFeature(object sender, RoutedEventArgs e)
		{
			if (stlb_features.SelectedItem == null)
			{
				return;
			}
			DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
			st.Features.RemoveAll(x => x.Name == stlb_features.SelectedItem.ToString());
		}

		private void StEditFeature(object sender, RoutedEventArgs e)
		{
			if (stlb_features.SelectedItem == null)
			{
				return;
			}
			DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
			DataTypes.SoftwareTypeSpecFeatures feat = st.Features.First(x => x.Name == stlb_features.SelectedItem.ToString());
			FeaturesPopup popup = new FeaturesPopup(feat);
			if (popup.ShowDialog() == true)
			{
				int index = st.Features.FindIndex(x => x.Name == popup.Feature.Name);
				st.Features[index] = popup.NewFeature;
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void StNewFeature(object sender, RoutedEventArgs e)
		{
			FeaturesPopup popup = new FeaturesPopup(null);
			if (popup.ShowDialog() == true)
			{
				DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
				st.Features.Add(popup.NewFeature);
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void StRemoveCategory(object sender, RoutedEventArgs e)
		{
			if (stlb_categories.SelectedItem == null)
			{
				return;
			}
			DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
			st.Categories.RemoveAll(x => x.Name == stlb_categories.SelectedItem.ToString());
			int index = Database.Instance.SoftwareTypes.FindIndex(x => x.Name == st.Name);
			Database.Instance.SoftwareTypes[index] = st;
			GenerateSoftwareTypesList();
			IsSaved = false;
			SetWindowTitle();
		}

		private void StEditCategory(object sender, RoutedEventArgs e)
		{
			DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
			DataTypes.SoftwareTypeCategories cat = st.Categories.First(x => x.Name == stlb_categories.SelectedItem.ToString());
			CategoriesPopup popup = new CategoriesPopup(cat);
			if(popup.ShowDialog() == true)
			{
				int index = st.Categories.FindIndex(x => x.Name == popup.Category.Name);
				st.Categories[index] = popup.NewCategory;
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void StNewCategory(object sender, RoutedEventArgs e)
		{
			CategoriesPopup popup = new CategoriesPopup(null);
			if (popup.ShowDialog() == true)
			{
				DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
				st.Categories.Add(popup.NewCategory);
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void ShowManufacturing(object sender, RoutedEventArgs e)
		{
			DataTypes.SoftwareType st = lb_softwaretypes.SelectedItem as DataTypes.SoftwareType;
			ManufacturingPopup popup = new ManufacturingPopup(st);
			if(popup.ShowDialog() == true)
			{
				st.Manufacturing = ManufacturingPopup.Result;
				if(st.Manufacturing == null)
				{
					st.Hardware = false;
				}
				else
				{
					st.Hardware = true;
				}
				Database.Instance.SoftwareTypes.First(x => x.Location == st.Location).Manufacturing = st.Manufacturing;
				Database.Instance.SoftwareTypes.First(x => x.Location == st.Location).Hardware = st.Hardware;
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void MenuGitClick(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/daredloco/DataSINC");
		}

		private void MenuHelpClick(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://softwareinc.coredumping.com/wiki/index.php/Data_Modding");
		}

		private void CtRemoveType(object sender, RoutedEventArgs e)
		{
			if (ctlb_types.SelectedItem == null)
			{
				return;
			}
			if (MessageBox.Show("Do you really want to remove the Type?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				DataTypes.CompanyType ct = lb_companytypes.SelectedItem as DataTypes.CompanyType;
				List<DataTypes.CompanyTypeTypes> typeslist = new List<DataTypes.CompanyTypeTypes>();
				typeslist.AddRange(ct.Types);
				int index = typeslist.FindIndex(x => x.Software == (ctlb_types.SelectedItem as string).Split(':')[0]);
				typeslist.RemoveAt(index);
				ct.Types = typeslist.ToArray();
				GenerateCompanyTypesList();
				IsSaved = false;
				SetWindowTitle();
				ctlb_types.Items.Clear();
				foreach (DataTypes.CompanyTypeTypes ctt in ct.Types)
				{
					ctlb_types.Items.Add(ctt.Software + ": " + ctt.Chance);
				}
			}
		}

		private void CtEditType(object sender, RoutedEventArgs e)
		{
			if (ctlb_types.SelectedItem == null)
			{
				return;
			}
			DataTypes.CompanyType ct = lb_companytypes.SelectedItem as DataTypes.CompanyType;
			TypePopup popup = new TypePopup(ct, ct.Types.First(x => x.Software == (ctlb_types.SelectedItem as string).Split(':')[0]));
			if (popup.ShowDialog() == true)
			{
				GenerateCompanyTypesList();
				IsSaved = false;
				SetWindowTitle(); 
				ctlb_types.Items.Clear();
				foreach (DataTypes.CompanyTypeTypes ctt in ct.Types)
				{
					ctlb_types.Items.Add(ctt.Software + ": " + ctt.Chance);
				}
			}
		}

		private void CtNewType(object sender, RoutedEventArgs e)
		{
			TypePopup popup = new TypePopup(lb_companytypes.SelectedItem as DataTypes.CompanyType);
			if(popup.ShowDialog() == true)
			{
				GenerateCompanyTypesList();
				IsSaved = false;
				SetWindowTitle();
				ctlb_types.Items.Clear();
				foreach (DataTypes.CompanyTypeTypes ctt in (lb_companytypes.SelectedItem as DataTypes.CompanyType).Types)
				{
					ctlb_types.Items.Add(ctt.Software + ": " + ctt.Chance);
				}
			}
		}

		private void NewCompanyType(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.CompanyType);
			if (popup.ShowDialog() == true)
			{
				GenerateCompanyTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void SaveCompanyType(object sender, RoutedEventArgs e)
		{
			DataTypes.CompanyType ct = (DataTypes.CompanyType)lb_companytypes.SelectedItem;
			ct.Specialization = cttb_spec.Text;
			ct.peryear = double.Parse(cttb_peryear.Text);
			ct.Min = uint.Parse(cttb_min.Text);
			ct.Max = uint.Parse(cttb_max.Text);
			ct.NameGenerator = cttb_namegen.Text;

			//TODO: Handle ct.Types
			int index = Database.Instance.CompanyTypes.FindIndex(x => x.Title == ct.Title);
			Database.Instance.CompanyTypes[index] = ct;
			GenerateCompanyTypesList();
			IsSaved = false;
			SetWindowTitle();
			MessageBox.Show("Companytype " + ct.Title + " updated!", "Data updated!", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void SaveSoftwareType(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void NewSoftwareType(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.SoftwareType);
			if (popup.ShowDialog() == true)
			{
				GenerateSoftwareTypesList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void OnControlKeys(object sender, KeyEventArgs e)
		{
			if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
			{
				Debug.Info("Saving mod with hotkey!");
				Database.Instance.Save();
				IsSaved = true;
				SetWindowTitle();
			}
		}

		private void OnShutdown(object sender, CancelEventArgs e)
		{
			if(!IsSaved)
			{
				Debug.Info("Trying to shut application down without saving!");
				MessageBoxResult result = MessageBox.Show("Do you want to save your changes before leaving?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
				if (result == MessageBoxResult.Yes)
				{
					Debug.Info("User saves mod");
					Database.Instance.Save();
				}else if(result == MessageBoxResult.Cancel)
				{
					Debug.Info("User aborts shutdown");
					e.Cancel = true;
				}
				else
				{
					Debug.Info("User doesn't save mod");
				}
			}
			Debug.Info("Bye bye User, until next time!");
		}

		private void RemovePersonality(object sender, RoutedEventArgs e)
		{
			if(lb_persos.SelectedItem == null) { return; }

			if(MessageBoxResult.Yes != MessageBox.Show("Do you really want to delete " + ((DataTypes.Personality)lb_persos.SelectedItem).Name + "?","Warning!", MessageBoxButton.YesNo, MessageBoxImage.Hand)){
				Debug.Warn("User didn't want to remove personality " + ((DataTypes.Personality)lb_persos.SelectedItem).Name + "!");
				return;
			}
			Debug.Info("User did remove personality " + ((DataTypes.Personality)lb_persos.SelectedItem).Name);
			Database.Instance.RemoveData((DataTypes.Personality)lb_persos.SelectedItem);
			//Database.Instance.Personalities.Remove((DataTypes.Personality)lb_persos.SelectedItem);
			IsSaved = false;
			SetWindowTitle();
		}

		private void RemoveNameGenerator(object sender, RoutedEventArgs e)
		{
			if (lb_namegens.SelectedItem == null) { return; }

			if (MessageBoxResult.Yes != MessageBox.Show("Do you really want to delete " + ((DataTypes.NameGenerator)lb_namegens.SelectedItem).Title + "?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Hand)){
				Debug.Warn("User didn't want to remove namegenerator " + ((DataTypes.NameGenerator)lb_namegens.SelectedItem).Title + "!");
				return;
			}
			Debug.Info("User did remove namegenerator " + ((DataTypes.NameGenerator)lb_namegens.SelectedItem).Title);
			Database.Instance.RemoveData((DataTypes.NameGenerator)lb_namegens.SelectedItem);
			//Database.Instance.NameGenerators.Remove((DataTypes.NameGenerator)lb_namegens.SelectedItem);
			IsSaved = false;
			SetWindowTitle();
		}

		private void RemoveSoftwareType(object sender, RoutedEventArgs e)
		{
			if (lb_softwaretypes.SelectedItem == null) { return; }

			if (MessageBoxResult.Yes != MessageBox.Show("Do you really want to delete " + ((DataTypes.SoftwareType)lb_softwaretypes.SelectedItem).Title + "?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Hand)){
				Debug.Warn("User didn't want to remove softwaretype " + ((DataTypes.SoftwareType)lb_softwaretypes.SelectedItem).Title + "!");
				return;
			}
			Debug.Info("User did remove softwaretype " + ((DataTypes.SoftwareType)lb_softwaretypes.SelectedItem).Title);
			Database.Instance.RemoveData((DataTypes.SoftwareType)lb_softwaretypes.SelectedItem);
			//Database.Instance.SoftwareTypes.Remove((DataTypes.SoftwareType)lb_softwaretypes.SelectedItem);
			IsSaved = false;
			SetWindowTitle();
		}

		private void RemoveCompanyType(object sender, RoutedEventArgs e)
		{
			if (lb_companytypes.SelectedItem == null) { return; }

			if (MessageBoxResult.Yes != MessageBox.Show("Do you really want to delete " + ((DataTypes.CompanyType)lb_companytypes.SelectedItem).Title + "?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Hand)){
				Debug.Warn("User didn't want to remove companytype " + ((DataTypes.CompanyType)lb_companytypes.SelectedItem).Title + "!");
				return;
			}
			Debug.Info("User did remove companytype " + ((DataTypes.CompanyType)lb_companytypes.SelectedItem).Title);
			Database.Instance.RemoveData((DataTypes.CompanyType)lb_companytypes.SelectedItem);
			//Database.Instance.CompanyTypes.Remove((DataTypes.CompanyType)lb_companytypes.SelectedItem);
			IsSaved = false;
			SetWindowTitle();
		}

		private void NewPersonality(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.Personality);
			if(popup.ShowDialog() == true)
			{
				GeneratePersonalityList();
				IsSaved = false;
				SetWindowTitle();
			}
		}

		private void NewNameGen(object sender, RoutedEventArgs e)
		{
			AddPopup popup = new AddPopup(AddPopup.PopupType.NameGenerator);
			if(popup.ShowDialog() == true)
			{
				GenerateNameGenList();
				IsSaved = false;
				SetWindowTitle();
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

			IsSaved = false;
			SetWindowTitle();
			MessageBox.Show("Personality " + perso.Name + " updated!", "Data updated!", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void PeNewRelation(object sender, EventArgs args)
		{
			RelationshipPopup popup = new RelationshipPopup(lb_persos.SelectedItem as DataTypes.Personality);
			if (popup.ShowDialog() == true)
			{
				GeneratePersonalityList();
				IsSaved = false;
				SetWindowTitle();
				pelb_relationships.Items.Clear();
				foreach (KeyValuePair<string, double> kvp in (lb_persos.SelectedItem as DataTypes.Personality).Relationships)
				{
					pelb_relationships.Items.Add(kvp);
				}
			}
		}

		private void PeEditRelation(object sender, EventArgs args)
		{
			DataTypes.Personality perso = lb_persos.SelectedItem as DataTypes.Personality;
			RelationshipPopup popup = new RelationshipPopup(perso, perso.Relationships.ElementAt(pelb_relationships.SelectedIndex).Key, perso.Relationships.ElementAt(pelb_relationships.SelectedIndex).Value);
			if (popup.ShowDialog() == true)
			{
				GeneratePersonalityList();
				IsSaved = false;
				SetWindowTitle();
				pelb_relationships.Items.Clear();
				foreach (KeyValuePair<string, double> kvp in (lb_persos.SelectedItem as DataTypes.Personality).Relationships)
				{
					pelb_relationships.Items.Add(kvp);
				}
			}
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

			cttb_spec.Text = ct.Specialization;
			cttb_peryear.Text = ct.peryear.ToString();
			cttb_min.Text = ct.Min.ToString();
			cttb_max.Text = ct.Max.ToString();
			cttb_namegen.Text = ct.NameGenerator;
			ctlb_types.Items.Clear();
			foreach(DataTypes.CompanyTypeTypes ctt in ct.Types)
			{
				ctlb_types.Items.Add(ctt.Software + ": " + ctt.Chance);
			}
		}

		private void Lb_softwaretypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count < 1) { return; }
			DataTypes.SoftwareType st = (DataTypes.SoftwareType)e.AddedItems[0];

			sttb_name.Text = st.Name;
			stcb_override.IsChecked = st.Override;
			sttb_category.Text = st.Category;
			stlb_categories.Items.Clear();
			foreach (DataTypes.SoftwareTypeCategories cat in st.Categories)
			{
				stlb_categories.Items.Add(cat.Name);
			}
			sttb_desc.Text = st.Description;
			sttb_unlock.Text = st.Unlock.ToString();
			sttb_random.Text = st.Random.ToString();
			sttb_idealprice.Text = st.IdealPrice.ToString();
			sttb_optimaldevtime.Text = st.OptimalDevTime.ToString();
			sttb_popularity.Text = st.Popularity.ToString();
			sttb_retention.Text = st.Retention.ToString();
			sttb_iterative.Text = st.Iterative.ToString();
			stlb_ossupport.Items.Clear();
			foreach(string ossupport in st.OSSupport)
			{
				stlb_ossupport.Items.Add(ossupport);
			}
			stcb_oneclient.IsChecked = st.OneClient;
			stcb_inhouse.IsChecked = st.InHouse;
			sttb_namegens.Text = st.NameGenerator;
			sttb_submarket1.Text = st.SubmarketNames[0];
			sttb_submarket2.Text = st.SubmarketNames[1];
			sttb_submarket3.Text = st.SubmarketNames[2];
			stlb_features.Items.Clear();
			foreach(DataTypes.SoftwareTypeSpecFeatures feat in st.Features)
			{
				stlb_features.Items.Add(feat.Name);
			}
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

			IsSaved = false;
			SetWindowTitle();
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
						IsSaved = true;
						SetWindowTitle();
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
						IsSaved = false;
						SetWindowTitle();
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
			IsSaved = true;
			SetWindowTitle();
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
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			MessageBox.Show("Editor for the game Software INC made by daRedLoCo.\nVersion: " + version + "\n\nUses TyDSharp from https://github.com/khornel/TyDSharp", "About", MessageBoxButton.OK, MessageBoxImage.Information);
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

		private void SetWindowTitle()
		{
			if(IsSaved)
			{
				Title = "Data SINC";
			}
			else
			{
				Title = "Data SINC (unsaved changes)";
			}
		}
	}
}
