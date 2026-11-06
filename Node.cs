using Opal.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Opal.Managers;
using Opal.Runtime;

namespace Opal.Hierarchy
{
  public class Node : Token,
    IEnterTree,
    IPhysicsProcess,
    IProcess,
    ICall,
    IExitTree
  {
    private readonly List<Node> children = new();
    private Node parent;

    public IReadOnlyList<Node> Children => children.AsReadOnly();

    /// <summary>
    /// Signal for when a parent is changed.
    /// </summary>
    public event Action<Node> OnParentChanged;

    /// <summary>
    /// Signal for when a child is added.
    /// </summary>
    public event Action<Node> OnChildAdded;

    /// <summary>
    /// Signal for when a child is removed.
    /// </summary>
    public event Action<Node> OnChildRemoved;

    public Node()
    {
      _Ready();
      Ready();
    }

    /// <summary>
    /// Returns the generic type parent of this node.
    /// </summary>
    /// <returns></returns>
    public Node GetParent()
    {
      return parent;
    }

    /// <summary>
    /// Returns the speficic type parent of this node.
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <returns></returns>
    public TParent GetParent<TParent>() where TParent : Node
    {
      return parent as TParent;
    }

    /// <summary>
    /// Sets the parent of this node.
    /// </summary>
    /// <param name="newParent"></param>
    /// <exception cref="Exception"></exception>
    public void SetParent(Node newParent)
    {
      if (newParent == this)
        throw new Exception("Node attempted to become its own parent!");

      if (parent == newParent)
        return;

      if (WouldCreateCycle(newParent))
        throw new Exception("Parenting would create a cycle in the node tree.");

      parent?.children.Remove(this);
      parent = newParent;
      parent?.children.Add(this);

      OnParentChanged?.Invoke(newParent);
    }

    /// <summary>
    /// Checks if a new parent would create a cycle.
    /// A - B - A
    /// </summary>
    /// <param name="newParent"></param>
    /// <returns></returns>
    private bool WouldCreateCycle(Node newParent)
    {
      var p = newParent;
      while (p != null)
      {
        if (p == this) return true;
        p = p.parent;
      }
      return false;
    }

    /// <summary>
    /// Adds a child to this node.
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(Node child)
    {
      if (child == null || children.Contains(child))
        return;

      child.SetParent(this);

      OnChildAdded?.Invoke(child);
    }

    /// <summary>
    /// Gets the first specified child.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : Node
    {
      return children.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets all specified children.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReadOnlyList<T> GetAll<T>() where T : Node
    {
      return children.OfType<T>().ToList();
    }

    /// <summary>
    /// Removes a child from this node.
    /// </summary>
    /// <param name="child"></param>
    public void RemoveChild(Node child)
    {
      if (child == null || !children.Contains(child))
        return;

      child.SetParent(null);

      OnChildRemoved?.Invoke(child);
    }

    /// <summary>
    /// Clears the parent and children.
    /// </summary>
    internal override void ClearData()
    {
      base.ClearData();

      GetParent()?.RemoveChild(this);

      foreach (var child in Children.ToList())
        child.FreeImmediate();

      parent = null;
      children.Clear();
    }
    
    public virtual void _Ready() { }
    public virtual void Ready() { }

    public virtual void _EnterTree() { }
    public virtual void EnterTree() { }

    public virtual void _ExitTree() { }
    public virtual void ExitTree() { }

    public virtual void _Process(float delta) { }
    public virtual void Process(float delta) { }

    public virtual void _PhysicsProcess(float delta) { }
    public virtual void PhysicsProcess(float delta) { }

    public virtual void _Submit(Canvas2D canvas) { }
    public virtual void Submit(Canvas2D canvas) { }
  }
}

