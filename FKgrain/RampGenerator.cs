using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FKgrain
{
    public static class RampGenerator
    {
        private struct C
        {
           
            public C(int r ,int g , int b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
            public int r;
            public int g;
            public int b;
        
        }
        static readonly C red = new C(255,0,0) ;
        static readonly C yellow = new C(255,255,128) ;
        static readonly C blue = new C(0,0,255) ;
        static Color ToColor(this C self)
        {
            return Color.FromArgb(self.r, self.g, self.b);
        }
        public static Color HotCold(float data, float max, float min)
        {
            if (max < min)
            {
                float tmp = max;
                max = min;
                min = tmp;
            }

            if (data >= max)
            {
                return red.ToColor();
            }
            if (data <= min)
            {
                return blue.ToColor();
            }
            float part = (data - min) / (max - min);
            if (part == 0.5f)
            {
                return yellow.ToColor();
            }
            else if (part < 0.5f)
            {
                C a = ColorBetween(blue, yellow, part * 2);
                return a.ToColor();
            }
            else
            {
                return ColorBetween(yellow, red, (part - 0.5f) * 2).ToColor();
            }
        }
        private static C ColorBetween(C from, C to, float part)
        {
            int r = from.r + (int)Math.Floor((float)(to.r - from.r) * part);
            int g = from.g + (int)Math.Floor((float)(to.g - from.g) * part);
            int b = from.b + (int)Math.Floor((float)(to.b - from.b) * part);
            return new C(r, g, b);
        }
    }
}
