using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.Utils;
using ISBD.View;

namespace ISBD.ModelView.State
{
	class StartupUIState : ConnectorState<IStartupUI, StartupPage>
	{
		private const int ContinueTicks = 10;
		private int TicksCount = 0;
		private DispatcherTimer DispatcherTimer;

		private OsobaModel LastUser
		{
			get
			{
				var (login, password) = Database.Database.Instance.GetLastLoginData();
				return LoginUIState.TryLogin(login, password);
			}
		}

		public override void StartState()
		{
			base.StartState();
			Connector.LogoVisible = true;
			Connector.NotMeButton.ClearClick();
			Connector.NotMeButton.Click += GoToLogin;
			StartDispatcher(LogoTick, 2);
		}

		private void GoToLogin(object sender, RoutedEventArgs e)
		{
			DispatcherTimer.Stop();
			Database.Database.Instance.SaveLastLogin("","");
			StateMachine.Instance.PushState<LoginUIState>(null);
		}

		private void LogoTick()
		{
			DispatcherTimer.Stop();
			Connector.LogoVisible = false;
			LogoTimeout();
		}

		private void LogoTimeout()
		{
			if (LastUser == null)
			{
				GoToLogin(null, null);
			}
			else
			{
				TicksCount = 0;
				Connector.ContinueButton.ClearClick();
				Connector.ContinueButton.Click += (a, b) => LogIn();
				Connector.ContinueButton.Content = $"Kontunuj ({ContinueTicks - TicksCount})";
				Connector.HelloMessage = $"Witaj {LastUser.Imie} {LastUser.Nazwisko} ({LastUser.Login})";
				StartDispatcher(ContinueTick, 1);
			}
		}

		private void ContinueTick()
		{
			TicksCount++;
			Connector.ContinueButton.Content = $"Kontunuj ({ContinueTicks - TicksCount})";
			if (TicksCount == ContinueTicks)
			{
				LogIn();
			}
		}

		private void LogIn()
		{
			DispatcherTimer.Stop();
			StateMachine.Instance.PushState<LoggedinLogicState>(new LoggedinStatePushParameters()
			{
				user = LastUser,
				saveLogged = true
			});
		}

		private void StartDispatcher(Action function, int interval)
		{
			DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			DispatcherTimer.Tick += (a, b) => function();
			DispatcherTimer.Interval = new TimeSpan(0, 0, interval);
			DispatcherTimer.Start();
		}
	}
}
