using System;
using Microsoft.Xna.Framework;

namespace Opal.Input
{
  public interface InputInfo<T> where T : Enum
  {
    T GetDown();
    bool IsDown(T input);

    T GetUp();
    bool IsUp(T input);

    T GetJustPressed();
    bool WasJustPressed(T input);

    T GetJustReleased();
    bool WasJustReleased(T input);

    int GetAxis(T negative, T positive);
    Point GetAxis(T negativeX, T positiveX, T negativeY, T positiveY);
  }
}
