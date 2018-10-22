using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Model
{
	class TransakcjaModel
	{
		public long IdT { get; set; }
		public double Kwota { get; set; }
		public string Tytul { get; set; }
		public string Opis { get; set; }
		public string Data { get; set; }
		public long IdO { get; set; }
		public long IdK { get; set; }
	}
}
