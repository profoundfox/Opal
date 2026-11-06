using Opal.Managers;
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Opal.Geometry;
using Opal.Managers;

namespace Opal.Hierarchy
{
  public class StaticBody2D : PhysicsBody2D
  {
    public StaticBody2D() { }

    public override void _EnterTree()
    {
      base._EnterTree();
    }

    public override void _PhysicsProcess(float delta)
    {
      base._PhysicsProcess(delta);
    }
  }

}
