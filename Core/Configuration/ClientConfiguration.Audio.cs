using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Core.Configuration;

public sealed partial class ClientConfiguration : ModConfig
{
    /// <summary>
    ///     Whether inventory sounds are enabled or not.
    /// </summary>
    [Header("Audio")]
    [DefaultValue(true)]
    public bool EnableInventorySounds { get; set; } = true;
}