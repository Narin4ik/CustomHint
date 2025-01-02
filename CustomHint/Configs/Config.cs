using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace CustomHint.Configs
{
    public class Config : IConfig
    {
        [Description("Plugin enabled (bool)?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug mode?")]
        public bool Debug { get; set; } = false;

        [Description("Enable or disable HUD-related commands.")]
        public bool EnableHudCommands { get; set; } = true;

        [Description("Enable or disable automatic plugin updates.")]
        public bool AutoUpdater { get; set; } = true;

        [Description("Enable or disable game hints.")]
        public bool GameHint { get; set; } = true;

        [Description("Enable or disable hints for spectators.")]
        public bool HintForSpectatorsIsEnabled { get; set; } = true;

        [Description("The interval for changing spectator hints (in seconds).")]
        public float HintMessageTime { get; set; } = 5f;

        [Description("Default role name for players without a role.")]
        public string DefaultRoleName { get; set; } = "Player";

        [Description("Default role color (for players without roles).")]
        public string DefaultRoleColor { get; set; } = "white";

        [Description("Server timezone for placeholder. Use 'UTC' by default or a valid timezone ID (e.g., 'Europe/Kyiv').")]
        public string ServerTimeZone { get; set; } = "UTC";

        [Description("Enable counting Overwatch players in placeholder {spectators_num}.")]
        public bool EnableOverwatchCounting { get; set; } = true;

        [Description("List of custom HUD configurations.")]
        public List<CustomHud> Huds { get; set; } = new();
    }

    public class CustomHud
    {
        [Description("HUD text with placeholders.")]
        public string Text { get; set; }

        [Description("Can this HUD be hidden by the .hidehud command?")]
        public bool CanBeHidden { get; set; }

        [Description("Roles to which this HUD will be displayed.")]
        public List<RoleTypeId> Roles { get; set; } = new();

        [Description("HUD position settings.")]
        public HudPosition Position { get; set; } = new();
    }

    public class HudPosition
    {
        [Description("X position of the HUD.")]
        public float PositionX { get; set; } = 0f;

        [Description("Y position of the HUD.")]
        public float PositionY { get; set; } = 500f;
    }
}
