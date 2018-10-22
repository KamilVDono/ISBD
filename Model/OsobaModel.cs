using System;
using System.Collections.Generic;
using ISBD.Database;

namespace ISBD.Model
{
	class OsobaModel : Database.IDBInsertable
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
					new NameValuePair("Login", Login),
					new NameValuePair("Haslo", Haslo),
					new NameValuePair("Imie", Imie),
					new NameValuePair("Nazwisko", Nazwisko),
					new NameValuePair("DataU", DataU.Ticks.ToString())
				};
				return list;
			}
		}

		public string Table => "Osoby";
	}
}
