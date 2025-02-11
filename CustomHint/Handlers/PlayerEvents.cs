using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace CustomHint.Handlers
{
    public class PlayerEvents
    {
        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;

            if (player.DoNotTrack)
            {
                if (!Plugin.Instance.HiddenHudPlayers.Remove(player.UserId))
                    return;

                Plugin.Instance.SaveHiddenHudPlayers();
                Log.Debug($"Player {player.Nickname} ({player.UserId}) has DNT enabled and was removed from HiddenHudPlayers.");
            }

            Plugin.Instance.Hints.AssignHints(ev.Player);
        }

        public void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            Plugin.Instance.Hints.AssignHints(ev.Player);
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Player player = ev.Player;

            if (player == null)
            {
                Log.Warn("OnChangingRole: Player is null, skipping hint update.");
                return;
            }

            Log.Debug($"Player {player.Nickname} ({player.UserId}) is changing role to {ev.NewRole}. Removing old hints...");

            Plugin.Instance.Hints.RemoveHints(player);

            Timing.CallDelayed(0.5f, () =>
            {
                if (player.Role.Type == ev.NewRole)
                {
                    Log.Debug($"Assigning new hints to {player.Nickname} ({player.UserId}) after role change...");
                    Plugin.Instance.Hints.AssignHints(player);
                }
                else
                {
                    Log.Warn($"Role mismatch detected for {player.Nickname}. Expected {ev.NewRole}, but got {player.Role.Type}. Skipping hint assignment.");
                }
            });
        }

    }
}
