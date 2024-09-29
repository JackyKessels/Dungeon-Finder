using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Missile", menuName = "Unit/Other/Missile")]
public class MissileObject : ScriptableObject
{
    public float travelTime;
    public List<ParticleSystem> casterEffects;
    public ParticleSystem missileEffect;
    public List<ParticleSystem> trailingEffects;
    public List<ParticleSystem> impactEffects;

    public IEnumerator LaunchMissile(Unit caster, Unit target)
    {
        ObjectUtilities.CreateSpecialEffects(casterEffects, caster);

        ObjectUtilities.LaunchSpecialEffects(new List<ParticleSystem>() { missileEffect }, caster, target, travelTime, false);

        ObjectUtilities.LaunchSpecialEffects(trailingEffects, caster, target, travelTime, true);

        yield return new WaitForSeconds(travelTime);

        ObjectUtilities.CreateSpecialEffects(impactEffects, target);
    }
}
