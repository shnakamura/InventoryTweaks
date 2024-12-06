namespace InventoryTweaks.Core.Enums;

[Flags]
public enum StackType
{
    Default = 0,
    Half = 1 << 0,
    Full = 1 << 1 
}