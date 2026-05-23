using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Common
{
	public enum eSEASON_EVENT
	{
		NONE = -1,
		HALLOWEEN = 1,
		CRISTMAS = 2,
		SUMMER = 3
	}

	public class Conditions
	{
		public enum eHotelStartRate
		{
			NONE = -1,
			STAR_0,
			STAR_1,
			STAR_2,
			STAR_3
		}

		public HashSet<Facility.eType> facilities = new HashSet<Facility.eType>();

		public Facility.eItem tree = Facility.eItem.NONE;

		public HashSet<FarmAnimal.eType> animals = new HashSet<FarmAnimal.eType>();

		public eSEASON_EVENT season_event = eSEASON_EVENT.NONE;

		public int level = -1;

		public eHotelStartRate hotel_star_rate = eHotelStartRate.NONE;

		public Data.eFarmType efarm_type = Data.eFarmType.IGNORE;

		public void Add(Facility.eType cond)
		{
			facilities.Add(cond);
		}

		public void Add(FarmAnimal.eType cond)
		{
			animals.Add(cond);
		}

		public void Add(Facility.eItem cond)
		{
			tree = cond;
		}

		public void Add(eSEASON_EVENT cond)
		{
			season_event = cond;
		}

		public void Add(int need_level)
		{
			level = need_level;
		}

		public void Add(eHotelStartRate star_rate)
		{
			hotel_star_rate = star_rate;
		}

		public void Add(Data.eFarmType need_farm_type)
		{
			efarm_type = need_farm_type;
		}

		public bool ChkSeasonEvent()
		{
			if (season_event != eSEASON_EVENT.NONE && SeasonEvent() != season_event)
			{
				return false;
			}
			return true;
		}

		public bool ChkLevel()
		{
			return manager.data.level >= level;
		}

		public bool ChkFacility(HashSet<Facility.eType> compare)
		{
			foreach (Facility.eType facility in facilities)
			{
				if (!compare.Contains(facility))
				{
					return false;
				}
			}
			return true;
		}

		public bool ChkTree(HashSet<Facility.eItem> compare)
		{
			return tree == Facility.eItem.NONE || compare.Contains(tree);
		}

		public bool ChkAnimals(HashSet<FarmAnimal.eType> compare)
		{
			foreach (FarmAnimal.eType animal in animals)
			{
				if (!compare.Contains(animal))
				{
					return false;
				}
			}
			return true;
		}

		public bool ChkWildAnimal(WildAnimal.eCATEGORY category, ConditionCompare compare)
		{
			if (category != WildAnimal.eCATEGORY.MAX)
			{
				int num = compare.wild_animal_category_count[(int)category] + 1;
				switch (category)
				{
				case WildAnimal.eCATEGORY.CHILD:
				{
					int num3 = compare.facility_count[1];
					if (num > num3 * 2)
					{
						return false;
					}
					break;
				}
				case WildAnimal.eCATEGORY.FOWL:
				{
					int num2 = compare.facility_count[3];
					if (Data.farm_type == Data.eFarmType.RESORT)
					{
						num2 = compare.facility_count[14];
					}
					if (num > num2)
					{
						return false;
					}
					break;
				}
				}
			}
			return true;
		}

		public bool ChkHotelStarRate()
		{
			eHotelStartRate eHotelStartRate = (eHotelStartRate)(manager.data.hotel_data.level - 1);
			return eHotelStartRate >= hotel_star_rate;
		}

		public bool ChkFarmType()
		{
			return Data.farm_type == efarm_type;
		}
	}

	public class ConditionCompare
	{
		public HashSet<Facility.eType> facilities = new HashSet<Facility.eType>();

		public HashSet<Facility.eItem> items = new HashSet<Facility.eItem>();

		public HashSet<FarmAnimal.eType> animals = new HashSet<FarmAnimal.eType>();

		public int[] facility_count = new int[22];

		public int[] wild_animal_category_count = new int[5];

		public ConditionCompare(Map map, bool wild_animal = false)
		{
			for (int i = 0; i < facility_count.Length; i++)
			{
				facility_count[i] = 0;
			}
			for (int j = 0; j < map.facility_list.Count; j++)
			{
				Facility facility = map.facility_list[j];
				Facility.eState eState = facility.State();
				if (eState == Facility.eState.ACTIVE)
				{
					string[] array = facility.Type().ToString().Split('_');
					if (array.Length > 1)
					{
						Facility.eType item = (Facility.eType)Enum.Parse(typeof(Facility.eType), array[0] + "_1");
						facilities.Add(item);
					}
					array = facility.Tree().ToString().Split('_');
					if (array.Length > 1)
					{
						Facility.eItem eItem = (Facility.eItem)Enum.Parse(typeof(Facility.eItem), array[0] + "_1");
						items.Add(facility.Tree());
					}
					animals.Add(facility.Animal(0));
					animals.Add(facility.Animal(1));
					animals.Add(facility.Animal(2));
					if (wild_animal)
					{
						string[] array2 = facility.type.ToString().Split('_');
						Facility.eType eType = (Facility.eType)Enum.Parse(typeof(Facility.eType), array2[0] + "_1");
						facility_count[(int)eType]++;
					}
				}
			}
			if (!wild_animal)
			{
				return;
			}
			Data.WildAnimalData wild_animal_data = manager.data.wild_animal_data;
			for (int k = 0; k < wild_animal_category_count.Length; k++)
			{
				wild_animal_category_count[k] = 0;
				for (int l = 0; l < wild_animal_data.visit[k].areas.Count; l++)
				{
					if (wild_animal_data.visit[k].areas[l].type != WildAnimal.eType.NONE)
					{
						wild_animal_category_count[k]++;
					}
				}
			}
		}
	}

	public class Bag
	{
		public enum eType
		{
			COIN,
			EXP,
			COIN_MINI
		}

		public eType type;

		public TouchEvent touch;

		public int value;

		public UnityAction call;

		public Bag(eType _type, TouchEvent t, int _value, UnityAction after_event)
		{
			type = _type;
			touch = t;
			value = _value;
			call = after_event;
		}

		public void AddValue(int _value)
		{
			value += _value;
		}

		public void SetOrderInLayer(int order_in_layer)
		{
			touch.GetComponent<SpriteRenderer>().sortingOrder += order_in_layer;
			Utils.SetOrderInLayer(touch.gameObject, order_in_layer);
		}

		public void Destroy()
		{
			if (touch != null)
			{
				UnityEngine.Object.Destroy(touch.gameObject);
			}
		}
	}

	public enum eCOIN_EXP
	{
		COIN,
		EXP,
		COIN_RESORT,
		MAX
	}

	[Serializable]
	public class Bus
	{
		public enum eState
		{
			COME,
			STOP,
			LEAVE
		}

		public eState state;

		public UnityEvent arrival = new UnityEvent();

		public UnityEvent leave = new UnityEvent();

		public Animator animator;

		public AudioSource audio;
	}

	public class SugorokuFarmPrompt
	{
		public GameObject root;

		public Transform coin;

		public Transform exp;
	}

	private static Manager manager;

	private static Conditions[] customer_conds = new Conditions[17];

	private static Conditions[] wild_animals_conds = new Conditions[26];

	private static Conditions[] fish_conds = new Conditions[20];

	public static readonly Color TextDisableColor = new Color(1f, 10f / 51f, 10f / 51f, 1f);

	public static readonly Color TextBlackColor = new Color(74f / 255f, 73f / 255f, 14f / 51f, 1f);

	public static readonly Color TextWhiteTranslucentColor = new Color(1f, 1f, 1f, 0.6f);

	public static readonly Color TextWhiteResortTranslucentColor = new Color(1f, 1f, 1f, 0.8f);

	private const string BUS_IDLE = "bus_idle";

	private const string BUS_OPEN = "bus_open";

	private const string BUS_CLOSE = "bus_close";

	private const string HELI_IDLE = "helicopter_idle";

	private const string HELI_IDLE2 = "helicopter_idle2";

	private const string HELI_TANK = "helicopter_tank";

	public static void Init()
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		for (int i = 0; i < customer_conds.Length; i++)
		{
			customer_conds[i] = new Conditions();
			int conditionLength = Customer.GetConditionLength();
			for (int j = 0; j < conditionLength; j++)
			{
				Data.Condition conditions = Customer.GetConditions((Customer.eType)i, j);
				if (conditions != null)
				{
					SetConditions(customer_conds[i], conditions);
				}
			}
		}
		for (int k = 0; k < wild_animals_conds.Length; k++)
		{
			wild_animals_conds[k] = new Conditions();
			int conditionLength2 = WildAnimal.GetConditionLength();
			for (int l = 0; l < conditionLength2; l++)
			{
				Data.Condition conditions2 = WildAnimal.GetConditions((WildAnimal.eType)k, l);
				if (conditions2 != null)
				{
					SetConditions(wild_animals_conds[k], conditions2);
				}
			}
		}
		for (int m = 0; m < fish_conds.Length; m++)
		{
			fish_conds[m] = new Conditions();
			int conditionLength3 = Fish.GetConditionLength();
			for (int n = 0; n < conditionLength3; n++)
			{
				Data.Condition conditions3 = Fish.GetConditions((Fish.eType)m, n);
				if (conditions3 != null)
				{
					SetConditions(fish_conds[m], conditions3);
				}
			}
		}
	}

	public static eSEASON_EVENT SeasonEvent()
	{
		DateTime today = DateTime.Today;
		if (today.Month == 10)
		{
			return eSEASON_EVENT.HALLOWEEN;
		}
		if (today.Month == 12)
		{
			return eSEASON_EVENT.CRISTMAS;
		}
		if (today.Month == 8)
		{
			return eSEASON_EVENT.SUMMER;
		}
		return eSEASON_EVENT.NONE;
	}

	private static void SetConditions(Conditions conditions, Data.Condition cond)
	{
		if (cond.category == Data.Condition.eCATEGORY.FACILITY)
		{
			conditions.Add((Facility.eType)cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.FACILITYITEM)
		{
			conditions.Add((Facility.eItem)cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.FARMANIMAL)
		{
			conditions.Add((FarmAnimal.eType)cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.LEVEL)
		{
			conditions.Add(cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.SEASON_EVENT)
		{
			conditions.Add((eSEASON_EVENT)cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.HOTEL)
		{
			conditions.Add((Conditions.eHotelStartRate)cond.type);
		}
		else if (cond.category == Data.Condition.eCATEGORY.FARM_TYPE)
		{
			conditions.Add((Data.eFarmType)cond.type);
		}
	}

	public static ConditionCompare GetCompareData(Manager m, bool wild_animal = false)
	{
		return new ConditionCompare(m.map, wild_animal);
	}

	public static bool IsMeetConditions(Customer.eType type, ConditionCompare compare)
	{
		return isMeetConditions(customer_conds[(int)type], compare);
	}

	public static bool IsMeetConditions(WildAnimal.eType type, ConditionCompare compare)
	{
		return isMeetConditions(wild_animals_conds[(int)type], compare, WildAnimal.TypeToCategory(type));
	}

	public static bool IsMeetConditions(Fish.eType type, ConditionCompare compare)
	{
		return isMeetConditions(fish_conds[(int)type], compare);
	}

	private static bool isMeetConditions(Conditions conditions, ConditionCompare compare, WildAnimal.eCATEGORY category = WildAnimal.eCATEGORY.MAX)
	{
		if (conditions.ChkSeasonEvent() && conditions.ChkLevel() && conditions.ChkFacility(compare.facilities) && conditions.ChkTree(compare.items) && conditions.ChkAnimals(compare.animals) && conditions.ChkWildAnimal(category, compare) && conditions.ChkHotelStarRate() && conditions.ChkFarmType())
		{
			return true;
		}
		return false;
	}

	public static Bag OccurCoinBag(int coin, Vector3 pos, UnityAction after_event, Transform parent)
	{
		GameObject prefab = Resources.Load("Prefab/pay_bag_large") as GameObject;
		return OccurBag(prefab, Bag.eType.COIN, coin, pos, after_event, parent);
	}

	public static Bag OccurCoinBagMini(int coin, Vector3 pos, UnityAction after_event, Transform t_parent = null)
	{
		GameObject prefab = Resources.Load("Prefab/pay_bag") as GameObject;
		return OccurBag(prefab, Bag.eType.COIN, coin, pos, after_event, t_parent);
	}

	public static Bag OccurExpBag(int exp, Vector3 pos, UnityAction after_event, Transform t_parent = null)
	{
		GameObject prefab = Resources.Load("Prefab/exp_bag_large") as GameObject;
		return OccurBag(prefab, Bag.eType.EXP, exp, pos, after_event, t_parent);
	}

	public static Bag OccurExpBagMini(int exp, Vector3 pos, UnityAction after_event, Transform t_parent = null)
	{
		GameObject prefab = Resources.Load("Prefab/exp_bag") as GameObject;
		return OccurBag(prefab, Bag.eType.EXP, exp, pos, after_event, t_parent);
	}

	private static Bag OccurBag(GameObject prefab, Bag.eType type, int value, Vector3 pos, UnityAction after_event, Transform t_parent = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, t_parent, worldPositionStays: false);
		gameObject.transform.position = pos;
		TouchEvent component = gameObject.GetComponent<TouchEvent>();
		if (component == null)
		{
			UnityEngine.Debug.unityLogger.LogError("Error", "Need TouchEvent component!");
			return null;
		}
		Bag bag = new Bag(type, component, value, after_event);
		component.ClickUp.AddListener(delegate
		{
			TouchBag(bag, delegate
			{
			});
		});
		component.param.value_obj1 = gameObject;
		component.param.value1 = value;
		return bag;
	}

	public static void TouchBag(Bag bag, UnityAction call)
	{
		TouchEvent touch = bag.touch;
		Animation component = touch.param.value_obj1.GetComponent<Animation>();
		component.Play("pay_bag_get");
		if (bag.type == Bag.eType.COIN)
		{
			Manager.sound.PlaySe(Sound.eSe.COINS);
			Effect.Coin(bag.value, 10, component.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
			{
				call();
				Manager.sound.PlaySe(Sound.eSe.COINS_ARRIVAL);
			});
		}
		else if (bag.type == Bag.eType.EXP)
		{
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(bag.value, 10, component.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				call();
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
		}
		BoxCollider2D component2 = component.GetComponent<BoxCollider2D>();
		component2.enabled = false;
		touch.SetEnabled(enabled: false);
		touch.param.value_obj1 = null;
		if (bag.call != null)
		{
			bag.call();
		}
	}

	public static TouchEvent CreateLockIcon(Transform t_parent, int level, int need_level, int value, UnityAction call, int order_in_layer, bool chg_ignore_color = true)
	{
		GameObject original = Resources.Load("Prefab/lock_icon") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Animation anim = touch.GetComponent<Animation>();
		TextMesh component = touch.transform.Find("contents/text").GetComponent<TextMesh>();
		component.text = need_level.ToString();
		if (level >= need_level)
		{
			Utils.Play(anim, "PushIconFloating", 1f, 0f);
			touch.param.value1 = value;
			touch.param.call1 = call;
			touch.ClickDown.AddListener(delegate
			{
				LockIconDown(touch);
			});
			touch.ClickUp.AddListener(Manager.sound.ClickSound);
			touch.ClickUp.AddListener(delegate
			{
				LockIconUP(touch);
			});
			AnimEvent component2 = touch.GetComponent<AnimEvent>();
			component2.SetFinishCallback(delegate
			{
				Utils.Play(anim, "PushIconFloating", 1f, 0f);
			});
		}
		else if (chg_ignore_color)
		{
			touch.ClickDown.AddListener(delegate
			{
				Utils.Play(anim, "PushIconDeny", 1f, 0f);
			});
			touch.ClickDown.AddListener(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.BEEP);
			});
			component.color = Color.red;
		}
		Utils.SetOrderInLayer(touch.gameObject, order_in_layer);
		return touch;
	}

	public static TouchEvent CreateConstructIcon(Transform t_parent, int value, UnityAction call)
	{
		GameObject original = Resources.Load("Prefab/construct_icon") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		touch.ClickDown.AddListener(delegate
		{
			Utils.Play(touch.GetComponent<Animation>(), "PushIconDeny", 1f, 0f);
		});
		touch.param.value1 = value;
		touch.param.call1 = call;
		touch.ClickUp.AddListener(delegate
		{
			LockIconUP(touch);
		});
		return touch;
	}

	public static void ResetPushIcon(TouchEvent touch)
	{
		Animation anim = touch.GetComponent<Animation>();
		Utils.Play(anim, "PushIconFloating", 1f, 0f);
		touch.ClickDown.RemoveAllListeners();
		touch.ClickDown.AddListener(delegate
		{
			Utils.Play(anim, "PushIcon", 1f, 0f);
		});
	}

	public static void LockIconDown(TouchEvent touch)
	{
		Animation component = touch.GetComponent<Animation>();
		component.Play();
	}

	public static void LockIconUP(TouchEvent touch)
	{
		if (touch.param.call1 != null)
		{
			touch.param.call1();
		}
	}

	public static void OccurGrass(Transform t_parent, Vector2 pos, UnityAction call)
	{
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/eat_grass") as GameObject;
		AnimEvent component = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<AnimEvent>();
		component.transform.localPosition = pos;
		component.SetFinishCallback(call);
	}

	public static void OccurGetHarvest(FarmAnimal.eType type, Vector2 pos, Vector2 target, int order_in_layer)
	{
		GameObject original = Resources.Load("Prefab/get_harvest") as GameObject;
		SpriteRenderer sr = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false).GetComponent<SpriteRenderer>();
		sr.transform.position = pos;
		sr.sprite = SpriteManager.GetHarvest(type);
		sr.sortingOrder = order_in_layer;
		original = (Resources.Load("Prefab/halo") as GameObject);
		GameObject gameObject = UnityEngine.Object.Instantiate(original, sr.transform, worldPositionStays: false);
		Utils.SetOrderInLayer(gameObject, sr.sortingOrder - 2);
		Sequence sequence = DOTween.Sequence();
		if (type.ToString().Contains("HORSE_"))
		{
			AboveSequence(sequence, pos, sr.gameObject, sr, gameObject);
			sequence.Append(DOTween.ToAlpha(() => sr.color, delegate(Color color)
			{
				sr.color = color;
			}, 0f, 0.3f));
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN);
				Effect.Coin(Price.HarvestPrice(type), 3, sr.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
				});
			});
			sequence.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(sr.gameObject);
			});
		}
		else if (type.ToString().Contains("DOLPHIN_") || type.ToString().Contains("SEA_LION_") || type.ToString().Contains("WALRUS_") || type.ToString().Contains("KILLER_WHALE_"))
		{
			Effect.Confetti(type, pos, manager.transform);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN);
				Effect.Coin(Price.HarvestPrice(type), 3, sr.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
				});
			});
			sequence.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(sr.gameObject);
			});
		}
		else
		{
			AboveSequence(sequence, pos, sr.gameObject, sr, gameObject);
			GotoStore(sequence, target, sr.gameObject, sr);
		}
		sequence.Play();
	}

	public static void OccurGetHarvest(Fish.eType type, Vector2 pos, Vector2 target, int order_in_layer)
	{
		GameObject original = Resources.Load("Prefab/get_fish") as GameObject;
		Animator component = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false).GetComponent<Animator>();
		component.transform.position = pos;
		component.runtimeAnimatorController = (RuntimeAnimatorController)UnityEngine.Object.Instantiate(Resources.Load("Animation/fish/" + type.ToString().ToLower()));
		Utils.Play(component, type.ToString().ToLower() + "_stay_2_down", 1f, 0f);
		SpriteRenderer component2 = component.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component2.sprite = SpriteManager.GetHarvest(type);
		component2.sortingOrder = order_in_layer;
		original = (Resources.Load("Prefab/halo") as GameObject);
		GameObject gameObject = UnityEngine.Object.Instantiate(original, component.transform, worldPositionStays: false);
		Utils.SetOrderInLayer(gameObject, component2.sortingOrder - 2);
		Sequence sequence = DOTween.Sequence();
		AboveSequence(sequence, pos, component.gameObject, component2, gameObject, 1f);
		GotoStore(sequence, target, component.gameObject, component2);
		sequence.Play();
	}

	private static void AboveSequence(Sequence sequence, Vector2 pos, GameObject harvest, SpriteRenderer harvest_sr, GameObject halo, float interval = 0.5f)
	{
		sequence.Append(harvest.transform.DOMove(new Vector2(pos.x, pos.y + 0.15f), 0.5f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.HARVEST);
		});
		harvest_sr.color = Utils.Alpha(harvest_sr.color, 0f);
		sequence.Join(DOTween.ToAlpha(() => harvest_sr.color, delegate(Color color)
		{
			harvest_sr.color = color;
		}, 1f, 0.5f));
		sequence.AppendInterval(interval);
		sequence.AppendCallback(delegate
		{
			UnityEngine.Object.Destroy(halo);
		});
	}

	private static void GotoStore(Sequence sequence, Vector2 target, GameObject harvest, SpriteRenderer harvest_sr)
	{
		sequence.Append(harvest.transform.DOMove(target, 0.3f));
		sequence.Join(DOTween.ToAlpha(() => harvest_sr.color, delegate(Color color)
		{
			harvest_sr.color = color;
		}, 0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			UnityEngine.Object.Destroy(harvest);
		});
	}

	public static void OccurHeart(WildAnimal.eType type, Transform t_parent, int order_in_layer, UnityAction finish_call)
	{
		GameObject original = Resources.Load("Prefab/heart") as GameObject;
		Animator animator = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Animator>();
		TouchEvent component = animator.GetComponent<TouchEvent>();
		component.ClickDown.AddListener(delegate
		{
			TouchHeart(type, t_parent, animator.gameObject, finish_call);
		});
		component.ClickMove.AddListener(delegate
		{
			TouchHeart(type, t_parent, animator.gameObject, finish_call);
		});
		SpriteRenderer component2 = animator.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component2.sortingOrder = order_in_layer;
		Utils.Play(animator, "heart_appear", 1f, 0f);
	}

	private static void TouchHeart(WildAnimal.eType type, Transform t_parent, GameObject obj, UnityAction finish_call)
	{
		if (obj != null)
		{
			Manager.sound.PlaySe(Sound.eSe.HEART);
			GameObject prefab = Resources.Load("Prefab/effect_heart_mini") as GameObject;
			Effect.Run(prefab, t_parent.transform.TransformPoint(new Vector2(0f, 0.2f)), t_parent);
			int num = Price.ExpWildAnimalHeart(type);
			if (manager.data.AddLevelCondCount(Data.CharacterData.eType.WILDANIMAL, (int)type))
			{
				int level = manager.data.character_data[1].contents[(int)type].level;
				Vector2 pos = obj.transform.TransformPoint(new Vector2(0f, 0.1f));
				Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
				Effect.LevelupSmall(level, pos, t_parent, Color.white);
				num *= 2;
			}
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(num, 3, obj.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			finish_call?.Invoke();
			UnityEngine.Object.Destroy(obj);
		}
	}

	public static GameObject OccurInterstitialSet(Transform t, Vector2 pos)
	{
		GameObject original = Resources.Load("Prefab/commercial") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, t, worldPositionStays: false);
		Transform transform = gameObject.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = gameObject.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		HashSet<FarmAnimal.eType> hashSet = new HashSet<FarmAnimal.eType>();
		hashSet.Add(FarmAnimal.eType.HONEY_1);
		hashSet.Add(FarmAnimal.eType.HONEY_2);
		hashSet.Add(FarmAnimal.eType.FRUIT_1);
		hashSet.Add(FarmAnimal.eType.FRUIT_2);
		hashSet.Add(FarmAnimal.eType.FRUIT_3);
		HashSet<FarmAnimal.eType> hashSet2 = hashSet;
		List<FarmAnimal.eType> list = new List<FarmAnimal.eType>();
		for (int i = 0; i < Convert.FarmAnimalLength((int)Data.farm_type); i++)
		{
			int num = i + Convert.FarmAnimalInitValue((int)Data.farm_type);
			if (manager.data.character_data[0].contents[num].reg == 1 && !hashSet2.Contains((FarmAnimal.eType)num))
			{
				list.Add((FarmAnimal.eType)num);
			}
		}
		if (list.Count == 0)
		{
			list.Add(FarmAnimal.eType.SHEEP_1);
		}
		FarmAnimal.eType eType = list[UnityEngine.Random.Range(0, list.Count)];
		Animator component = gameObject.GetComponent<Animator>();
		component.runtimeAnimatorController = (Resources.Load("Animation/farmanimal/" + eType.ToString().ToLower()) as RuntimeAnimatorController);
		Utils.Play(component, eType.ToString().ToLower() + "_stay_1_down", 1f, 0f);
		SpriteRenderer component2 = gameObject.transform.Find("balloon").GetComponent<SpriteRenderer>();
		component2.sprite = SpriteManager.GetCommercial(manager.data.lang);
		return gameObject;
	}

	public static void SetIntertitialAfter(GameObject commercial)
	{
		commercial.transform.Find("balloon").GetComponent<SpriteRenderer>().sprite = SpriteManager.GetCommercial(Data.eLang.MAX);
	}

	public static GameObject OccurGiveGrassBalloon(Transform t_parent, Vector2 pos, int order_in_layer, bool flipX)
	{
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/give_grass") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false);
		gameObject.transform.localPosition = pos;
		SpriteRenderer component = gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component.sortingOrder += order_in_layer;
		component.flipX = flipX;
		Animation component2 = gameObject.GetComponent<Animation>();
		Utils.Play(component2, component2.clip.name, 1f, UnityEngine.Random.Range(0f, 0.5f));
		return gameObject;
	}

	public static void OccurStoreMaxBalloon(Transform t_parent, int order_in_layer, UnityAction finish_call)
	{
		GameObject original = Resources.Load("Prefab/store_max_balloon") as GameObject;
		GameObject obj = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false);
		SpriteRenderer component = obj.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component.sortingOrder = order_in_layer;
		AnimEvent component2 = obj.GetComponent<AnimEvent>();
		component2.SetFinishCallback(delegate
		{
			BalloonFinish(obj, finish_call);
		});
	}

	private static void BalloonFinish(GameObject obj, UnityAction finish_call)
	{
		finish_call?.Invoke();
		UnityEngine.Object.Destroy(obj);
	}

	public static GameObject OccurMapCoin(int coin, Vector2 pos, int order_in_layer, UnityAction call_back)
	{
		if (Data.farm_type == Data.eFarmType.NORMAL)
		{
			return occurMapCoinExp(SpriteManager.GetMapCoinExp(eCOIN_EXP.COIN), pos, order_in_layer, delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN);
				Effect.Coin(coin, 3, pos, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
				});
				call_back();
			});
		}
		if (Data.farm_type == Data.eFarmType.RESORT)
		{
			return occurMapCoinExp(SpriteManager.GetMapCoinExp(eCOIN_EXP.COIN_RESORT), pos, order_in_layer, delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN);
				Effect.Coin(coin, 3, pos, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
				});
				call_back();
			});
		}
		return null;
	}

	public static GameObject OccurMapExp(int exp, Vector2 pos, int order_in_layer, UnityAction call_back)
	{
		return occurMapCoinExp(SpriteManager.GetMapCoinExp(eCOIN_EXP.EXP), pos, order_in_layer, delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(exp, 3, pos, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			call_back();
		});
	}

	private static GameObject occurMapCoinExp(Sprite sprite, Vector2 pos, int order_in_layer, UnityAction call)
	{
		GameObject original = Resources.Load("Prefab/map_coin_exp") as GameObject;
		GameObject obj = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false);
		Transform transform = obj.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 position = obj.transform.position;
		transform.position = new Vector3(x, y, position.z);
		Utils.SetOrderInLayer(obj, order_in_layer);
		SpriteRenderer component = obj.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component.sprite = sprite;
		component = obj.transform.Find("shadow").GetComponent<SpriteRenderer>();
		component.sortingOrder = 1;
		TouchEvent component2 = obj.GetComponent<TouchEvent>();
		Timer.TimeData timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, delegate
		{
			UnityEngine.Object.Destroy(obj);
		});
		component2.ClickDown.AddListener(delegate
		{
			TouchCoinExp(obj, timer, call);
		});
		component2.ClickMove.AddListener(delegate
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj);
				call();
			}
		});
		Sequence sequence = DOTween.Sequence();
		obj.transform.localScale = new Vector3(0f, 0f, 1f);
		sequence.Append(obj.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.2f));
		sequence.Append(obj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
		BoxCollider2D collider = obj.GetComponent<BoxCollider2D>();
		collider.enabled = false;
		sequence.AppendCallback(delegate
		{
			collider.enabled = true;
		});
		sequence.Play();
		return obj;
	}

	private static void TouchCoinExp(GameObject obj, Timer.TimeData timer, UnityAction call)
	{
		call();
		if (timer != null)
		{
			Timer.Remove(timer);
		}
		UnityEngine.Object.Destroy(obj);
	}

	public static GameObject CreatePresentStoreBox(Transform t_parent, Vector3 pos, int base_order_in_layer, UnityAction ok, UnityAction cancel)
	{
		GameObject original = Resources.Load("Prefab/present_video") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Animation anim = touch.GetComponent<Animation>();
		touch.ClickDown.AddListener(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.HARVEST);
			anim.Play();
		});
		touch.ClickUp.AddListener(delegate
		{
			Prompt.CreateVideoStorePrompt(manager.transform, Vector2.zero, ok, cancel);
			UnityEngine.Object.Destroy(touch.gameObject);
		});
		Utils.SetOrderInLayer(touch.gameObject, base_order_in_layer);
		AppearPresent(touch.transform, pos);
		return touch.gameObject;
	}

	public static GameObject CreatePresentConstructBox(Transform parent, Vector3 pos, int base_order_in_layer, UnityAction ok, UnityAction cancel)
	{
		GameObject original = Resources.Load("Prefab/present_video") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Utils.SetOrderInLayer(touch.gameObject, base_order_in_layer);
		Animation anim = touch.GetComponent<Animation>();
		touch.ClickDown.AddListener(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.HARVEST);
			anim.Play();
		});
		touch.ClickUp.AddListener(delegate
		{
			Prompt prompt = Prompt.CreateVideoConstructPrompt(manager.transform, Vector2.zero, ok, cancel);
			UnityEngine.Object.Destroy(touch.gameObject);
		});
		AppearPresent(touch.transform, pos);
		return touch.gameObject;
	}

	public static GameObject CreatePresentConstructBoxEarly(Transform t_parent, Vector3 pos, int base_order_in_layer, UnityAction call)
	{
		GameObject original = Resources.Load("Prefab/present_video") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Utils.SetOrderInLayer(touch.gameObject, base_order_in_layer);
		Animation anim = touch.GetComponent<Animation>();
		touch.ClickDown.AddListener(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.HARVEST);
			anim.Play();
		});
		touch.ClickUp.AddListener(delegate
		{
			call();
			UnityEngine.Object.Destroy(touch.gameObject);
		});
		AppearPresent(touch.transform, pos);
		return touch.gameObject;
	}

	private static void AppearPresent(Transform t, Vector3 pos)
	{
		Sequence sequence = DOTween.Sequence();
		Vector2 vector = t.parent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height)));
		t.transform.localPosition = new Vector3(pos.x, vector.y, pos.z);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.FALL);
		});
		sequence.Append(t.DOLocalMoveY(pos.y + 0.1f, 1f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.PRESENT);
		});
		sequence.Append(t.DOLocalMoveY(pos.y, 1f));
		sequence.Loops();
		sequence.Play();
	}

	public static GameObject CreatePresentCoinEggBox(Transform t_parent, Vector2 pos, int base_order_in_layer, UnityAction touch_egg)
	{
		Utils.Log("CreatePresentCoinEggBox: " + base_order_in_layer);
		GameObject original = Resources.Load("Prefab/egg_coin") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Transform transform = touch.gameObject.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = touch.gameObject.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		Animator animator = touch.GetComponent<Animator>();
		Utils.Play(animator, "egg_open", 1f, 0f);
		Manager.sound.PlaySe(Sound.eSe.GET);
		UnityAction call = delegate
		{
			BoxCollider2D collider = touch.GetComponent<BoxCollider2D>();
			int coin = Price.CoinEgg();
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				collider.enabled = false;
			});
			sequence.AppendCallback(delegate
			{
				Utils.Play(animator, "egg_break", 1f, 0f);
				Manager.sound.PlaySe(Sound.eSe.EGG_BREAK);
				animator.GetComponent<AnimEvent>().auto_destroy = true;
			});
			sequence.AppendInterval(0.3f);
			sequence.AppendCallback(delegate
			{
				Prompt.CreateVideoCoinPrompt(coin, manager.transform, Vector2.zero, delegate
				{
					manager.PlayVideo(delegate
					{
						PlayVideoCompletedCoin(coin, t_parent, pos);
					}, null);
				});
				if (animator.gameObject != null)
				{
					UnityEngine.Object.Destroy(animator.gameObject);
				}
			});
			sequence.AppendCallback(delegate
			{
				touch_egg();
			});
			sequence.Play();
		};
		touch.ClickUp.AddListener(call);
		Utils.SetOrderInLayer(touch.gameObject, base_order_in_layer + 1);
		return touch.gameObject;
	}

	private static void PlayVideoCompletedCoin(int coin, Transform parent, Vector2 pos)
	{
		Manager.sound.PlaySe(Sound.eSe.COINS);
		Effect.Coin(coin, 10, parent.transform.TransformPoint(pos), CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.COINS_ARRIVAL);
		});
		manager.LoadVideo();
	}

	public static GameObject CreatePresentExpEggBox(Transform t_parent, Vector2 pos, int base_order_in_layer, UnityAction touch_egg)
	{
		GameObject original = Resources.Load("Prefab/egg_exp") as GameObject;
		TouchEvent touch = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<TouchEvent>();
		Transform transform = touch.gameObject.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = touch.gameObject.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		Animator animator = touch.GetComponent<Animator>();
		Utils.Play(animator, "egg_open", 1f, 0f);
		Manager.sound.PlaySe(Sound.eSe.GET);
		UnityAction call = delegate
		{
			BoxCollider2D collider = touch.GetComponent<BoxCollider2D>();
			int exp = Price.ExpEgg();
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				collider.enabled = false;
			});
			sequence.AppendCallback(delegate
			{
				Utils.Play(animator, "egg_break", 1f, 0f);
				Manager.sound.PlaySe(Sound.eSe.EGG_BREAK);
				animator.GetComponent<AnimEvent>().auto_destroy = true;
			});
			sequence.AppendInterval(0.3f);
			sequence.AppendCallback(delegate
			{
				Prompt.CreateVideoExpPrompt(exp, manager.transform, Vector2.zero, delegate
				{
					manager.PlayVideo(delegate
					{
						PlayVideoCompletedExp(exp, t_parent, pos);
					}, null);
				});
				if (animator.gameObject != null)
				{
					UnityEngine.Object.Destroy(animator.gameObject);
				}
			});
			sequence.AppendCallback(delegate
			{
				touch_egg();
			});
			sequence.Play();
		};
		touch.ClickUp.AddListener(call);
		Utils.SetOrderInLayer(touch.gameObject, base_order_in_layer);
		return touch.gameObject;
	}

	private static void PlayVideoCompletedExp(int exp, Transform parent, Vector2 pos)
	{
		Manager.sound.PlaySe(Sound.eSe.EXP);
		Effect.Exp(exp, 10, parent.transform.TransformPoint(pos), LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
		});
		manager.LoadVideo();
	}

	public static Bus ComeBus(Vector2 start_pos, Vector2 end_pos, bool flipX, float dulation, float interval, int order_in_layer, UnityAction arrival_call, UnityAction leave_call)
	{
		Bus bus = new Bus();
		GameObject original = null;
		string anim_name = "bus_idle";
		if (Data.farm_type == Data.eFarmType.NORMAL)
		{
			original = (Resources.Load("Prefab/bus") as GameObject);
		}
		else if (Data.farm_type == Data.eFarmType.RESORT)
		{
			original = (Resources.Load("Prefab/helicopter") as GameObject);
			anim_name = "helicopter_idle";
		}
		bus.animator = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false).GetComponent<Animator>();
		bus.audio = bus.animator.gameObject.GetComponent<AudioSource>();
		Utils.Play(bus.animator, anim_name, 1f, 0f);
		Utils.SetOrderInLayer(bus.animator.gameObject, order_in_layer);
		Transform transform = bus.animator.transform;
		transform.localScale = new Vector3((!flipX) ? 1 : (-1), 1f, 1f);
		Transform transform2 = transform;
		float x = start_pos.x;
		float y = start_pos.y;
		Vector3 localPosition = transform.localPosition;
		transform2.localPosition = new Vector3(x, y, localPosition.z);
		Sequence s = DOTween.Sequence();
		s.AppendCallback(delegate
		{
			bus.audio.Play();
		});
		s.Append(transform.DOLocalMoveX(end_pos.x, dulation));
		if (Data.farm_type != Data.eFarmType.RESORT)
		{
			s.AppendCallback(delegate
			{
				bus.audio.Stop();
				bus.audio.PlayOneShot(Manager.sound.SE[26]);
			});
		}
		s.AppendInterval(0.2f);
		anim_name = ((Data.farm_type != 0) ? "helicopter_tank" : "bus_open");
		int se_index = (Data.farm_type != 0) ? 70 : 27;
		s.AppendCallback(delegate
		{
			bus.audio.PlayOneShot(Manager.sound.SE[se_index]);
			Utils.Play(bus.animator, anim_name, 1f, 0f);
		});
		s.AppendInterval(interval);
		bus.state = Bus.eState.COME;
		if (arrival_call != null)
		{
			bus.arrival.AddListener(arrival_call);
		}
		if (leave_call != null)
		{
			bus.leave.AddListener(leave_call);
		}
		s.AppendCallback(delegate
		{
			bus.state = Bus.eState.STOP;
			bus.arrival.Invoke();
		});
		return bus;
	}

	public static bool RegistCallbackBus(Bus bus, UnityAction arrival_call, UnityAction leave_call)
	{
		bool result = false;
		if (bus.state == Bus.eState.COME)
		{
			bus.arrival.AddListener(arrival_call);
			bus.leave.AddListener(leave_call);
			result = true;
		}
		return result;
	}

	public static void LeaveBus(Bus bus, Vector2 end_pos, bool flipX, float dulation, UnityAction leave_call)
	{
		Utils.Play(bus.animator, (Data.farm_type != 0) ? "helicopter_idle2" : "bus_close", 1f, 0f);
		bus.audio.PlayOneShot(Manager.sound.SE[27]);
		flipX = ((Data.farm_type != 0) ? (!flipX) : flipX);
		bus.animator.transform.localScale = new Vector3((!flipX) ? 1 : (-1), 1f, 1f);
		Sequence s = DOTween.Sequence();
		s.AppendInterval(0.5f);
		s.AppendCallback(delegate
		{
			bus.audio.Play();
		});
		s.Append(bus.animator.transform.DOLocalMoveX(end_pos.x, dulation));
		s.AppendCallback(bus.leave.Invoke);
		GameObject obj = bus.animator.gameObject;
		s.AppendCallback(delegate
		{
			UnityEngine.Object.Destroy(obj);
		});
		if (leave_call != null)
		{
			s.AppendCallback(leave_call.Invoke);
		}
	}

	public static void CreateLevelUpEffect(int level, Data.eMainType type)
	{
		GameObject original = Resources.Load("Prefab/levelup") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		gameObject.GetComponent<LevelUpEvent>().Run(level, type);
	}

	public static Animation CreateNotice()
	{
		GameObject original = Resources.Load("Prefab/notice") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false);
		return gameObject.GetComponent<Animation>();
	}

	public static void CreateQuestion(Vector2 pos, float dulation)
	{
		GameObject original = Resources.Load("Prefab/question") as GameObject;
		GameObject obj = UnityEngine.Object.Instantiate(original, manager.transform, worldPositionStays: false);
		obj.transform.localPosition = pos;
		Sequence s = DOTween.Sequence();
		s.AppendInterval(dulation);
		s.AppendCallback(delegate
		{
			UnityEngine.Object.Destroy(obj);
		});
	}

	public static GameObject CreateSelector(Rect rect, Transform parent, Vector2 pos, int order_in_layer)
	{
		GameObject original = Resources.Load("Prefab/selector") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false);
		gameObject.transform.localPosition = pos;
		SpriteRenderer[] array = new SpriteRenderer[4]
		{
			gameObject.transform.Find("top_L").GetComponent<SpriteRenderer>(),
			gameObject.transform.Find("top_R").GetComponent<SpriteRenderer>(),
			gameObject.transform.Find("bottom_L").GetComponent<SpriteRenderer>(),
			gameObject.transform.Find("bottom_R").GetComponent<SpriteRenderer>()
		};
		Utils.SetOrderInLayer(gameObject, order_in_layer);
		Vector2[] array2 = new Vector2[4]
		{
			new Vector2(rect.xMin, rect.yMax),
			new Vector2(rect.xMax, rect.yMax),
			new Vector2(rect.xMin, rect.xMin),
			new Vector2(rect.xMax, rect.xMin)
		};
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].transform.localPosition = array2[i] * 1.3f;
			sequence.Join(array[i].transform.DOLocalMove(array2[i], 0.2f));
		}
		sequence.Play();
		return gameObject;
	}

	public static SugorokuFarmPrompt CreateSugorokuFarmPrompt(int coin, int exp, Transform t_parent, Vector2 pos)
	{
		SugorokuFarmPrompt sugorokuFarmPrompt = new SugorokuFarmPrompt();
		GameObject original = Resources.Load("Prefab/sugoroku_farm_prompt") as GameObject;
		sugorokuFarmPrompt.root = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false);
		Transform transform = sugorokuFarmPrompt.root.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = sugorokuFarmPrompt.root.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		PartsController component = sugorokuFarmPrompt.root.transform.Find("bg/character").GetComponent<PartsController>();
		component.Init(MainCharacter.style[(int)manager.data.main_type]);
		component.Play(PartsController.eAnimType._BINOCULARS_1_DOWN, 1f, 0f);
		sugorokuFarmPrompt.coin = sugorokuFarmPrompt.root.transform.Find("bg/coin");
		TextMesh component2 = sugorokuFarmPrompt.coin.transform.Find("text").GetComponent<TextMesh>();
		component2.text = coin.ToString();
		Transform coin2 = sugorokuFarmPrompt.coin;
		Vector3 localPosition2 = sugorokuFarmPrompt.coin.localPosition;
		float x2 = localPosition2.x - 0.03f * (float)(component2.text.Length - 1);
		Vector3 localPosition3 = sugorokuFarmPrompt.coin.localPosition;
		coin2.localPosition = new Vector2(x2, localPosition3.y);
		sugorokuFarmPrompt.exp = sugorokuFarmPrompt.root.transform.Find("bg/exp");
		component2 = sugorokuFarmPrompt.exp.Find("text").GetComponent<TextMesh>();
		component2.text = exp.ToString();
		Transform exp2 = sugorokuFarmPrompt.exp;
		Vector3 localPosition4 = sugorokuFarmPrompt.exp.localPosition;
		float x3 = localPosition4.x - 0.03f * (float)(component2.text.Length - 1);
		Vector3 localPosition5 = sugorokuFarmPrompt.exp.localPosition;
		exp2.localPosition = new Vector2(x3, localPosition5.y);
		return sugorokuFarmPrompt;
	}
}
