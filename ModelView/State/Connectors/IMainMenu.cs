using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.ModelView.State.UIStates;

namespace ISBD.ModelView.State
{
	public interface IMainMenu
	{
		void RegisterForSelectedUserChange(Action<OsobaModel> selectionAction);

		void UnregisterForSelectedUserChange(Action<OsobaModel> selectionAction);

		ObservableTransactionsCollection Transactions { set; }

		ObservableCategoriesCollection Categories { set; }

		ObservableSymbolsCollection Symbols { set; }

		List<OsobaModel> ValidUsers { set; }

		List<string> CategoriesNames { set; }

		bool CanAdd { set; }

		bool CanDelete { set; }

		bool CanEdit { set; }

		ChartParams ChartParams { set; }

		Button PreviousMonthButton { get; }

		Button NextMonthButton { get; }

		void SetMonthSummary(string monthName, double income, double expense);

		void SetMonthList(List<CategorySummary> categories);
	}
}