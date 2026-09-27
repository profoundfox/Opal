using Opal.Managers;

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Opal.Geometry;
using Opal.Tools;

namespace Opal.Hierarchy
{
  ///<summary>
  /// The class for all bodies which posses phsyics and are required to be queued from the server.
  ///</summary>
  public class PhysicsBody2D : CollisionNode2D 
  {
    public PhysicsBody2D() { }

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
    }

    public override void _Process(float delta)
    {
      base._Process(delta);
    }

    public override void _Submit(Canvas2D canvas)
    {
      base._Submit(canvas);
    }
  }
}
