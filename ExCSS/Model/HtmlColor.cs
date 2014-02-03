﻿using System;
using System.Runtime.InteropServices;
using ExCSS.Model;
using ExCSS.Model.Extensions;

// ReSharper disable once CheckNamespace
namespace ExCSS
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, CharSet = CharSet.Unicode)]
    public struct HtmlColor : IEquatable<HtmlColor>
    {
        [FieldOffset(0)]
        private byte alpha;
        [FieldOffset(1)]
        private byte red;
        [FieldOffset(2)]
        private byte green;
        [FieldOffset(3)]
        private byte blue;
        [FieldOffset(0)]
        private readonly int hashcode;

        public HtmlColor(byte r, byte g, byte b)
        {
            hashcode = 0;
            alpha = 255;
            red = r;
            blue = b;
            green = g;
        }

        public HtmlColor(byte a, byte r, byte g, byte b)
        {
            hashcode = 0;
            alpha = a;
            red = r;
            blue = b;
            green = g;
        }

        public HtmlColor(Double a, byte r, byte g, byte b)
        {
            hashcode = 0;
            alpha = (byte)Math.Max(Math.Min(Math.Ceiling(255 * a), 255), 0);
            red = r;
            blue = b;
            green = g;
        }

        public static HtmlColor FromRgba(byte r, byte g, byte b, Single a)
        {
            return new HtmlColor(a, r, g, b);
        }

        public static HtmlColor FromRgba(byte r, byte g, byte b, Double a)
        {
            return new HtmlColor(a, r, g, b);
        }

        public static HtmlColor FromRgb(byte r, byte g, byte b)
        {
            return new HtmlColor(r, g, b);
        }

        public static HtmlColor FromHsl(Single h, Single s, Single l)
        {
            const Single third = 1f / 3f;

            var m2 = l <= 0.5f ? (l * (s + 1f)) : (l + s - l * s);
            var m1 = 2f * l - m2;
            var r = (Byte)Math.Round(255 * HueToRgb(m1, m2, h + third));
            var g = (Byte)Math.Round(255 * HueToRgb(m1, m2, h));
            var b = (Byte)Math.Round(255 * HueToRgb(m1, m2, h - third));
            return new HtmlColor(r, g, b);
        }

        public static HtmlColor FromHex(string color)
        {
            if (color.Length == 3)
            {
                var r = color[0].FromHex();
                r += r * 16;

                var g = color[1].FromHex();
                g += g * 16;
                
                var b = color[2].FromHex();
                b += b * 16;

                return new HtmlColor((byte)r, (byte)g, (byte)b);
            }
           
            if (color.Length == 6)
            {
                var r = 16 * color[0].FromHex();
                var g = 16 * color[2].FromHex();
                var b = 16 * color[4].FromHex();
                
                r += color[1].FromHex();
                g += color[3].FromHex();
                b += color[5].FromHex();

                return new HtmlColor((byte)r, (byte)g, (byte)b);
            }

            return new HtmlColor();
        }

        public static bool TryFromHex(string color, out HtmlColor htmlColor)
        {
            htmlColor = new HtmlColor {alpha = 255};

            if (color.Length == 3)
            {
                if (!color[0].IsHex() || !color[1].IsHex() || !color[2].IsHex())
                {
                    return false;
                }

                var r = color[0].FromHex();
                r += r * 16;
                
                var g = color[1].FromHex();
                g += g * 16;
                
                var b = color[2].FromHex();
                b += b * 16;

                htmlColor.red = (byte)r;
                htmlColor.green = (byte)g;
                htmlColor.blue = (byte)b;
                
                return true;
            }
            
            if (color.Length == 6)
            {
                if (!color[0].IsHex() || !color[1].IsHex() || !color[2].IsHex() ||
                    !color[3].IsHex() || !color[4].IsHex() || !color[5].IsHex())
                {
                    return false;
                }

                var r = 16 * color[0].FromHex();
                var g = 16 * color[2].FromHex();
                var b = 16 * color[4].FromHex();
                
                r += color[1].FromHex();
                g += color[3].FromHex();
                b += color[5].FromHex();

                htmlColor.red = (byte)r;
                htmlColor.green = (byte)g;
                htmlColor.blue = (byte)b;
                
                return true;
            }

            return false;
        }

        public int Value
        {
            get { return hashcode; }
        }

        public byte A
        {
            get { return alpha; }
        }

        public double Alpha
        {
            get { return alpha / 255.0; }
        }

        public byte R
        {
            get { return red; }
        }

        public byte G
        {
            get { return green; }
        }

        public byte B
        {
            get { return blue; }
        }

        public static bool operator ==(HtmlColor a, HtmlColor b)
        {
            return a.hashcode == b.hashcode;
        }

        public static bool operator !=(HtmlColor a, HtmlColor b)
        {
            return a.hashcode != b.hashcode;
        }

        public override bool Equals(Object obj)
        {
            if (obj is HtmlColor)
            {
                return Equals((HtmlColor)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return hashcode;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool friendlyFormat, int indentation = 0)
        {
            return string.Format("rgba({0}, {1}, {2}, {3})", red, green, blue, alpha / 255.0)
                .Indent(friendlyFormat, indentation);
        }

        public bool Equals(HtmlColor other)
        {
            return hashcode == other.hashcode;
        }

        public string ToCss()
        {
            if (alpha == 255)
            {
                return "rgb(" + red + ", " + green + ", " + blue + ")";
            }

            return "rgba(" + red + ", " + green + ", " + blue + ", " + Alpha + ")";
        }

        public string ToHtml()
        {
            return "#" + red.ToHex() + green.ToHex() + blue.ToHex();
        }

        private static Single HueToRgb(Single m1, Single m2, Single h)
        {
            const Single sixth = 1f / 6f;
            const Single third2 = 2f / 3f;

            if (h < 0f)
            {
                h += 1f;
            }
            else if (h > 1f)
            {
                h -= 1f;
            }

            if (h < sixth)
            {
                return m1 + (m2 - m1) * h * 6f;
            }
            if (h < 0.5)
            {
                return m2;
            }
            if (h < third2)
            {
                return m1 + (m2 - m1) * (third2 - h) * 6f;
            }

            return m1;
        }
    }
}