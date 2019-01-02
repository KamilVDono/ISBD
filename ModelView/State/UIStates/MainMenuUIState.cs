using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ISBD.Model;
using ISBD.ModelView.State.LogicStates;
using ISBD.Utils;
using ISBD.View.Pages;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ISBD.ModelView.State.UIStates
{
	class MainMenuUIState : ConnectorState<IMainMenu, MainMenuPage>
	{
		private LoggedinLogicState LoggedinLogicState => StateMachine.Instance.GetStateInstance<LoggedinLogicState>();

		private DateTime CurrentDate;

		private ChartParams CharParamsCache;

		private string[] MonthNames =
		{
			"Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec",
			"Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień"
		};

		private ChartType[] ChartTypes =
		{
			new ChartType("Liniowy", typeof(LineSeries), 4),
			new ChartType("Kolumnowy", typeof(ColumnSeries), 1),
			new ChartType("Sterta powierzchniowa", typeof(StackedAreaSeries), 4),
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

			UpdateCategories();
			Connector.ValidUsers = LoggedinLogicState.ValidUsers;

			var categoriesCollection = new ObservableCategoriesCollection(LoggedinLogicState.Categories);
			categoriesCollection.CollectionChanged += CategoriesCollectionChanged;
			Connector.Categories = categoriesCollection;

			var symbolsCollection = new ObservableSymbolsCollection(LoggedinLogicState.Symbols);
			symbolsCollection.CollectionChanged += SymbolsCollectionChanged;
			Connector.Symbols = symbolsCollection;

			UpdateUserView();

			Connector.ChartParams = GetChartParams();

			SetMonth();

			Connector.CanDelete = false;
			Connector.CanEdit = true;
		}

		private void SetMonth()
		{
			var (month, income, expense) = GetCurrentMonthData();
			Connector.SetMonthSummary(month, income, expense);

			Connector.SetMonthList(LoggedinLogicState.GetMonthCategoriesSummary(CurrentDate));
			UpdateUserView();
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

		private void UpdateUserView(OsobaModel selectedUser)
		{
			LoggedinLogicState.CurrentSelectedUser = selectedUser;
			SetMonth();
		}

		private void UpdateUserView()
		{
			var transactions = new ObservableTransactionsCollection(LoggedinLogicState.GetUserTransactions(LoggedinLogicState.CurrentSelectedUser).
				Where(t => t.Data.Month == CurrentDate.Month && t.Data.Year == CurrentDate.Year).ToList());
			transactions.CollectionChanged += TransactionsChanged;
			Connector.Transactions = transactions;
			Connector.CanAdd = LoggedinLogicState.CanWriteToUser(LoggedinLogicState.CurrentSelectedUser);
		}

		private void UpdateCategories()
		{
			Connector.CategoriesNames = LoggedinLogicState.Categories.Select(cat => cat.Nazwa).ToList();
		}

		private void TransactionsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (object newItem in e.NewItems)
				{
					var newTransaction = (TransakcjaModel) newItem;
					newTransaction.RaiseEvents = false;
					newTransaction.Data = DateTime.Now;
					newTransaction.IdO = LoggedinLogicState.CurrentSelectedUser.IdO;
					newTransaction.IdK = LoggedinLogicState.Categories[0].IdK;
					newTransaction.Kwota = 0;
					newTransaction.Tytul = "Transakcja";
					newTransaction.RaiseEvents = true;
					LoggedinLogicState.InsertToDB(newTransaction);
				}
				SetMonth();
				UpdateChart();
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (object newItem in e.NewItems)
				{
					var newTransaction = (TransakcjaModel)newItem;
					LoggedinLogicState.UpdateDBEntry(newTransaction);
				}
				SetMonth();
				UpdateChart();
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void CategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (object newItem in e.NewItems)
				{
					var newCategory = (KategoriaModel)newItem;
					newCategory.RaiseEvents = false;
					newCategory.Nazwa = "Kategoria";
					newCategory.Rodzaj = -1;
					newCategory.IdS = LoggedinLogicState.Symbols[0].IdS;
					newCategory.RaiseEvents = true;
					LoggedinLogicState.InsertToDB(newCategory);
				}
				SetMonth();
				UpdateCategories();
				Connector.ChartParams = GetChartParams(true);
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (object newItem in e.NewItems)
				{
					var newCategory = (KategoriaModel)newItem;
					LoggedinLogicState.UpdateDBEntry(newCategory);
				}
				SetMonth();
				UpdateCategories();
				Connector.ChartParams = GetChartParams(true);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void SymbolsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (object newItem in e.NewItems)
				{
					var newSymbol = (SymbolModel)newItem;
					newSymbol.Kolor = Color.FromRgb(255, 215, 64);
					newSymbol.Ikona = "";
					newSymbol.RaiseEvents = true;
					LoggedinLogicState.InsertToDB(newSymbol);
				}
				SetMonth();
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (object newItem in e.NewItems)
				{
					var newSymbol = (SymbolModel)newItem;
					LoggedinLogicState.UpdateDBEntry(newSymbol);
				}
				SetMonth();
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private ChartParams GetChartParams(bool force = false)
		{
			if (CharParamsCache != null && force == false)
			{
				return CharParamsCache;
			}

			CharParamsCache = new ChartParams();
			CharParamsCache.ChartTypes = new ObservableCollection<ChartType>(ChartTypes);
			CharParamsCache.CategoriesTree = GetMainTreeCategoryData();
			CharParamsCache.UsersTree = GetMainUsersTreeDatas();
			CharParamsCache.FromDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			CharParamsCache.ToDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 31);
			CharParamsCache.SeriesCollection = new SeriesCollection();
			CharParamsCache.Labels = new ObservableCollection<string>();

			CharParamsCache.OnChartChange -= UpdateChart;
			CharParamsCache.OnChartChange += UpdateChart;

			UpdateChart();

			return CharParamsCache;
		}

		private ObservableCollection<TreeData> GetMainTreeCategoryData()
		{
			var treeData = GetTreeCategoryData();

			MainTreeCategoryData income = new MainTreeCategoryData()
			{
				Name = "Przychody",
				Children = new ObservableCollection<TreeData>(),
				OnSelectionChange = TreeSelectionChange
			};

			MainTreeCategoryData expenses = new MainTreeCategoryData()
			{
				Name = "Wydatki",
				Children = new ObservableCollection<TreeData>(),
				OnSelectionChange = TreeSelectionChange
			};

			foreach (var categoryTreeData in treeData)
			{
				if (((CategoryTreeData)categoryTreeData).Category.Rodzaj < 0)
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

			return new ObservableCollection<TreeData>(){ expenses, income };
		}

		private ObservableCollection<TreeData> GetTreeCategoryData()
		{
			var categories = LoggedinLogicState.Categories;
			var categoriesSet = new HashSet<KategoriaModel>(categories);

			List<CategoryTreeData> treeData = categories.Where(cat => cat.IdKRodzic.HasValue == false).Select(cat =>
				new CategoryTreeData()
				{
					Category = cat,
					Children = new ObservableCollection<TreeData>(),
					Parent = null,
					OnSelectionChange = TreeSelectionChange
				}).ToList();
			treeData.ForEach(data => categoriesSet.Remove(data.Category));
			treeData.ForEach(data => AssignChildren(data, categoriesSet));

			return new ObservableCollection<TreeData>(treeData);
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
				Children = new ObservableCollection<TreeData>(),
				OnSelectionChange = TreeSelectionChange
			});

			children.ToList().ForEach(dataa =>
			{
				AssignChildren(dataa, categories);
				data.Children.Add(dataa);
			});
		}

		private ObservableCollection<TreeData> GetMainUsersTreeDatas()
		{
			MainUserTreeData allUser = new MainUserTreeData()
			{
				Children = new ObservableCollection<TreeData>(),
				OnSelectionChange = TreeSelectionChange,
			};

			LoggedinLogicState.ValidUsers.ForEach(user =>
			{
				UserTreeData data = new UserTreeData()
				{
					Children = new ObservableCollection<TreeData>(),
					OnSelectionChange = TreeSelectionChange,
					Parent = allUser,
					User = user,
				};
				allUser.Children.Add(data);
			});

			return new ObservableCollection<TreeData>() { allUser };
		}

		private void TreeSelectionChange(TreeData data)
		{
			if (data.Children == null) return;
			foreach (var categoryTreeData in data.Children)
			{
				categoryTreeData.Selected = data.Selected;
			}

			if (data.Parent != null && data.Parent.Selected != data.Selected && data.Parent.Children.All(child => child.Selected == data.Selected))
			{
				data.Parent.Selected = data.Selected;
			}

			CharParamsCache.OnChartChange?.Invoke();
		}

		private void UpdateChart()
		{
			var validUser = CharParamsCache.UsersTree[0].Children.Where(u => u.Selected).Select(u => ((UserTreeData)u).User);
			var validCategories = ObtainCategories(CharParamsCache.CategoriesTree);
			Database.Database.Instance.Connect();

			var allSymbols = Database.Database.Instance.SelectAll<SymbolModel>();

			int days = ChartTypes[CharParamsCache.SelectedType].MaxXPoints();
			int daysSpan = (int) (CharParamsCache.ToDateTime - CharParamsCache.FromDateTime).TotalDays;
			if (daysSpan < days)
			{
				days = daysSpan;
			}
			int daysStep = (int)(CharParamsCache.ToDateTime - CharParamsCache.FromDateTime).TotalDays / days;
			DateTime startDay = new DateTime(CharParamsCache.FromDateTime.Year, CharParamsCache.FromDateTime.Month,
				CharParamsCache.FromDateTime.Day);

			List<List<TransakcjaModel>> validTransactions = new List<List<TransakcjaModel>>(days);

			for (int i = 0; i < days; i++)
			{
				validTransactions.Add(new List<TransakcjaModel>());
				validTransactions[i].AddRange(Database.Database.Instance.SelectAll<TransakcjaModel>().
					Where(trans => startDay <= trans.Data && (trans.Data - startDay).TotalDays < daysStep).
					Where(trans => validUser.Any(user => user.IdO == trans.IdO)));
				startDay = startDay.AddDays(daysStep);
			}

			Database.Database.Instance.Dispose();

			CharParamsCache.SeriesCollection.Clear();
			CharParamsCache.Labels.Clear();

			validCategories.ForEach(category =>
			{
				startDay = new DateTime(CharParamsCache.FromDateTime.Year, CharParamsCache.FromDateTime.Month,
					CharParamsCache.FromDateTime.Day);

				var column = ChartTypes[CharParamsCache.SelectedType].GetChartSeries();
				column.Title = category.Nazwa;
				column.Values = new ChartValues<DateTimePoint>();
				column.Stroke = new SolidColorBrush(allSymbols.First(s => s.IdS == category.IdS).Kolor);
				if (column.GetType() == typeof(StackedAreaSeries))
				{
					((StackedAreaSeries)column).LineSmoothness = 0;
				}
				else if (column.GetType() == typeof(LineSeries))
				{
					((LineSeries)column).LineSmoothness = 0;
				}
				if (column.GetType() == typeof(ColumnSeries))
				{
					column.Fill = new SolidColorBrush(allSymbols.First(s => s.IdS == category.IdS).Kolor);
				}
				column.DataLabels = true;
				column.LabelPoint = (point) => $"{point.Y:F}zł {category.Nazwa}";

				for (int i = 0; i < days; i++)
				{
					double sum = validTransactions[i].Where(trans => trans.IdK == category.IdK).Sum(t => t.Kwota);
					column.Values.Add(new DateTimePoint(startDay, sum));
					startDay = startDay.AddDays(daysStep);
				}


				CharParamsCache.SeriesCollection.Add(column);
				CharParamsCache.Labels.Add(category.Nazwa);
			});
		}

		private static List<KategoriaModel> ObtainCategories(ObservableCollection<TreeData> categoriesTree)
		{
			List<KategoriaModel> categories = new List<KategoriaModel>();
			categoriesTree.ToList().ForEach(cat =>
			{
				if (!cat.Selected) return;
				if (cat is CategoryTreeData data)
				{
					categories.Add(data.Category);
				}
				categories.AddRange(ObtainCategories(cat.Children));
			});
			return categories;
		}
	}

	public class ChartParams
	{
		public Action OnChartChange;

		public ObservableCollection<TreeData> CategoriesTree { get;  set; }

		public ObservableCollection<TreeData> UsersTree { get; set; }

		public DateTime FromDateTime { get; set; }

		public DateTime ToDateTime { get; set; }

		public ObservableCollection<ChartType> ChartTypes { get; set; }
		public int SelectedType { get; set; } = 0;

		public SeriesCollection SeriesCollection { get; set; }
		public ObservableCollection<string> Labels { get; set; }

		public void OnDataChange(object sender, EventArgs e)
		{
			OnChartChange?.Invoke();
		}
	}

	public class ChartType
	{
		public string Name { get; set; }
		public Type Type { get; set; }

		private int MaxPoints;

		public ChartType(string name, Type type, int maxPoints)
		{
			Name = name;
			Type = type;
			MaxPoints = maxPoints;
		}

		public Series GetChartSeries()
		{
			return (Series) Activator.CreateInstance(Type);
		}

		public int MaxXPoints()
		{
			return MaxPoints;
		}

		public bool Fill()
		{
			return false;
		}
	}
}
