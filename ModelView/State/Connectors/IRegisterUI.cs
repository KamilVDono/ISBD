using System.Windows.Controls;

namespace ISBD.ModelView.State.Connectors
{
    public interface IRegisterUI
    {
        string Login { get; }

        void _Login(bool imageType, string message);

        string Password { get; }
        void _Password(bool imageType, string message);

        string RepeatPassword { get; }
        //UName == User name
        string UName { get; }
        void _UName(bool imageType, string message);
        string Surname { get; }

        void _Surname(bool imageType, string message);

        Button Next { get; }
        Button Back { get; }

    }
}
