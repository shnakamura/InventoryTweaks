using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Core.Graphics;
using Terraria.Audio;

namespace InventoryTweaks.Core.Input;

public sealed class StackRefillGlobalItem : GlobalItem
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

        for (var i = 0; i < player.inventory.Length; i++)
        {
            var inventory = player.inventory[i];

            if (!inventory.IsAir && inventory != player.HeldItem && inventory.type == player.HeldItem.type)
            {
                var stack = inventory.stack;
                var value = Math.Min(inventory.stack, player.HeldItem.maxStack);

                inventory.stack -= value;
                
                player.HeldItem.stack += value;

                if (config.EnableInventorySounds)
                {
                    SoundEngine.PlaySound(in SoundID.MenuTick, Main.MouseWorld);
                }
                
                break;
            }
        }
    }
}