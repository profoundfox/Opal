using Opal.Managers;
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Opal.Tools;
using Opal.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Opal.Managers
{
  public partial class InputManager : BaseObject
  {
    public KeyboardInfo Keyboard { get; private set; }
    public MouseInfo Mouse { get; private set; }
    public GamePadInfo[] GamePads { get; private set; }
    public GamePadInfo CurrentGamePad { get; private set; }

    public InputManager()
    {
      Keyboard = new KeyboardInfo();
      Mouse = new MouseInfo();

      GamePads = new GamePadInfo[4];
      for (int i = 0; i < 4; i++)
        GamePads[i] = new GamePadInfo((PlayerIndex)i);

      CurrentGamePad = GamePads[0];
    }

    /// <summary>
    /// Updates the keyboard, mouse, and all gamepads.
    /// Also updates the currently active gamepad.
    /// </summary>
    public void Update(GameTime gameTime)
    {
      Keyboard.Update();
      Mouse.Update();

      foreach (var pad in GamePads)
        pad.Update(gameTime);

      CurrentGamePad = GamePads[0];
    }
  }
}
