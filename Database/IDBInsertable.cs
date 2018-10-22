using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Database
{
	interface IDBInsertable
	{
		IList<NameValuePair> NamedValues { get; }
		string Table { get; }
	}
}
