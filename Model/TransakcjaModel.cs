using System;
using System.Collections.Generic;
using ISBD.Database;

namespace ISBD.Model
{
	class TransakcjaModel : Database.IDBInsertable
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
					new NameValuePair("Tytul", Tytul),
				};
				if (string.IsNullOrEmpty(Opis) == false)
				{
					list.Add(new NameValuePair("Opis", Opis));
				}
				list.Add(new NameValuePair("Data", Data.Ticks.ToString()));
				list.Add(new NameValuePair("IdO", IdO.ToString()));
				list.Add(new NameValuePair("IdK", IdK.ToString()));
				return list;
			}
		}

		public string Table => "Transakcje";
	}
}
