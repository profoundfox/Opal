using Opal.Managers;
#nullable disable

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Opal.Geometry;
using Opal.Params;
using Opal.Tools;
using Opal.Graphics;
using Opal.Tools;

namespace Opal.Hierarchy
{
  public class CollisionShape2D : Node2D
  {
    private bool _oneWayState = false;

    [Export]
    public bool Disabled { get; set; }
    [Export]
    public bool OneWay { get; set; }
    [Export]
    public IShape2D Shape { get; set; }

    [Export]
    public int Width
    {
      get => Shape?.Size.Width ?? 0;
    }

    [Export]
    public int Height
    {
      get => Shape?.Size.Height ?? 0;
    }

    public CollisionShape2D() { }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _ExitTree()
    {
      base._ExitTree();
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
      CheckOneWay();
    }

    public override void _Submit(Canvas2D canvas)
    {
      base._Submit(canvas);

      if (!Core.Prefs.General.ShowCollision)
        return;

      Color color;
      if (Disabled)
        color = Color.Gray;
      else
        color = Color.Blue;

      var call = ObjectPool<TextureDrawCall>.Get();

      if (Shape is RectangleShape2D rs)
      {
        call.Texture = Core.Resources.Pixel;
        call.Depth = 99;
        call.Params = CanvasParams.Identity with
        {
          Scale = new Vector2(rs.Size.Width, rs.Size.Height),
          Color = color * 0.5f,
          Position = Transform.Global.Position
        };
        call.Key = BatchKey.Default with
        {
          Matrix = Core.Token.Get<Camera2D>()?.GetTransform()
        };
      }

      if (Shape is CircleShape2D cs)
      {
        call.Texture = GraphicsE.CreateCircle(cs.Radius);
        call.Depth = 99;
        call.Params = CanvasParams.Identity with
        {
          Color = color * 0.5f,
          Position = Transform.Global.Position - new Vector2(cs.Radius)
        };
        call.Key = BatchKey.Default with
        {
          Matrix = Core.Token.Get<Camera2D>()?.GetTransform()
        };
      }

      Core.Canvas.Submit(call);
    }

    private void CheckOneWay()
    {
      if (!OneWay || Shape == null)
        return;

      var kb = Core.Token.Get<CharacterBody2D>();

      if (kb == null)
        return;

      var thisTop = Shape.GetAABB(Transform.Global.Position.ToPoint()).Top;
      var thatBottom = kb.CollisionShapes[0].Shape.GetAABB(kb.Transform.Global.Position.ToPoint()).Bottom;

      if (thatBottom > thisTop)
        Disabled = true;
      else
        Disabled = false;
    }

    public bool Intersects(CollisionShape2D other)
    {
      if (Disabled || other?.Shape == null || other.Disabled || Shape == null)
        return false;

      return Shape.Intersect(other.Shape, Transform.Global.Position.ToPoint(), other.Transform.Global.Position.ToPoint());
    }

    public bool IntersectsAt(Vector2 offset, CollisionShape2D other)
    {
      if (Disabled || other?.Shape == null || other.Disabled || Shape == null)
        return false;

      return Shape.IntersectsAt(offset.ToPoint(), other.Shape, Transform.Global.Position.ToPoint(), other.Transform.Global.Position.ToPoint());
    }

    public bool Contains(Vector2 position)
    {
      if (Disabled || Shape == null)
        return false;
        
      return Shape.Contains(position.ToPoint(), Transform.Global.Position.ToPoint());
    }


    public CollisionShape2D Clone()
    {
      IShape2D clonedShape = Shape?.Clone();

      return new CollisionShape2D()
      {
        Disabled = this.Disabled,
        OneWay = this.OneWay,
        Shape = clonedShape,
      };
    }
  }
}
