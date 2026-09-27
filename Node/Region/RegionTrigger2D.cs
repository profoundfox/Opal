using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Opal.Managers;
using Opal.Params;

namespace Opal.Hierarchy
{
  public class RegionNode2D : CollisionNode2D
  {
    [Export]
    public bool Monitoring { get; set; } = true;

    [Export]
    public bool Monitorable { get; set; } = true;

    public event Action<RegionNode2D> RegionInside;
    public event Action<PhysicsBody2D> BodyInside;

    private List<RegionNode2D> _regionCache = new List<RegionNode2D>();
    private List<PhysicsBody2D> _bodyCache = new List<PhysicsBody2D>();

    public bool OverlappingRegions(List<RegionNode2D> regions)
    {
      if (!Monitoring)
        return false;
      
      var foundOverlapping = false;

      var query = Core.Physics.Query(Bounds);
      for (int i = 0; i < query.Count; i++)
      {
        var body = query[i];
        if (body is RegionNode2D region && region.Monitorable && body.Intersects(this))
        {
          regions.Add(region);
          foundOverlapping = true;
        }
      }

      return foundOverlapping;
    }

    public bool OverlappingBodies(List<PhysicsBody2D> bodies)
    {
      if (!Monitoring)
        return false;

      var foundOverlapping = false;

      var query = Core.Physics.Query(Bounds);
      for (int i = 0; i < query.Count; i++)
      {
        var body = query[i];
        if (body is PhysicsBody2D pBody && pBody.Intersects(this))
        {
          bodies.Add(pBody);
          foundOverlapping = true;
        }
      }

      return foundOverlapping;
    }

    public List<RegionNode2D> GetOverlappingRegions()
    {
      _regionCache.Clear();
      OverlappingRegions(_regionCache); 
      return _regionCache;
    }

    public List<PhysicsBody2D> GetOverlappingBodies()
    {
      _bodyCache.Clear();
      OverlappingBodies(_bodyCache);
      return _bodyCache;
    }

    public bool HasOverlappingAreas()
    {
      if (!Monitoring)
        return false;

      var query = Core.Physics.Query(Bounds);
      for (int i = 0; i < query.Count; i++)
      {
        var body = query[i];
        if (body is RegionNode2D region && region.Monitorable && body.Intersects(this))
          return true;
      }

      return false;
    }
    
    public bool HasOverlappingBodies()
    {
      if (!Monitoring)
        return false;

      var query = Core.Physics.Query(Bounds);
      for (int i = 0; i < query.Count; i++)
      {
        var body = query[i];
        if (body is PhysicsBody2D pBody && pBody.Intersects(this))
          return true;
      }
      
      return false;
    }

    public bool OverlapsArea(RegionNode2D area)
    {

      return this.Intersects(area) && Monitoring;
    }

    public bool OverlapsBody(PhysicsBody2D body)
    {
      return this.Intersects(body) && Monitoring;
    }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _Process(float delta)
    {
      base._Process(delta);
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);
    }

    public override void _Submit(Canvas2D canvas)
    {
      base._Submit(canvas);
    }

    public override void _ExitTree()
    {
      base._ExitTree();
    }
  }
}
