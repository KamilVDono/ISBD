using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISBD.Model;

namespace ISBD.ModelView
{
	public class ObservableCategoriesCollection : TrulyObservableCollection<KategoriaModel>
	{
		public ObservableCategoriesCollection(List<KategoriaModel> list): base(list) { }
	}
}
