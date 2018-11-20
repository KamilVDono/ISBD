using System;
using System.Linq;
using System.Windows;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.Utils;
using ISBD.View;

namespace ISBD.ModelView.State
{
	public class LoginUIState : ConnectorState<ILoginUI, LoginPage>
	{
		public override void StartState()
		{
			base.StartState();
			Connector.LoginButton.ClearClick();
			Connector.LoginButton.Click += Login;

			Connector.RegisterButton.ClearClick();
			Connector.RegisterButton.Click += Register;

			Connector.Logins = GetLogins();
		}

		private void Register(object sender, RoutedEventArgs e)
		{
			//TODO: Implement registration
			throw new System.NotImplementedException("Implement Registration");
		}

		private void Login(object sender, RoutedEventArgs e)
		{
			Database.Database.Instance.Connect();

			var users = Database.Database.Instance.SelectAll<OsobaModel>();
			var currentLogin =
				users.FirstOrDefault(user => user.Login == Connector.Login && user.Haslo == Connector.Password);

			Database.Database.Instance.Dispose();

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
				//TODO: Implement wrong Login-Password set
				//throw new System.NotImplementedException("Implement wrong Login-Password set");
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