using ISBD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ISBD.ModelView.State.Connectors
{
    public interface IAdminUI
    {
        DataGrid data{ get; }
        ComboBox users { get; }
        OsobaModel currentChosen { get; }
        Button saveData { get; }
        ToggleButton isAdmin { get; }
        Button buttonBack { get; }
    }
}
