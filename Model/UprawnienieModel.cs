using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Database;

namespace ISBD.Model
{
	class UprawnienieModel : Database.IDBInsertable
	{
		public long IdU { get; set; }
		public long Poziom { get; set; }
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
	}
}
