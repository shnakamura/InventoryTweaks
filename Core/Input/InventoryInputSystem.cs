using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

[Autoload(Side = ModSide.Client)]
public sealed class InventoryInputSystem : ModSystem
{
    private static int lastTrashedSlot = -1;

    public override void Load()
    {
        base.Load();

        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
        On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
    }

    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var config = ClientConfiguration.Instance;

        if (ItemSlotUtils.IsInventoryContext(context) && config.EnableQuickControl && ItemSlot.ControlInUse)
        {
            lastTrashedSlot = slot;
        }

        return orig(inv, context, slot);
    }

    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (!ItemSlotUtils.IsInventoryContext(context))
        {
            return;
        }

        var config = ClientConfiguration.Instance;

        var canQuickShift = config.EnableQuickShift && ItemSlot.ShiftInUse;
        var canQuickControl = config.EnableQuickControl && ItemSlot.ControlInUse;

        if (canQuickShift)
        {
            var menu = Main.cursorOverride == CursorOverrideID.InventoryToChest && (Main.CreativeMenu.IsShowingResearchMenu() || Main.InReforgeMenu || Main.InGuideCraftMenu);

            if (Main.cursorOverride == -1 || menu)
            {
                return;
            }

            Main.mouseLeftRelease = true;

            ItemSlot.LeftClick(inv, context, slot);
        }

        if (canQuickControl)
        {
            Main.mouseLeftRelease = true;
        }
    }
}