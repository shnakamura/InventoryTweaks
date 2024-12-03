using InventoryTweaks.Utilities;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace InventoryTweaks.Core.Input;

public sealed class MouseRefillSystem : ModSystem
{
    /// <summary>
    /// 
    /// </summary>
    public const int INVENTORY_LENGTH = 57;

    public override void PostUpdateInput()
    {
        base.PostUpdateInput();

        if (!InputUtils.JustMiddleClicked)
        {
            return;
        }
        
        RefillMouseItem();
    }

    private static void RefillMouseItem()
    {
        if (Main.mouseItem.IsAir || Main.mouseItem.IsFull())
        {
            return;
        }
        
        for (var i = 0; i < INVENTORY_LENGTH; i++)
        {
            var item = Main.LocalPlayer.inventory[i];

            if (item.IsAir || item.type != Main.mouseItem.type)
            {
                continue;
            }

            var stack = Main.mouseItem.maxStack - Main.mouseItem.stack;
            var value = Math.Min(item.stack, stack);

            item.stack -= value;

            Main.mouseItem.stack += value;
        }
    }
}