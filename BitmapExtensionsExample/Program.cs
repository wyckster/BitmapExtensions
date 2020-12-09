using BitmapExtensionsLib;
using RGBLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapExtensionsExample
{
    class Program
    {
        static double Checker(int x, int y)
        {
            bool c = ((x ^ y) & 1) != 0;
            return c ? 1.0 : 0.0;
        }

        static void Main(string[] args)
        {
            // Example 1:
            //   1) Create a 4x4 orange bitmap with a blue square in the middle.
            //   2) Convert it to a 2-dimensional array of linear RGB values.
            //   3) Examine some of the pixels' linear RGB values.
            Bitmap bitmap = new Bitmap(4, 4);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.FromArgb(255, 128, 0)); // orange
            g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 255)), 1, 1, 2, 2);
            RGB[,] rgb = bitmap.ToRGBArray();
            Console.WriteLine("Should be orange.  The values are linear, so expect full red, green value of about 0.215 and no blue:\n\t{0}", rgb[0, 0]);
            Console.WriteLine("Should be blue:\n\t{0}", rgb[1, 1]);


            // Example 2:  Render, where pixel colour is a function of x and y.
            //   1) Create a 256x256 image.
            //   2) Fill the image with colours.
            //   3) Save the file.
            //
            // The image consists of:
            //   - A linear gradient on the top half,
            //   - An SRGB gradient on the bottom half,
            //   - A strip between them with 50% black and white dot pattern.
            Bitmap ramp = new Bitmap(256, 256);
            ramp.Render(delegate (int x, int y) {
                if (y < 128 - 16) {
                    return new RGB(x / 255.0, x / 255.0, x / 255.0);
                } else if (y >= (128 + 16)) {
                    return new SRGBA((byte)x, (byte)x, (byte)x).ToRGB();
                } else {
                    return new RGB(Checker(x, y), Checker(x, y), Checker(x, y));
                }
            });
            ramp.Save("linear-vs-nonlinear.png", ImageFormat.Png);
        }
    }
}
