using Opal.Managers;

using Microsoft.Xna.Framework;
using Opal.Graphics;

namespace Opal.Params
{
  public record struct ParticleParams
  {
    public TextureRegion Texture { get; set; }
    public float Lifespan { get; set; }
    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }
    public float OpacityStart { get; set; }
    public float OpacityEnd { get; set; }
    public float SizeStart { get; set; }
    public float SizeEnd { get; set; }
    public float Speed { get; set; }
    public float Angle { get; set; }
    public Vector2 Gravity { get; set; }

    public static readonly ParticleParams Identity = new(
        texture: Core.Resources.Pixel, lifespan: 2f, colorStart: Color.Yellow,
        colorEnd: Color.Red, opacityStart: 1f,
        opacityEnd: 0f, sizeStart: 32f, sizeEnd: 4f,
        speed: 100f, angle: 0f, Vector2.Zero
    );

    public ParticleParams(
        TextureRegion texture, float lifespan,
        Color colorStart, Color colorEnd, float opacityStart,
        float opacityEnd, float sizeStart, float sizeEnd,
        float speed, float angle, Vector2 gravity)
    {
      Texture = texture;
      Lifespan = lifespan;
      ColorStart = colorStart;
      ColorEnd = colorEnd;
      OpacityStart = opacityStart;
      OpacityEnd = opacityEnd;
      SizeStart = sizeStart;
      SizeEnd = sizeEnd;
      Speed = speed;
      Angle = angle;
      Gravity = gravity;
    }
  }

}
