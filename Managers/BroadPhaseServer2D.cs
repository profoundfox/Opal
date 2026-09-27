using Opal.Managers;
#nullable disable

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Opal.Geometry;
using Opal.Hierarchy;

namespace Opal.Managers
{
  public class BroadPhaseServer2D : BaseObject
  {
    private readonly SpatialHash<CollisionNode2D> _broadphase;

    public BroadPhaseServer2D()
    {
      _broadphase = new SpatialHash<CollisionNode2D>(16);
    }

    /// <summary>
    /// Registers a physics body to the server.
    /// </summary>
    public void RegisterBody(CollisionNode2D body)
    {
      _broadphase.Insert(body);
    }

    /// <summary>
    /// Removes a physics body from the server.
    /// </summary>
    public void UnregisterBody(CollisionNode2D body)
    {
      _broadphase.Remove(body);
    }

    /// <summary>
    /// Notify the broadphase that a body moved.
    /// </summary>
    public void NotifyMoved(CollisionNode2D body)
    {
      _broadphase.Update(body);
    }

    /// <summary>
    /// Queries all bodies intersecting the given area.
    /// </summary>
    public List<CollisionNode2D> Query(List<Rectangle> area)
    {
      return _broadphase.Query(area.ToArray());
    }
  }
}
