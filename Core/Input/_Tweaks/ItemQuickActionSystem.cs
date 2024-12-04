using System.Runtime.CompilerServices;
using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Core.Tweaks;

public sealed partial class ItemQuickActionSystem : ILoadable
{
    /// <summary>
    ///     The index of the last item slot that the player used to trash an item.
    /// </summary>
    public static int LastTrashSlot { get; private set; } = -1;
    
    /// <summary>
    ///     The index of the last item slot that the player used to equip an item.
    /// </summary>
    public static int LastEquipSlot { get; private set; } = -1;
    
    void ILoadable.Load(Mod mod)
    {
        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
        
        IL_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Edit;
        IL_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Edit;
    }

    void ILoadable.Unload() { }

    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);
        
        var config = ClientConfiguration.Instance;

        if (result && config.EnableQuickControl && ItemSlot.ControlInUse && ItemSlotUtils.IsInventoryContext(context))
        {
            LastTrashSlot = slot;
        }

        return result;
    }
    
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        
        var config = ClientConfiguration.Instance;
        
        if (!config.EnableQuickShift || !ItemSlotUtils.IsInventoryContext(context))
        {
            return;
        }
        
        LastEquipSlot = slot;
    }

    private static void ItemSlot_LeftClick_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            
            if (!c.TryGotoNext(MoveType.Before, static i => i.MatchStloc1()))
            {
                throw new Exception();
            }

            c.Index++;

            c.EmitLdarg0();
            c.EmitLdarg1();
            c.EmitLdarg2();

            c.EmitLdloca(1);
            
            c.EmitDelegate(static (Item[] inv, int context, int slot, ref bool value) =>
            {
                var config = ClientConfiguration.Instance;
                
                var hasAction = Main.cursorOverride != -1;

                value |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft
                        && config.EnableQuickShift
                        && ItemSlot.ShiftInUse
                        && hasAction;

                value |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft
                        && config.EnableQuickControl
                        && ItemSlot.ControlInUse
                        && slot != LastTrashSlot
                        && hasAction;
                
                if (!ModLoader.HasMod("MagicStorage"))
                {
                    return;
                }
                
                value |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft 
                        && config.EnableQuickShift 
                        && ItemSlot.ShiftInUse 
                        && MagicStorageUtils.IsStorageOpen(inv, context, slot);
            });
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }
    
    private static void ItemSlot_RightClick_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            while (c.TryGotoNext(MoveType.After, static i => i.MatchLdsfld<Main>(nameof(Main.mouseRightRelease))))
            {
                c.EmitLdarg2();
                
                c.EmitDelegate
                (
                    static (bool value, int slot) =>
                    {
                        return slot != LastEquipSlot || Main.mouseRightRelease;
                    }
                );
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanShiftClick(Item[] inv, int context, int slot)
    {
        var hasContext = Math.Abs(context) == 10 && ItemSlot.isEquipLocked(inv[slot].type);
        
        var hasShift = ItemSlot.ShiftInUse;

		var hasOverride = Main.cursorOverride == CursorOverrideID.InventoryToChest
		                  || Main.cursorOverride == CursorOverrideID.ChestToInventory
		                  || Main.cursorOverride == CursorOverrideID.BackInventory
		                  || Main.cursorOverride == CursorOverrideID.FavoriteStar
		                  || Main.cursorOverride == CursorOverrideID.Magnifiers;

        var hasTileEntity = Main.LocalPlayer.tileEntityAnchor.IsInValidUseTileEntity() 
                            && Main.LocalPlayer.tileEntityAnchor.GetTileEntity().OverrideItemSlotLeftClick(inv, context, slot);

        return hasContext || hasShift || hasOverride || hasTileEntity;
    }
}