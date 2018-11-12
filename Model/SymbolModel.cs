using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ISBD.Database;
using ISBD.Utils;

namespace ISBD.Model
{
	public class SymbolModel : Database.IDBInsertable, IDBTableItem, IDBSelectable
	{
		public long IdS { get; set; }
		public Color Kolor { get; set; }
		public string Ikona { get; set; }

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Kolor", Kolor.Int().ToString()),
				};
				if (string.IsNullOrEmpty(Ikona) == false)
				{
					list.Add(new NameValuePair("Ikona", Ikona.ToString()).AddQuotes());
				}
				return list;
			}
		}
		public string Table => "Symbole";
		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 3) return false;

			IdS = reader.GetInt64(0);
			Kolor = reader.GetInt32(1).Color();
			Ikona = reader.GetString(2);

			return true;
		}
		public static bool operator ==(SymbolModel left, SymbolModel right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (ReferenceEquals(left, null)) return false;
			if (ReferenceEquals(right, null)) return false;
			return left.IdS == right.IdS;
		}

		public static bool operator !=(SymbolModel left, SymbolModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdS: {IdS} | Kolor: {Kolor} | Ikona: {Ikona}";
		}
	}
}
