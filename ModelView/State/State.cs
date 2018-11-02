using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.ModelView.State
{
	public class StatePushParameters
	{

	}

	public abstract class State
	{
		public abstract void Push(StatePushParameters pushParameters);
		public abstract void StartState();
		public abstract void PauseState();
		public abstract void ResumeState();
		public abstract void StopState();
	}
}
