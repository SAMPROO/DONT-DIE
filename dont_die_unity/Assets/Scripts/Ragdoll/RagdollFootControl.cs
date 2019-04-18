/*
Sampo's Gaming Department
Leo Tamminen
*/

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class RagdollFootControl : MonoBehaviour
{
	private int collisionCount;
	private readonly SmoothFloat smoothGrounded = new SmoothFloat(5);
	public bool Grounded => smoothGrounded.Value > 0.5f;
	
	public float JumpBonusValue { get; private set; }
	private readonly List<IJumpBonus> jumpBonusList = new List<IJumpBonus>();
	private void OnCollisionEnter(Collision collision)
	{
		collisionCount++;

		var jumpBonus = collision.collider.GetComponent<IJumpBonus>();

		if (jumpBonus != null)
		{
			jumpBonusList.Add(jumpBonus);

			JumpBonusValue = Mathf.Max(JumpBonusValue, jumpBonus.Value);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		collisionCount--;
		
		var jumpBonus = collision.collider.GetComponent<IJumpBonus>();

		if (jumpBonus != null)
		{
			jumpBonusList.Remove(jumpBonus);

			if (jumpBonusList.Count == 0)
				JumpBonusValue = 0;
			else
				JumpBonusValue = jumpBonusList.Select(x => x.Value).Max();
		}
	}

	public new Rigidbody rigidbody { get; private set; }
	public Vector3 position => transform.position;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		smoothGrounded.Put(collisionCount > 0 ? 1f : 0f);
	}
	
	
}
