using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using MagicStorage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

public sealed partial class QuickActionSystem : ILoadable
{
    public static int LastTrashSlot { get; private set; } = -1;
    
    public static int LastRightSlot { get; private set; } = -1;
    
    // public static int LastInsertionSlot { get; private set; } = -1;
    
    void ILoadable.Load(Mod mod)
    {
        // On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
        
        IL_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Edit;
    }

    void ILoadable.Unload() { }

    /*
    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (Main.mouseItem.IsAir || !Main.mouseRight || slot == LastInsertionSlot)
        {
            return;
        }

        var item = inv[slot];
        
        if (!item.IsAir && item.type != Main.mouseItem.type)
        {
            return;
        }

        if (item.IsAir)
        {
            item.SetDefaults(Main.mouseItem.type);
        }
        else
        {
            item.stack++;
        }

        LastInsertionSlot = slot;
    }
    */
    
    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);
        
        var config = ClientConfiguration.Instance;

        if (result && ItemSlotUtils.IsInventoryContext(context) && config.EnableQuickControl && ItemSlot.ControlInUse)
        {
            LastTrashSlot = slot;
        }

        return result;
    }
    
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        
        LastRightSlot = slot;
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
                    static (bool flag, int slot) =>
                    {
                        return slot != LastRightSlot || Main.mouseRightRelease;
                    }
                );
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
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
            
            c.EmitDelegate(static (Item[] inv, int context, int slot, ref bool flag) =>
            {
                var config = ClientConfiguration.Instance;
                
                var hasAction = Main.cursorOverride != -1;

                flag |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft
                        && config.EnableQuickShift
                        && ItemSlot.ShiftInUse
                        && hasAction;

                flag |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft
                        && config.EnableQuickControl
                        && ItemSlot.ControlInUse
                        && slot != LastTrashSlot
                        && hasAction;
                
                if (!ModLoader.HasMod("MagicStorage"))
                {
                    return;
                }
                
                flag |= ItemSlotUtils.IsInventoryContext(context)
                        && Main.mouseLeft 
                        && config.EnableQuickShift 
                        && ItemSlot.ShiftInUse 
                        && HasStorageUIEnabled(inv, context, slot);
            });
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