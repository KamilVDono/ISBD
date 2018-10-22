using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Database;

namespace ISBD.Model
{
	class KategoriaModel: Database.IDBInsertable
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
					new NameValuePair("IdK", IdK.ToString()),
					new NameValuePair("Nazwa", Nazwa),
					new NameValuePair("Rodzaj", Rodzaj.ToString()),
				};
				if (IdKRodzic.HasValue)
				{
					list.Add(new NameValuePair("IdKRodzic", IdKRodzic.ToString()));
				}
				list.Add(new NameValuePair("IdS", IdS));
				return list;
			}
		}

		public string Table => "Kategorie";
	}
}
