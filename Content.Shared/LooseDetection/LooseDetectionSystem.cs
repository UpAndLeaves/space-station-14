using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Physics;
using Content.Shared.Singularity.EntitySystems;
using Content.Shared.Singularity.Events;

namespace Content.Shared.LooseDetection;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class LooseDetectionSystem : EntitySystem
{
    [Dependency] private SharedSingularityGeneratorSystem _generator = default!;
    [Dependency] private ISharedAdminLogManager _adminLog = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {

        SubscribeAllEvent<ContainmentShutdownEvent>(OnContainmentShutdown);
        SubscribeLocalEvent<LooseDetectionComponent, ComponentShutdown>(OnDetectionComponentShutdown);
        SubscribeLocalEvent<LooseDetectionComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<CurrentlyLoosingComponent, ComponentShutdown>(OnLoosingComponentShutdown);
    }

    private void OnContainmentShutdown(ContainmentShutdownEvent ev)
    {
        CheckAllLooses();
    }

    private void OnComponentStartup(EntityUid ent, LooseDetectionComponent component, ComponentStartup args)
    {
        CheckAllLooses();
    }

    public void CheckAllLooses()
    {
        var query = EntityQueryEnumerator<LooseDetectionComponent>();

        foreach (var ent in query)
        {
            var contained = false;
            var transform = Transform(ent);
            var directions = Enum.GetValues<Direction>().Length;
            for (var i = 0; i < directions - 1; i += 2) // Skip every other direction, checking only cardinals
            {
                if (_generator.CheckContainmentField((Direction)i, ent, transform, (int)CollisionGroup.FullTileMask, 24))
                    contained = true;
            }

            if (contained)
            {
                RemComp<CurrentlyLoosingComponent>(ent);
                continue;
            }

            if (HasComp<CurrentlyLoosingComponent>(ent))
                continue;

            EnsureComp<CurrentlyLoosingComponent>(ent, out var comp);
            _adminLog.Add(LogType.Action, LogImpact.Extreme, $"{ToPrettyString(ent):entity} has loosed!");
            RaiseLocalEvent(ent, new LooseEvent((ent, comp)), broadcast: true);
        }
    }

    private void OnDetectionComponentShutdown(EntityUid ent, LooseDetectionComponent component, ComponentShutdown args)
    {
        RemComp<CurrentlyLoosingComponent>(ent);
    }

    private void OnLoosingComponentShutdown(EntityUid ent, CurrentlyLoosingComponent component, ComponentShutdown args)
    {
        _adminLog.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(ent):entity} is no longer loose.");
        RaiseLocalEvent(ent, new EndLooseEvent(ent));
    }
}
