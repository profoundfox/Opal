using Opal.Managers;
#nullable disable

using System;
using Microsoft.Xna.Framework;
using Opal.Params;
using Opal.Hierarchy;

namespace Opal.Graphics
{
  public class Particle : BaseObject
  {
    private readonly ParticleParams _initialData;

    public ParticleParams Params { get => _initialData; }
    
    public Vector2 Position { get; set; }
    public float LifespanLeft { get; set; }
    public float LifespanAmount { get; set; }
    public Color Color { get; set; }
    public float Opacity { get; set; }
    public bool IsFinished { get; set; }
    public float Scale { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 Gravity { get; set; }

    public Particle(Vector2 pos, ParticleParams data)
    {
      _initialData = data;

      LifespanLeft = _initialData.Lifespan;
      LifespanAmount = 1f;
      Position = pos;
      Color = _initialData.ColorStart;
      Opacity = _initialData.OpacityStart;
      Origin = new(
          _initialData.Texture.Width / 2,
          _initialData.Texture.Height / 2
      );

      if (_initialData.Speed != 0)
      {
        Direction = new Vector2(
            (float)Math.Sin(_initialData.Angle),
            (float)Math.Cos(_initialData.Angle)
        );
      }
      else
      {
        Direction = Vector2.Zero;
      }
    }

    public void Modulate(float delta)
    {
      LifespanLeft -= delta;

      if (LifespanLeft <= 0f)
      {
        IsFinished = true;
        return;
      }

      LifespanAmount = MathHelper.Clamp(
          LifespanLeft / _initialData.Lifespan,
          0,
          1
      );

      Color = Color.Lerp(
          _initialData.ColorEnd,
          _initialData.ColorStart,
          LifespanAmount
      );

      Opacity = MathHelper.Clamp(
          MathHelper.Lerp(
              _initialData.OpacityEnd,
              _initialData.OpacityStart,
              LifespanAmount
          ),
          0,
          1
      );

      Scale = MathHelper.Lerp(
          _initialData.SizeEnd,
          _initialData.SizeStart,
          LifespanAmount
      ) / _initialData.Texture.Width;

      Position += Direction * _initialData.Speed * delta;
    }

  }
}
