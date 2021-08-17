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
	/// Interaktionslogik für TypePopup.xaml
	/// </summary>
	public partial class TypePopup : Window
	{
		private DataTypes.CompanyTypeTypes type;
		private DataTypes.CompanyType ct;

		public TypePopup(DataTypes.CompanyType ct, DataTypes.CompanyTypeTypes type = null)
		{
			InitializeComponent();
			this.ct = ct;
			LoadSoftwareTypes();
			if (type != null) { this.type = type; LoadTypeInfo(); }

			bt_add.Click += AddType;

		}

		private void AddType(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(cb_software.Text))
			{
				MessageBox.Show("You need to select a SoftwareType!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if(string.IsNullOrWhiteSpace(tb_chance.Text))
			{
				MessageBox.Show("You need to enter a chance value!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if(!double.TryParse(tb_chance.Text, out _))
			{
				MessageBox.Show("The chance value must be a double value (eg. 0.5)!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

			if(type == null)
			{
				DataTypes.CompanyTypeTypes newtype = new DataTypes.CompanyTypeTypes() { Software = cb_software.Text, Chance = double.Parse(tb_chance.Text) };
				if (Database.Instance.CompanyTypes.First(x => x == ct).Types.FirstOrDefault(x => x.Software == newtype.Software) != null)
				{
					MessageBox.Show("You can't add a Software that already exists!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				Database.Instance.CompanyTypes.First(x => x == ct).Types = Database.Instance.CompanyTypes.First(x => x == ct).Types.Append(newtype).ToArray();
			}
			else
			{
				DataTypes.CompanyTypeTypes newtype = new DataTypes.CompanyTypeTypes() { Software = cb_software.Text, Chance = double.Parse(tb_chance.Text) };
				List<DataTypes.CompanyTypeTypes> typelist = new List<DataTypes.CompanyTypeTypes>();
				typelist.AddRange(Database.Instance.CompanyTypes.First(x => x == ct).Types);
				typelist[typelist.FindIndex(x => x == type)] = newtype;
				Database.Instance.CompanyTypes.First(x => x == ct).Types = typelist.ToArray();
			}

			DialogResult = true;
			Close();
		}

		private void LoadTypeInfo()
		{
			cb_software.SelectedItem = type.Software;
			tb_chance.Text = type.Chance.ToString();
		}

		private void LoadSoftwareTypes()
		{
			cb_software.Items.Clear();
			foreach(DataTypes.SoftwareType st in Database.Instance.SoftwareTypes)
			{
				cb_software.Items.Add(st.Name);
			}
		}
	}
}
