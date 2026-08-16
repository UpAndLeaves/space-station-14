using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
    public enum PlaytimeUpdateType
    {
        Set,
        Add,
        SetMinimum
    }

    [Serializable, NetSerializable]
    public sealed class PlaytimesManagerEuiState : EuiStateBase
    {
    }

    public static class PlaytimesManagerEuiMsg
    {
        [Serializable, NetSerializable]
        public sealed class PlaytimesData : EuiMessageBase
        {
            public string ForkID = "";
            public TimeSpan OverallPlaytime = default!;
            public Dictionary<string, TimeSpan> RolesPlaytimeList = default!;
            public NetUserId PlayerID;
            public PlaytimeUpdateType UpdateType = PlaytimeUpdateType.SetMinimum;
        }
    }
}
