using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Model
{
	class KategoriaModel
	{
		public Int64 IdK { get; set; }
		public String Nazwa { get; set; }
		public int Rodzaj { get; set; }
		public Int64? IdKRodzic { get; set; }
		public Int64 IdS { get; set; }
	}
}
