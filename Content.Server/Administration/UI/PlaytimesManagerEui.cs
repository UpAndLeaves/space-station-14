using Content.Server.Administration.Managers;
using Content.Server.Chat;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.EUI;
using Content.Server.Players.PlayTimeTracking;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Shared.Player;

namespace Content.Server.Administration.UI
{
    public sealed partial class PlaytimesManagerEui : BaseEui
    {
        [Dependency] private IAdminManager _adminManager = default!;
        [Dependency] private PlayTimeTrackingManager _ptMan = default!;
        [Dependency] private ISharedPlayerManager _player = default!;


        public PlaytimesManagerEui()
        {
            IoCManager.InjectDependencies(this);
        }

        public override void Opened()
        {
            StateDirty();
        }

        public override EuiStateBase GetNewState()
        {
            return new PlaytimesManagerEuiState();
        }

        public override void HandleMessage(EuiMessageBase msg)
        {
            base.HandleMessage(msg);

            switch (msg)
            {
                case PlaytimesManagerEuiMsg.PlaytimesData playtimesData:
                    if (!_adminManager.HasAdminFlag(Player, AdminFlags.Admin) || !_player.TryGetSessionById(playtimesData.PlayerID, out var session))
                    {
                        Close();
                        break;
                    }

                    _ptMan.UpdateTimeToOverallPlaytime(session, playtimesData.OverallPlaytime, playtimesData.UpdateType);

                    foreach (var pair in playtimesData.RolesPlaytimeList)
                    {
                        _ptMan.UpdateTimeToTracker(session, pair.Key, pair.Value, playtimesData.UpdateType);
                    }

                    StateDirty();
                    break;
            }
        }
    }
}
