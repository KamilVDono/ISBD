using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ISBD.ModelView.State
{
	public class StatePushParameters
	{

	}

	public abstract class State
	{
		public abstract void Push(StatePushParameters pushParameters);
		public abstract void StartState();
		public abstract void StopState();
	}

	public abstract class LogicState : State
	{

	}

	public class UIPushParameters: StatePushParameters
	{
		public Type WindowType;
	}

	public abstract class UIState : State
	{
		protected Window UIWindow;
		protected UIPushParameters Parameters;

		public override void Push(StatePushParameters pushParameters)
		{
			Parameters = pushParameters as UIPushParameters;
			CreateWindow();
			PrepareWindow();

		}

		public override void StartState()
		{
			ShowWindow();
		}

		public abstract void PauseState();
		public abstract void ResumeState();

		protected void CreateWindow()
		{
			if (Parameters != null)
			{
				UIWindow = (Window)Activator.CreateInstance(Parameters.WindowType);
			}
			else
			{
				CustomCreateWindow();
			}
		}

		protected abstract void CustomCreateWindow();

		protected abstract void PrepareWindow();

		protected virtual void ShowWindow()
		{
			UIWindow.Show();
		}
	}
}
