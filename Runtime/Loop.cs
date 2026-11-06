using Opal.Managers;
using System;
using System.Collections.Generic;
using Opal.Hierarchy;
using Opal.Tools;

namespace Opal.Runtime
{
  public abstract class Loop : BaseObject
  {
    private readonly Queue<Action> continuations = new();

    internal void Update(TimeContext context, int steps)
    {
      if (steps < 0)
        throw new ArgumentOutOfRangeException(nameof(steps));

      for (int i = 0; i < steps; i++)
      {
        _PhysicsProcess(context.FixedDelta);
      }

      _Process(context.FrameDelta);

      int count = continuations.Count;
      for (int i = 0; i < count; i++)
      {
        continuations.Dequeue().Invoke();
      }
    }

    internal void Post(Action action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof(action));

      continuations.Enqueue(action);
    }

    public virtual void _PhysicsProcess(TimeSpan delta) { }
    public virtual void _Process(TimeSpan delta) { }
  }
}
