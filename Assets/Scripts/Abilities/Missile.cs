using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
	private ParticleSystem _particleSystem { get; set; }

	public void Setup(Unit caster, Unit target, float travelTime, bool trailing)
    {
		_particleSystem = GetComponent<ParticleSystem>();
		FaceToTarget(target.transform);
		StartCoroutine(MoveOverSeconds(caster, target.transform.position, travelTime, trailing));
    }

	private void FaceToTarget(Transform target)
    {
		Vector3 offset = target.transform.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), offset);
		transform.rotation = rotation;
    }

	public IEnumerator MoveOverSeconds(Unit caster, Vector3 target, float travelTime, bool trailing)
	{
		float elapsedTime = 0;
		float distance = Vector3.Distance(caster.transform.position, target);

		while (elapsedTime < travelTime)
        {
			transform.position = Vector3.MoveTowards(transform.position, target, (distance / travelTime) * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		if (trailing)
        {
			DetachParticles();
        }
        else
        {
			Destroy(gameObject);
        }

		transform.position = target;
	}

	private void DetachParticles()
	{
		transform.parent = null;
		ParticleSystem.EmissionModule emission = _particleSystem.emission;
		emission.rateOverTime = 0;
		Destroy(gameObject, 1);
	}
}
