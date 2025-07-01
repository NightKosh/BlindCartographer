namespace BlindCartographer.Modifier
{
    public static class HoverTextModifier
    {
        public static void AddHoverText<T>(T __instance, ref string __result)
        {
            if (__instance == null && !(BlindCartographer._enableHotKey.Value && BlindCartographer._showHotKeyText.Value)) return;
            __result += $"\n[<color=yellow>LEFT ALT + {BlindCartographer.ConfigPinKey.Value}</color>] Pin to the map";
        }
    }
}
