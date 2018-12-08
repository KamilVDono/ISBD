using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private string[] ChartTypes =
		{
			"Liniowy", "Kolumnowy", "Sterta powierzchniowa", "Sterta kolumnowa"
		};

		public override void StartState()
		{
			base.StartState();

			CurrentDate = DateTime.Today;

			SetupConnector();
		}

		private void SetupConnector()
		{
			Connector.NextMonthButton.Click -= NextMonth;
			Connector.NextMonthButton.Click += NextMonth;
			Connector.PreviousMonthButton.Click -= PreviousMonth;
			Connector.PreviousMonthButton.Click += PreviousMonth;

			Connector.UnregisterForSelectedUserChange(UpdateUserView);
			Connector.RegisterForSelectedUserChange(UpdateUserView);

			Connector.UnregisterForAddingNewItem(AddNewItem);
			Connector.RegisterForAddingNewItem(AddNewItem);

			Connector.Categories = LoggedinLogicState.Categories.Select(cat => cat.Nazwa).ToList();
			Connector.ValidUsers = LoggedinLogicState.ValidUsers;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(LoggedinLogicState.CurrentSelectedUser);
			Connector.ChartTypes = ChartTypes.ToList();

			Connector.CategoriesTree = GetMainTreeCategoryData();

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
			return ($"{MonthNames[CurrentDate.Month - 1]} {CurrentDate.Year}",
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

			LoggedinLogicState.AddTransaction((TransakcjaModel) args.NewItem);

			SetMonth();
		}

		private void UpdateUserView(OsobaModel selectedUser)
		{
			LoggedinLogicState.CurrentSelectedUser = selectedUser;
			Connector.Transactions = LoggedinLogicState.GetUserTransactions(selectedUser);
			Connector.CanAdd = LoggedinLogicState.CanWriteToUser(selectedUser);

			SetMonth();
		}

		private ObservableCollection<MainTreeCategoryData> GetMainTreeCategoryData()
		{
			var treeData = GetTreeCategoryData();

			MainTreeCategoryData income = new MainTreeCategoryData()
			{
				Name = "Przychody",
				Children = new ObservableCollection<CategoryTreeData>(),
				OnSelectionChange = TreeSelectionChange
			};

			MainTreeCategoryData expenses = new MainTreeCategoryData()
			{
				Name = "Wydatki",
				Children = new ObservableCollection<CategoryTreeData>(),
				OnSelectionChange = TreeSelectionChange
			};

			foreach (var categoryTreeData in treeData)
			{
				if (categoryTreeData.Category.Rodzaj < 0)
				{
					expenses.Children.Add(categoryTreeData);
					categoryTreeData.Parent = expenses;
				}
				else
				{
					income.Children.Add(categoryTreeData);
					categoryTreeData.Parent = income;
				}
			}

			return new ObservableCollection<MainTreeCategoryData>(){ expenses, income };
		}

		private ObservableCollection<CategoryTreeData> GetTreeCategoryData()
		{
			var categories = LoggedinLogicState.Categories;
			var categoriesSet = new HashSet<KategoriaModel>(categories);

			List<CategoryTreeData> treeData = categories.Where(cat => cat.IdKRodzic.HasValue == false).Select(cat =>
				new CategoryTreeData()
				{
					Category = cat,
					Children = new ObservableCollection<CategoryTreeData>(),
					Parent = null,
					OnSelectionChange = TreeSelectionChange
				}).ToList();
			treeData.ForEach(data => categoriesSet.Remove(data.Category));
			treeData.ForEach(data => AssignChildren(data, categoriesSet));

			return new ObservableCollection<CategoryTreeData>(treeData);
		}

		private void AssignChildren(CategoryTreeData data, HashSet<KategoriaModel> categories)
		{
			if (categories == null) return;
			var childrenRaw = categories.Where(cat => cat.IdKRodzic == data.Category.IdK).ToList();
			foreach (var model in childrenRaw)
			{
				categories.Remove(model);
			}

			var children = childrenRaw.Select(cat => new CategoryTreeData()
			{
				Category = cat,
				Parent = data,
				Children = new ObservableCollection<CategoryTreeData>(),
				OnSelectionChange = TreeSelectionChange
			});

			children.ToList().ForEach(dataa =>
			{
				AssignChildren(dataa, categories);
				data.Children.Add(dataa);
			});
		}

		private void TreeSelectionChange(TreeData data)
		{
			foreach (var categoryTreeData in data.Children)
			{
				categoryTreeData.Selected = data.Selected;
			}

			if (data.Parent != null && data.Parent.Selected != data.Selected && data.Parent.Children.All(child => child.Selected == data.Selected))
			{
				data.Parent.Selected = data.Selected;
			}
		}
	}
}
