using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ISBD.Database;
using ISBD.Utils;

namespace ISBD.Model
{
	class SymbolModel : Database.IDBInsertable
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
					list.Add(new NameValuePair("Ikona", Ikona));
				}
				return list;
			}
		}

		public string Table => "Symbole";
	}
}
