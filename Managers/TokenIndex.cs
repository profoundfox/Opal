using Opal.Managers;
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Opal.Geometry;
using Opal.Graphics;
using Opal.Params;
using Opal.Runtime;
using Opal.Tools;
using Opal.Tools;
using Microsoft.Xna.Framework;

namespace Opal.Managers
{
  public class TokenIndex : Loop
  {
    private readonly List<Token> tokens = new();
    private readonly Dictionary<string, List<Token>> byName = new();

    private readonly List<Token> iterationBuffer = new();

    private readonly List<Token> pendingAdd = new();
    private readonly List<Token> pendingRemove = new();
    
    /// <summary>
    /// The manager for <see cref="Opal.Tools.Anchor"/>. Has methods for interacting with the current one. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Keeps track of the <see cref="Token"/> instances chained to the current anchor -- if it is deactivated -- all chained are removed.
    /// </para>
    /// <para>
    /// The usual interface for using anchors is <see cref="Scene"/>, which has general lifecycle functions -- though, you can just use an anchor.
    /// </para>
    /// </remarks>
    public AnchorManager Anchor { get; private set; }

    public TokenIndex()
    {
      Anchor = new AnchorManager();
    }
    
    ///<summary>
    /// Wrapper for creating an <see cref="Token"/>. 
    ///</summary>
    ///<remarks>
    /// It is highly encouraged to use this as the only method of created tokens,
    /// to ensure the engine has control over the order of operations.
    ///</remarks>
    ///<returns>The token which has been created, so it can be continually modfied.</returns>
    public T Create<T>()
        where T : Token, new()
    {
      var inst = new T();

      return inst;
    }

    ///<summary>
    /// Wrapper for creating a <see cref="Tween{T}"/>
    ///</summary>
    ///<returns> The tween which has been created, so it can be continually modified.</returns>
    public Tween<T> CreateTween<T>(
        Action<T> setter,
        T start,
        T end,
        float duration,
        Func<T, T, float, T> lerpFunc,
        Func<float, float> easingFunction = null)
    {
      if (setter == null)
        throw new ArgumentNullException(nameof(setter));

      if (lerpFunc == null)
        throw new ArgumentNullException(nameof(lerpFunc));

      if (easingFunction == null)
        easingFunction = EasingFunctions.Linear;

      return new Tween<T>(
          start,
          end,
          duration,
          lerpFunc,
          setter,
          easingFunction
      );
    }


    /// <summary>
    /// Queues an token to be added to this tree.
    /// </summary>
    /// <param name="token"></param>
    public void QueueAdd(Token token)
    {
      pendingAdd.Add(token);
      
      Anchor.GetCurrentAnchor()?.Attach(token);
    }

    /// <summary>
    /// Queues an intance to be removed from this tree.
    /// </summary>
    /// <param name="token"></param>
    public void QueueRemove(Token token) => pendingRemove.Add(token);


    /// <summary>
    /// Flushes all tokens.
    /// </summary>
    internal void Flush()
    {
      if (pendingAdd.Count == 0 && pendingRemove.Count == 0)
        return;
      var toAdd = pendingAdd.ToList();
      pendingAdd.Clear();

      foreach (var n in toAdd)
      {
        AddInternal(n);
      }

      foreach (var n in toAdd)
      {
        if (n is IEnterTree enter)
        {
          enter._EnterTree();
          enter.EnterTree();
        }
      }

      var toRemove = pendingRemove.ToList();
      pendingRemove.Clear();

      foreach (var n in toRemove)
        RemoveInternal(n);
    }

    /// <summary>
    /// Adds an token.
    /// </summary>
    /// <param name="token"></param>
    private void AddInternal(Token token)
    {
      tokens.Add(token);

      if (!string.IsNullOrEmpty(token.Name))
      {
        if (!byName.TryGetValue(token.Name, out var list))
        {
          list = new List<Token>();
          byName[token.Name] = list;
        }
        list.Add(token);
      }
    }

    /// <summary>
    /// Removes an token.
    /// </summary>
    /// <param name="token"></param>
    private void RemoveInternal(Token token)
    {
      if (token is IExitTree i)
      {
        i._ExitTree();
        i.ExitTree();
      }

      tokens.Remove(token);

      if (!string.IsNullOrEmpty(token.Name)
          && byName.TryGetValue(token.Name, out var list))
      {
        list.Remove(token);
        if (list.Count == 0)
          byName.Remove(token.Name);
      }

      token.ClearData();
    }

    /// <summary>
    /// Removes an token without queueing.
    /// </summary>
    /// <param name="token"></param>
    internal void RemoveNow(Token token) => RemoveInternal(token);

    /// <summary>
    /// Frees and removes all tokens immediately.
    /// </summary>
    public void Clear()
    {
      foreach (var token in tokens.ToList())
      {
        RemoveInternal(token);
      }

      tokens.Clear();
      byName.Clear();

      pendingAdd.Clear();
      pendingRemove.Clear();
    }


    public override void _Process(TimeSpan delta)
    {
      Flush();

      iterationBuffer.Clear();
      iterationBuffer.AddRange(tokens);

      float dt = (float)delta.TotalSeconds;

      for (int i = 0; i < iterationBuffer.Count; i++)
      {
        var inst = iterationBuffer[i];

        if (inst is IProcess processor && !pendingRemove.Contains(inst))
        {
          processor._Process(dt);
          processor.Process(dt);
        }

        if (inst is ICall caller && !pendingRemove.Contains(inst))
        {
          caller._Submit(Core.Canvas);
          caller.Submit(Core.Canvas);
        }
      }

      iterationBuffer.Clear();
      Flush();
    }

    public override void _PhysicsProcess(TimeSpan delta)
    {
      Flush();

      iterationBuffer.Clear();
      iterationBuffer.AddRange(tokens);

      float dt = (float)delta.TotalSeconds;

      for (int i = 0; i < iterationBuffer.Count; i++)
      {
        var inst = iterationBuffer[i];

        if (inst is IPhysicsProcess physics && !pendingRemove.Contains(inst))
        {
          physics._PhysicsProcess((float)delta.TotalSeconds);
          physics.PhysicsProcess((float)delta.TotalSeconds);
        }
      }

      iterationBuffer.Clear();
      Flush();
    }

    /// <summary>
    /// Gets the first token by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Token Get(string name)
        => GetAll(name).FirstOrDefault();

    /// <summary>
    /// Gets all tokens by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IReadOnlyList<Token> GetAll(string name)
        => string.IsNullOrEmpty(name)
            ? Array.Empty<Token>()
            : byName.TryGetValue(name, out var list)
                ? list
                : Array.Empty<Token>();

    /// <summary>
    /// Gets the first token by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : Token
        => GetAll<T>().FirstOrDefault();

    /// <summary>
    /// Gets all tokens by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReadOnlyList<T> GetAll<T>() where T : Token
        => tokens.OfType<T>().ToList();

    /// <summary>
    /// Gets all tokens 
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Token> GetAll()
    {
      return tokens.AsReadOnly();
    }

  }
}
