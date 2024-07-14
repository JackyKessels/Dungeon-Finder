using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
	public void Setup(Unit caster, Unit target, float travelTime)
    {
		StartCoroutine(MoveOverSeconds(caster, target.transform.position, travelTime));
    }

	public IEnumerator MoveOverSeconds(Unit caster, Vector3 target, float travelTime)
	{
		float elapsedTime = 0;
		Vector3 startPosition = caster.transform.position;
		while (elapsedTime < travelTime)
		{
			transform.position = Vector3.Lerp(startPosition, target, (elapsedTime / travelTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		transform.position = target;
	}
}
