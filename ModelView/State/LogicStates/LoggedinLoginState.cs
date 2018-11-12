using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;

namespace ISBD.ModelView.State.LogicStates
{
	class LoggedinStatePushParameters : StatePushParameters
	{
		public OsobaModel user;
		public bool saveLogged;
	}

	class LoggedinLoginState : LogicState
	{
		public bool IsAdmin => Permissions.Any(permission => permission.IdOBene == permission.IdOD && permission.Poziom == 0);

		private LoggedinStatePushParameters Parameters { get; set; }
		private List<UprawnienieModel> Permissions
		{
			get
			{
				Database.Database.Instance.Connect();

				var allPermissions = Database.Database.Instance.SelectAll<UprawnienieModel>();
				var userPermissions = allPermissions.Where(permission => permission.IdOBene == Parameters.user.IdO);

				Database.Database.Instance.Dispose();
				return userPermissions.ToList();
			}
		}
		public override void Push(StatePushParameters pushParameters)
		{
			Parameters = pushParameters as LoggedinStatePushParameters;
		}

		public override void StartState()
		{
			//Add main menu
			//StateMachine.Instance.PushState<MainMenuState>();
		}
	}
}
