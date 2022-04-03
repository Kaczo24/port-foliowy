using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ConsolGraphicsEngine
{
    public class CGE
    {
        public char[,] pixels;
        public int width, height;
        public char[] colors = new char[5] 
        {
            ' ',
            Convert.ToChar(9617),
            Convert.ToChar(9618),
            Convert.ToChar(9619),
            Convert.ToChar(9608)
        };
        public int strokeWeight = 0;
        public int fill = -1;
        public int stroke = 4;
         

        public CGE(int w, int h)
        {
            width = w;
            height = h;
            pixels = new char[width, height];
            for (int n = 0; n < width; n++)
                for (int m = 0; m < height; m++)
                    pixels[n, m] = colors[0];
            Console.SetWindowSize(w + 1, h + 1);
            Console.SetBufferSize(w + 1, h + 1);
        }

        public void Redraw()
        {
            string[] strs = new string[height];
            for (int n = 0; n < width; n++)
                for (int m = 0; m < height; m++)
                    strs[m] += pixels[n, m];
            string outp = string.Join("\n", strs);
            Console.Clear();
            Console.Write(outp);
        }

        public void Clear()
        {
            pixels = new char[width, height];
            for (int n = 0; n < width; n++)
                for (int m = 0; m < height; m++)
                    pixels[n, m] = colors[0];
        }

        public void set(int x, int y, char c)
        {
            if (x > 0 && x < width && y > 0 && y < height)
                pixels[x, y] = c;
        }


        public void errace(int x, int y)
        {
            for (int m = -strokeWeight; m <= strokeWeight; m++)
                for (int n = -strokeWeight; n <= strokeWeight; n++)
                    if (n * n + m * m <= strokeWeight * strokeWeight)
                        set(x + n, y + m, colors[0]);
        }


        public void point(int x, int y)
        {
            if(stroke != -1)
            for (int m = -strokeWeight; m <= strokeWeight; m++)
                for (int n = -strokeWeight; n <= strokeWeight; n++)
                    if (n * n + m * m <= strokeWeight * strokeWeight)
                        set(x + n, y + m, colors[stroke]);
        }

        public void circle(int x, int y, int radius)
        {
            if (fill != -1)
                for (int m = -radius; m <= radius; m++)
                    for (int n = -radius; n <= radius; n++)
                        if (n * n + m * m < radius * radius)
                            set(x + n, y + m, colors[fill]);

            if (stroke != -1)
            {
                int xs = radius - 1;
                int ys = 0;
                int dx = 1;
                int dy = 1;
                int err = dx - (radius << 1);

                while (xs >= ys)
                {
                    point(x + xs, y + ys);
                    point(x + ys, y + xs);
                    point(x - ys, y + xs);
                    point(x - xs, y + ys);
                    point(x - xs, y - ys);
                    point(x - ys, y - xs);
                    point(x + ys, y - xs);
                    point(x + xs, y - ys);

                    if (err <= 0)
                    {
                        ys++;
                        err += dy;
                        dy += 2;
                    }

                    if (err > 0)
                    {
                        xs--;
                        dx += 2;
                        err += dx - (radius << 1);
                    }
                }
            }
        }



        public void rect(int x, int y, int w, int h)
        {
            int ex = x + w;
            int ey = y + h;
            if (fill != -1)
            {
                if (x < ex)
                    for (int xn = x; xn < ex; xn++)
                        if (y < ey)
                            for (int yn = y; yn < ey; yn++)
                                set(xn, yn, colors[fill]);
                        else
                            for (int yn = ey; yn < y; yn++)
                                set(xn, yn, colors[fill]);
                else
                    for (int xn = ex; xn < x; xn++)
                        if (y < ey)
                            for (int yn = y; yn < ey; yn++)
                                set(xn, yn, colors[fill]);
                        else
                            for (int yn = ey; yn < y; yn++)
                                set(xn, yn, colors[fill]);
            }

            if (stroke != -1)
            {

                if (x < ex)
                    for (int n = x; n <= ex; n++)
                    {
                        point(n, y);
                        point(n, ey);
                    }
                else
                    for (int n = ex; n <= x; n++)
                    {
                        point(n, y);
                        point(n, ey);
                    }

                if (y < ey)
                    for (int n = y; n <= ey; n++)
                    {
                        point(x, n);
                        point(ex, n);
                    }
                else
                    for (int n = ey; n <= y; n++)
                    {
                        point(x, n);
                        point(ex, n);
                    }
            }
        }
        public void line(int xs, int ys, int xe, int ye)
        {
            if (stroke != -1)
            {
                int d, dx, dy, ai, bi, xi, yi;
                int xc = xs;
                int yc = ys;
                if (xs < xe)
                {
                    xi = 1;
                    dx = xe - xs;
                }
                else
                {
                    xi = -1;
                    dx = xs - xe;
                }

                if (ys < ye)
                {
                    yi = 1;
                    dy = ye - ys;
                }
                else
                {
                    yi = -1;
                    dy = ys - ye;
                }
                point(xc, yc);
                if (dx > dy)
                {
                    ai = (dy - dx) * 2;
                    bi = dy * 2;
                    d = bi - dx;
                    while (xc != xe)
                    {
                        if (d >= 0)
                        {
                            xc += xi;
                            yc += yi;
                            d += ai;
                        }
                        else
                        {
                            d += bi;
                            xc += xi;
                        }
                        point(xc, yc);
                    }
                }
                else
                {
                    ai = (dx - dy) * 2;
                    bi = dx * 2;
                    d = bi - dy;
                    while (yc != ye)
                    {
                        if (d >= 0)
                        {
                            xc += xi;
                            yc += yi;
                            d += ai;
                        }
                        else
                        {
                            d += bi;
                            yc += yi;
                        }
                        point(xc, yc);
                    }
                }
            }
        }
    }
}

