using Content.Shared.Emag.Systems;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Singularity.EntitySystems;

/// <summary>
/// Shared part of SingularitySingularityGeneratorSystem
/// </summary>
public abstract partial class SharedSingularityGeneratorSystem : EntitySystem
{
    #region Dependencies
    [Dependency] protected SharedPopupSystem PopupSystem = default!;
    [Dependency] private EmagSystem _emag = default!;
    [Dependency] private SharedTransformSystem _transformSystem = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;
    [Dependency] private EntityQuery<ContainmentFieldComponent> _containmentFieldQuery = default!;
    #endregion Dependencies

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SingularityGeneratorComponent, GotEmaggedEvent>(OnEmagged);
    }

    private void OnEmagged(EntityUid uid, SingularityGeneratorComponent component, ref GotEmaggedEvent args)
    {
        if (!_emag.CompareFlag(args.Type, EmagType.Interaction))
            return;

        if (_emag.CheckFlag(uid, EmagType.Interaction))
            return;

        if (component.FailsafeDisabled)
            return;

        component.FailsafeDisabled = true;
        args.Handled = true;
    }

    /// <summary>
    /// Checks whether there's a containment field in a given direction away from the generator
    /// </summary>
    /// <param name="transform">The transform component of the singularity generator.</param>
    /// <remarks>Mostly copied from <see cref="ContainmentFieldGeneratorSystem"/> </remarks>
    public bool CheckContainmentField(Direction dir, EntityUid generator, TransformComponent transform, int collisionMask, float safeDistance)
    {

        var (worldPosition, worldRotation) = _transformSystem.GetWorldPositionRotation(transform);
        var dirRad = dir.ToAngle() + worldRotation;

        var ray = new CollisionRay(worldPosition, dirRad.ToVec(), collisionMask);
        var rayCastResults = _physics.IntersectRay(transform.MapID, ray, safeDistance, generator, false);

        RayCastResults? closestResult = null;

        foreach (var result in rayCastResults)
        {
            if (!_containmentFieldQuery.HasComponent(result.HitEntity))
                continue;

            closestResult = result;
            break;
        }

        if (closestResult == null)
            return false;

        var ent = closestResult.Value.HitEntity;

        // Check that the field can't be moved. The fields' transform parenting is weird, so skip that
        return TryComp<PhysicsComponent>(ent, out var collidableComponent) && collidableComponent.BodyType == BodyType.Static;
    }
}
