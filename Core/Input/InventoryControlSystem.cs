using InventoryTweaks.Core.Configuration;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

[Autoload(Side = ModSide.Client)]
public sealed class InventoryControlSystem : ModSystem
{
    public override void Load() {
        base.Load();
        
        On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
    }

    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        orig(inv, context, slot);

        var config = ClientConfiguration.Instance;

        if (!config.EnableQuickControl || !ItemSlot.ControlInUse) {
            return;
        }

        Main.mouseLeftRelease = true;
    }
}