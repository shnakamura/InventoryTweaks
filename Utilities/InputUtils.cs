using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace InventoryTweaks.Utilities;

public static class InputUtils
{
    public static bool JustMiddleClicked => PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed && PlayerInput.MouseInfoOld.MiddleButton == ButtonState.Released;
}