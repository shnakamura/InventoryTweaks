using System.Reflection;
using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

[Autoload(Side = ModSide.Client)]
public sealed class InventoryShiftSystem : ModSystem
{
    /// <summary>
    ///     The rate at which input can be processed.
    /// </summary>
    public const int INPUT_RATE = 2;

    /// <summary>
    ///     The cooldown of the current input action, in ticks.
    /// </summary>
    public static int InputCooldown { get; private set; }

    public override void Load() {
        base.Load();

        On_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Hook;
        On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
    }

    private static void ItemSlot_LeftClick_Hook(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        var config = ClientConfiguration.Instance; 
        
        if (Main.cursorOverride != -1 || !ItemSlot.Equippable(inv, context, slot) || !ItemSlot.ShiftInUse || !config.EnableQuickShift || !Main.mouseLeft || !Main.mouseLeftRelease) {
            orig(inv, context, slot);
        }
        else {
            ItemSlot.SwapEquip(inv, context, slot);
        }
    }

    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot) {
        orig(inv, context, slot);

        if (!ItemSlotUtils.IsInventoryContext(context)) {
            return;
        }
        
        var config = ClientConfiguration.Instance;      

        var menu = Main.cursorOverride == CursorOverrideID.InventoryToChest && (Main.CreativeMenu.IsShowingResearchMenu() || Main.InReforgeMenu || Main.InGuideCraftMenu);
        
        if (!config.EnableQuickShift || !ItemSlot.ShiftInUse || Main.cursorOverride == -1 || menu) {
            return;
        }

        Main.mouseLeftRelease = true;

        ItemSlot.LeftClick(inv, context, slot);
    }
}
