using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Syringe))]
public class SyringeTest : MonoBehaviour
{
	public float duration;

	public IEnumerator Start()
	{
		float percent = 1;

		var syringe = GetComponent<Syringe>();

		// Run syringe from full to empty in time of duration
		while (percent > 0)
		{
			syringe.Fill = percent;
			percent -= Time.deltaTime / duration;
			yield return null;
		}

		Destroy(this);
	}
}