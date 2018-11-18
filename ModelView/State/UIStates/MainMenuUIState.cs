using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.ModelView.State.LogicStates;
using ISBD.View.Pages;

namespace ISBD.ModelView.State.UIStates
{
	class MainMenuUIState : ConnectorState<IMainMenu, MainMenuPage>
	{
		private LoggedinLogicState LoggedinLogicState => StateMachine.Instance.GetStateInstance<LoggedinLogicState>();

		public override void StartState()
		{
			base.StartState();
			Connector.MessageText = $"Witaj {LoggedinLogicState.Name} {LoggedinLogicState.LastName}. Jesteś adminem {LoggedinLogicState.IsAdmin}";
		}
	}
}
