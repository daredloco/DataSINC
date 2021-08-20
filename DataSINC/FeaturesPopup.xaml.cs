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

		public FeaturesPopup(DataTypes.SoftwareTypeSpecFeatures feat)
		{
			InitializeComponent();
			Feature = feat;
			sl_codeart.ValueChanged += CodeArtChanged;
			if (feat != null)
			{
				LoadFeature();
			}

			bt_add.Click += AddClicked;
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
				foreach (DataTypes.SoftwareTypeSubFeatures subfeature in Feature.Features)
				{
					lb_features.Items.Add(subfeature.Name);
				}
			}
			cb_optional.IsChecked = Feature.Optional;
			sl_codeart.Value = Feature.codeart;
		}

		private void CodeArtChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			l_codeart.Content = "CodeArt: (" + sl_codeart.Value + ")";
		}

		private void AddClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
