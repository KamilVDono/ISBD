using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;
using ISBD.ModelView;

namespace ISBD.ModelView
{
	public class ObservableSymbolsCollection : TrulyObservableCollection<SymbolModel>
	{
		public ObservableSymbolsCollection(List<SymbolModel> list): base(list) { }
	}
}
