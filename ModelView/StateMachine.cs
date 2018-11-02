using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model.Tests;
using ISBD.Utils;

namespace ISBD.ModelView
{
	public class StateMachine: Singleton<StateMachine>
	{
		public List<Action> StartupActions = new List<Action>()
		{
			() => {new QueryTest();},

		};

		public StateMachine()
		{
			StartupActions.ForEach(action => action());
		}
	}
}
