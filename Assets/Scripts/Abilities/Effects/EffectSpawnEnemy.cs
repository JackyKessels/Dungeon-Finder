using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Effect", menuName = "Unit/Effect Object/Spawn Enemy")]
public class EffectSpawnEnemy : EffectObject
{
    [Header("Spawn Enemy")]
    public EnemyObject enemyObject;
    public bool two;
    public bool instant;
    public List<ParticleSystem> specialEffects = new List<ParticleSystem>();

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }
}
