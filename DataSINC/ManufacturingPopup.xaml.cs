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
	/// Interaktionslogik für ManufacturingPopup.xaml
	/// </summary>
	public partial class ManufacturingPopup : Window
	{
		public static List<DataTypes.ManufacturingComponents> Components;
		public static List<DataTypes.ManufacturingProcesses> Processes;
		public static DataTypes.Manufacturing Result;

		public ManufacturingPopup(DataTypes.SoftwareType st)
		{
			InitializeComponent();
			Result = null;
			if(st.Manufacturing == null)
			{
				Components = new List<DataTypes.ManufacturingComponents>();
				Processes = new List<DataTypes.ManufacturingProcesses>();
				tb_finaltime.Text = "0";
			}
			else
			{
				Components.AddRange(st.Manufacturing.Components);
				Processes.AddRange(st.Manufacturing.Processes);
				tb_finaltime.Text = st.Manufacturing.FinalTime.ToString();
			}
			cb_hardware.IsChecked = st.Hardware;
			foreach(DataTypes.ManufacturingComponents comp in Components)
			{
				lb_components.Items.Add(comp);
			}
			foreach(DataTypes.ManufacturingProcesses proc in Processes)
			{
				lb_processes.Items.Add(proc);
			}
			bt_add.Click += AddClicked;
		}

		private void AddClicked(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(tb_finaltime.Text) || !int.TryParse(tb_finaltime.Text, out _))
			{
				MessageBox.Show("The FinalTime must be a valid Integer value!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			Result = new DataTypes.Manufacturing()
			{
				Components = Components.ToArray(),
				Processes = Processes.ToArray(),
				FinalTime = int.Parse(tb_finaltime.Text)
			};
			if(cb_hardware.IsChecked != true)
			{
				Result = null;
			}
			DialogResult = true;
			Close();
		}
	}
}
