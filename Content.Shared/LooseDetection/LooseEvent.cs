namespace Content.Shared.LooseDetection;

/// <summary>
/// An event raised whenever a singularity or tesla looses.
/// </summary>
public sealed class LooseEvent : EntityEventArgs
{
    private Entity<CurrentlyLoosingComponent> Entity;

    public LooseEvent(Entity<CurrentlyLoosingComponent> entity)
    {
        Entity = entity;
    }
}
