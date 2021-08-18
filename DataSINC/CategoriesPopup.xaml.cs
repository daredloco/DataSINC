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
		public DataTypes.SoftwareTypeCategories Category;
		public CategoriesPopup(DataTypes.SoftwareTypeCategories cat)
		{
			Category = cat;
			InitializeComponent();

			if(cat != null)
			{
				LoadCategory();
			}
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
