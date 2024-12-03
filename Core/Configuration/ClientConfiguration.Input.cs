using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Core.Configuration;

public sealed partial class ClientConfiguration : ModConfig
{
    /// <summary>
    ///     Whether quick-shift is enabled or not.
    /// </summary>
    [Header("Input")]
    [DefaultValue(true)]
    public bool EnableQuickShift { get; set; } = true;

    /// <summary>
    ///     Whether quick-control is enabled or not.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableQuickControl { get; set; } = true;

    /// <summary>
    ///     Whether stack refill is enabled or not.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableStackRefill { get; set; } = true;
}