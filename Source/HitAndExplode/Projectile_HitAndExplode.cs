using Verse;

namespace HitAndExplode;

public class Projectile_HitAndExplode : Projectile
{
    private int hitAPawn;
    private Thing hitThingPawn;

    protected Map map;

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        map = Map;
        base.Impact(hitThing, blockedByShield);
        if (hitThing == null)
        {
            Explode();
            return;
        }

        var pawn = hitThing as Pawn;
        if (pawn is { stances: not null })
        {
            hitAPawn = 1;
            hitThingPawn = hitThing;
        }

        if (pawn is { stances: not null } && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
        {
            pawn.stances.stagger.StaggerFor(95);
        }

        Explode();
    }

    protected virtual void Explode()
    {
        if (def.projectile.explosionEffect != null)
        {
            var effecter = def.projectile.explosionEffect.Spawn();
            effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
            effecter.Cleanup();
        }

        var position = Position;
        if (hitAPawn == 1 && hitThingPawn != null)
        {
            position = hitThingPawn.Position;
        }

        GenExplosion.DoExplosion(position, map, def.projectile.explosionRadius, def.projectile.damageDef, launcher,
            DamageAmount, ArmorPenetration,
            def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing,
            def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance,
            def.projectile.postExplosionSpawnThingCount, def.projectile.postExplosionGasType,
            def.projectile.applyDamageToExplosionCellsNeighbors,
            def.projectile.preExplosionSpawnThingDef, def.projectile.preExplosionSpawnChance,
            def.projectile.preExplosionSpawnThingCount,
            def.projectile.explosionChanceToStartFire, def.projectile.explosionDamageFalloff,
            origin.AngleToFlat(destination), null, null, true, def.projectile.damageDef.expolosionPropagationSpeed, 0f,
            true, def.projectile.postExplosionSpawnThingDefWater, def.projectile.screenShakeFactor);
    }
}