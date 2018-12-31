using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsLab
{
	public class RopeSolver : MonoBehaviour {
		public Transform ParticlePrefab;
		public int Count = 3;
		public int Space = 1;
		[Range(0, 1)]
		public float Damping = 0.1f;
		public Vector3 ExternForce = Vector3.zero;
		public int SolveCount = 1;

		[Header("Collider")]
		public RopeSphereCollider sphereCollider;

		private List<Transform> chain = new List<Transform>();
		private List<Particle> particleList = new List<Particle>();

		void Start()
		{
			for (int i=0; i<Count; i++)
			{
				var obj = Instantiate(ParticlePrefab, transform, true);
				obj.Translate(0, -i * Space, 0);
				chain.Add(obj);

				// Construct Particles
				var particle = new Particle();
				particle.radius = 0.5f * Space;
				particle.pos = particle.prevPos = new Vector3(0, -i * Space, 0);
				particle.velocity = Vector3.zero;
				particleList.Add(particle);
			}
		}

		void FixedUpdate ()
		{
			// Update Velocity
			for (int i=0; i<Count; i++)
			{
				var particle = particleList[i];

				// Verlet Integration
				Vector3 vel = particle.velocity + ExternForce * Time.fixedDeltaTime * Time.fixedDeltaTime;
				particle.prevPos = particle.pos;
				particle.pos = particle.pos + (1-Damping) * vel;
			}

			// Resolve Constraints
			// 1. Attach Root Particle to base transform
			particleList[0].pos = transform.position;

			// 2. Keep Length Constraint from top to bottom
			for (int n=0; n<SolveCount; n++)
			{
				for (int i=1; i<Count; i++)
				{
					Vector3 offsetToParent = particleList[i].pos - particleList[i-1].pos;
					// Strategy 1: only move child particle
					//particleList[i].pos = particleList[i-1].pos + Space * offsetToParent.normalized;
					// Strategy 2: Position Based Dynamics, iteratively
					offsetToParent = Space * offsetToParent.normalized - offsetToParent;
					particleList[i-1].pos -= 0.5f * offsetToParent;
					particleList[i].pos += 0.5f * offsetToParent;
				}

				// Attach Root Particle to base transform
				particleList[0].pos = transform.position;
			}

			// Collision Detection & Response
			if (sphereCollider != null)
			{
				for (int i=0; i<Count; i++)
				{
					sphereCollider.UpdateCollision(particleList[i]);
				}
			}

			// Update velocity
			for (int i=0; i<Count; i++)
			{
				particleList[i].velocity = particleList[i].pos - particleList[i].prevPos;
			}

			// Apply Particle Data to Transform
			for (int i=0; i<Count; i++)
			{
				chain[i].position = particleList[i].pos;
			}
		}
	}

	public class Particle
	{
		public float radius;
		public Vector3 pos;
		public Vector3 prevPos;
		public Vector3 velocity;
	}
}
