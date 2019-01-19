using ISBD.Model;
using ISBD.ModelView;
using ISBD.ModelView.State.Connectors;
using ISBD.ModelView.State.LogicStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISBD.View.Pages
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page , IAdminUI
    {
        public AdminPage()
        {
            InitializeComponent();

        }

        //private LoggedinLogicState LoggedinLogicState => StateMachine.Instance.GetStateInstance<LoggedinLogicState>();

       // private OsobaModel _CurrentUser => LoggedinLogicState.CurrentSelectedUser;

        public DataGrid data { get => dataGrid; }
        public ComboBox users { get => UsersComboBox;  }
        public OsobaModel currentChosen => (OsobaModel)users.SelectedItem;

        public Button saveData => saveButton;

        public ToggleButton isAdmin => gibAdmin;

        Button IAdminUI.buttonBack => buttonBack;
    }
}
