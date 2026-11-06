using Opal.Managers;

namespace Opal.Runtime
{
  public interface IPhysicsUpdate
  {
    /// <summary>
    /// The 'production' function called every frame with a fixed delta.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Should only be used for specifc use cases where a stable delta is required (such as physics).
    /// </para>
    /// <para>
    /// For production functions you need to call <c>base._PhysicsUpdate(delta);</c>;
    /// a rule of thumb for production functions vs. regular ones is whether it will be inherited by others, if yes – use this.
    /// </para>
    /// </remarks>
    /// <param name="delta">The space between frames.</param>

    
    void _PhysicsUpdate(float delta);
    /// <summary>
    /// The 'regular' function called every frame with a fixed delta.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In opposition to production functions, you do not need to call <c>base.PhysicsUpdate(delta)</c> – it is intended for classes that do not have children (e.g a player).
    /// </para>
    /// <para>
    /// Note that if you plan to create a system of classes working together (with inheritance), use instead <see cref="_PhysicsUpdate(float)"/>.
    /// </para>
    /// </remarks>
    /// <param name="delta">The space between frames.</param>
    void PhysicsUpdate(float delta); 
  }
}
