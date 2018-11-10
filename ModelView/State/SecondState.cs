using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Utils;
using ISBD.View;

namespace ISBD.ModelView.State
{
	class SecondState : ConnectorState<ISecondPage>
	{
		protected override Type DefaultType => typeof(SecondPage);

		public override void StartState()
		{
			base.StartState();
			Connector.ExitButton.ClearClick();
			Connector.ExitButton.Click += (a,b) => System.Windows.Application.Current.Shutdown();
		}
	}
}
