using System.Windows.Controls;

namespace ISBD.ModelView.State
{
	public interface ILoginUI
	{
		string Login { get; }
		string Password { get; }
		Button LoginButton { get; }
		Button RegisterButton { get; }
		string Message { get; set; }
		bool SaveCurrentUser { get; }
	}
}