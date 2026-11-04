using Opal.Managers;
#nullable disable

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Opal.Input
{
  public class GamePadInfo : InputInfo<Buttons>
  {
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

    /// <summary>
    /// Gets the index of the player this gamepad is for.
    /// </summary>
    public PlayerIndex PlayerIndex { get; }


    /// <summary>
    /// Gets the state of input for this gamepad during the previous update cycle.
    /// </summary>
    public GamePadState PreviousState { get; private set; }

    /// <summary>
    /// Gets the state of input for this gamepad during the current update cycle.
    /// </summary>
    public GamePadState CurrentState { get; private set; }

    /// <summary>
    /// Gets a value that indicates if this gamepad is currently connected.
    /// </summary>
    public bool IsConnected => CurrentState.IsConnected;


    /// <summary>
    /// Gets the value of the left thumbstick of this gamepad.
    /// </summary>
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

    /// <summary>
    /// Gets the value of the right thumbstick of this gamepad.
    /// </summary>
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    /// <summary>
    /// Gets the value of the left trigger of this gamepad.
    /// </summary>
    public float LeftTrigger => CurrentState.Triggers.Left;

    /// <summary>
    /// Gets the value of the right trigger of this gamepad.
    /// </summary>
    public float RightTrigger => CurrentState.Triggers.Right;

    /// <summary>
    /// Creates a new GamePadInfo for the gamepad connected at the specified player index.
    /// </summary>
    /// <param name="playerIndex">The index of the player for this gamepad.</param>
    public GamePadInfo(PlayerIndex playerIndex)
    {
      PlayerIndex = playerIndex;
      PreviousState = new GamePadState();
      CurrentState = GamePad.GetState(playerIndex);
    }

    /// <summary>
    /// Updates the state information for this gamepad input.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
      PreviousState = CurrentState;
      CurrentState = GamePad.GetState(PlayerIndex);

      if (_vibrationTimeRemaining > TimeSpan.Zero)
      {
        _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

        if (_vibrationTimeRemaining <= TimeSpan.Zero)
        {
          StopVibration();
        }
      }
    }

    /// <summary>
    /// Sets the vibration for all motors of this gamepad.
    /// </summary>
    /// <param name="strength">The strength of the vibration from 0.0f (none) to 1.0f (full).</param>
    /// <param name="time">The amount of time the vibration should occur.</param>
    public void SetVibration(float strength, TimeSpan time)
    {
      _vibrationTimeRemaining = time;
      GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    /// <summary>
    /// Stops the vibration of all motors for this gamepad.
    /// </summary>
    public void StopVibration()
    {
      GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }

    public Buttons GetDown()
    {
      foreach (var button in ButtonArrays.AllGamePadButtons)
      {
        if (CurrentState.IsButtonDown(button))
          return button;
      }
      return Buttons.None;
    }

    public bool IsDown(Buttons button)
    {
      return CurrentState.IsButtonDown(button);
    }

    public Buttons GetUp()
    {
      foreach (var button in ButtonArrays.AllGamePadButtons)
      {
        if (CurrentState.IsButtonUp(button))
          return button;
      }
      return Buttons.None;
    }

    public bool IsUp(Buttons button)
    {
      return CurrentState.IsButtonUp(button);
    }

    public Buttons GetJustPressed()
    {
      foreach (var button in ButtonArrays.AllGamePadButtons)
      {
        if (PreviousState.IsButtonUp(button) && CurrentState.IsButtonDown(button))
          return button;
      }
      return Buttons.None;
    }

    public bool WasJustPressed(Buttons button)
    {
      return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    public Buttons GetJustReleased()
    {
      foreach (var button in ButtonArrays.AllGamePadButtons)
      {
        if (PreviousState.IsButtonDown(button) && CurrentState.IsButtonUp(button))
          return button;
      }
      return Buttons.None;
    }

    public bool WasJustReleased(Buttons button)
    {
      return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }

    public int GetAxis(Buttons negative, Buttons positive)
    {
      return (IsDown(positive) ? 1 : 0) + (IsDown(negative) ? -1 : 0);
    }

    public Point GetAxis(Buttons negativeX, Buttons positiveX, Buttons negativeY, Buttons positiveY)
    {
      return new Point(GetAxis(negativeX, positiveX), GetAxis(negativeY, positiveY));
    }
  }
}
