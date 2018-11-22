using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ISBD.Database;

namespace ISBD.Model
{
	public class TransakcjaModel : Database.IDBInsertable, IDBTableItem, IDBSelectable
	{
		public long IdT { get; set; }
		public double Kwota { get; set; }
		public string Tytul { get; set; }
		public string Opis { get; set; }
		public DateTime Data { get; set; }
		public long IdO { get; set; }
		public long IdK { get; set; }

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Kwota", Kwota.ToString()),
					new NameValuePair("Tytul", Tytul).AddQuotes(),
				};
				if (string.IsNullOrEmpty(Opis) == false)
				{
					list.Add(new NameValuePair("Opis", Opis).AddQuotes());
				}
				list.Add(new NameValuePair("Data", Data.Ticks.ToString()));
				list.Add(new NameValuePair("IdO", IdO.ToString()));
				list.Add(new NameValuePair("IdK", IdK.ToString()));
				return list;
			}
		}

		public string Table => "Transakcje";
		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 7) return false;

			IdT = reader.GetInt64(0);
			Kwota = reader.GetDouble(1);
			Tytul = reader.GetString(2);
			Opis = reader.IsDBNull(3) ? "" : reader.GetString(3);
			Data = new DateTime(reader.GetInt64(4));
			IdO = reader.GetInt64(5);
			IdK = reader.GetInt64(6);

			return true;
		}
		public static bool operator ==(TransakcjaModel left, TransakcjaModel right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(right, null)) return false;
			return left.IdT == right.IdT;
		}

		public static bool operator !=(TransakcjaModel left, TransakcjaModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdT: {IdT} | Kwota: {Kwota} | Tytul: {Tytul} | Opis: {Opis} | Data: {Data} | IdO: {IdO} | IdK: {IdK}";
		}
	}
}
