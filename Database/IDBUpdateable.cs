using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Database
{
	public interface IDBUpdateable
	{
		string IndexName { get; }
		long Index { get; }
	}
}
