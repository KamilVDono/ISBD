using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.ModelView.State
{
	class StartupLogicState : LogicState
	{
		public override void Push(StatePushParameters pushParameters)
		{
			
		}

		public override void StartState()
		{
			StateMachine.Instance.PushState<StartupUIState>(null);
		}

		public override void StopState()
		{
			
		}
	}
}
