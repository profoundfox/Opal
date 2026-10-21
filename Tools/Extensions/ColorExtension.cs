using Opal.Managers;

using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Opal.Tools
{
  public static class ColorExtension
  {
    public static Color FromHex(this Color _, string hex)
    {
      hex = hex.TrimStart('#');

      if (hex.Length == 6)
        return new Color(
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16));

      if (hex.Length == 8)
        return new Color(
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16),
            Convert.ToByte(hex.Substring(6, 2), 16),
            Convert.ToByte(hex.Substring(0, 2), 16));

      throw new ArgumentException("Hex color must be 6 or 8 characters.");
    }
    public static Color GetOppositeColor(Color original)
    {
      byte r = (byte)(255 - original.R);
      byte g = (byte)(255 - original.G);
      byte b = (byte)(255 - original.B);


      byte a = original.A;

      return new Color(r, g, b, a);
    }

    public static Color Multiply(Color a, Color b)
    {
      return new Color(
          (byte)(a.R * b.R / 255),
          (byte)(a.G * b.G / 255),
          (byte)(a.B * b.B / 255),
          (byte)(a.A * b.A / 255)
      );
    }

    public static Color Hex(string hex)
    {
      hex = hex.TrimStart('#');

      if (hex.Length == 3)
        return new Color(
            Convert.ToByte(hex.Substring(0, 1), 16),
            Convert.ToByte(hex.Substring(1, 1), 16),
            Convert.ToByte(hex.Substring(2, 1), 16));
      if (hex.Length == 4)
        return new Color(
            Convert.ToByte(hex.Substring(0, 1), 16),
            Convert.ToByte(hex.Substring(1, 1), 16),
            Convert.ToByte(hex.Substring(2, 1), 16),
            Convert.ToByte(hex.Substring(3, 1), 16));
      if (hex.Length == 6)
        return new Color(
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16));
      if (hex.Length == 8)
        return new Color(
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16),
            Convert.ToByte(hex.Substring(6, 2), 16));
      
      Debug.WriteLine("Returning Color.White. Hex was invalid, it can only be 3, 4, 6 or 8 digits");
      return Color.White;
    }

  }
}
