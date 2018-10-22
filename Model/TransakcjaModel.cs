using System;
using System.Collections.Generic;

namespace ISBD.Model
{
	class TransakcjaModel
	{
		public long IdT { get; set; }
		public double Kwota { get; set; }
		public string Tytul { get; set; }
		public string Opis { get; set; }
		public DateTime Data { get; set; }
		public long IdO { get; set; }
		public long IdK { get; set; }
	}
}
