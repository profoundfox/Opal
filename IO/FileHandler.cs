using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Opal.IO
{
  public static class FileT
  {
    /// <summary>
    /// Writes to binary all primitives, <see cref="Dictionary{TKey, TValue}"/>s and <see cref="Vector2"/>s.
    /// </summary>
    /// <remarks>
    /// If the type is not supported, such as a custom class, it will simply be skipped. There may be functionality for this in the future.
    /// </remarks>
    public static void ToBinary(object info, string fullPath)
    {
      var fields = info.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(f =>
        {
          var value = f.GetValue(info);
          if (value == null) return false;

          var t = f.FieldType;
          return Supported(t);
        }).ToList();

      string directiory = Path.GetDirectoryName(fullPath);
      if (!string.IsNullOrEmpty(fullPath))
        Directory.CreateDirectory(directiory);

      using var bw = new BinaryWriter(File.Open(fullPath, FileMode.Create));

      bw.Write(fields.Count);
      
      for (int i = 0; i < fields.Count; i++)
      {
        var f = fields[i];
        var t = f.FieldType;

        bw.Write(f.Name);

        var value = f.GetValue(info);

        Write(bw, f.FieldType, value);
      };
    }

    private static void Write(BinaryWriter bw, Type t, object value)
    {
      if (t == typeof(int))
        bw.Write((int)value);
      else if (t == typeof(float))
        bw.Write((float)value);
      else if (t == typeof(double))
        bw.Write((double)value);
      else if (t == typeof(bool))
        bw.Write((bool)value);
      else if (t == typeof(string))
        bw.Write((string)value ?? "");
      else if (t == typeof(Vector2))
      {
        Vector2 v = (Vector2)value;
        bw.Write(v.X);
        bw.Write(v.Y);
      }
      else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
      {
        var dict = (System.Collections.IDictionary)value;

        bw.Write(dict.Count);

        Type keyType = t.GetGenericArguments()[0];
        Type valueType = t.GetGenericArguments()[1];

        foreach (System.Collections.DictionaryEntry entry in dict)
        {
          Write(bw, keyType, entry.Key);
          Write(bw, valueType, entry.Value);
        }
      }
      else if (t.IsEnum)
      {
        Type underlying = Enum.GetUnderlyingType(t);
        object rawValue = Convert.ChangeType(value, underlying);
        Write(bw, underlying, rawValue);
      }
    }

    private static bool Supported(Type t)
    {
      if (t.IsPrimitive || t == typeof(string) || t == typeof(Vector2) || t.IsEnum)
        return true;

      if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
      {
        var genericArguments = t.GetGenericArguments();
        return Supported(genericArguments[0]) && Supported(genericArguments[1]);
      }

      return false;
    }
    
    /// <summary>
    /// Reads from binary and translates all primitives, <see cref="Dictionary{TKey, TValue}"/>s and <see cref="Vector2"/>s.
    /// </summary>
    /// <remarks>
    /// If the type is not supported, such as a custom class, it will simply be skipped. There may be functionality for this in the future.
    /// </remarks>
    public static void FromBinary(object target, string fullPath)
    {
      if (!File.Exists(fullPath))
        return;

      using var br = new BinaryReader(File.Open(fullPath, FileMode.Open));

      int fieldCounter = br.ReadInt32();

      var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .ToDictionary(f => f.Name, f => f);

      for (int i = 0; i < fieldCounter; i++)
      {
        string name = br.ReadString();

        if (fields.TryGetValue(name, out var f))
        {
          object value = Read(br, f.FieldType);
          
          f.SetValue(target, value);
        }
      }
    }

    private static object Read(BinaryReader br, Type t)
    {
      if (t == typeof(int))
        return br.ReadInt32();
      else if (t == typeof(float))
        return br.ReadSingle();
      else if (t == typeof(double))
        return br.ReadDouble();
      else if (t == typeof(bool))
        return br.ReadBoolean();
      else if (t == typeof(string))
        return br.ReadString();
      else if (t == typeof(Vector2))
      {
        var x = br.ReadSingle();
        var y = br.ReadSingle();
        return new Vector2(x, y);
      }
      else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
      {
        var dict = (System.Collections.IDictionary)Activator.CreateInstance(t);

        int count = br.ReadInt32();

        Type keyType = t.GetGenericArguments()[0];
        Type valueType = t.GetGenericArguments()[1];

        for (int i = 0; i < count; i++)
        {
          object key = Read(br, keyType);
          object val = Read(br, valueType);

          dict.Add(key, val);
        }
        return dict;
      }
      else if (t.IsEnum)
      {
        Type underlyingType = Enum.GetUnderlyingType(t);
        object rawValue = Read(br, underlyingType);
        return Enum.ToObject(t, rawValue);
      }

      return null;
    }
  }
}
