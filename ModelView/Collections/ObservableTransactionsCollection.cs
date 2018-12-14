using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;

namespace ISBD.ModelView
{
	public class ObservableTransactionsCollection : TrulyObservableCollection<TransakcjaModel>
	{
		public ObservableTransactionsCollection(List<TransakcjaModel> list) : base(list)
		{
		}
	}
}
