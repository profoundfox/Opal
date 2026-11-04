using Opal.Managers;
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Opal.Input
{
  public class KeyboardInfo : InputInfo<Keys>
  {
    /// <summary>
    /// Gets the keyboard's previous state state, that is to say from the previous update cycle.
    /// </summary>
    public KeyboardState PreviousState { get; private set; }

    /// <summary>
    /// Gets the keyboard's current state.
    /// </summary>
    public KeyboardState CurrentState { get; private set; }

    public KeyboardInfo()
    {
      PreviousState = new KeyboardState();
      CurrentState = Keyboard.GetState();
    }
    
    /// <summary>
    /// Updates the keyboard states, setting both the current frame's, and the previous'.
    /// </summary>
    public void Update()
    {
      PreviousState = CurrentState;
      CurrentState = Keyboard.GetState();
    }
    
    /// <summary>
    /// Returns the first key detected as held down this update cycle.
    /// </summary>
    public Keys GetDown()
    {
      return CurrentState.GetPressedKeys().FirstOrDefault();
    }
    
    /// <summary>
    /// Returns whether the inputed key is held down this update cycle.
    /// </summary>
    /// <param name="key">The key in question.</param>
    public bool IsDown(Keys key)
    {
      return CurrentState.IsKeyDown(key);
    }
    
    /// <summary>
    /// Returns the first key detected as up (not held down) this update cycle.
    /// </summary>
    public Keys GetUp()
    {
      var pressed = CurrentState.GetPressedKeys();
      var values = Enum.GetValues<Keys>();

      for (int i = 0; i < values.Length; i++)
      {
        var value = values[i];

        if (pressed.Contains(value))
          return value;
      }
      return Keys.None;
    }
      
    /// <summary>
    /// Returns whether the inputed key is up (not held down) this update cycle.
    /// </summary>
    /// <param name="key">The key in question.</param>
    public bool IsUp(Keys key)
    {
      return CurrentState.IsKeyUp(key);
    }
    
    /// <summary>
    /// Returns the first key detected as being "just pressed" this update cycle; previously up, now down.
    /// </summary>
    public Keys GetJustPressed()
    {
      var curPressedKeys = CurrentState.GetPressedKeys();
      for (int i = 0; i < curPressedKeys.Length; i++)
      {
        var key = curPressedKeys[i];
        if (PreviousState.IsKeyUp(key))
          return key;
      }
      return Keys.None;
    }
    
    /// <summary>
    /// Returns whether the inputed key has been "just pressed" this update cyle; previously up, now down.
    /// </summary>
    /// <param name="key">The key in question.</param>
    public bool WasJustPressed(Keys key)
    {
      return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }
    
    /// <summary>
    /// Returns the first key detected as being "just released" this update cycle; previously down, now up.
    /// </summary>
    public Keys GetJustReleased()
    {
      var prevPressedKeys = PreviousState.GetPressedKeys();
      for (int i = 0; i < prevPressedKeys.Length; i++)
      {
        var key = prevPressedKeys[i];
        if (CurrentState.IsKeyUp(key))
          return key;
      }
      return Keys.None;
    }
    
    /// <summary>
    /// Returns whether the inputed key has been "just released" this update cycle; previously down, now up.
    /// </summary>
    ///<param name="key">The key in question.</param>
    public bool WasJustReleased(Keys key)
    {
      return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
    }

    public int GetAxis(Keys negative, Keys positive)
    {
      return (IsDown(positive) ? 1 : 0) - (IsDown(negative) ? -1 : 0);
    }

    public Point GetAxis(Keys negativeX, Keys positiveX, Keys negativeY, Keys positiveY)
    {
      return new Point(GetAxis(negativeX, positiveX), GetAxis(negativeY, positiveY));
    }
  }
}
