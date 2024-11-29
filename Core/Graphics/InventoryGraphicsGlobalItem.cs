namespace InventoryTweaks.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public sealed class InventoryGraphicsGlobalItem : GlobalItem
{
    private Vector2 inventoryDrawPosition;

    /// <summary>
    ///     The inventory draw scale of the item attached to this global.
    /// </summary>
    public float DrawScale { get; internal set; }

    /// <summary>
    ///     The inventory draw position of the item attached to this global.
    /// </summary>
    public Vector2 InventoryDrawPosition
    {
        get => inventoryDrawPosition;
        internal set
        {
            inventoryDrawPosition = value;

            HasInventoryDrawPosition = true;
        }
    }

    /// <summary>
    ///     Indicates whether the inventory draw position of the item attached to this global has been initialized or not.
    /// </summary>
    /// <remarks>
    ///     This flag ensures that during the first rendering update, the animation of the item starts from the correct
    ///     position,
    ///     rather than defaulting to zero.
    /// </remarks>
    public bool HasInventoryDrawPosition { get; private set; }

    public override bool InstancePerEntity { get; } = true;
}