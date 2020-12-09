using RGBLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapExtensionsLib
{
    public static class BitmapExtensions
    {
        public static RGB[,] ToRGBArray(this Bitmap me)
        {
            int width = me.Width;
            int height = me.Height;
            RGB[,] result = new RGB[width, height];
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmdata = me.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            unsafe {
                byte* image = (byte*)bmdata.Scan0.ToPointer();
#if DEBUG
                for (int y = 0; y < height; ++y) {
#else
                Parallel.For(0, height, delegate (int y) {
#endif
                    byte* pixel = image + y * bmdata.Stride;
                    for (int x = 0; x < width; ++x) {
                        result[x, y] = new RGB(
                            ColorExtensions.SRGBToLinear(ColorExtensions.Unnormalize8(pixel[2])),
                            ColorExtensions.SRGBToLinear(ColorExtensions.Unnormalize8(pixel[1])),
                            ColorExtensions.SRGBToLinear(ColorExtensions.Unnormalize8(pixel[0]))
                        );
                        pixel += 4;
                    }
#if DEBUG
                }
#else
                });
#endif
            }
            me.UnlockBits(bmdata);

            return result;
        }

        public static void Render(this Bitmap me, Func<int, int, RGB> renderPixel)
        {
            int width = me.Width;
            int height = me.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmdata = me.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe {
                byte* ptr = (byte*)bmdata.Scan0.ToPointer();
#if DEBUG
                for (int y = 0; y < rect.Height; ++y) {
#else
                Parallel.For(0, rect.Height, delegate (int y) {
#endif
                    byte* pixel = ptr + bmdata.Stride * y;
                    for (int x = 0; x < rect.Width; ++x) {
                        RGB rgb = renderPixel(x, y);
                        SRGBA srgba = rgb.ToSRGBA();
                        pixel[0] = srgba.B;
                        pixel[1] = srgba.G;
                        pixel[2] = srgba.R;
                        pixel[3] = srgba.A;
                        pixel += 4;
                    }
#if DEBUG
                }
#else
                });
#endif
            }
            me.UnlockBits(bmdata);
        }

    }
}

