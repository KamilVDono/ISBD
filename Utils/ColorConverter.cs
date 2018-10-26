using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ISBD.Utils
{
	public static class ColorConverter
	{
		public static int Int(this Color color)
		{
			return (color.R << 24) | (color.G << 16) | (color.B << 8) | (color.A);
		}

		public static Color Color(this int colorInt)
		{
			uint redMask = 0xFF000000;
			int greenMask = 0xFF0000;
			int blueMask = 0xFF00;
			int alphaMask = 0xFF;
			Color color = new Color();
			color.R = (byte)((colorInt & redMask) >> 24);
			color.G = (byte)((colorInt & greenMask) >> 16);
			color.B = (byte)((colorInt & blueMask) >> 8);
			color.A = (byte)(colorInt & alphaMask);

			return color;
		}
	}
}
