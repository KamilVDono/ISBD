using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.View.Pages;

namespace ISBD.ModelView.State.UIStates
{
	class MainMenuUIState : ConnectorState<IMainMenu, MainMenuPage>
	{
		private OsobaModel CurrentSelectedUser;

		private LoggedinLogicState LoggedinLogicState => StateMachine.Instance.GetStateInstance<LoggedinLogicState>();

		public override void StartState()
		{
			base.StartState();

			Connector.UnregisterForSelectedUserChange(UpdateUserView);
			Connector.RegisterForSelectedUserChange(UpdateUserView);

			Connector.ValidUsers = LoggedinLogicState.ValidUsers;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(LoggedinLogicState.CurrentSelectedUser);
		}

		private void UpdateUserView(OsobaModel selectedUser)
		{
			LoggedinLogicState.CurrentSelectedUser = selectedUser;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(selectedUser);
		}
	}
}
