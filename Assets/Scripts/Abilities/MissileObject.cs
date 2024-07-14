using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Missile", menuName = "Unit/Other/Missile")]
public class MissileObject : ScriptableObject
{
    public float travelTime;
    public List<ParticleSystem> casterEffects;
    public List<ParticleSystem> missileEffects;
    public List<ParticleSystem> impactEffects;

    public IEnumerator LaunchMissile(Unit caster, Unit target)
    {
        ObjectUtilities.CreateSpecialEffects(casterEffects, caster);

        ObjectUtilities.LaunchSpecialEffects(missileEffects, caster, target, travelTime);

        yield return new WaitForSeconds(travelTime);

        ObjectUtilities.CreateSpecialEffects(impactEffects, caster);
    }
}
