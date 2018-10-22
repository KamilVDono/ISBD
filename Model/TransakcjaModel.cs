using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Model
{
	class TransakcjaModel
	{
		public Int64 IdT { get; set; }
		public Double Kwota { get; set; }
		public String Tytul { get; set; }
		public String Opis { get; set; }
		public String Data { get; set; }
		public Int64 IdO { get; set; }
		public Int64 IdK { get; set; }
	}
}
