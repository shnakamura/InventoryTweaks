using System.ComponentModel;
using System.Runtime.CompilerServices;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Core.Configuration;

public sealed partial class ClientConfiguration : ModConfig
{
    public static ClientConfiguration Instance => ModContent.GetInstance<ClientConfiguration>();
    
    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;

    /// <summary>
    ///     Whether hover effects are enabled or not.
    /// </summary>
    [Header("Graphics")]
    [DefaultValue(true)]
    public bool EnableHoverEffects { get; set; } = true;
    
    /// <summary>
    ///     Whether movement effects are enabled or not.
    /// </summary>
    [DefaultValue(true)]
    public bool EnableMovementEffects { get; set; } = true;
    
    /// <summary>
    ///     The draw scale of hovered items.
    /// </summary>
    [Increment(0.05f)]
    [Range(0.8f, 2f)]
    [DefaultValue(1.2f)]
    public float HoveredItemScale { get; set; } = 1.2f;

    /// <summary>
    ///     The draw scale of unhovered items.
    /// </summary>
    [Increment(0.05f)]
    [Range(0.4f, 1f)]
    [DefaultValue(0.8f)]
    public float UnhoveredItemScale { get; set; } = 0.8f;
}