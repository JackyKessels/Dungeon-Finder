using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomRange
{
    public static int Range(params IntRange[] ranges)
    {
        if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
        if (ranges.Length == 1) return Random.Range(ranges[0].min, ranges[0].max);

        float total = 0f;
        for (int i = 0; i < ranges.Length; i++) total += ranges[i].weight;

        float r = Random.value;
        float s = 0f;

        int cnt = ranges.Length - 1;
        for (int i = 0; i < cnt; i++)
        {
            s += ranges[i].weight / total;
            if (s >= r)
            {
                return Random.Range(ranges[i].min, ranges[i].max);
            }
        }
        return Random.Range(ranges[cnt].min, ranges[cnt].max);
    }

    public static float Range(params FloatRange[] ranges)
    {
        if (ranges.Length == 0) throw new System.ArgumentException("At least one range must be included.");
        if (ranges.Length == 1) return Random.Range(ranges[0].Max, ranges[0].Min);

        float total = 0f;
        for (int i = 0; i < ranges.Length; i++) total += ranges[i].Weight;

        float r = Random.value;
        float s = 0f;

        int cnt = ranges.Length - 1;
        for (int i = 0; i < cnt; i++)
        {
            s += ranges[i].Weight / total;
            if (s >= r)
            {
                return Random.Range(ranges[i].Max, ranges[i].Min);
            }
        }
        return Random.Range(ranges[cnt].Max, ranges[cnt].Min);
    }
}

public struct IntRange
{
    public int min;
    public int max;
    public float weight;

    public IntRange(int min, int max, float weight)
    {
        this.min = min;
        this.max = max;
        this.weight = weight;
    }
}

public struct FloatRange
{
    public float Min;
    public float Max;
    public float Weight;
}
