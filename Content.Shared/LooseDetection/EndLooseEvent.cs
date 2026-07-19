namespace Content.Shared.LooseDetection;

/// <summary>
/// An event raised whenever a singularity or tesla looses.
/// </summary>
public sealed class EndLooseEvent : EntityEventArgs
{
    private EntityUid Entity;

    public EndLooseEvent(EntityUid entity)
    {
        Entity = entity;
    }
}
