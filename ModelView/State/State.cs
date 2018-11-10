using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ISBD.ModelView.State
{
	public class StatePushParameters
	{

	}

	public abstract class State
	{
		public abstract void Push(StatePushParameters pushParameters);
		public abstract void StartState();

		public virtual void StopState()
		{
		}
	}

	public abstract class LogicState : State
	{

	}

	public class UIPushParameters: StatePushParameters
	{
		public Type PageType;
		public bool CreateNewPage;
	}

	public abstract class UIState : State
	{
		protected Page UIPage;
		protected UIPushParameters Parameters;
		protected virtual Type DefaultType { get; }

		public UIState()
		{
			if (DefaultType != null)
			{
				UIPage = (Page)Activator.CreateInstance(DefaultType);
			}
			else
			{
				CustomCreatePage();
			}
		}

		public override void Push(StatePushParameters pushParameters)
		{
			Parameters = pushParameters as UIPushParameters;
			if(Parameters != null && Parameters.CreateNewPage) CreatePage();
			PreparePage();

		}

		public override void StartState()
		{
			ShowPage();
		}

		public virtual void PauseState() { }
		public virtual void ResumeState() { }

		protected void CreatePage()
		{
			if (Parameters != null)
			{
				UIPage = (Page)Activator.CreateInstance(Parameters.PageType);
			}
			else
			{
				CustomCreatePage();
			}
		}

		protected virtual void CustomCreatePage()
		{

		}

		protected virtual void PreparePage()
		{

		}

		protected virtual void ShowPage()
		{
			MainWindow.Instance.Frame.NavigationService.Navigate(UIPage);
		}
	}

	public abstract class ConnectorState<T> : UIState where T : class
	{
		protected T Connector => UIPage as T;
	}
}
