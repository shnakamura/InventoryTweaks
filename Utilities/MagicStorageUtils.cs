using MagicStorage;
using Terraria.UI;

namespace InventoryTweaks.Utilities;

[ExtendsFromMod("MagicStorage")]
public static class MagicStorageUtils
{
    [JITWhenModsEnabled("MagicStorage")]
    public static bool IsStorageOpen(Item[] inv, int context, int slot)
    {
        var player = Main.LocalPlayer;

        if (!player.TryGetModPlayer(out StoragePlayer storagePlayer))
        {
            return false;
        }

        var hasContext = context == ItemSlot.Context.InventoryItem
                         || context == ItemSlot.Context.InventoryCoin
                         || context == ItemSlot.Context.InventoryAmmo;

        var item = inv[slot];
        var hasItem = !item.favorited && !item.IsAir;

        var storage = storagePlayer.ViewingStorage();
        var hasStorage = storage.X >= 0 && storage.Y >= 0;
        
        return hasContext && hasItem && hasStorage;
    }
}