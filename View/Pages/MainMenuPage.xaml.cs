using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ISBD.Model;
using ISBD.ModelView.State;
using ISBD.Utils;

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
			UsersViewsChooser.SelectionChanged += (sender, e) =>
			{
				OnSelectionChange?.Invoke((OsobaModel)UsersViewsChooser.SelectedItem);
			};
		}

		public void RegisterForSelectedUserChange(Action<OsobaModel> selectionAction)
		{
			OnSelectionChange += selectionAction;
		}

		public void UnregisterForSelectedUserChange(Action<OsobaModel> selectionAction)
		{
			OnSelectionChange -= selectionAction;
		}

		public List<TransakcjaModel> Transactions
		{
			set => HistoryTable.ItemsSource = value;
		}

		public List<OsobaModel> ValidUsers
		{
			set
			{
				UsersViewsChooser.ItemsSource = value;
				UsersViewsChooser.SelectedIndex = 0;
			}
		}
	}

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
			throw new NotImplementedException();
		}
	}

	public class IdK2Category : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Database.Database.Instance.Connect();

			long idK = (long) value;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);

			Database.Database.Instance.Dispose();

			return $"{category?.Nazwa}";
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
			if(value is TransakcjaModel == false) return new SolidColorBrush(Colors.WhiteSmoke);
			Database.Database.Instance.Connect();

			TransakcjaModel tr = (TransakcjaModel)value;
			long idK = tr.IdK;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);
			if (category == null)
			{
				Database.Database.Instance.Dispose();
				return new SolidColorBrush(Colors.WhiteSmoke);
			}
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
			if (value is TransakcjaModel == false) return new SolidColorBrush(Colors.Black);
			Database.Database.Instance.Connect();

			TransakcjaModel tr = (TransakcjaModel)value;
			long idK = tr.IdK;
			var category = Database.Database.Instance.SelectAll<KategoriaModel>().FirstOrDefault(cat => cat.IdK == idK);
			if (category == null)
			{
				Database.Database.Instance.Dispose();
				return new SolidColorBrush(Colors.Black);
			}
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
}
