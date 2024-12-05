using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CustomHintPlugin
{
    public class Plugin : Plugin<Config, Translations>
    {
        public static Plugin Instance { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public HashSet<string> HiddenHudPlayers { get; private set; } = new HashSet<string>();

        private CoroutineHandle _hintCoroutine;
        private string HudConfig = FileDotNet.GetPath("HiddenHudPlayers.yml");

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private const string RepositoryUrl = "https://api.github.com/repos/BTF-SCPSL/CustomHint/releases/latest";
        private const string UserAgent = "CustomHint-Updater";

        public bool IsHintSystemEnabled { get; internal set; } = true;

        public override string Name => "CustomHint";
        public override string Author => "Narin";
        public override Version Version => new Version(1, 3, 0);
        public override Version RequiredExiledVersion => new Version(8, 14, 0);

        public override void OnEnabled()
        {
            Instance = this;

            if (!Config.IsEnabled)
                return;

            GenerateHintsFile();
            LoadHiddenHudPlayers();
            EventHandlers = new EventHandlers();

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;
            Exiled.Events.Handlers.Player.Verified += EventHandlers.OnPlayerVerified;

            Log.Debug($"{Name} has been enabled.");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;
            Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;

            Timing.KillCoroutines(_hintCoroutine);
            SaveHiddenHudPlayers();

            Instance = null;
            EventHandlers = null;

            Log.Debug($"{Name} has been disabled.");
            base.OnDisabled();
        }

        private async void OnWaitingForPlayers()
        {
            Log.Info($"Current version of the plugin: v{Version}");
            Log.Info("Checking for updates...");

            try
            {
                var latestVersion = await GetLatestVersionAsync();

                if (latestVersion == null)
                {
                    Log.Warn("Failed to fetch the latest plugin version.");
                    return;
                }

                CompareVersions(latestVersion);
            }
            catch (Exception ex)
            {
                Log.Warn($"Error while checking for updates: {ex}");
            }
        }

        private async Task<string> GetLatestVersionAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

                var response = await client.GetAsync(RepositoryUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Warn($"Failed to fetch data from GitHub API. Status code: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var tag = json["tag_name"]?.ToString();
                var assets = json["assets"];

                if (string.IsNullOrEmpty(tag) || assets == null)
                    return null;

                return tag;
            }
        }

        private void CompareVersions(string latestVersion)
        {
            var currentVersion = Version.ToString();
            var latestVersionClean = latestVersion.TrimStart('v');

            int comparison = CompareSemanticVersions(currentVersion, latestVersionClean);

            if (comparison < 0)
            {
                if (Config.AutoUpdater)
                {
                    Log.Warn("Attention! The plugin version is older than the latest version, downloading the update...");
                    Task.Run(() => DownloadAndInstallLatestVersion());
                }
                else
                {
                    Log.Warn($"Attention! The plugin version is older than the latest version, it is recommended to update the plugin: https://github.com/BTF-SCPSL/CustomHint/releases/tag/{latestVersion}");
                }
            }
            else if (comparison == 0)
            {
                Log.Info("No new version has been found. You are on the latest version of the plugin.");
            }
            else
            {
                Log.Info("No new version has been found. You must be using the pre-release version of the plugin.");
            }
        }

        private int CompareSemanticVersions(string current, string latest)
        {
            var currentParts = current.Split('.');
            var latestParts = latest.Split('.');

            for (int i = 0; i < Math.Max(currentParts.Length, latestParts.Length); i++)
            {
                int currentPart = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;
                int latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;

                if (currentPart < latestPart) return -1;
                if (currentPart > latestPart) return 1;
            }

            return 0;
        }

        private async Task DownloadAndInstallLatestVersion()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

                    var response = await client.GetAsync(RepositoryUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        Log.Warn($"Failed to fetch release data. Status code: {response.StatusCode}");
                        return;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);

                    var assets = json["assets"];
                    if (assets == null)
                    {
                        Log.Warn("No assets found in the release.");
                        return;
                    }

                    string pluginsPath = Path.Combine(Paths.Plugins, "CustomHint", "Auto Updater");
                    Directory.CreateDirectory(pluginsPath);

                    string dependenciesPath = Path.Combine(Paths.Plugins, "dependencies");
                    Directory.CreateDirectory(dependenciesPath);

                    foreach (var asset in assets)
                    {
                        var downloadUrl = asset["browser_download_url"]?.ToString();
                        var fileName = asset["name"]?.ToString();

                        if (downloadUrl == null || fileName == null)
                            continue;

                        string targetPath;
                        if (fileName.Equals("CustomHint.dll", StringComparison.OrdinalIgnoreCase) ||
                            fileName.Equals("AdvancedHints.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            targetPath = Path.Combine(Paths.Plugins, fileName);
                        }
                        else if (fileName.Equals("Newtonsoft.Json.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            targetPath = Path.Combine(dependenciesPath, fileName);
                        }
                        else
                        {
                            targetPath = Path.Combine(pluginsPath, fileName);
                        }

                        var fileBytes = await client.GetByteArrayAsync(downloadUrl);
                        File.WriteAllBytes(targetPath, fileBytes);

                        Log.Info($"Downloaded and installed: {fileName} to {targetPath}");
                    }

                    Log.Info("The plugin has been successfully updated! Please restart the server using the command 'sr'.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to download or install the update: {ex}");
            }
        }

        public void LoadHiddenHudPlayers()
        {
            try
            {
                string directory = Path.GetDirectoryName(HudConfig);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(HudConfig))
                {
                    string yamlContent = File.ReadAllText(HudConfig);
                    HiddenHudPlayers = Deserializer.Deserialize<HashSet<string>>(yamlContent) ?? new HashSet<string>();
                    Log.Debug($"Loaded {HiddenHudPlayers.Count} hidden HUD player(s): {string.Join(", ", HiddenHudPlayers)}");
                }
                else
                {
                    HiddenHudPlayers = new HashSet<string>();
                    SaveHiddenHudPlayers();
                    Log.Info("Created new HUD configuration file.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to load hidden HUD players: {ex}");
                HiddenHudPlayers = new HashSet<string>();
            }
        }

        public void SaveHiddenHudPlayers()
        {
            try
            {
                var validPlayers = new HashSet<string>();
                foreach (var userId in HiddenHudPlayers)
                {
                    var player = Player.Get(userId);
                    if (player == null || !player.DoNotTrack)
                    {
                        validPlayers.Add(userId);
                    }
                }

                string yamlContent = Serializer.Serialize(validPlayers);

                yamlContent = "# List of SteamID64 players who have HUD disabled.\n" + yamlContent;

                File.WriteAllText(HudConfig, yamlContent);
                Log.Debug("Saved hidden HUD players.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to save hidden HUD players: {ex}");
            }
        }

        public void GenerateHintsFile()
        {
            string filePath = FileDotNet.GetPath("Hints.txt");

            try
            {
                if (!File.Exists(filePath))
                {
                    string defaultContent = "# List of hints for {hints} placeholder\nHint 1\nHint 2\nHint 3";
                    File.WriteAllText(filePath, defaultContent);
                    Log.Info("Generated Hints.txt config with default content.");
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to create Hints.txt: {ex}");
            }
        }

        public bool IsHudHiddenForPlayer(Player player)
        {
            if (player.DoNotTrack)
                return false;

            return HiddenHudPlayers.Contains(player.UserId);
        }

        public static string ReplaceColorsInString(string input)
        {
            const string serverNamePlaceholder = "[SERVERNAME_PLACEHOLDER]";
            input = input.Replace("{servername}", serverNamePlaceholder);

            Dictionary<string, string> colorMapping = new Dictionary<string, string>
            {
                { "pink", "#FF96DE" },
                { "red", "#C50000" },
                { "brown", "#944710" },
                { "silver", "#A0A0A0" },
                { "light_green", "#32CD32" },
                { "crimson", "#DC143C" },
                { "cyan", "#00B7EB" },
                { "aqua", "#00FFFF" },
                { "deep_pink", "#FF1493" },
                { "tomato", "#FF6448" },
                { "yellow", "#FAFF86" },
                { "magenta", "#FF0090" },
                { "blue_green", "#4DFFB8" },
                { "orange", "#FF9666" },
                { "lime", "#BFFF00" },
                { "green", "#228B22" },
                { "emerald", "#50C878" },
                { "carmine", "#960018" },
                { "nickel", "#727472" },
                { "mint", "#98FF98" },
                { "army_green", "#4B5320" },
                { "pumpkin", "#EE7600" }
            };

            foreach (var pair in colorMapping)
            {
                input = ReplaceIgnoreCase(input, $"<color={pair.Key}>", $"<color={pair.Value}>");
            }

            input = input.Replace(serverNamePlaceholder, "{servername}");

            return input;
        }

        private static string ReplaceIgnoreCase(string input, string oldValue, string newValue)
        {
            int index = input.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);

            while (index != -1)
            {
                input = input.Remove(index, oldValue.Length).Insert(index, newValue);
                index = input.IndexOf(oldValue, index + newValue.Length, StringComparison.OrdinalIgnoreCase);
            }

            return input;
        }
    }
}
