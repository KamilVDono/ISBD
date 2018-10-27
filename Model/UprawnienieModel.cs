using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Database;

namespace ISBD.Model
{
	public class UprawnienieModel : Database.IDBInsertable, IDBTableItem, IDBSelectable
	{
		public long IdU { get; set; }
		public int Poziom { get; set; }
		public long IdOBene { get; set; }
		public long IdOD { get; set; }

		public IList<NameValuePair> NamedValues
		{
			get
			{
				var list = new List<NameValuePair>()
				{
					new NameValuePair("Poziom", Poziom.ToString()),
					new NameValuePair("IdOBene", IdOBene.ToString()),
					new NameValuePair("IdOD", IdOD.ToString()),
				};
				return list;
			}
		}
		public string Table => "Uprawnienia";
		public bool Init(SQLiteDataReader reader)
		{
			if (reader.HasRows == false) return false;
			if (reader.FieldCount != 4) return false;

			IdU = reader.GetInt64(0);
			Poziom = reader.GetInt32(1);
			IdOBene = reader.GetInt64(2);
			IdOD = reader.GetInt64(3);

			return true;
		}

		public static bool operator ==(UprawnienieModel left, UprawnienieModel right)
		{
			return left.IdU == right.IdU;
		}

		public static bool operator !=(UprawnienieModel left, UprawnienieModel right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"IdU: {IdU} | Poziom: {Poziom} | IdOBene: {IdOBene} | IdOD: {IdOD}";
		}
	}
}
