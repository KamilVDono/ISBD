using System.Collections.Generic;
using System.Data.SQLite;
using ISBD.Database;

namespace ISBD.Model
{
	public class KategoriaModel: Database.IDBInsertable, IDBTableItem, IDBSelectable
	{
		public long IdK { get; set; }
		public string Nazwa { get; set; }
		public int Rodzaj { get; set; }
		public long? IdKRodzic { get; set; }
		public long IdS { get; set; }

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Nazwa", Nazwa).AddQuotes(),
					new NameValuePair("Rodzaj", Rodzaj.ToString()),
				};
				if (IdKRodzic.HasValue)
				{
					list.Add(new NameValuePair("IdKRodzic", IdKRodzic.ToString()));
				}
				list.Add(new NameValuePair("IdS", IdS.ToString()));
				return list;
			}
		}
		public string Table => "Kategorie";
		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 5) return false;


			IdK = reader.GetInt64(0);
			Nazwa = reader.GetString(1);
			Rodzaj = reader.GetInt32(2);
			IdKRodzic = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3);
			IdS = reader.GetInt64(4);

			return true;
		}

		public static bool operator ==(KategoriaModel left, KategoriaModel right)
		{
			return left.IdK == right.IdK;
		}

		public static bool operator !=(KategoriaModel left, KategoriaModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdK: {IdK} | Nazwa: {Nazwa} | Rodzaj: {Rodzaj} | IdKRodzic: {IdKRodzic} | IdS: {IdS}";
		}
	}
}
