using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissileMovement
{
    CasterToTarget,
    TargetToCaster,
    CasterToTargetToCaster,
    TargetToCasterToTarget
}

[CreateAssetMenu(fileName = "New Missile", menuName = "Unit/Other/Missile")]
public class MissileObject : ScriptableObject
{
    public MissileMovement missileMovement = MissileMovement.CasterToTarget;
    public float travelTime;
    public List<ParticleSystem> casterEffects;
    public ParticleSystem missileEffect;
    public List<ParticleSystem> trailingEffects;
    public List<ParticleSystem> impactEffects;

    public IEnumerator LaunchMissile(Unit caster, Unit target)
    {
        switch (missileMovement)
        {
            case MissileMovement.CasterToTarget:
                {
                    ObjectUtilities.CreateSpecialEffects(casterEffects, caster);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, caster, target, travelTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, caster, target, travelTime, true);

                    yield return new WaitForSeconds(travelTime);

                    ObjectUtilities.CreateSpecialEffects(impactEffects, target);
                }
                break;
            case MissileMovement.TargetToCaster:
                {
                    ObjectUtilities.CreateSpecialEffects(casterEffects, target);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, target, caster, travelTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, target, caster, travelTime, true);

                    yield return new WaitForSeconds(travelTime);

                    ObjectUtilities.CreateSpecialEffects(impactEffects, caster);
                }
                break;
            case MissileMovement.CasterToTargetToCaster:
                {
                    float missileTime = travelTime / 2;

                    ObjectUtilities.CreateSpecialEffects(casterEffects, caster);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, caster, target, missileTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, caster, target, missileTime, true);

                    yield return new WaitForSeconds(missileTime);

                    ObjectUtilities.CreateSpecialEffects(impactEffects, target);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, target, caster, missileTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, target, caster, missileTime, true);

                    yield return new WaitForSeconds(missileTime);

                    ObjectUtilities.CreateSpecialEffects(casterEffects, caster);
                }
                break;
            case MissileMovement.TargetToCasterToTarget:
                {
                    float missileTime = travelTime / 2;

                    ObjectUtilities.CreateSpecialEffects(impactEffects, target);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, target, caster, missileTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, target, caster, missileTime, true);

                    yield return new WaitForSeconds(missileTime);

                    ObjectUtilities.CreateSpecialEffects(casterEffects, caster);
                    ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, caster, target, missileTime, false);
                    ObjectUtilities.LaunchSpecialEffects(trailingEffects, caster, target, missileTime, true);

                    yield return new WaitForSeconds(missileTime);

                    ObjectUtilities.CreateSpecialEffects(impactEffects, target);
                }
                break;
        }
    }
}
