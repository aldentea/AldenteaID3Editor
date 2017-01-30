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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aldentea.ID3Editor.Otameshi
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var time = 16384;
			var address = System.Net.IPAddress.HostToNetworkOrder(time);
			var bytes = new System.Net.IPAddress(address).GetAddressBytes();
			System.Diagnostics.Debug.WriteLine(time);
			System.Diagnostics.Debug.WriteLine(address);
			System.Diagnostics.Debug.WriteLine(bytes.Length);
			System.Diagnostics.Debug.WriteLine(bytes[0]);
			System.Diagnostics.Debug.WriteLine(bytes[1]);
			System.Diagnostics.Debug.WriteLine(bytes[2]);
			System.Diagnostics.Debug.WriteLine(bytes[3]);
		}
	}
}
