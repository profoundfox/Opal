using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Opal.Hierarchy
{
  public class CharacterBody2D : PhysicsBody2D 
  {
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 WallNormal { get; private set; } = Vector2.Zero;

    public bool IsOnWall { get; private set; }
    public bool IsOnFloor { get; private set; }
    public bool IsOnRoof { get; private set; }

    public CollisionNode2D WallBody { get; private set; }
    public CollisionNode2D FloorBody { get; private set; }
    
    public Vector2 LastWallGlobalPosition { get; private set; }
    public Vector2 LastFloorGlobalPosition { get; private set; }

    public float WallTolerance { get; set; } = 2f;
    public float FloorTolerance { get; set; } = 2f;
    
    /// <summary>
    /// Uses the provided <see cref="Velocity"/> to move the body. 
    /// </summary>
    public void MoveAndSlide(float delta)
    {
      if (CollisionShapes.Count == 0)
        return;

      Vector2 movement = Velocity * delta;
      
      Platforms();

      var nearby = Core.Physics.Query(Bounds);

      StaticPenetration(nearby);

      IsOnWall = false;
      IsOnFloor = false;
      IsOnRoof = false;
      WallNormal = Vector2.Zero;
      FloorBody = null;
      WallBody = null;

      Horizontal(ref movement, nearby);
      Vertical(ref movement, nearby);
    }
    
    /// <summary>
    /// Handles horizontal collision, sets the <see cref="WallBody"/> and penetration.
    /// </summary>
    private void Horizontal(ref Vector2 movement, List<CollisionNode2D> nearby)
    {
      Vector2 horizontalMovement = new Vector2(movement.X, 0);
      Position += horizontalMovement;

      for (int i = 0; i < nearby.Count; i++)
      {
        var other = nearby[i];

        if (other == this)
          continue;
        
        if (this.Intersects(other))
        {
          IsOnWall = true; 
          WallBody = other;

          LastWallGlobalPosition = other.Transform.Global.Position;

          WallNormal = movement.X > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);

          Position -= horizontalMovement;
          Velocity = new Vector2(0, Velocity.Y);
          break;
        }

        bool nearWall = false;
        Vector2 nearWallNormal = Vector2.Zero;

        if (this.IntersectsAt(new Vector2(WallTolerance, 0), other))
        {
          nearWall = true;
          nearWallNormal = new Vector2(-1, 0);
        }
        else if (this.IntersectsAt(new Vector2(-WallTolerance, 0), other))
        {
          nearWall = true;
          nearWallNormal = new Vector2(1, 0);
        }

        if (nearWall && movement.X != 0)
        {
          IsOnWall = true;
          WallNormal = nearWallNormal;
          break;
        }
      }
    }
    
    /// <summary>
    /// Handles vertical collision, setting the current <see cref="FloorBody"/> and penetration.
    /// </summary>
    private void Vertical(ref Vector2 movement, List<CollisionNode2D> nearby)
    {
      Vector2 verticalMovement = new Vector2(0, movement.Y);
      Position += verticalMovement;

      for (int i = 0; i < nearby.Count; i++)
      {
        var other = nearby[i];

        if (this.Intersects(other))
        {
          if (movement.Y > 0)
          {
            VerticalPenetration(other, true);

            IsOnFloor = true;
            FloorBody = other;
            LastFloorGlobalPosition = other.Transform.Global.Position;
          }
          if (movement.Y < 0)
          {
            VerticalPenetration(other, false);

            IsOnRoof = true;
          }

          Velocity = new Vector2(Velocity.X, 0);
          break;
        }

        if (movement.Y >= 0 && this.IntersectsAt(new Vector2(0, FloorTolerance), other))
        {
          IsOnFloor = true;
          FloorBody = other;
          LastFloorGlobalPosition = other.Transform.Global.Position;
          break;
        }
      }
    }

    /// <summary>
    /// Handles vertical penetration to prevent that the body gets stuck inside a shape. Essentially "pushes" the body out of collision.
    /// </summary>
    /// <remarks>
    /// If the velocity is too high, you can *briefly* clip into the ground. The body will still get pushed out, but it can look a bit off.
    /// </remarks>
    private void VerticalPenetration(CollisionNode2D other, bool fromBottom)
    {
      foreach (var a in this.Bounds)
      {
        foreach (var b in other.Bounds)
        {
          if (!a.Intersects(b))
            continue;

          if (fromBottom)
          {
            float penetration = a.Bottom - b.Top;
            Position -= new Vector2(0, penetration);
          }
          else
          {
            float penetration = b.Bottom - a.Top;
            Position += new Vector2(0, penetration);
          }
        }
      }
    }

    private void StaticPenetration_(List<CollisionNode2D> nearby)
    {
      foreach (var other in nearby.Where(b => b != this))
      {
        if (!this.Intersects(other)) continue;

        foreach (var a in this.Bounds)
          foreach (var b in other.Bounds)
          {
            if (!a.Intersects(b)) continue;

            float moveRight = b.Right - a.Left;
            float moveLeft = a.Right - b.Left;
            float moveDown = b.Bottom - a.Top;
            float moveUp = a.Bottom - b.Top;

            float minX = Math.Min(moveRight, moveLeft);
            float minY = Math.Min(moveDown, moveUp);

            if (minX < minY)
            {
              Position += new Vector2(
                  moveRight < moveLeft ? moveRight : -moveLeft,
                  0);
            }
            else
            {
              Position += new Vector2(
                  0,
                  moveDown < moveUp ? moveDown : -moveUp);
            }
          }
      }
    }
    
    private void StaticPenetration(List<CollisionNode2D> nearby)
    {
      for (int i = 0; i < nearby.Count; i++)
      {
        var other = nearby[i];

        if (other == this)
          continue;

        if (!this.Intersects(other))
          continue;

        foreach (var a in this.Bounds)
          foreach (var b in other.Bounds)
          {
            if (!a.Intersects(b))
              continue;

            float lengthToRight = b.Right - a.Left;
            float lengthToLeft = a.Right - b.Left;
            float lengthToBottom = b.Bottom - a.Top;
            float lengthToTop = a.Bottom - b.Top;

            float minX = Math.Min(lengthToLeft, lengthToRight);
            float minY = Math.Min(lengthToTop, lengthToBottom);

            if (minX < minY)
            {
              Position += new Vector2(
                  lengthToRight < lengthToLeft ? lengthToRight : -lengthToLeft,
                  0);
            }
            else
            {
              Position += new Vector2(
                  0,
                  lengthToBottom < lengthToTop ? lengthToBottom : -lengthToTop);
            }
          }
      }
    }
    
    /// <summary>
    /// Handles moving bodies that interact with the player.
    /// </summary>
    private void Platforms()
    {
      if (IsOnFloor && FloorBody != null)
      {
        Vector2 platformDelta = FloorBody.Transform.Global.Position - LastFloorGlobalPosition;
        Position += platformDelta;
        LastFloorGlobalPosition = FloorBody.Transform.Global.Position;
      }
      if (IsOnWall && WallBody != null)
      {
        Vector2 platformDelta = WallBody.Transform.Global.Position - LastWallGlobalPosition;
        Position += platformDelta;
        LastWallGlobalPosition = WallBody.Transform.Global.Position;
      }
    }
  }
}
