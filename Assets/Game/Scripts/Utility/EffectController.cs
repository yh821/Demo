using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public sealed class EffectController : MonoBehaviour
	{
		private enum PlayState
		{
			Stopping,
			Pending,
			Playing,
			Pausing,
			Fadeout,
		}

		[SerializeField] private bool looping = false;
		[SerializeField] private bool noScalable = false;
		[SerializeField] private float delay = 0;
		[SerializeField] private float duration = 5f;
		[SerializeField] private float fadeout = 1f;

		private PlayState state = PlayState.Stopping;
		private float playbackSpeed = 1f;
		private float exitesTime = 0;
		private float timer;
		private ParticleSystem[] _particleSystems;
		private Animator[] _animators;
		private Animation[] _animations;

		public event Action FadeoutEvent;
		public event Action FinishEvent;

		public bool IsLooping
		{
			get => looping;
			set => looping = value;
		}

		public float Duration
		{
			get => duration;
			set => duration = value;
		}

		public float Fadeout
		{
			get => fadeout;
			set => fadeout = value;
		}

		public bool IsPaused => state == PlayState.Pausing;
		public bool IsStopped => state == PlayState.Stopping;
		public bool IsNoScalable => noScalable;

		private ParticleSystem[] ParticleSystems
		{
			get
			{
				if (_particleSystems == null)
				{
					_particleSystems = GetComponentsInChildren<ParticleSystem>(true);
				}

				return _particleSystems;
			}
		}

		private Animator[] Animators
		{
			get
			{
				if (_animators == null)
				{
					_animators = GetComponentsInChildren<Animator>(true);
					foreach (var animator in _animators)
						animator.speed = playbackSpeed;
				}

				return _animators;
			}
		}

		private Animation[] Animations
		{
			get
			{
				if (_animations == null)
				{
					_animations = GetComponentsInChildren<Animation>(true);
					foreach (var animation in _animations)
					{
						var clip = animation.clip;
						if (clip != null)
							animation[clip.name].speed = playbackSpeed;
					}
				}

				return _animations;
			}
		}

		public void WaitFinish(Action callback)
		{
			FinishEvent = callback;
			if (!IsLooping) return;
			Debug.LogErrorFormat("该特效已设置循环，Finish不会回调。可能造成内存泄露：{0}", gameObject.name);
		}

		public void WaitFadeout(Action callback)
		{
			FadeoutEvent = callback;
		}

		public float PlaybackSpeed
		{
			get => playbackSpeed;
			set
			{
				playbackSpeed = value;
				foreach (var animator in _animators)
					animator.speed = playbackSpeed;
				foreach (var animation in _animations)
				{
					var clip = animation.clip;
					if (clip != null)
						animation[clip.name].speed = playbackSpeed;
				}
			}
		}

		public void EstimateDuration()
		{
			looping = false;
			duration = 0;
			fadeout = 0;
			foreach (var system in ParticleSystems)
			{
				if (system == null) continue;
				var main = system.main;
				if (main.loop) looping = true;
				double duration1 = duration;
				double duration2 = main.duration;
				if (duration1 < duration2)
					duration = main.duration;
				double lifetimeMultiplier = main.startLifetimeMultiplier;
				if (fadeout < lifetimeMultiplier)
					fadeout = main.startLifetimeMultiplier;
			}
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip == null) continue;
				if (clip.isLooping) looping = true;
				if (duration < clip.length)
					duration = clip.length;
			}
			foreach (var animator in Animators)
			{
				if (animator == null) continue;
				var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				if (stateInfo.loop) looping = true;
				if (duration < stateInfo.length)
					duration = stateInfo.length;
			}
		}

		public void SimulateInit()
		{
			foreach (var animator in Animators)
			{
				if (animator == null || animator.runtimeAnimatorController == null) continue;
				int frameCount = Mathf.FloorToInt(animator.GetCurrentAnimatorStateInfo(0).length * 30 + 2);
				animator.Rebind();
				animator.StopPlayback();
				animator.recorderStartTime = 0;
				animator.StartRecording(frameCount);
				for (int i = 0; i < frameCount - 1; i++)
					animator.Update(i / 30f);
				animator.StopRecording();
				animator.StartPlayback();
			}
		}

		public void SimulateStart()
		{
			foreach (var particle in ParticleSystems)
			{
				if (particle == null) continue;
				particle.Simulate(0, false, true);
				particle.time = 0;
				particle.Play();
			}
			foreach (var animator in Animators)
			{
				if (animator == null || animator.runtimeAnimatorController == null) continue;
				animator.playbackTime = 0;
				animator.Update(0);
			}
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip != null) clip.SampleAnimation(anim.gameObject, 0);
			}
		}

		public void SimulateDelta(float time, float deltaTime)
		{
			foreach (var particle in ParticleSystems)
			{
				if (particle == null) continue;
				particle.Simulate(deltaTime, false, false);
			}
			foreach (var animator in Animators)
			{
				if (animator == null || animator.runtimeAnimatorController == null) continue;
				animator.playbackTime = time;
				animator.Update(0);
			}
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip != null) clip.SampleAnimation(anim.gameObject, time);
			}
		}

		public void Simulate(float time)
		{
			var dict = new Dictionary<ParticleSystem, KeyValuePair<bool, uint>>();
			foreach (var particle in ParticleSystems)
			{
				if (particle == null) continue;
				particle.Stop(false);
				var kv = new KeyValuePair<bool, uint>(particle.useAutoRandomSeed, particle.randomSeed);
				dict.Add(particle, kv);
				if (!particle.isPlaying)
				{
					particle.useAutoRandomSeed = false;
					particle.randomSeed = 0U;
				}
				particle.Simulate(0, false, true);
				particle.time = 0;
				particle.Play();
			}
			for (float i = 0; i < time; i += 0.02f)
			{
				foreach (var particle in ParticleSystems)
				{
					if (particle == null) continue;
					particle.Simulate(0.02f, false, false);
				}
			}
			foreach (var particle in ParticleSystems)
			{
				if (particle == null) continue;
				particle.Stop(false);
				var kv = dict[particle];
				particle.useAutoRandomSeed = kv.Key;
				particle.randomSeed = kv.Value;
			}
			foreach (var animator in Animators)
			{
				if (animator == null || animator.runtimeAnimatorController == null) continue;
				animator.playbackTime = time;
				animator.Update(0);
			}
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip != null) clip.SampleAnimation(anim.gameObject, time);
			}
		}

		public void Play()
		{
			if (state == PlayState.Playing) Stop();
			state = PlayState.Pending;
		}

		public void Pause()
		{
			if (state != PlayState.Playing) return;
			foreach (var particle in ParticleSystems)
				particle.Pause(false);
			foreach (var animator in Animators)
				animator.speed = 0;
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip != null) anim[clip.name].speed = 0;
			}
			state = PlayState.Pausing;
		}

		public void Resume()
		{
			if (state != PlayState.Pausing) return;
			foreach (var particle in ParticleSystems)
				particle.Play(false);
			foreach (var animator in Animators)
				animator.speed = playbackSpeed;
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				var clip = anim.clip;
				if (clip != null) anim[clip.name].speed = playbackSpeed;
			}
			state = PlayState.Playing;
		}

		public void Stop()
		{
			if (state <= 0U) return;
			foreach (var particle in ParticleSystems)
				particle.Stop(false);
			foreach (var animator in Animators)
				animator.enabled = false;
			foreach (var anim in Animations)
			{
				if (anim == null) continue;
				if (anim.playAutomatically) anim.enabled = false;
				else anim.Stop();
			}
			if (FadeoutEvent != null)
			{
				FadeoutEvent();
				FadeoutEvent = null;
			}
			state = PlayState.Fadeout;
		}

		public void Reset()
		{
			timer = 0;
			state = PlayState.Stopping;
			FinishEvent = null;
			FadeoutEvent = null;
			exitesTime = 0;
		}

		private void Awake()
		{
			Reset();
		}

		private void OnDestroy()
		{
			FinishEvent = null;
			FadeoutEvent = null;
		}

		private void LateUpdate()
		{
			if (state != PlayState.Pausing)
			{
				exitesTime += Time.deltaTime;
				if (exitesTime >= duration + fadeout + 5)
					exitesTime = 0;
			}
			if (state == PlayState.Stopping || state == PlayState.Pausing) return;
			if (state == PlayState.Pending && timer >= delay)
			{
				foreach (var particle in ParticleSystems)
					particle.Play(false);
				foreach (var animator in Animators)
				{
					animator.enabled = true;
					animator.gameObject.SetActive(false);
					animator.gameObject.SetActive(true);
				}
				foreach (var anim in Animations)
				{
					if (anim == null) continue;
					if (anim.playAutomatically)
					{
						anim.enabled = false;
						anim.enabled = true;
					}
					else
					{
						anim.Stop();
						anim.Play();
					}
				}
				state = PlayState.Playing;
			}
			if (!looping && state == PlayState.Playing && timer >= duration)
				Stop();
			if (state != PlayState.Fadeout || timer < duration + fadeout)
				return;
			state = PlayState.Stopping;
			if (FinishEvent != null)
			{
				FinishEvent();
				FinishEvent = null;
			}
			else
				Debug.LogErrorFormat("没有设置WaitFinish回调将会容易内存泄漏，请添加回调并主动删除，否则不要添加EffectController，{0}",
					gameObject.name);
		}
	}
}