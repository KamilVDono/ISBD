using System.Globalization;
using System.Threading;
using System.Windows;
using ISBD.ModelView;
using ISBD.ModelView.State;

namespace ISBD
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainWindow Instance;

		public MainWindow()
		{
			InitializeComponent();
			InitApplication();
		}

		void InitApplication()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			Instance = this;

			var stateMachine = StateMachine.Instance;
			stateMachine.InitStateMachine<StartupLogicState>();
		}
	}
}
