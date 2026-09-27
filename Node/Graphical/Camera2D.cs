using Opal.Managers;
#nullable disable

using Opal.Tools;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Opal.Managers;
using Opal.Params;

namespace Opal.Hierarchy
{
  public class Camera2D : Node2D
  {
    [Export]
    public Vector2 Zoom { get; set; } = Vector2.One;

    [Export]
    public bool Disabled { get; set; } = false;

    [Export]
    public Rectangle Bounds
    {
      get
      {
        float width = Core.Canvas.RenderTarget.Width / Zoom.X;
        float height = Core.Canvas.RenderTarget.Height / Zoom.Y;

        Vector2 posToUse = Transform.Global.Position;

        float left = posToUse.X - width * 0.5f;
        float top = posToUse.Y - height * 0.5f;

        return new Rectangle(
            (int)left,
            (int)top,
            (int)width,
            (int)height
        );
      }
    }

    [Export]
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public Camera2D() { }

    /// <summary>
    /// Returns the rectangle of world space currently visible by this camera
    /// </summary>
    public Rectangle GetWorldViewRectangle(bool useOffset = true)
    {
      Matrix inverse = Matrix.Invert(GetTransform(useOffset));

      Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverse);
      Vector2 bottomRight = Vector2.Transform(
          new Vector2(Core.Canvas.RenderTarget.Width, Core.Canvas.RenderTarget.Height),
          inverse
      );

      var rect = new Rectangle(
          (int)topLeft.X,
          (int)topLeft.Y,
          (int)(bottomRight.X - topLeft.X),
          (int)(bottomRight.Y - topLeft.Y)
      );
    
      Console.WriteLine(rect.ToString());
        
      return rect;
    }

    /// <summary>
    /// Returns the camera transform matrix for spritebatch.
    /// Centers the camera so <see cref="Node2D.Position"/> maps to the center of the canvas.
    /// </summary>
    public Matrix GetTransform(bool useOffset = true)
    {
      Vector2 canvasCenter = new(
          Core.Canvas.RenderTarget.Width * 0.5f,
          Core.Canvas.RenderTarget.Height * 0.5f
      );

      Vector2 cameraPosition = Transform.Global.Position;

      if (useOffset)
        cameraPosition += Offset;

      return
          Matrix.CreateTranslation(new Vector3(-cameraPosition, 0f))
          * Matrix.CreateRotationZ(Transform.Global.Rotation)
          * Matrix.CreateScale(Zoom.X, Zoom.Y, 1f)
          * Matrix.CreateTranslation(new Vector3(canvasCenter, 0f));
    }

    public override void _Process(float delta)
    {
      base._Process(delta);
    }
  }
}
