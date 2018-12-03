using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ISBD.Utils
{
	static class ButtonExtension
	{
		public static void ClearClick(this Button button)
		{
			FieldInfo f1 = typeof(Button).GetField("Click",
				BindingFlags.Static | BindingFlags.NonPublic);
			object obj = f1?.GetValue(button);
			PropertyInfo pi = button.GetType().GetProperty("Events",
				BindingFlags.NonPublic | BindingFlags.Instance);
			EventHandlerList list = (EventHandlerList)pi?.GetValue(button, null);
			list?.RemoveHandler(obj, list[obj]);
		}
	}
}
