using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static ISBD.Database.Database;

namespace ISBD.Model.Tests
{
	public class QueryTest
	{
		List<Action> testActions => new List<Action>()
		{
			InsertPerson
		};

		public QueryTest()
		{
			testActions.ForEach(action => action());
		}

		public void InsertPerson()
		{
			OsobaModel osoba = new OsobaModel() { DataU = DateTime.Now, Haslo = "qwe123", Imie = "Władek", Nazwisko = "Władkowski", Login = "Wlad" };

			using (Database.Database database = new Database.Database())
			{
				database.Connect();

				database.Insert(osoba);

				database.SelectAll<OsobaModel>().ForEach(osobaFE =>
				{
					Debug.WriteLine(osobaFE);
					Debug.WriteLine(osoba);
				});
			}
		}
	}
}
