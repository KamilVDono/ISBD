using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model.Tests;
using ISBD.ModelView.State;
using ISBD.Utils;

namespace ISBD.ModelView
{
	public class StateMachine: Singleton<StateMachine>
	{
		private List<Action> StartupActions = new List<Action>()
		{
			() => {new QueryTest();},
		};
		private List<Type> StateTypes = new List<Type>()
		{

		};

		private Dictionary<Type, State.State> StateInstances = new Dictionary<Type, State.State>();
		private Stack<UIState> UIStatesStack = new Stack<UIState>();
		private Stack<LogicState> LogicStatesStack = new Stack<LogicState>();

		public StateMachine()
		{
			StartupActions.ForEach(action => action());
			InitDictionary();
		}

		public void InitStateMachine<T>() where T : LogicState
		{
			PushState<T>(null);
		}

		public void PushState<T>(StatePushParameters parameters) where T : State.State
		{
			GetStateInstance<T>(out var stateToPush);
			if (stateToPush == null)
			{
				return;
			}
			stateToPush.Push(parameters);

			if (stateToPush is UIState)
			{
				PushUIState(stateToPush as UIState);
			}
			else
			{
				PushLogicState(stateToPush as LogicState);
			}
		}

		public bool PopUIUntil<T>() where T : UIState
		{
			GetStateInstance<T>(out var state);
			if (state == null)
			{
				return false;
			}

			return PopUIUntil(state);
		}

		public bool PopUIUntil(UIState state)
		{
			if (UIStatesStack.Contains(state))
			{
				while (UIStatesStack.Peek() != state)
				{
					UIStatesStack.Pop().StopState();
				}
				return true;
			}

			return false;
		}

		public void PopUIState()
		{
			if (UIStatesStack.Count > 1)
			{
				UIStatesStack.Pop().StopState();
			}
			System.Console.Error.WriteLine("There is only one state in stack, can not pop this");
		}

		public UIState CurrentUIState()
		{
			return UIStatesStack.Peek();
		}

		public LogicState CurrentLogicState()
		{
			return LogicStatesStack.Count > 0 ? LogicStatesStack.Peek() : null;
		}

		public bool IsUIStateInStack<T>() where T : UIState
		{
			GetStateInstance<T>(out var state);
			return state != null && UIStatesStack.Contains(state);
		}

		public T GetStateInstance<T>() where T : State.State
		{
			if (StateInstances.TryGetValue(typeof(T), out var state) == false)
			{
				System.Console.Error.WriteLine($"There is no {typeof(T).FullName} state in states dictionary, please fix it!");
				return null;
			}

			return (T)state;
		}

		public void GetStateInstance<T>(out T state) where T : State.State
		{
			state = GetStateInstance<T>();
		}

		private void InitDictionary()
		{
			StateTypes.Clear();
			StateTypes = (new HashSet<Type>(StateTypes)).ToList();
			StateTypes.ForEach(stateType =>
			{
				if (stateType.IsSubclassOf(typeof(State.State)))
				{
					var state = (State.State)Activator.CreateInstance(stateType);
					StateInstances.Add(stateType, state);
				}
			} );
		}

		private void PushUIState(UIState state)
		{
			if (PopUIUntil(state))
			{
				state.ResumeState();
			}
			else
			{
				UIStatesStack.Peek().PauseState();
				UIStatesStack.Push(state);
				state.StartState();
			}
		}

		private void PushLogicState(LogicState state)
		{
			if (LogicStatesStack.Count > 0)
			{
				LogicStatesStack.Peek().StopState();
			}
			LogicStatesStack.Push(state);
			state.StartState();
		}

	}
}
