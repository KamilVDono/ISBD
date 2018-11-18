using System.Windows.Controls;
using ISBD.ModelView.State;

namespace ISBD.View.Pages
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class MainMenuPage : Page, IMainMenu
	{
		public string MessageText
		{
			get => Message.Text;
			set => Message.Text = value;
		}

		public MainMenuPage()
		{
			InitializeComponent();
		}
	}
}
