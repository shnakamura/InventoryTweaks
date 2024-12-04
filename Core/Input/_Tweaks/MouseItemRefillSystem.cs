using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Core.Input;
using InventoryTweaks.Utilities;
using Terraria.Audio;

namespace InventoryTweaks.Core.Tweaks;

public sealed class MouseItemRefillSystem : ModSystem
{
    public const int INVENTORY_LENGTH = 57;

    public override void PostUpdateInput()
    {
        base.PostUpdateInput();

        if (!KeybindSystem.MouseRefillKeybind.JustPressed)
        {
            return;
        }
        
        RefillMouseItem();
    }

    private static void RefillMouseItem()
    {
        var config = ClientConfiguration.Instance;
        
        if (!config.EnableMouseRefill || Main.mouseItem.IsAir || Main.mouseItem.IsFull())
        {
            return;
        }

        var indices = new int[INVENTORY_LENGTH];
        
        for (var i = 0; i < INVENTORY_LENGTH; i++)
        {
            indices[i] = i;
        }

        Array.Sort(indices, static (left, right) =>
        {
            var leftItem = Main.LocalPlayer.inventory[left];
            var rightItem = Main.LocalPlayer.inventory[right];

            return leftItem.stack.CompareTo(rightItem.stack);
        });
        
        for (var i = 0; i < INVENTORY_LENGTH; i++)
        {
            var index = indices[i];
            var item = Main.LocalPlayer.inventory[index];

            if (item.IsAir || item.type != Main.mouseItem.type)
            {
                continue;
            }
            
            if (config.EnableInventorySounds)
            {
                SoundEngine.PlaySound(in SoundID.MenuTick);
            }

            var stack = Main.mouseItem.maxStack - Main.mouseItem.stack;
            var value = Math.Min(item.stack, stack);

            item.stack -= value;

            Main.mouseItem.stack += value;
        }
    }
}