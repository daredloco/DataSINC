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
	/// Interaktionslogik für AddStringPopup.xaml
	/// </summary>
	public partial class AddStringPopup : Window
	{
		public string Output;

		public AddStringPopup(string typelabel, string windowtitle, string oldvalue = "")
		{
			InitializeComponent();

			Title = windowtitle;
			l_string.Content = typelabel;
			tb_string.Text = oldvalue;

			bt_add.Click += ClickAdd;
		}

		private void ClickAdd(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(tb_string.Text))
			{
				MessageBox.Show("Invalid Input!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			Output = tb_string.Text;
			DialogResult = true;
			Close();
		}
	}
}
