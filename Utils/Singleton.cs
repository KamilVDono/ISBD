using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Utils
{
	public class Singleton<T> where T : class , new()
	{
		private static T _instance = null;
		static readonly object _padlock = new object();
		public static T Instance
		{
			get
			{
				lock (_padlock)
				{
					return _instance ?? (_instance = new T());
				}
			}
			private set { _instance = value; }
		}
	}
}
