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
	/// Interaction logic for LoginPage.xaml
	/// </summary>
	public partial class LoginPage : Page, ILoginUI
	{
		public LoginPage()
		{
			InitializeComponent();
			LoginBox.SelectionChanged += DataChanged;
			PasswordBox.PasswordChanged += DataChanged;
			SetActiveStateMessage(false);
		}

		public string[] Logins
		{
			set => LoginBox.ItemsSource = value;
		}

		public string Login => LoginBox.Text;

		public string Password => PasswordBox.Password;

		Button ILoginUI.RegisterButton => RegisterButton;

		Button ILoginUI.LoginButton => LoginButton;

		public string Message
		{
			get => MessageBlock.Text;
			set => MessageBlock.Text = value;
		}

		public bool SaveCurrentUser => RememberMeCheck.IsChecked.Value;

		private void DataChanged(object sender, RoutedEventArgs e)
		{
			SetActiveStateMessage(false);
		}

		public void SetActiveStateMessage(bool visible)
		{
			MessageBlock.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
