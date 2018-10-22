using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Model
{
	class OsobaModel
	{
		public long IdO { get; set; }
		public string Login { get; set; }
		public string Haslo { get; set; }
		public string Imie { get; set; }
		public string Nazwisko { get; set; }
		public DateTime DataU { get; set; }
	}
}
