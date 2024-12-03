using System.Reflection;
using MagicStorage;
using MagicStorage.UI.States;

namespace InventoryTweaks.Core.Compatibility;

[JITWhenModsEnabled("MagicStorage")]
public sealed class MagicStoragePatches : ILoadable
{
    private delegate Item GetRecipeCallback(CraftingUIState.RecipesPage self, int slot, ref int context);

    private delegate Item GetHeaderCallback(int slot, ref int context);
    
    void ILoadable.Load(Mod mod)
    {
        MonoModHooks.Add(typeof(CraftingUIState).GetNestedType("RecipesPage").GetMethod("GetRecipe", BindingFlags.NonPublic | BindingFlags.Instance), CraftingUIState_GetRecipe_Hook);
        MonoModHooks.Add(typeof(CraftingGUI).GetMethod("GetHeader", BindingFlags.NonPublic | BindingFlags.Static), CraftingGUI_GetHeader_Hook);
    }
    
    void ILoadable.Unload() { }

    private static Item CraftingUIState_GetRecipe_Hook(GetRecipeCallback orig, CraftingUIState.RecipesPage self, int slot, ref int context)
    {
        return orig(self, slot, ref context).Clone();
    }
   
    private static Item CraftingGUI_GetHeader_Hook(GetHeaderCallback orig, int slot, ref int context)
    {
        return orig(slot, ref context).Clone();
    }
}