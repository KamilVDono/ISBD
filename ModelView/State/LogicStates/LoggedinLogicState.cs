using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
		private Random _Random;
		private Random Random => _Random ?? (_Random = new Random());

		public bool IsAdmin => Permissions.Any(permission => permission.IdOBene == permission.IdOD && permission.Poziom == 0);

		public OsobaModel LoggedInPerson => Parameters.user;
		public string Name => Parameters.user.Imie;
		public string LastName => Parameters.user.Nazwisko;

		public OsobaModel CurrentSelectedUser { get; set; }

		private LoggedinStatePushParameters Parameters { get; set; }

		public List<UprawnienieModel> Permissions
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

				var bannedUsers = userPermissions.Where(permission => permission.Poziom < UprawnienieModel.WRITE_BAN).Select(p => p.IdOD).ToList();

				allPermissionsUsers = allPermissionsUsers.Where(user => bannedUsers.Contains(user.IdO) == false).ToList();

				if(allPermissionsUsers.Contains(Parameters.user)) allPermissionsUsers.Add(Parameters.user);


				Database.Database.Instance.Dispose();
				return allPermissionsUsers;
			}
		}

		public List<KategoriaModel> Categories
		{
			get
			{
				Database.Database.Instance.Connect();

				var categories = Database.Database.Instance.SelectAll<KategoriaModel>().ToList();

				Database.Database.Instance.Dispose();
				return categories;
			}
		}

		public List<TransakcjaModel> GetUserTransactions(OsobaModel user)
		{
			Database.Database.Instance.Connect();

			var allUserTransactions = Database.Database.Instance.SelectAll<TransakcjaModel>().Where(trans => trans.IdO == user.IdO);

			Database.Database.Instance.Dispose();
			return allUserTransactions.ToList();
		}

		public bool CanWriteToUser(OsobaModel other)
		{
			if (IsAdmin)
			{
				bool val = Permissions.Where(p => p.IdOD == other.IdO).All(p => p.Poziom > UprawnienieModel.WRITE_BAN);
				return val;
			}
			else
			{
				bool val = Permissions.Where(p => p.IdOD == other.IdO).Any(p => p.Poziom == UprawnienieModel.WRITE_ALLOW);
				return val;
			}
		}

		public void AddTransaction(TransakcjaModel trans)
		{
			Database.Database.Instance.Connect();
			Database.Database.Instance.Insert(trans);
			Database.Database.Instance.Dispose();
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

		public double GetMonthIncomes(DateTime currentDate)
		{
			return GetMonthSum(currentDate, 1);
		}

		public double GetMonthExpenses(DateTime currentDate)
		{
			return GetMonthSum(currentDate, -1);
		}

		private double GetMonthSum(DateTime currentDate, int kind)
		{
			var allUserTransactions = GetUserTransactions(CurrentSelectedUser).ToList();
			var allFromThisMonth =
				allUserTransactions.Where(t => t.Data.Month == currentDate.Month && t.Data.Year == currentDate.Year).ToList();
			var allIncomes =
				allFromThisMonth.Where(t => Categories.FirstOrDefault(cat => cat.IdK == t.IdK)?.Rodzaj == kind).ToList();
			return allIncomes.Sum(t => t.Kwota);
		}

		public List<CategorySummary> GetMonthCategoriesSummary(DateTime currentDate)
		{
			var allUserTransactions = GetUserTransactions(CurrentSelectedUser).ToList();
			var allFromThisMonth =
				allUserTransactions.Where(t => t.Data.Month == currentDate.Month && t.Data.Year == currentDate.Year).ToList();

			Database.Database.Instance.Connect();
			var categorySummaries = Database.Database.Instance.SelectAll<KategoriaModel>().Select(cat =>
			{
				var symbol = Database.Database.Instance.SelectAll<SymbolModel>().FirstOrDefault(s => s.IdS == cat.IdS);
				return new CategorySummary()
				{
					Name = cat.Nazwa, Percent = Random.NextDouble(), Ico = symbol.Ikona, CategoryColor = symbol.Kolor,
					Sum = allFromThisMonth.Where(t => t.IdK == cat.IdK).Sum(t => t.Kwota),
				};
			}).OrderByDescending(c => c.Sum).ToList();

			Database.Database.Instance.Dispose();
			return categorySummaries;
		}
	}

	public class CategorySummary
	{
		public string Name { get; set; }
		public double Sum { get; set; }

		public string Amount => string.Format(CultureInfo.CurrentUICulture, "{0:C2}", Sum);

		public string Ico { get; set; }
		public double Percent { get; set; }

		public Color CategoryColor { get; set; }
	}
}
