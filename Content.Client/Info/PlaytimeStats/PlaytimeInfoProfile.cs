using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Client.Info.PlaytimeStats;

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class PlaytimeInfoProfile
{
    [DataField]
    public string ForkID = "";

    [DataField]
    public TimeSpan OverallPlaytime = TimeSpan.Zero;

    [DataField]
    public Dictionary<string, TimeSpan> RolesPlaytimeList;

    [DataField]
    public NetUserId PlayerID;

    public PlaytimeInfoProfile(string forkID, TimeSpan overallPlaytime, Dictionary<string, TimeSpan>  rolePlaytimes, NetUserId playerID)
    {
        ForkID = forkID;
        OverallPlaytime = overallPlaytime;
        RolesPlaytimeList = rolePlaytimes;
        PlayerID = playerID;
    }
}
