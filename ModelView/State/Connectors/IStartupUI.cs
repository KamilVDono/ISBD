using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ISBD.ModelView.State
{
	interface IStartupUI
	{
		bool LogoVisible { set; }

		string HelloMessage { set; }
		Button ContinueButton { get; }
		Button NotMeButton { get; }
	}
}
