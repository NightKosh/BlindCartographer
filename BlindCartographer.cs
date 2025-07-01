using BlindCartographer.Loader;
using BlindCartographer.Ui;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace BlindCartographer
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class BlindCartographer : BaseUnityPlugin
    {
        private const string PluginGuid = "nightkosh." + PluginName;
        private const string PluginName = "BlindCartographer";
        private const string PluginVersion = "1.0.0";
        public static ConfigEntry<KeyCode> ConfigPinKey;
        private static ConfigEntry<bool> _modEnabled;
        public static ConfigEntry<bool> _enableCartographyTableInNoMapMode;
        public static ConfigEntry<bool> _enableHotKey;
        public static ConfigEntry<bool> _showHotKeyText;

        public static Harmony Harmony;

        private GameObject _lastHoveredObject;
        private string _lastObjectName;
        private bool _showPopup;
        private string _textInput = "";

        public void Awake()
        {
            _modEnabled = Config.Bind("General", "ModEnabled", true, "Enable or disable the mod.");
            _enableCartographyTableInNoMapMode = Config.Bind("Map", "EnableCartographyTableInNoMapMode", true, "Enable cartography table in no map mode.");
            _enableHotKey = Config.Bind("Hotkeys", "EnableHotKey", true, "Enable hot key.");
            _showHotKeyText = Config.Bind("Hotkeys", "ShowHotKeyText", true, "Show hot key text.");
            ConfigPinKey = Config.Bind("Hotkeys", "HotKey", KeyCode.T,
                "Key to press with LEFT ALT to pin hovered object to the map");
            if (_modEnabled.Value)
            {
                Harmony = Harmony.CreateAndPatchAll(typeof(BlindCartographer).Assembly, PluginGuid);
                HoverTextPatchLoader.ApplyPatches();
            }
        }

        private void Update()
        {
            if (!_modEnabled.Value) return;
            if (IsAnyGameUIOpen()) return;
            if (_enableHotKey.Value && Input.GetKeyDown(ConfigPinKey.Value) && Input.GetKey(KeyCode.LeftAlt))
            {
                var player = Player.m_localPlayer;
                if (player == null) return;

                GetHoverObject(player);

                TextInput.instance.RequestText(
                    new PinTextReceiver(_lastObjectName, AddToMap),
                    "Enter pin name",
                    255
                );
            }
        }

        private bool IsAnyGameUIOpen()
        {
            return InventoryGui.IsVisible()
                   || Minimap.IsOpen()
                   || TextInput.IsVisible()
                   || StoreGui.IsVisible()
                   || Chat.instance?.HasFocus() == true
                   || Console.IsVisible()
                   || Menu.IsVisible();
        }

        private void GetHoverObject(Player player)
        {
            _lastHoveredObject = null;
            _lastObjectName = "";
            var hoverObject = player.GetHoverObject();
            if (hoverObject != null)
            {
                var hoverable = hoverObject.GetComponentInParent<Hoverable>();
                if (hoverable != null)
                {
                    _lastHoveredObject = hoverObject;

                    var hoverText = hoverable.GetHoverText();
                    if (!string.IsNullOrEmpty(hoverText))
                    {
                        var split = hoverText.Split('\n');
                        if (split.Length > 1)
                            _lastObjectName = split[0];
                    }
                }
            }
        }

        private void AddToMap(string pinName)
        {
            var minimap = Minimap.instance;
            if (minimap == null) return;
            var position = _lastHoveredObject == null
                ? Player.m_localPlayer.transform.position
                : _lastHoveredObject.transform.position;

            minimap.AddPin(position, Minimap.PinType.Icon3, pinName, true, false);

            Player.m_localPlayer?.Message(
                MessageHud.MessageType.TopLeft, 
                $"Pin `{pinName}` added!");
        }
    }
}
