using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Core.Enums;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Core.Tweaks;

public sealed class ItemPickupSystem : ILoadable
{
    void ILoadable.Load(Mod mod)
    {
        IL_ItemSlot.PickupItemIntoMouse += ItemSlot_PickupItemIntoMouse_Edit;
    }

    void ILoadable.Unload() { }
    
    private static void ItemSlot_PickupItemIntoMouse_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After, static i => i.MatchLdcI4(1)))
            {
                throw new Exception();
            }

            c.EmitLdarg0();
            c.EmitLdarg2();

            c.EmitDelegate
            (
                static (int value, Item[] inv, int slot) =>
                {
                    var config = ClientConfiguration.Instance;

                    var item = inv[slot];

                    var stack = config.StackType switch
                    {
                        StackType.Default => value,
                        StackType.Half => item.stack / 2,
                        StackType.Full => item.stack,
                        _ => value
                    };
                    
                    return stack;
                }
            );
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }
}