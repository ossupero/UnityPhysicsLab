using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsLab
{
	[RequireComponent(typeof(Transform))]
	public class RopeSphereCollider : MonoBehaviour 
	{
		private Transform sphereTfm;
		
		void OnEnable()
		{
			sphereTfm = GetComponent<Transform>();
		}
		
		public void UpdateCollision(Particle particle)
		{
			Vector3 colliderCenter = sphereTfm.position;
			float colliderR = sphereTfm.lossyScale.x * 0.5f;
			Vector3 offset = particle.pos - colliderCenter;
			if (offset.magnitude < colliderR + particle.radius) // Collision Detected
			{
				// Collision Response
				particle.pos = colliderCenter + offset.normalized * (colliderR + particle.radius);
			}
		}
	}
}