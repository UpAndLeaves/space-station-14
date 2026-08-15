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
    public IEnumerable<KeyValuePair<string, TimeSpan>> RolesPlaytimeList = [];

    public PlaytimeInfoProfile(string forkID, TimeSpan overallPlaytime, IEnumerable<KeyValuePair<string, TimeSpan>>  rolePlaytimes)
    {
        ForkID = forkID;
        OverallPlaytime = overallPlaytime;
        RolesPlaytimeList = rolePlaytimes;
    }
}
