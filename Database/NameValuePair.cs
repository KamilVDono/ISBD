using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Database
{
	public class NameValuePair
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public NameValuePair() { }

		public NameValuePair(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public NameValuePair AddQuotes()
		{
			Value = $"'{Value}'";
			return this;
		}
	}
}
