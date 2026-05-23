using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class Effect : MonoBehaviour
{
	public enum eType
	{
		NORMAL,
		COIN,
		EXP,
		FLOWER
	}

	public eType type;

	public bool running;

	public bool auto_destroy = true;

	private Transform Target;

	private ParticleSystem system;

	private ParticleSystem.Particle[] particles;

	private int value;

	private bool[] finish;

	private static Manager manager;

	private UnityAction finish_call;

	private List<ParticleSystem> system_list = new List<ParticleSystem>();

	public static Effect Flower(Transform t, int level, int front_oil, int back_oil)
	{
		GameObject gameObject = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/effect_flower") as GameObject;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, t, worldPositionStays: false);
		gameObject2.name = gameObject.name;
		Effect component = gameObject2.GetComponent<Effect>();
		component.system = gameObject2.GetComponent<ParticleSystem>();
		component.FlowerLevelup(level);
		component.system.GetComponent<Renderer>().sortingOrder = front_oil;
		component.transform.Find("back").GetComponent<ParticleSystem>().GetComponent<Renderer>()
			.sortingOrder = back_oil;
		component.type = eType.FLOWER;
		component.running = true;
		return component;
	}

	public void FlowerLevelup(int level)
	{
		int num = level / 10;
		num = ((num > 3) ? 3 : ((num >= 0) ? num : 0));
		SetFlower(system, num);
		num = level / 5;
		num = ((num > 6) ? 6 : ((num >= 0) ? num : 0));
		SetFlower(base.transform.Find("back").GetComponent<ParticleSystem>(), num);
	}

	private static ParticleSystem SetFlower(ParticleSystem ps, int max_particle)
	{
		var main = ps.main;
		main.maxParticles = max_particle;
		if (max_particle <= 0)
		{
			ps.Stop();
		}
		ps.Simulate(0.1f);
		ps.Pause();
		return ps;
	}

	public static Effect Coin(int coin, int count, Vector2 pos, Transform target, Color color)
	{
		GameObject prefab = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/effect_coin") as GameObject;
		Effect effect = Release(prefab, coin, count, pos, target, color);
		effect.type = eType.COIN;
		return effect;
	}

	public static Effect Exp(int exp, int count, Vector2 pos, Transform target, Color color, float size_coef = 1f)
	{
		GameObject prefab = Resources.Load("Prefab/effect_exp") as GameObject;
		Effect effect = Release(prefab, exp, count, pos, target, color, size_coef);
		effect.type = eType.EXP;
		return effect;
	}

	public void AddFinishCallback(UnityAction call = null)
	{
		finish_call = call;
	}

	public static Effect Release(GameObject prefab, int value, int count, Vector2 pos, Transform target, Color color, float size_coef = 1f)
	{
		if (manager == null)
		{
			manager = GameObject.Find("Manager").GetComponent<Manager>();
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, null, worldPositionStays: false);
		gameObject.transform.position = pos;
		Effect component = gameObject.GetComponent<Effect>();
		component.Target = target;
		component.particles = new ParticleSystem.Particle[count];
		component.finish = new bool[count];
		component.system = gameObject.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = component.system.main;
		main.maxParticles = count;
		main.startSize = new ParticleSystem.MinMaxCurve(0.1f * size_coef, 0.1f * size_coef);
		component.Clean();
		component.running = true;
		component.system.Play();
		TextMesh component2 = gameObject.transform.Find("text").GetComponent<TextMesh>();
		component.value = value;
		component2.text = "+" + component.value;
		component2.color = color;
		return component;
	}

	public static Effect Levelup(int level, Vector2 pos, Transform t_parent, Color color)
	{
		GameObject prefab = Resources.Load("Prefab/effect_levelup") as GameObject;
		Effect effect = Run(prefab, pos, t_parent);
		TextMesh component = effect.transform.Find("text").GetComponent<TextMesh>();
		component.text = level.ToString();
		component.color = color;
		return effect;
	}

	public static Effect LevelupSmall(int level, Vector2 pos, Transform t_parent, Color color)
	{
		GameObject prefab = Resources.Load("Prefab/effect_levelup_mini") as GameObject;
		Effect effect = Run(prefab, pos, t_parent);
		TextMesh component = effect.transform.Find("text").GetComponent<TextMesh>();
		component.text = level.ToString();
		component.color = color;
		return effect;
	}

	public static Effect Confetti(FarmAnimal.eType type, Vector2 _pos, Transform t_parent)
	{
		GameObject prefab = Resources.Load("Prefab/effect_paper_mini") as GameObject;
		Vector2 vector = default(Vector2);
		vector = ((type.ToString().Contains("DOLPHIN_") || type.ToString().Contains("KILLER_WHALE_")) ? new Vector2(_pos.x, _pos.y + 0.5f) : new Vector2(_pos.x, _pos.y + 0.3f));
		return Run(prefab, vector, t_parent);
	}

	public static Effect Run(GameObject prefab, Vector2 pos, Transform t_parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, t_parent, worldPositionStays: false);
		gameObject.transform.position = pos;
		Effect component = gameObject.GetComponent<Effect>();
		if (component.auto_destroy)
		{
			component.GetChildren(component.gameObject);
		}
		component.system = gameObject.GetComponent<ParticleSystem>();
		component.running = true;
		component.system.Play();
		return component;
	}

	public void SetOrderInLayer(int base_order_in_layer)
	{
		Utils.SetOrderInLayer(base.gameObject, base_order_in_layer);
	}

	public void GetChildren(GameObject obj)
	{
		Transform componentInChildren = obj.GetComponentInChildren<Transform>();
		if (componentInChildren.childCount > 0)
		{
			IEnumerator enumerator = componentInChildren.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					ParticleSystem component = transform.GetComponent<ParticleSystem>();
					if (component != null)
					{
						system_list.Add(component);
					}
					GetChildren(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}

	private void Clean()
	{
		for (int i = 0; i < finish.Length; i++)
		{
			finish[i] = false;
		}
	}

	private void Update()
	{
		if (running)
		{
			if (type == eType.COIN || type == eType.EXP)
			{
				DoCoinLevel();
			}
			else if (type == eType.NORMAL)
			{
				UpdateNormal();
			}
		}
	}

	private void DoCoinLevel()
	{
		int num = system.GetParticles(particles);
		for (int i = 0; i < num; i++)
		{
			ParticleSystem.Particle particle = particles[i];
			if (finish[i])
			{
				float num2 = Vector3.Distance(particle.position, Target.position);
				Vector3 a = system.transform.TransformPoint(particle.position);
				Vector3 position = Target.transform.position;
				float t = 1f - particle.remainingLifetime / particle.startLifetime;
				Vector3 vector2 = particle.position = system.transform.InverseTransformPoint(Vector3.Lerp(a, position, t));
				if (Vector3.Distance(a, position) < 0.05f)
				{
					particle.remainingLifetime = 0f;
					finish[i] = false;
				}
				particles[i] = particle;
			}
			else if (particle.remainingLifetime < particle.startLifetime * 0.6f)
			{
				finish[i] = true;
			}
		}
		system.SetParticles(particles, num);
		if (num == 0 && system.time == system.main.duration)
		{
			if (finish_call != null)
			{
				finish_call();
			}
			if (type == eType.COIN)
			{
				manager.data.SetCoinCount(manager.data.coin + value);
			}
			else if (type == eType.EXP)
			{
				manager.data.SetExpCount(manager.data.exp + value);
			}
			Clean();
			running = false;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateNormal()
	{
		if (!auto_destroy || system.particleCount != 0 || system.time != system.main.duration)
		{
			return;
		}
		for (int i = 0; i < system_list.Count; i++)
		{
			if (system_list[i].isPlaying)
			{
				return;
			}
		}
		running = false;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}