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
using ISBD.ModelView.State;

namespace ISBD.View
{
	/// <summary>
	/// Interaction logic for Page1.xaml
	/// </summary>
	public partial class StartupPage : Page, IStartupUI
	{
		public StartupPage()
		{
			InitializeComponent();
		}

		public void SetText(string text)
		{
			Txt.Text = text;
		}
	}
}
