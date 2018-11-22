using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;
using ISBD.ModelView.State.UIStates;

namespace ISBD.ModelView.State.LogicStates
{
	class LoggedinStatePushParameters : StatePushParameters
	{
		public OsobaModel user;
		public bool saveLogged;
	}

	class LoggedinLogicState : LogicState
	{
		public bool IsAdmin => Permissions.Any(permission => permission.IdOBene == permission.IdOD && permission.Poziom == 0);

		public OsobaModel LoggedInPerson => Parameters.user;
		public string Name => Parameters.user.Imie;
		public string LastName => Parameters.user.Nazwisko;

		public OsobaModel CurrentSelectedUser { get; set; }

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

		public List<OsobaModel> ValidUsers
		{
			get
			{
				Database.Database.Instance.Connect();

				var allPermissions = Database.Database.Instance.SelectAll<UprawnienieModel>().ToList();
				var userPermissions = allPermissions.Where(permission => permission.IdOBene == Parameters.user.IdO).ToList();
				var allPermissionsUsers = Database.Database.Instance.SelectAll<OsobaModel>().ToList();
				if (IsAdmin == false)
				{
					allPermissionsUsers = allPermissionsUsers.Where(user =>
						userPermissions.Any(permission => permission.IdOD == user.IdO)).ToList();
				}

				var bannedUsers = userPermissions.Where(permission => permission.Poziom < 0).Select(p => p.IdOD).ToList();

				allPermissionsUsers = allPermissionsUsers.Where(user => bannedUsers.Contains(user.IdO) == false).ToList();

				if(allPermissionsUsers.Contains(Parameters.user)) allPermissionsUsers.Add(Parameters.user);


				Database.Database.Instance.Dispose();
				return allPermissionsUsers;
			}
		}

		public List<TransakcjaModel> GetUserTransactions(OsobaModel user)
		{
			Database.Database.Instance.Connect();

			var allUserTransactions = Database.Database.Instance.SelectAll<TransakcjaModel>().Where(trans => trans.IdO == user.IdO);

			Database.Database.Instance.Dispose();
			return allUserTransactions.ToList();
		}

		public override void Push(StatePushParameters pushParameters)
		{
			Parameters = pushParameters as LoggedinStatePushParameters;
			CurrentSelectedUser = Parameters.user;
		}

		public override void StartState()
		{
			Database.Database.Instance.SaveLastLogin(LoggedInPerson.Login, LoggedInPerson.Haslo);
			StateMachine.Instance.PushState<MainMenuUIState>(null);
		}
	}
}
