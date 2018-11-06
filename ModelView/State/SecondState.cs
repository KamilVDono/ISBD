using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Utils;
using ISBD.View;

namespace ISBD.ModelView.State
{
	class SecondState : UIState
	{
		protected override Type DefaultType => typeof(SecondPage);
		private ISecondPage Target => (ISecondPage) UIPage;

		public override void StartState()
		{
			base.StartState();
			Target.ExitButton.ClearClick();
			Target.ExitButton.Click += (a,b) => System.Windows.Application.Current.Shutdown();
		}
	}
}
