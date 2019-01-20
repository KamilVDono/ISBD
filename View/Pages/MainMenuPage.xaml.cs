using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ISBD.Model;
using ISBD.ModelView;
using ISBD.ModelView.State;
using ISBD.ModelView.State.LogicStates;
using ISBD.ModelView.State.UIStates;
using ISBD.Utils;
using LiveCharts;
using LiveCharts.Wpf;

namespace ISBD.View.Pages
{
	/// <summary>
	/// Interaction logic for MainPage.xaml
	/// </summary>
	public partial class MainMenuPage : Page, IMainMenu
	{
		private Action<OsobaModel> OnSelectionChange;

		public MainMenuPage()
		{
			InitializeComponent();

			this.Language = XmlLanguage.GetLanguage("pl-PL");

			KindsCategoriesDataGrid.ItemsSource = new[]{ "Wydatek", "Przychód"};

			UsersViewsChooser.SelectionChanged += (sender, e) =>
			{
				OnSelectionChange?.Invoke((OsobaModel)UsersViewsChooser.SelectedItem);
			};

			AddTransaction.Click += (_, _2) =>
			{
				((ObservableTransactionsCollection)HistoryTable.ItemsSource).Add(new TransakcjaModel());
			};

			AddCategoryButton.Click += (_, _2) =>
			{
				((ObservableCategoriesCollection) CategoriesDataGrid.ItemsSource).Add(new KategoriaModel());
			};

			AddSymbolButton.Click += (_, _2) =>
			{
				((ObservableSymbolsCollection) SymbolsDataGrid.ItemsSource).Add(new SymbolModel());
			};

			HistoryTable.CanUserAddRows = false;
			CategoriesDataGrid.CanUserAddRows = false;
			SymbolsDataGrid.CanUserAddRows = false;
		}

		#region Callbacks

		public void RegisterForSelectedUserChange(Action<OsobaModel> selectionAction)
		{
			OnSelectionChange += selectionAction;
		}

		public void UnregisterForSelectedUserChange(Action<OsobaModel> selectionAction)
		{
			OnSelectionChange -= selectionAction;
		}

		#endregion Callbacks

		#region Setters

		public ObservableTransactionsCollection Transactions
		{
			set => HistoryTable.ItemsSource = value;
		}

		public ObservableCategoriesCollection Categories
		{
			set => CategoriesDataGrid.ItemsSource = value;
		}

		public ObservableSymbolsCollection Symbols
		{
			set => SymbolsDataGrid.ItemsSource = value;
		}

		public List<OsobaModel> ValidUsers
		{
			set
			{
				UsersViewsChooser.ItemsSource = value;
				UsersViewsChooser.SelectedIndex = 0;
			}
		}

		public List<string> CategoriesNames
		{
			set
			{
				DataGridComboBoxColumn.ItemsSource = value;
				value.Add("Brak");
				CategoriesCategoriesDataGrid.ItemsSource = value;
			}
		}

		public bool CanAdd { set => AddTransaction.IsEnabled = value; }

		public bool CanDelete { set => HistoryTable.CanUserDeleteRows = value; }

		public bool CanEdit { set => HistoryTable.IsReadOnly = !value; }

		public ChartParams ChartParams
		{
			set
			{
				ChartUserTreeView.ItemsSource = value.UsersTree;
				CategoriesSelectListView.ItemsSource = value.CategoriesTree;

				FromChartDate.SelectedDate = value.FromDateTime;
				FromChartDate.SelectedDateChanged += (_, _2)=>
				{
					value.FromDateTime = FromChartDate.SelectedDate.Value;
					value.OnDataChange(null,null);
				};

				ToChartDate.SelectedDate = value.ToDateTime;
				ToChartDate.SelectedDateChanged += (_,_2)=>
				{
					value.ToDateTime = ToChartDate.SelectedDate.Value;
					value.OnDataChange(null, null);
				};

				StatsChartType.ItemsSource = value.ChartTypes;
				StatsChartType.SelectedIndex = value.SelectedType;
				StatsChartType.SelectionChanged += (_,_2) =>
				{
					value.SelectedType = StatsChartType.SelectedIndex;
					value.OnDataChange(null,null);
				};
				
				MainChart.Series = value.SeriesCollection;
				AxisX.LabelFormatter = val => new DateTime((long)val).ToString("dd.MM.yyyy");
				AxisY.LabelFormatter = val => $"{val:F} PLN";
			}
		}

		#endregion Setters

		#region Getters

		public Button PreviousMonthButton => PreviousMonth;

		public Button NextMonthButton => NextMonth;

        public Button SettingsButton => Settings;

        #endregion Getters

        public void SetMonthSummary(string monthName, double income, double expense)
		{
			MonthName.Text = monthName;
			IncomeText.Text = string.Format(CultureInfo.CurrentUICulture, "{0:C2}", income);
			ExpenseText.Text = string.Format(CultureInfo.CurrentUICulture, "{0:C2}", expense);
			BalanceText.Text = string.Format(CultureInfo.CurrentUICulture, "{0:C2}", income - expense);
			if (expense == 0)
			{
				IncomesSlider.Offset = 1;
				ExpensesSlider.Offset = 1;
			}
			else if (income == 0)
			{
				IncomesSlider.Offset = 0;
				ExpensesSlider.Offset = 0;
			}
			else
			{
				double val = expense / (income);
				if (val > 0.999f)
				{
					IncomesSlider.Offset = 0.01f;
					ExpensesSlider.Offset = 0.02f;
				}
				else
				{
					double incomeVal = Math.Max(0, Math.Min(1, val - 0.01));
					double expenseVal = Math.Max(0, Math.Min(1, val + 0.01));
					IncomesSlider.Offset = incomeVal;
					ExpensesSlider.Offset = expenseVal;
				}
			}
		}

		public void SetMonthList(List<CategorySummary> categories)
		{
			MonthList.ItemsSource = categories;
		}

		private void DriversDataGrid_PreviewDeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Command == DataGrid.DeleteCommand)
			{
				if (MessageBox.Show("Na pewno chcesz usunąć daną transakcję?", "Potwierdz!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					e.Handled = true;
				}
			}
		}

		private void SelectedTab(object sender, SelectionChangedEventArgs e)
		{
			TabItem tab = (TabItem) ((Dragablz.TabablzControl) sender)?.SelectedItem;
			MainText.Text = (tab?.Header as TextBlock)?.Text ?? "Główne menu";
		}

		private void SymbolDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			if (((string) e.Column.Header).Equals("Ikona"))
			{
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



				// Set filter for file extension and default file extension 
				dlg.DefaultExt = ".png";
				dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


				// Display OpenFileDialog by calling ShowDialog method 
				bool? result = dlg.ShowDialog();


				// Get the selected file name and display in a TextBox 
				if (result == true)
				{
					// Open document 
					string filename = dlg.FileName;
					((SymbolModel)e.Row.Item).Ikona = filename;
				}

				e.EditingEventArgs.Handled = true;
			}
			else
			{
				e.EditingEventArgs.Handled = false;
			}
		}
	}

	#region Converters

	public class OsobaModel2NameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var user = (OsobaModel)value;

			return $"{user.Imie} {user.Nazwisko} ({user.Login})";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class Double2Currency : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return string.Format(CultureInfo.CurrentUICulture, "{0:C2}", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string input = (string)value;
			if (string.IsNullOrWhiteSpace(input)) return false;
			const string Numbers = "0123456789.,";
			var numberBuilder = new StringBuilder();
			foreach (char c in input)
			{
				if (Numbers.IndexOf(c) > -1)
					numberBuilder.Append(c);
			}
			return double.Parse(numberBuilder.ToString().Replace(',', '.'));
		}
	}

	public class IdK2Category : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return "Brak";
			}

			Database.Database.Instance.Connect();

			long idK = (long)value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);

			Database.Database.Instance.Dispose();

			return $"{category?.Nazwa}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((string)value == "Brak")
			{
				return null;
			}

			Database.Database.Instance.Connect();

			string categoryName = (string)value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.Nazwa == categoryName);

			Database.Database.Instance.Dispose();
			return category.IdK;
		}
	}

	public class IdK2Icon : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Database.Database.Instance.Connect();

			long idK = (long)value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);
			var symbol = Database.Database.Instance.SelectAll<SymbolModel>().FirstOrDefault(sym => sym.IdS == category.IdS);

			Database.Database.Instance.Dispose();

			return symbol.Ikona;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class Transaction2Color : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var category = value as KategoriaModel;
			if (category == null)
			{
				if (value is TransakcjaModel == false) return new SolidColorBrush(Colors.WhiteSmoke);
				Database.Database.Instance.Connect();

				TransakcjaModel tr = (TransakcjaModel)value;
				long idK = tr.IdK;
				category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);
				Database.Database.Instance.Dispose();
				if (category == null)
				{
					return new SolidColorBrush(Colors.WhiteSmoke);
				}
			}
			Database.Database.Instance.Connect();
			var symbol = Database.Database.Instance.SelectAll<SymbolModel>().FirstOrDefault(s => s.IdS == category.IdS);

			Database.Database.Instance.Dispose();

			var color = new SolidColorBrush(symbol?.Kolor ?? Colors.WhiteSmoke);

			return color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class Transaction2ColorForeground : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var category = value as KategoriaModel;
			if (category == null)
			{
				if (value is TransakcjaModel == false) return new SolidColorBrush(Colors.Black);
				Database.Database.Instance.Connect();

				TransakcjaModel tr = (TransakcjaModel) value;
				long idK = tr.IdK;
				category = Database.Database.Instance.SelectAll<KategoriaModel>()
					.FirstOrDefault(cat => cat.IdK == idK);
				Database.Database.Instance.Dispose();
				if (category == null)
				{
					return new SolidColorBrush(Colors.Black);
				}
			}
			Database.Database.Instance.Connect();
			var symbol = Database.Database.Instance.SelectAll<SymbolModel>().FirstOrDefault(s => s.IdS == category.IdS);
			Database.Database.Instance.Dispose();

			if (symbol != null)
			{
				return new SolidColorBrush(symbol.Kolor.SaturatedValue() > 0.5f ? Colors.Black : Colors.White);
			}

			return new SolidColorBrush(Colors.Black);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RodzajIncome2Visibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Database.Database.Instance.Connect();

			long idK = (long)value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);

			Database.Database.Instance.Dispose();

			return (category?.Rodzaj ?? 1) == 1 ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RodzajExpense2Visibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Database.Database.Instance.Connect();

			long idK = (long)value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);

			Database.Database.Instance.Dispose();
			return (category?.Rodzaj ?? 1) == -1 ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class Rodzaj2Category : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (int) value < 0 ? "Wydatek" : "Przychód";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((string)value).Equals("Wydatek") ? -1 : 1;
		}
	}

	#endregion Converters
}
