using Opal.Managers;
#nullable disable

using Opal.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Opal.Params;
using Opal.Graphics;
using Opal.Tools;
using System;
using System.Collections.Generic;

namespace Opal.Hierarchy
{
  public class ParticleEmitter2D : Node2D
  {
    private readonly List<Particle> _particles = [];

    private float IntervalLeft;

    [Export]
    public EmitterParams Params { get; set; } = EmitterParams.Identity;

    [Export]
    public IReadOnlyList<Particle> Particles => _particles;

    public ParticleEmitter2D()
    {
      IntervalLeft = Params.Interval;
    }

    private void Spawn(Vector2 position)
    {
      ParticleParams d = Params.Params;

      d.Lifespan = MathE.RandomFloat(Params.LifespanMin, Params.LifespanMax);
      d.Speed = MathE.RandomFloat(Params.SpeedMin, Params.SpeedMax);
      d.Angle = MathE.RandomFloat(
          Params.Angle - Params.AngleVariance,
          Params.Angle + Params.AngleVariance);

      Particle p = new(position, d);
      _particles.Add(p);
    }

    /// <summary>
    /// Emits the particle at the emitter's positon and with the default count.
    /// </summary>
    public void Emit()
    {
      Emit(Transform.Global.Position, Params.EmitCount);
    }

    /// <summary>
    /// Emits the particle at the emitter's position with the specified count.
    /// </summary>
    /// <param name="count"></param>
    public void Emit(int count)
    {
      Emit(Transform.Global.Position, count);
    }

    /// <summary>
    /// Emits the particles with a specified position and count.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="count"></param>
    public void Emit(Vector2 position, int count = 1)
    {
      for (int i = 0; i < count; i++)
      {
        Spawn(position);
      }
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      IntervalLeft -= delta;

      while (IntervalLeft <= 0f)
      {
        IntervalLeft += Params.Interval;

        Emit();
      }

      for (int i = 0; i < _particles.Count; i++)

      {
        var p = _particles[i];
        p.Modulate(delta);
      }

      _particles.RemoveAll(p => p.IsFinished);
    }

    public override void _Submit(Canvas2D canvas)
    {
      base._Submit(canvas);

      foreach (var particle in _particles)
      {
        var call = ObjectPool<TextureDrawCall>.Get();

        call.Texture = particle.Params.Texture;

        call.Params = CanvasParams.Identity with
        {
          Position = particle.Position,
          Color = particle.Color * particle.Opacity,
          Rotation = 0f,
          Origin = particle.Origin,
          Scale = new Vector2(particle.Scale),
        };
        call.Key = BatchKey.Default with
        {
          Matrix = Core.Token.Get<Camera2D>().GetTransform()
        };
        call.Depth = 99;

        Core.Canvas.Submit(call);
      }
    }
  }
}
