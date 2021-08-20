using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DataSINC
{
	/// <summary>
	/// Interaktionslogik für SubFeaturePopup.xaml
	/// </summary>
	public partial class SubFeaturePopup : Window
	{
		public DataTypes.SoftwareTypeSubFeatures SubFeature;
		public DataTypes.SoftwareTypeSubFeatures NewSubFeature;

		private string[] scripts = new string[5];

		public SubFeaturePopup(DataTypes.SoftwareTypeSubFeatures subfeature = null)
		{
			InitializeComponent();
			SubFeature = subfeature;

			sl_codeart.ValueChanged += CodeArtChanged;
			sl_level.ValueChanged += LevelChanged;

			if (subfeature != null)
			{
				tb_name.Text = subfeature.Name;
				tb_description.Text = subfeature.Description;
				tb_devtime.Text = subfeature.DevTime.ToString();
				tb_sever.Text = subfeature.server.ToString();
				tb_unlock.Text = subfeature.Unlock.ToString();
				if(subfeature.Submarkets.Length == 3)
				{
					tb_submarket0.Text = subfeature.Submarkets[0].ToString();
					tb_submarket1.Text = subfeature.Submarkets[1].ToString();
					tb_submarket2.Text = subfeature.Submarkets[2].ToString();
				}
				sl_level.Value = subfeature.level;
				sl_codeart.Value = subfeature.level;
				lb_categories.Items.Clear();
				if(subfeature.SoftwareCategories != null)
				{
					foreach (string category in subfeature.SoftwareCategories)
					{
						lb_categories.Items.Add(category);
					}
				}

				scripts[0] = subfeature.Script_EndOfDay;
				scripts[1] = subfeature.Script_AfterSales;
				scripts[2] = subfeature.Script_OnRelease;
				scripts[3] = subfeature.Script_NewCopies;
				scripts[4] = subfeature.Script_WorkItemChange;
			}

			tb_script.Text = scripts[0];
			cb_script.SelectedIndex = 0;
			cb_script.SelectionChanged += ScriptTypeChanged;
			tb_script.TextChanged += ScriptChanged;

			categories_new.Click += NewCategory;
			categories_remove.Click += RemoveCategory;
			bt_add.Click += AddClicked;
		}

		private void ScriptChanged(object sender, TextChangedEventArgs e)
		{
			scripts[cb_script.SelectedIndex] = tb_script.Text;
		}

		private void ScriptTypeChanged(object sender, SelectionChangedEventArgs e)
		{
			tb_script.Text = scripts[cb_script.SelectedIndex];
		}

		private void AddClicked(object sender, RoutedEventArgs e)
		{
			NewSubFeature = new DataTypes.SoftwareTypeSubFeatures()
			{
				Name = tb_name.Text,
				Description = tb_description.Text,
				codeart = sl_codeart.Value,
				level = int.Parse(sl_codeart.Value.ToString()),
				DevTime = int.Parse(tb_devtime.Text),
				Script_EndOfDay = scripts[0],
				Script_AfterSales = scripts[1],
				Script_OnRelease = scripts[2],
				Script_NewCopies = scripts[3],
				Script_WorkItemChange = scripts[4],
				server = double.Parse(tb_sever.Text),
				Unlock = int.Parse(tb_unlock.Text)
			};
			foreach(string str in lb_categories.Items)
			{
				NewSubFeature.SoftwareCategories.Append(str);
			}
			if(string.IsNullOrWhiteSpace(tb_submarket0.Text) && string.IsNullOrWhiteSpace(tb_submarket1.Text) && string.IsNullOrWhiteSpace(tb_submarket2.Text))
			{
				NewSubFeature.Submarkets.Append(0);
			}
			else
			{
				NewSubFeature.Submarkets.Append(int.Parse(tb_submarket0.Text));
				NewSubFeature.Submarkets.Append(int.Parse(tb_submarket1.Text));
				NewSubFeature.Submarkets.Append(int.Parse(tb_submarket2.Text));
			}

			DialogResult = true;
			Close();
		}

		private void LevelChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			l_level.Content = "Level (" + e.NewValue +")";
		}

		private void CodeArtChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			l_codeart.Content = "CodeArt (" + e.NewValue + ")";
		}

		private void RemoveCategory(object sender, RoutedEventArgs e)
		{
			lb_categories.Items.Remove(lb_categories.SelectedItem);
		}

		private void NewCategory(object sender, RoutedEventArgs e)
		{
			AddStringPopup popup = new AddStringPopup("Category:", "Add Category");
			if(popup.ShowDialog() == true)
			{
				lb_categories.Items.Add(popup.Output);
			}
		}
	}
}
