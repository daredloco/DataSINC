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
	/// Interaktionslogik für CategoriesPopup.xaml
	/// </summary>
	public partial class CategoriesPopup : Window
	{
		//TODO: Implement hardware

		public DataTypes.SoftwareTypeCategories Category;
		public DataTypes.SoftwareTypeCategories NewCategory;

		public CategoriesPopup(DataTypes.SoftwareTypeCategories cat)
		{
			Category = cat;
			InitializeComponent();

			if(cat != null)
			{
				LoadCategory();
			}

			bt_add.Click += UpdateCategory;
		}

		private void UpdateCategory(object sender, RoutedEventArgs e)
		{
			//TODO: Checks if all values are correct and filled (where necessary)
			if(string.IsNullOrWhiteSpace(tb_name.Text))
			{
				MessageBox.Show("Name is a mandatory field!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!int.TryParse(tb_idealprice.Text, out _))
			{
				MessageBox.Show("IdealPrice needs to be a valid integer!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!double.TryParse(tb_iterative.Text, out _))
			{
				MessageBox.Show("Iterative needs to be a valid double!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!int.TryParse(tb_lagbehind.Text, out _))
			{
				MessageBox.Show("LagBehind needs to be a valid integer!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!double.TryParse(tb_popularity.Text, out _))
			{
				MessageBox.Show("Populairty needs to be a valid double!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!int.TryParse(tb_retention.Text, out _))
			{
				MessageBox.Show("Retention needs to be a valid integer!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!int.TryParse(tb_submarket0.Text, out _) || !int.TryParse(tb_submarket1.Text, out _) || !int.TryParse(tb_submarket2.Text, out _))
			{
				MessageBox.Show("Submarkets need to be valid integers!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!double.TryParse(tb_timescale.Text, out _))
			{
				MessageBox.Show("TimeScale needs to be a valid double!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if(!int.TryParse(tb_unlock.Text, out _))
			{
				MessageBox.Show("Unlock needs to be a valid integer!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			NewCategory = new DataTypes.SoftwareTypeCategories()
			{
				Name = tb_name.Text,
				Description = tb_desc.Text,
				IdealPrice = int.Parse(tb_idealprice.Text),
				iterative = double.Parse(tb_iterative.Text),
				LagBehind = int.Parse(tb_lagbehind.Text),
				NameGenerator = tb_namegen.Text,
				popularity = double.Parse(tb_popularity.Text),
				Retention = int.Parse(tb_retention.Text),
				Submarkets = new int[3] { int.Parse(tb_submarket0.Text), int.Parse(tb_submarket1.Text), int.Parse(tb_submarket2.Text) },
				timeScale = double.Parse(tb_timescale.Text),
				Unlock = int.Parse(tb_unlock.Text)
			};
			DialogResult = true;
			Close();
		}

		private void LoadCategory()
		{
			tb_name.Text = Category.Name;
			tb_desc.Text = Category.Description;
			tb_idealprice.Text = Category.IdealPrice.ToString();
			tb_iterative.Text = Category.iterative.ToString();
			tb_lagbehind.Text = Category.LagBehind.ToString();
			tb_namegen.Text = Category.NameGenerator;
			tb_popularity.Text = Category.popularity.ToString();
			if(Category.Submarkets.Length == 3)
			{
				tb_submarket0.Text = Category.Submarkets[0].ToString();
				tb_submarket1.Text = Category.Submarkets[1].ToString();
				tb_submarket2.Text = Category.Submarkets[2].ToString();
			}
			else
			{
				tb_submarket0.Text = "0";
				tb_submarket1.Text = "0";
				tb_submarket2.Text = "0";
			}
			tb_retention.Text = Category.Retention.ToString();
			tb_timescale.Text = Category.timeScale.ToString();
			tb_unlock.Text = Category.Unlock.ToString();
		}
	}
}
