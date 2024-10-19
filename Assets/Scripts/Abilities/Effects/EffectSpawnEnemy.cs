using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Spawn Effect", menuName = "Unit/Effect Object/Spawn Enemy")]
public class EffectSpawnEnemy : EffectObject
{
    [Header("Spawn Enemy")]
    public EnemyObject enemyObject;
    public int level;
    public bool two;
    public bool instant;

    protected override string ParseDescription(string s, TooltipObject tooltipInfo)
    {
        return s;
    }
}
