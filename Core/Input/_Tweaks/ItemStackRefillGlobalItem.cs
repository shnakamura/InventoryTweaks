using InventoryTweaks.Core.Configuration;
using Terraria.Audio;

namespace InventoryTweaks.Core.Tweaks;

public sealed class ItemStackRefillGlobalItem : GlobalItem
{
    public override void OnConsumeItem(Item item, Player player)
    {
        base.OnConsumeItem(item, player);

        var config = ClientConfiguration.Instance;

        if (!config.EnableStackRefill)
        {
            return;
        }

        var refill = item == player.HeldItem && item.stack - 1 <= 0;

        if (!refill)
        {
            return;
        }

        var length = player.inventory.Length;
        var indices = new int[length];
        
        for (var i = 0; i < length; i++)
        {
            indices[i] = i;
        }

        Array.Sort(indices, static (left, right) =>
        {
            var leftItem = Main.LocalPlayer.inventory[left];
            var rightItem = Main.LocalPlayer.inventory[right];

            return rightItem.stack.CompareTo(leftItem.stack);
        });

        for (var i = 0; i < length; i++)
        {
            var index = indices[i];
            var inventory = player.inventory[index];

            if (!inventory.IsAir && inventory != player.HeldItem && inventory.type == player.HeldItem.type)
            {
                if (config.EnableInventorySounds)
                {
                    SoundEngine.PlaySound(in SoundID.MenuTick);
                }
                
                var stack = inventory.stack;
                var value = Math.Min(inventory.stack, player.HeldItem.maxStack);

                inventory.stack -= value;
                
                player.HeldItem.stack += value;
                
                break;
            }
        }
    }
}