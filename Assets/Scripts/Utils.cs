using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message + " t:" + DateTime.Now.Millisecond);
	}

	public static GameObject Load(string path, Transform t)
	{
		GameObject gameObject = Resources.Load(path) as GameObject;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, t, worldPositionStays: false);
		gameObject2.name = gameObject.name;
		return gameObject2;
	}

	public static bool IsPlaying(Animator animator)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || animator.GetFloat("Speed") == 0f)
		{
			return false;
		}
		return true;
	}

	public static bool IsReversePlaying(Animator animator)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f || animator.GetFloat("Speed") == 0f)
		{
			return false;
		}
		return true;
	}

	private static void play(Animator self, string name, float speed, float start_frame, bool rebind = true)
	{
		if (self.gameObject.activeInHierarchy)
		{
			if (rebind)
			{
				self.Rebind();
				self.enabled = false;
				self.enabled = true;
			}
			self.SetFloat("Speed", speed);
			self.Play(name, 0, start_frame);
		}
	}

	public static void Play(Animation anim, string name, float speed, float normalized_time)
	{
		anim[name].speed = speed;
		anim[name].normalizedTime = normalized_time;
		anim.Play(name);
	}

	public static void Play(Animator self, string name, float speed)
	{
		play(self, name, speed, 0f);
	}

	public static void Reverse(Animator self, string name, float speed)
	{
		play(self, name, 0f - speed, 1f);
	}

	public static void Play(Animator self, string name, float speed, float start_frame, bool rebind = true)
	{
		play(self, name, speed, start_frame, rebind);
	}

	public static void Reverse(Animator self, string name, float speed, float start_frame, bool rebind = true)
	{
		play(self, name, 0f - speed, start_frame, rebind);
	}

	public static void Stop(Animator self)
	{
		self.SetFloat("Speed", 0f);
	}

	public static void Restart(Animator self)
	{
		self.SetFloat("Speed", 1f);
	}

	public static int CompareFarm(FarmAnimal.eType a, FarmAnimal.eType b)
	{
		int num = Price.OpenFarmAnimalLevel(a);
		int num2 = Price.OpenFarmAnimalLevel(b);
		return a - b;
	}

	public static int CompareFarmPrefix(FarmAnimal.ePrefix a, FarmAnimal.ePrefix b)
	{
		FarmAnimal.eType a2 = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), a.ToString() + "_1");
		FarmAnimal.eType b2 = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), b.ToString() + "_1");
		return CompareFarm(a2, b2);
	}

	public static void SetLayer(GameObject obj, int layer)
	{
		obj.layer = layer;
		Transform componentInChildren = obj.GetComponentInChildren<Transform>();
		if (componentInChildren.childCount > 0)
		{
			IEnumerator enumerator = componentInChildren.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					SetLayer(transform.gameObject, layer);
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

	public static void SetOrderInLayer(GameObject obj, int base_order_in_layer)
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
					SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sortingOrder += base_order_in_layer;
					}
					else
					{
						CustomText component2 = transform.GetComponent<CustomText>();
						if (component2 != null)
						{
							component2.SetOrderInLayer(component2.order_in_layer + base_order_in_layer);
						}
						else
						{
							ParticleSystem component3 = transform.GetComponent<ParticleSystem>();
							if (component3 != null)
							{
								component3.GetComponent<Renderer>().sortingOrder += base_order_in_layer;
							}
						}
					}
					SetOrderInLayer(transform.gameObject, base_order_in_layer);
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

	public static void SetComponentList<T>(GameObject obj, List<T> list)
	{
		T component = obj.GetComponent<T>();
		if (component != null)
		{
			list.Add(component);
		}
		Transform componentInChildren = obj.GetComponentInChildren<Transform>();
		IEnumerator enumerator = componentInChildren.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				component = transform.GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
				SetComponentList(transform.gameObject, list);
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

	public static void SetList<T>(Transform parent, List<T> list, bool excluded_continue = true, params Type[] excluded_types)
	{
		if (parent.childCount > 0)
		{
			IEnumerator enumerator = parent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					T component = transform.GetComponent<T>();
					bool flag = false;
					foreach (Type type in excluded_types)
					{
						if (transform.GetComponent(type) != null)
						{
							flag = true;
							break;
						}
					}
					if (!flag && component != null && !component.Equals(default(T)))
					{
						list.Add(component);
					}
					if (!flag || excluded_continue)
					{
						SetList(transform, list, excluded_continue, excluded_types);
					}
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

	public static void SetLocalPositionX(this Transform t, float x)
	{
		Vector3 localPosition = t.localPosition;
		float y = localPosition.y;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(x, y, localPosition2.z);
	}

	public static void SetLocalPositionY(this Transform t, float y)
	{
		Vector3 localPosition = t.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(x, y, localPosition2.z);
	}

	public static void SetLocalPositionZ(this Transform t, float z)
	{
		Vector3 localPosition = t.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = t.localPosition;
		t.localPosition = new Vector3(x, localPosition2.y, z);
	}

	public static void SetPositionX(this Transform t, float x)
	{
		Vector3 position = t.position;
		float y = position.y;
		Vector3 position2 = t.position;
		t.position = new Vector3(x, y, position2.z);
	}

	public static void SetPositionY(this Transform t, float y)
	{
		Vector3 position = t.position;
		float x = position.x;
		Vector3 position2 = t.position;
		t.position = new Vector3(x, y, position2.z);
	}

	public static void SetPositionZ(this Transform t, float z)
	{
		Vector3 position = t.position;
		float x = position.x;
		Vector3 position2 = t.position;
		t.position = new Vector3(x, position2.y, z);
	}

	public static Color Alpha(Color c, float a)
	{
		return new Color(c.r, c.g, c.b, a);
	}

	public static int Round(int value, int t)
	{
		if (value < t)
		{
			return 1;
		}
		return value / t;
	}
}
