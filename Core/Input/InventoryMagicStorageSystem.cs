using System.Runtime.CompilerServices;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

[ExtendsFromMod("MagicStorage")]
[JITWhenModsEnabled("MagicStorage")]
public sealed class InventoryMagicStorageSystem : ModSystem
{
    public override void Load()
    {
        base.Load();

        On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
    }

    /// <summary>
    ///     Checks whether the storage interface from Magic Storage is open or not.
    /// </summary>
    /// <returns><c>true</c> if the storage interface is open; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStorageUIOpen()
    {
        return false;
    }

    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
    }
}