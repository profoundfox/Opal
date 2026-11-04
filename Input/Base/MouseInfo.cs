using Opal.Managers;
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Opal.Input
{
  public class MouseInfo : InputInfo<MouseButton>
  {
    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; }

    public Point Position
    {
      get => CurrentState.Position;
      set => SetPosition(value.X, value.Y);
    }

    public int X
    {
      get => CurrentState.X;
      set => SetPosition(value, CurrentState.Y);
    }

    public int Y
    {
      get => CurrentState.Y;
      set => SetPosition(CurrentState.X, value);
    }

    public Point PositionDelta => CurrentState.Position - PreviousState.Position;
    public int XDelta => CurrentState.X - PreviousState.X;
    public int YDelta => CurrentState.Y - PreviousState.Y;
    public bool WasMoved => PositionDelta != Point.Zero;
    public int ScrollWheel => CurrentState.ScrollWheelValue;
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    public MouseInfo()
    {
      PreviousState = new MouseState();
      CurrentState = Mouse.GetState();
    }

    public void Update()
    {
      PreviousState = CurrentState;
      CurrentState = Mouse.GetState();
    }

    private ButtonState GetButtonState(MouseState state, MouseButton button)
    {
      return button switch
      {
        MouseButton.Left => state.LeftButton,
        MouseButton.Middle => state.MiddleButton,
        MouseButton.Right => state.RightButton,
        MouseButton.XButton2 => state.XButton2,
        MouseButton.XButton1 => state.XButton1,
        _ => ButtonState.Released
      };
    }

    public void SetPosition(int x, int y)
    {
      Mouse.SetPosition(x, y);
      CurrentState = new MouseState(
          x, y,
          CurrentState.ScrollWheelValue,
          CurrentState.LeftButton,
          CurrentState.MiddleButton,
          CurrentState.RightButton,
          CurrentState.XButton1,
          CurrentState.XButton2
      );
    }

    public MouseButton GetDown()
    {
      for (int i = 0; i < ButtonArrays.AllMouseButtons.Length; i++)
      {
        var button = ButtonArrays.AllMouseButtons[i];
        if (GetButtonState(CurrentState, button) == ButtonState.Pressed)
          return button;
      }
      return MouseButton.None;
    }

    public bool IsDown(MouseButton button)
    {
      return GetButtonState(CurrentState, button) == ButtonState.Pressed;
    }

    public MouseButton GetUp()
    {
      for (int i = 0; i < ButtonArrays.AllMouseButtons.Length; i++)
      {
        var button = ButtonArrays.AllMouseButtons[i];
        if (GetButtonState(CurrentState, button) == ButtonState.Released)
          return button;
      }
      return MouseButton.None;
    }

    public bool IsUp(MouseButton button)
    {
      return GetButtonState(CurrentState, button) == ButtonState.Released;
    }

    public MouseButton GetJustPressed()
    {
      for (int i = 0; i < ButtonArrays.AllMouseButtons.Length; i++)
      {
        var button = ButtonArrays.AllMouseButtons[i];
        if (GetButtonState(CurrentState, button) == ButtonState.Pressed
            && GetButtonState(PreviousState, button) == ButtonState.Released)
          return button;
      }
      return MouseButton.None;
    }

    public bool WasJustPressed(MouseButton button)
    {
      return GetButtonState(CurrentState, button) == ButtonState.Pressed &&
        GetButtonState(PreviousState, button) == ButtonState.Released;
    }

    public MouseButton GetJustReleased()
    {
      for (int i = 0; i < ButtonArrays.AllMouseButtons.Length; i++)
      {
        var button = ButtonArrays.AllMouseButtons[i];
        if (GetButtonState(CurrentState, button) == ButtonState.Released &&
            GetButtonState(PreviousState, button) == ButtonState.Pressed)
          return button;
      }
      return MouseButton.None;
    }

    public bool WasJustReleased(MouseButton button)
    {
      return GetButtonState(CurrentState, button) == ButtonState.Released &&
        GetButtonState(PreviousState, button) == ButtonState.Pressed;
    }

    public int GetAxis(MouseButton negative, MouseButton positive)
    {
      return (IsDown(positive) ? 1 : 0) - (IsDown(negative) ? -1 : 0);
    }

    public Point GetAxis(MouseButton negativeX, MouseButton positiveX, MouseButton negativeY, MouseButton positiveY)
    {
      return new Point(GetAxis(negativeX, positiveX), GetAxis(negativeY, positiveY));
    }
  }
}
