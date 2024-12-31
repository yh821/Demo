using System;
using Game;
using UnityEngine;

namespace Game
{
	public sealed class ProjectileSingle : Projectile
	{
		[SerializeField] private float speed = 5f;
		[SerializeField] private float acceleration = 10f;
		[SerializeField] private AnimationCurve moveXCurve = AnimationCurve.Linear(0, 1, 1, 1);
		[SerializeField] private float moveXMultiplier = 0f;
		[SerializeField] private AnimationCurve moveYCurve = AnimationCurve.Linear(0, 1, 1, 1);
		[SerializeField] private EffectController hitEffect;
		[SerializeField] private float moveYMultiplier = 0f;
		[SerializeField] private bool hitEffectWithRotation = true;

		private Vector3 sourceScale;
		private Transform target;
		private int layer;
		private Action hitted;
		private Action complete;

		private Vector3 startPosition;
		private Vector3 normalPosition;
		private float currentSpeed;

		private EffectController effect;
		private Vector3 targetPosition;

		public override void Play(Vector3 sourceScale, Transform target, int layer, Action hitted, Action complete)
		{
			this.sourceScale = sourceScale;
			this.target = target;
			this.layer = layer;
			this.hitted = hitted;
			this.complete = complete;

			startPosition = transform.position;
			normalPosition = transform.position;
			currentSpeed = speed;

			if (effect != null)
			{
				effect.Reset();
				effect.Play();
			}
		}

		private void Awake()
		{
			effect = GetComponent<EffectController>();
		}

		private void Update()
		{
			if (hitted == null && complete == null) return;
			if (target != null) targetPosition = target.position;

			currentSpeed += acceleration * Time.deltaTime;

			var offset = targetPosition - normalPosition;
			var total = targetPosition - startPosition;
			var radio = 1f - offset.magnitude / total.magnitude;

			var direction = offset.normalized;
			var velocity = direction * currentSpeed;

			var movement = velocity * Time.deltaTime;
			if (movement.sqrMagnitude >= offset.sqrMagnitude)
			{
				transform.position = targetPosition;
				if (EventDispatcher.Instance != null)
					EventDispatcher.Instance.OnProjectileSingleEffect(hitEffect, transform.position, transform.rotation,
						hitEffectWithRotation, sourceScale, layer);
				else if (GameRoot.Instance == null)
				{
					var eff = GameObjectPool.Instance.Spawn(hitEffect, null);
					if (hitEffectWithRotation)
						eff.transform.SetPositionAndRotation(transform.position, transform.rotation);
					else eff.transform.position = transform.position;

					eff.transform.localScale = sourceScale;
					eff.gameObject.SetLayerRecursively(layer);

					eff.FinishEvent += () => GameObjectPool.Instance.Free(eff.gameObject);
					eff.Reset();
					eff.Play();
				}
				else effect = null;

				hitted?.Invoke();
				hitted = null;
				complete?.Invoke();
				complete = null;
			}
			else
			{
				normalPosition += movement;
				var movePos = normalPosition;
				var moveUp = Vector3.up;
				var moveRight = Vector3.Cross(direction, moveUp);
				if (!Mathf.Approximately(moveXMultiplier, 0))
				{
					var moveX = moveXMultiplier * moveXCurve.Evaluate(radio);
					movePos += moveRight * moveX;
				}
				if (!Mathf.Approximately(moveYMultiplier, 0))
				{
					var moveY = moveYMultiplier * moveYCurve.Evaluate(radio);
					movePos += moveUp * moveY;
				}
				transform.position = movePos;
				transform.LookAt(targetPosition);
			}
		}
	}
}