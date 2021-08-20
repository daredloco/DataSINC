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
	/// Interaktionslogik für FeaturesPopup.xaml
	/// </summary>
	public partial class FeaturesPopup : Window
	{
		public DataTypes.SoftwareTypeSpecFeatures Feature;
		public DataTypes.SoftwareTypeSpecFeatures NewFeature;
		List<DataTypes.SoftwareTypeSubFeatures> subfeatures = new List<DataTypes.SoftwareTypeSubFeatures>();

		public FeaturesPopup(DataTypes.SoftwareTypeSpecFeatures feat)
		{
			InitializeComponent();
			Feature = feat;
			sl_codeart.ValueChanged += CodeArtChanged;
			if (feat != null)
			{
				LoadFeature();
			}

			dependencies_new.Click += NewDependency;
			dependencies_remove.Click += RemoveDependency;
			categories_new.Click += NewCategory;
			categories_remove.Click += RemoveCategory;
			features_new.Click += NewSubFeature;
			features_edit.Click += EditSubFeature;
			features_remove.Click += RemoveSubFeature;

			bt_add.Click += AddClicked;
		}

		private void RemoveSubFeature(object sender, RoutedEventArgs e)
		{
			lb_features.Items.Remove(lb_features.SelectedItem);
		}

		private void EditSubFeature(object sender, RoutedEventArgs e)
		{
			if(lb_features.SelectedItem == null)
			{
				return;
			}
			DataTypes.SoftwareTypeSubFeatures subfeature = subfeatures.First(x => x.Name == lb_features.SelectedItem as string);
			SubFeaturePopup popup = new SubFeaturePopup(subfeature);
			if(popup.ShowDialog() == true)
			{
				//TODO: Add edited Subfeature to the Feature
				int index = subfeatures.IndexOf(subfeature);
				subfeatures[index] = popup.NewSubFeature;
				lb_features.Items[lb_features.SelectedIndex] = popup.NewSubFeature.Name;
			}
		}

		private void NewSubFeature(object sender, RoutedEventArgs e)
		{
			SubFeaturePopup popup = new SubFeaturePopup();
			if(popup.ShowDialog() == true)
			{
				subfeatures.Add(popup.NewSubFeature);
				lb_features.Items.Add(popup.NewSubFeature.Name);
			}
		}

		private void RemoveCategory(object sender, RoutedEventArgs e)
		{
			if (lb_categories.SelectedItem == null)
			{
				return;
			}
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

		private void RemoveDependency(object sender, RoutedEventArgs e)
		{
			if(lb_dependencies.SelectedItem == null)
			{
				return;
			}
			lb_dependencies.Items.Remove(lb_dependencies.SelectedItem);
		}

		private void NewDependency(object sender, RoutedEventArgs e)
		{
			AddStringPopup popup = new AddStringPopup("Dependency:", "Add Dependency");
			if (popup.ShowDialog() == true)
			{
				lb_dependencies.Items.Add(popup.Output);
			}
		}

		private void LoadFeature()
		{
			tb_name.Text = Feature.Name;
			tb_description.Text = Feature.Description;
			tb_spec.Text = Feature.Spec;
			tb_devtime.Text = Feature.DevTime.ToString();
			tb_server.Text = Feature.server.ToString();
			tb_unlock.Text = Feature.Unlock.ToString();
			if(Feature.Submarkets.Length == 3)
			{
				tb_submarket0.Text = Feature.Submarkets[0].ToString();
				tb_submarket1.Text = Feature.Submarkets[1].ToString();
				tb_submarket2.Text = Feature.Submarkets[2].ToString();
			}
			else
			{
				tb_submarket0.Text = "0";
				tb_submarket1.Text = "0";
				tb_submarket2.Text = "0";
			}
			lb_categories.Items.Clear();
			if(Feature.SoftwareCategories != null)
			{
				foreach (string category in Feature.SoftwareCategories)
				{
					lb_categories.Items.Add(category);
				}
			}
			lb_dependencies.Items.Clear(); 
			if(Feature.Dependencies != null)
			{
				foreach (string dependency in Feature.Dependencies)
				{
					lb_dependencies.Items.Add(dependency);
				}
			}
			lb_features.Items.Clear();
			if(Feature.Features != null)
			{
				subfeatures.Clear();
				foreach (DataTypes.SoftwareTypeSubFeatures subfeature in Feature.Features)
				{
					lb_features.Items.Add(subfeature.Name);
					subfeatures.Add(subfeature);
				}
			}
			cb_optional.IsChecked = Feature.Optional;
			sl_codeart.Value = Feature.codeart;
		}

		private void CodeArtChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			l_codeart.Content = "CodeArt (" + e.NewValue + ")";
		}

		private void AddClicked(object sender, RoutedEventArgs e)
		{
			//TODO: Add function to add/edit feature (Set NewFeature)
			DialogResult = true;
		}
	}
}
