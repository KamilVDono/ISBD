using System;
using System.Linq;
using System.Windows;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.ModelView.State.UIStates;
using ISBD.Utils;
using ISBD.View;

namespace ISBD.ModelView.State
{
	public class LoginUIState : ConnectorState<ILoginUI, LoginPage>
	{
		public override void StartState()
		{
			base.StartState();
			Connector.LoginButton.Click -= Login;
			Connector.LoginButton.Click += Login;

			Connector.RegisterButton.Click -= Register;
			Connector.RegisterButton.Click += Register;

			Connector.Logins = GetLogins();
		}

		public static OsobaModel TryLogin(string login, string password)
		{
			Database.Database.Instance.Connect();

			var users = Database.Database.Instance.SelectAll<OsobaModel>();
			var currentLogin =
				users.FirstOrDefault(user => user.Login == login && user.Haslo == password);

			Database.Database.Instance.Dispose();
			return currentLogin;
		}

		private void Register(object sender, RoutedEventArgs e)
		{
             StateMachine.Instance.PushState<RegisterUIState>(null);
        }

		private void Login(object sender, RoutedEventArgs e)
		{
			var currentLogin = TryLogin(Connector.Login, Connector.Password);

			if (currentLogin != null)
			{
				StateMachine.Instance.PushState<LoggedinLogicState>(new LoggedinStatePushParameters()
					{
						user = currentLogin,
						saveLogged = Connector.SaveCurrentUser
					});
			}
			else
			{
				Connector.Message = "Wrong login or password";
			}
		}

		private string[] GetLogins()
		{
			Database.Database.Instance.Connect();
			var logins = Database.Database.Instance.SelectAll<OsobaModel>().Select(person => person.Login);
			Database.Database.Instance.Dispose();
			return logins.ToArray();
		}
	}
}