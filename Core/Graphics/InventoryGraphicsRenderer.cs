using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using Terraria.UI;

namespace InventoryTweaks.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public sealed class InventoryGraphicsRenderer : ILoadable
{
    void ILoadable.Load(Mod mod) {
        On_ItemSlot.DrawItemIcon += ItemSlot_DrawItemIcon_Hook;
    }
    
    void ILoadable.Unload() { }
    
    private static float ItemSlot_DrawItemIcon_Hook(
        On_ItemSlot.orig_DrawItemIcon orig,
        Item item,
        int context,
        SpriteBatch spriteBatch,
        Vector2 screenPositionForItemCenter,
        float scale,
        float sizeLimit,
        Color environmentColor
    ) {
        if (!ItemSlotUtils.IsInventoryContext(context) || !item.TryGetGlobalItem(out InventoryGraphicsGlobalItem graphics)) {
            return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
        }

        var drawPosition = screenPositionForItemCenter;
        var drawScale = scale;
        
        var config = ClientConfiguration.Instance;

        if (config.EnableMovementEffects) {
            if (graphics.HasInventoryDrawPosition) {
                graphics.InventoryDrawPosition = Vector2.SmoothStep(graphics.InventoryDrawPosition, screenPositionForItemCenter, 0.5f);
            }
            else {
                graphics.InventoryDrawPosition = screenPositionForItemCenter;
            }
            
            drawPosition = graphics.InventoryDrawPosition;
        }

        if (config.EnableHoverEffects) {
            var hitbox = new Rectangle(
                (int)screenPositionForItemCenter.X - 20,
                (int)screenPositionForItemCenter.Y - 20,
                40,
                40
            );
            
            var hovering = hitbox.Contains(Main.MouseScreen.ToPoint());

            graphics.DrawScale = MathHelper.SmoothStep(graphics.DrawScale, (hovering || context == ItemSlot.Context.MouseItem) ? config.HoveredItemScale : config.UnhoveredItemScale, 0.5f);
            
            drawScale = graphics.DrawScale;
        }

        return orig(item, context, spriteBatch, drawPosition, drawScale, sizeLimit, environmentColor);
    }
}