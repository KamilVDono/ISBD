using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.Utils;
using ISBD.View.Pages;

namespace ISBD.ModelView.State.UIStates
{
	class MainMenuUIState : ConnectorState<IMainMenu, MainMenuPage>
	{
		private LoggedinLogicState LoggedinLogicState => StateMachine.Instance.GetStateInstance<LoggedinLogicState>();
		private DateTime CurrentDate;

		private string[] MonthNames =
		{
			"Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec",
			"Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień"
		};

		public override void StartState()
		{
			base.StartState();

			CurrentDate = DateTime.Today;

			SetupConnector();
		}

		private void SetupConnector()
		{
			Connector.NextMonthButton.ClearClick();
			Connector.NextMonthButton.Click += NextMonth;
			Connector.PreviousMonthButton.ClearClick();
			Connector.PreviousMonthButton.Click += PreviousMonth;

			Connector.UnregisterForSelectedUserChange(UpdateUserView);
			Connector.RegisterForSelectedUserChange(UpdateUserView);

			Connector.UnregisterForAddingNewItem(AddNewItem);
			Connector.RegisterForAddingNewItem(AddNewItem);

			Connector.Categories = LoggedinLogicState.Categories.Select(cat => cat.Nazwa).ToList();
			Connector.ValidUsers = LoggedinLogicState.ValidUsers;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(LoggedinLogicState.CurrentSelectedUser);

			SetMonth();

			Connector.CanDelete = false;
			Connector.CanEdit = true;
			Connector.CanAdd = true;
		}

		private void SetMonth()
		{
			var (month, income, expense) = GetCurrentMonthData();
			Connector.SetMonthSummary(month, income, expense);

			Connector.SetMonthList(LoggedinLogicState.GetMonthCategoriesSummary(CurrentDate));
		}

		private (string, double, double) GetCurrentMonthData()
		{
			return ($"{MonthNames[CurrentDate.Month-1]} {CurrentDate.Year}",
					LoggedinLogicState.GetMonthIncomes(CurrentDate),
					LoggedinLogicState.GetMonthExpenses(CurrentDate)
					);
		}

		private void PreviousMonth(object sender, RoutedEventArgs e)
		{
			CurrentDate = CurrentDate.AddMonths(-1);
			SetMonth();
		}

		private void NextMonth(object sender, RoutedEventArgs e)
		{
			CurrentDate = CurrentDate.AddMonths(1);
			SetMonth();
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

			SetMonth();
		}

		private void UpdateUserView(OsobaModel selectedUser)
		{
			LoggedinLogicState.CurrentSelectedUser = selectedUser;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(selectedUser);
			Connector.CanAdd = LoggedinLogicState.CanWriteToUser(selectedUser);

			SetMonth();
		}
	}
}
