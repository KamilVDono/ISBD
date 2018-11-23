using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ISBD.Model;
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

			Connector.UnregisterForSelectedUserChange(UpdateUserView);
			Connector.RegisterForSelectedUserChange(UpdateUserView);

			Connector.UnregisterForAddingNewItem(AddNewItem);
			Connector.RegisterForAddingNewItem(AddNewItem);

			Connector.Categories = LoggedinLogicState.Categories.Select(cat => cat.Nazwa).ToList();
			Connector.ValidUsers = LoggedinLogicState.ValidUsers;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(LoggedinLogicState.CurrentSelectedUser);

			Connector.CanDelete = false;
			Connector.CanEdit = true;
			Connector.CanAdd = true;
		}

		private void AddNewItem(AddingNewItemEventArgs args)
		{
			args.NewItem = new TransakcjaModel()
			{
				Data = DateTime.Now,
				IdO = LoggedinLogicState.CurrentSelectedUser.IdO,
				IdK = LoggedinLogicState.Categories[0].IdK,
				Kwota = 0,
				Tytul = "Transakcja"
			};

			LoggedinLogicState.AddTransaction((TransakcjaModel)args.NewItem);
		}

		private void UpdateUserView(OsobaModel selectedUser)
		{
			LoggedinLogicState.CurrentSelectedUser = selectedUser;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(selectedUser);
			Connector.CanAdd = LoggedinLogicState.CanWriteToUser(selectedUser);
		}
	}
}
