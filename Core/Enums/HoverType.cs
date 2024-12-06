namespace InventoryTweaks.Core.Enums;

[Flags]
public enum HoverType
{
    None = 0,
    Mouse = 1 << 0,
    Hover = 1 << 1,
    Held = 1 << 2,
    All = 1 << 3
}