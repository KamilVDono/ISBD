using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using ISBD.Database;

namespace ISBD.Model
{
	public class OsobaModel : Database.IDBInsertable, IDBSelectable, IDBTableItem
	{
		public long IdO { get; set; }
		public string Login { get; set; }
		public string Haslo { get; set; }
		public string Imie { get; set; }
		public string Nazwisko { get; set; }
		public DateTime DataU { get; set; }

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Login", Login).AddQuotes(),
					new NameValuePair("Haslo", Haslo).AddQuotes(),
					new NameValuePair("Imie", Imie).AddQuotes(),
					new NameValuePair("Nazwisko", Nazwisko).AddQuotes(),
					new NameValuePair("DataU", DataU.Ticks.ToString())
				};
				return list;
			}
		}
		public string Table => "Osoby";
		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 6) return false;

			IdO = reader.GetInt64(0);
			Login = reader.GetString(1);

			Haslo = reader.GetString(2);
			Imie = reader.GetString(3);
			Nazwisko = reader.GetString(4);
			DataU = new DateTime(reader.GetInt64(5));

			return true;
		}

		public static bool operator ==(OsobaModel left, OsobaModel right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(right, null)) return false;

			return left.IdO == right.IdO;
		}

		public static bool operator !=(OsobaModel left, OsobaModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdO: {IdO} | Login: {Login} | Haslo: {Haslo} | Imie: {Imie} | Nazwisko: {Nazwisko} | DataU: {DataU}";
		}
	}
}
