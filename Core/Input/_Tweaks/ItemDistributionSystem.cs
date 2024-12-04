/*
using InventoryTweaks.Core.Configuration;
using MonoMod.Cil;
using Terraria.Audio;
using Terraria.UI;

namespace InventoryTweaks.Core.Tweaks;

public sealed class ItemDistributionSystem : ILoadable
{
    /// <summary>
    ///     The index of the last item slot that the player used to distribute an item.
    /// </summary>
    public static int LastInsertionSlot { get; private set; } = -1;

    public static bool IsInserting { get; private set; }
    
    void ILoadable.Load(Mod mod)
    {
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
        On_ItemSlot.OverrideHover_ItemArray_int_int += ItemSlot_OverrideHover_Hook;
        On_ItemSlot.RefreshStackSplitCooldown += ItemSlot_RefreshStackSplitCooldown_Hook;
        
        On_Main.DoDraw += Main_DoDraw_Hook;
    }

    private static void ItemSlot_OverrideHover_Hook(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        
        IsInserting = false;

        var config = ClientConfiguration.Instance;

        if (!config.EnableInsertion || Main.mouseItem.IsAir)
        {
            return;
        }

        var item = inv[slot];
        
        if (!Main.mouseRight || (!Main.mouseRightRelease && slot == LastInsertionSlot))
        {
            return;
        }
        
        IsInserting = true;
        
        if (item.IsAir)
        {
            item.SetDefaults(Main.mouseItem.type);
        }
        else
        {
            if (config.EnableInventorySounds)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            
            item.stack++;
        }
            
        Main.mouseItem.stack--;
        
        LastInsertionSlot = slot;
    }

    void ILoadable.Unload() { }
    
    private static void ItemSlot_RefreshStackSplitCooldown_Hook(On_ItemSlot.orig_RefreshStackSplitCooldown orig)
    {
        orig();
        
        if (!IsInserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;     
    }

    private static void Main_DoDraw_Hook(On_Main.orig_DoDraw orig, Main self, GameTime gametime)
    {
        orig(self, gametime);
        
        if (!IsInserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;        
    }
    
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);


    }
}
*/