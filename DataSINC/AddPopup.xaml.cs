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
	/// Interaktionslogik für AddPopup.xaml
	/// </summary>
	public partial class AddPopup : Window
	{
		public enum PopupType
		{
			SoftwareType,
			CompanyType,
			NameGenerator,
			Personality
		}

		private PopupType ptype;

		public AddPopup(PopupType type)
		{
			InitializeComponent();
			ptype = type;
			switch(type)
			{
				case PopupType.CompanyType:
					l_title.Content = "Name the new Companytype";
					break;
				case PopupType.NameGenerator:
					l_title.Content = "Name the new Namegenerator";
					break;
				case PopupType.Personality:
					l_title.Content = "Name the new Personality";
					break;
				case PopupType.SoftwareType:
					l_title.Content = "Name the new SoftwareType";
					break;
				default:
					break;
			}

			bt_add.Click += Bt_add_Click;
		}

		private void Bt_add_Click(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(tb_name.Text))
			{
				//TODO: Show message for user
				return;
			}
			switch (ptype)
			{
				case PopupType.CompanyType:
					Database.Instance.CompanyTypes.Add(DataTypes.CompanyType.Create(tb_name.Text));
					DialogResult = true;
					Close();
					break;
				case PopupType.NameGenerator:
					Database.Instance.NameGenerators.Add(DataTypes.NameGenerator.Create(tb_name.Text));
					DialogResult = true;
					Close();
					break;
				case PopupType.Personality:
					Database.Instance.Personalities.Add(DataTypes.Personality.Create(tb_name.Text));
					DialogResult = true;
					Close();
					break;
				case PopupType.SoftwareType:
					Database.Instance.SoftwareTypes.Add(DataTypes.SoftwareType.Create(tb_name.Text));
					DialogResult = true;
					Close();
					break;
				default:
					//TODO: make error message if no popup type is selected
					break;
			}
		}
	}
}
