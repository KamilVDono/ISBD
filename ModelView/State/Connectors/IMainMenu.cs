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

		void   RegisterForAddingNewItem(Action<AddingNewItemEventArgs> newItemAction);

		void UnregisterForAddingNewItem(Action<AddingNewItemEventArgs> newItemAction);

		void   RegisterForDeleteRows(Action<HashSet<TransakcjaModel>> deleteRowsAction);

		void UnregisterForDeleteRows(Action<HashSet<TransakcjaModel>> deleteRowsAction);

		List<TransakcjaModel> Transactions { set; }

		List<OsobaModel> ValidUsers { set; }

		List<string> Categories { set; }

		bool CanAdd { set; }

		bool CanDelete { set; }

		bool CanEdit { set; }

		Button PreviousMonthButton { get; }

		Button NextMonthButton { get; }

		void SetMonthSummary(string monthName, double income, double expense);

		void SetMonthList(List<CategorySummary> categories);

		List<string> ChartTypes { set; }

		ObservableCollection<MainTreeCategoryData> CategoriesTree { set; }
	}
}