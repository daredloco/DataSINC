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
		private DataTypes.SoftwareTypeCategories category;
		public CategoriesPopup(DataTypes.SoftwareTypeCategories cat)
		{
			category = cat;
			InitializeComponent();
		}
	}
}
