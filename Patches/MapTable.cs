using HarmonyLib;

namespace BlindCartographer.Patches
{
    [HarmonyPatch(typeof(MapTable), "OnRead", typeof(Switch), typeof(Humanoid), typeof(ItemDrop.ItemData),
        typeof(bool))]
    public static class MapTableOnReadPatch
    {
        public static void Postfix()
        {
            if (BlindCartographer._enableCartographyTableInNoMapMode.Value && Game.m_noMap)
            {
                Game.m_noMap = false;
                Minimap.instance.SetMapMode(Minimap.MapMode.Large);
                Game.m_noMap = true;
            }
        }
    }
}
