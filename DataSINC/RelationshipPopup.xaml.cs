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
	/// Interaktionslogik für RelationshipPopup.xaml
	/// </summary>
	public partial class RelationshipPopup : Window
	{
		private DataTypes.Personality personality;
		private bool isEdit;

		public RelationshipPopup(DataTypes.Personality personality, string relperso = null, double relvalue = 0)
		{
			InitializeComponent();
			this.personality = personality;
			FillPersonalities();
			if(relperso != null)
			{
				cb_personality.SelectedItem = relperso;
				cb_personality.IsEnabled = false;
				tb_relation.Text = relvalue.ToString();
				isEdit = true;
			}

			bt_add.Click += ClickAdd;
		}

		private void ClickAdd(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(tb_relation.Text))
			{
				MessageBox.Show("You need to enter a value as Relation!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			if (!double.TryParse(tb_relation.Text, out _))
			{
				MessageBox.Show("You need to enter a valid double value as Relation!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			int index = Database.Instance.Personalities.FindIndex(x => x == personality);

			if(Database.Instance.Personalities[index].Relationships.ContainsKey(cb_personality.Text) && isEdit)
			{
				Database.Instance.Personalities[index].Relationships[cb_personality.Text] = double.Parse(tb_relation.Text);
			}
			else if(Database.Instance.Personalities[index].Relationships.ContainsKey(cb_personality.Text) && !isEdit)
			{
				MessageBox.Show("You can't create a relation that already exist! Use \"Edit Relation\" to edit existing relations!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			else
			{
				Database.Instance.Personalities[index].Relationships.Add(cb_personality.Text, double.Parse(tb_relation.Text));
			}
			DialogResult = true;
			Close();
		}

		private void FillPersonalities()
		{
			cb_personality.Items.Clear();
			foreach(DataTypes.Personality perso in Database.Instance.Personalities)
			{
				if(perso != personality)
				{
					cb_personality.Items.Add(perso.Name);
				}
			}
			if(cb_personality.Items.Count < 1)
			{
				MessageBox.Show("You do not have any other personalities and can't create a relationship!","Warning!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				Close();
			}
			cb_personality.SelectedIndex = 0;
		}
	}
}
