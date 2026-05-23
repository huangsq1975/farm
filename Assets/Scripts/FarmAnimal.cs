using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

public class FarmAnimal : MonoBehaviour
{
	public enum eType
	{
		SHEEP_1 = 0,
		COW_1 = 1,
		CHICKEN_1 = 2,
		HORSE_1 = 3,
		PIG_1 = 4,
		ALPACA_1 = 5,
		GOAT_1 = 6,
		TURKEY_1 = 7,
		COW_2 = 8,
		CHICKEN_2 = 9,
		PIG_2 = 10,
		HORSE_2 = 11,
		HORSE_3 = 12,
		SHEEP_2 = 13,
		ALPACA_2 = 14,
		CHICKEN_3 = 0xF,
		HONEY_1 = 0x10,
		HONEY_2 = 17,
		WATER_BUFFALO_1 = 18,
		WATER_BUFFALO_2 = 19,
		BLUE_WHALE_1 = 20,
		SPERM_WHALE_1 = 21,
		SEA_OTTER_1 = 22,
		SHARK_1 = 23,
		CROCODILE_1 = 24,
		TURTLE_1 = 25,
		GIANT_SQUID_1 = 26,
		DOLPHIN_1 = 27,
		DOLPHIN_2 = 28,
		DOLPHIN_3 = 29,
		SEA_LION_1 = 30,
		WALRUS_1 = 0x1F,
		FRUIT_1 = 0x20,
		FRUIT_2 = 33,
		FRUIT_3 = 34,
		KILLER_WHALE_1 = 35,
		MAX = 36,
		NONE = -1
	}

	public enum ePrefix
	{
		SHEEP = 0,
		COW = 1,
		CHICKEN = 2,
		HORSE = 3,
		PIG = 4,
		ALPACA = 5,
		GOAT = 6,
		TURKEY = 7,
		HONEY = 8,
		WATER_BUFFALO = 9,
		BLUE_WHALE = 10,
		SPERM_WHALE = 11,
		SEA_OTTER = 12,
		SHARK = 13,
		CROCODILE = 14,
		TURTLE = 0xF,
		GIANT_SQUID = 0x10,
		DOLPHIN = 17,
		SEA_LION = 18,
		WALRUS = 19,
		KILLER_WHALE = 20,
		FRUIT = 21,
		MAX = 22,
		NONE = -1
	}

	public enum eState
	{
		_ALBUM_1,
		_STAY_1,
		_STAY_2,
		_GET_1,
		_FULL_1,
		_WALK_1,
		_GET_2
	}

	public enum eDirection
	{
		_DOWN,
		_SIDE
	}

	public static Sound.eSe[] PrefixToSound = new Sound.eSe[22]
	{
		Sound.eSe.SHEEP,
		Sound.eSe.COW,
		Sound.eSe.CHICKEN,
		Sound.eSe.HORSE,
		Sound.eSe.PIG,
		Sound.eSe.ALPACA,
		Sound.eSe.GOAT,
		Sound.eSe.TURKEY,
		Sound.eSe.CLICK,
		Sound.eSe.COW,
		Sound.eSe.SPOUTING,
		Sound.eSe.SPOUTING,
		Sound.eSe.SPLASH,
		Sound.eSe.SPLASH,
		Sound.eSe.SPLASH,
		Sound.eSe.SPLASH,
		Sound.eSe.SPLASH,
		Sound.eSe.APPLAUSE,
		Sound.eSe.SEA_LION,
		Sound.eSe.WALRUS,
		Sound.eSe.APPLAUSE,
		Sound.eSe.CLICK
	};

	private Facility facility;

	public eType type;

	public int my_id;

	public ePrefix prefix;

	private int base_order;

	[SerializeField]
	private eState state;

	[SerializeField]
	private eDirection direction;

	[SerializeField]
	private Character.CharRenderers renderers;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TouchEvent touch;

	public const int GET_HARVEST_ORDER = 1999;

	public const int NORMAL_FARM_MAX = 18;

	public const int RESORT_FARM_MAX = 18;

	/// <summary>stay 動畫片段的 sprite keyframe 多在 clip 中後段，從接近結尾處播放可避免 Rebind 後 t=0 無圖。</summary>
	private const float STAY_ANIM_START_NORMALIZED = 0.999f;

	public static readonly Data.Condition[,] tCONDITION = new Data.Condition[36, 2]
	{
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 4)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 4)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 7)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 18)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 3)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 18)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 12)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 6)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 28)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 13)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 24)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 21)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 3)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 12)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 23)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.CASHER),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 18)
		}
	};

	private Vector2 reserve_pos;

	public static readonly eType[,] tFARMANIMAL_ORDER = new eType[2, 18]
	{
		{
			eType.SHEEP_1,
			eType.CHICKEN_1,
			eType.PIG_1,
			eType.COW_1,
			eType.HORSE_1,
			eType.HONEY_1,
			eType.ALPACA_1,
			eType.GOAT_1,
			eType.TURKEY_1,
			eType.COW_2,
			eType.CHICKEN_2,
			eType.PIG_2,
			eType.HORSE_2,
			eType.HONEY_2,
			eType.SHEEP_2,
			eType.ALPACA_2,
			eType.HORSE_3,
			eType.CHICKEN_3
		},
		{
			eType.WATER_BUFFALO_1,
			eType.WATER_BUFFALO_2,
			eType.BLUE_WHALE_1,
			eType.SPERM_WHALE_1,
			eType.SEA_OTTER_1,
			eType.SHARK_1,
			eType.CROCODILE_1,
			eType.TURTLE_1,
			eType.GIANT_SQUID_1,
			eType.DOLPHIN_1,
			eType.DOLPHIN_2,
			eType.DOLPHIN_3,
			eType.KILLER_WHALE_1,
			eType.SEA_LION_1,
			eType.WALRUS_1,
			eType.FRUIT_1,
			eType.FRUIT_2,
			eType.FRUIT_3
		}
	};

	public static int GetConditionLength()
	{
		return tCONDITION.GetLength(1);
	}

	public static Data.Condition GetConditions(eType type, int index)
	{
		return tCONDITION[(int)type, index];
	}

	private void Update()
	{
		if (state == eState._GET_1)
		{
			if (!Utils.IsPlaying(animator))
			{
				state = eState._STAY_2;
				touch.SetEnabled(enabled: true);
				facility.NotifyHarvest(this);
			}
		}
		else
		{
			if (state != eState._FULL_1)
			{
				return;
			}
			if (prefix == ePrefix.DOLPHIN || prefix == ePrefix.KILLER_WHALE)
			{
				if (!Utils.IsPlaying(animator))
				{
					state = eState._STAY_1;
					PlayAnimation();
				}
			}
			else if (!Utils.IsReversePlaying(animator))
			{
				state = eState._STAY_1;
				PlayAnimation();
			}
		}
	}

	public void Open(Facility f, eType t, eState sts, Vector3 pos, int id, int order, bool select)
	{
		facility = f;
		type = t;
		prefix = TypeToPrefix(type);
		my_id = id;
		state = sts;
		base_order = order;
		direction = eDirection._DOWN;
		renderers = new Character.CharRenderers(base.transform);
		renderers.SetSelect(select);
		animator = GetComponent<Animator>();
		base.transform.localPosition = pos;
		touch = GetComponent<TouchEvent>();
		touch.ClickDown.AddListener(delegate
		{
			TouchDown(touch);
		});
		touch.ClickMove.AddListener(delegate
		{
			TouchMove(touch);
		});
		touch.ClickUp.AddListener(delegate
		{
			TouchUp(touch);
		});
		if (prefix == ePrefix.HONEY || prefix == ePrefix.FRUIT)
		{
			touch.ChangeColliderPos(new Vector2(0.0003046244f, 0.0008628964f), new Vector2(0.1611447f, 0.1376368f));
			renderers.SetSortingOrderAll(base_order);
		}
		else if (prefix == ePrefix.SEA_LION || prefix == ePrefix.WALRUS)
		{
			renderers.SetSortingOrderAll(base_order);
		}
		else
		{
			renderers.SetSortingOrder(base_order);
		}
		SetAnimation();
	}

	public static ePrefix TypeToPrefix(eType t)
	{
		ePrefix result = ePrefix.SHEEP;
		if (t != eType.NONE)
		{
			result = (ePrefix)Enum.Parse(typeof(ePrefix), t.ToString().Substring(0, t.ToString().LastIndexOf("_")));
		}
		return result;
	}

	public void ChangeSate(eState sts)
	{
		state = sts;
		if (state == eState._GET_1)
		{
			touch.SetEnabled(enabled: false);
		}
		if (state == eState._STAY_1 && prefix != ePrefix.HORSE)
		{
			state = eState._FULL_1;
			if (prefix == ePrefix.DOLPHIN || prefix == ePrefix.KILLER_WHALE)
			{
				PlayShowAnimation();
			}
			else
			{
				PlayReverseAnimation();
			}
		}
		else
		{
			PlayAnimation();
		}
	}

	private void TouchDown(TouchEvent t)
	{
		if (facility.HarvestAnimal(this, facility.main))
		{
			touch.SetEnabled(enabled: false);
		}
	}

	private void TouchMove(TouchEvent t)
	{
		if (facility.HarvestAnimal(this, facility.main))
		{
			touch.SetEnabled(enabled: false);
		}
	}

	private void TouchUp(TouchEvent t)
	{
		facility.TouchAnimal(this);
	}

	public void FailedHarvest(bool balloon)
	{
		if (balloon)
		{
			Common.OccurStoreMaxBalloon(base.transform, 1000, delegate
			{
				touch.SetEnabled(enabled: true);
			});
			return;
		}
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			touch.SetEnabled(enabled: true);
		});
		sequence.Play();
	}

	public void AutoHarvest()
	{
		if (touch.GetEnabled() && facility.HarvestAnimal(this, facility.worker))
		{
			touch.SetEnabled(enabled: false);
		}
	}

	public void RideBus(Vector2 start_pos, Vector2 end_pos, float duration, bool flipX, UnityAction callback)
	{
		renderers.SetSortingOrder(facility.GetBaseOrder() + 175 + 1);
		state = eState._WALK_1;
		direction = eDirection._SIDE;
		reserve_pos = base.transform.localPosition;
		base.transform.parent = facility.manager.transform;
		base.transform.localPosition = start_pos;
		renderers.SetFlipX(flipX);
		PlayAnimation();
		Manager.sound.PlaySe(Sound.eSe.HORSE_WALK);
		base.transform.DOLocalMove(end_pos, duration).SetEase(Ease.Linear).OnComplete(delegate
		{
			callback();
			RideBusComplete();
		});
	}

	private void RideBusComplete()
	{
		base.transform.parent = facility.transform;
		base.transform.localPosition = reserve_pos;
		renderers.SetSortingOrder(base_order);
		state = eState._STAY_1;
		direction = eDirection._DOWN;
		renderers.SetFlipX(flip: false);
		PlayAnimation();
		base.gameObject.SetActive(value: false);
	}

	public void ReturnBase()
	{
		base.gameObject.SetActive(value: true);
		touch.SetEnabled(enabled: true);
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		Vector2 pos = new Vector2(x, position2.y + 0.15f);
		Common.OccurGetHarvest(type, pos, facility.manager.store.GetStorePos(), 1999);
		int num = Price.ExpHarvest(type);
		if (facility.manager.data.AddLevelCondCount(Data.CharacterData.eType.FARMANIMAL, (int)type))
		{
			int level = facility.manager.data.character_data[0].contents[(int)type].level;
			pos = base.transform.TransformPoint(new Vector2(0f, 0.1f));
			Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
			Effect.LevelupSmall(level, pos, base.transform, Color.white);
			num *= 2;
		}
		Manager.sound.PlaySe(Sound.eSe.EXP);
		Effect.Exp(num, 1, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
		});
		facility.NotifyHarvest(this);
	}

	public void SetAnimation()
	{
		if (type != eType.NONE)
		{
			animator.runtimeAnimatorController = (RuntimeAnimatorController)UnityEngine.Object.Instantiate(Resources.Load("Animation/" + GetType().FullName.ToLower() + "/" + type.ToString().ToLower()));
			PlayAnimation();
		}
	}

	private static bool IsStayAnimState(eState animState)
	{
		return animState == eState._STAY_1 || animState == eState._STAY_2;
	}

	private void ApplyAnimatorPose()
	{
		if (animator != null && animator.isActiveAndEnabled)
		{
			animator.Update(0f);
		}
	}

	public void PlayAnimation()
	{
		string name = type.ToString().ToLower() + state.ToString().ToLower() + direction.ToString().ToLower();
		float start = IsStayAnimState(state) ? STAY_ANIM_START_NORMALIZED : 0f;
		bool rebind = !IsStayAnimState(state);
		Utils.Play(animator, name, 1f, start, rebind);
		ApplyAnimatorPose();
	}

	public void PlayReverseAnimation()
	{
		string name = type.ToString().ToLower() + eState._GET_1.ToString().ToLower() + direction.ToString().ToLower();
		Utils.Reverse(animator, name, 1f, STAY_ANIM_START_NORMALIZED, rebind: false);
		ApplyAnimatorPose();
	}

	public void PlayShowAnimation()
	{
		string name = type.ToString().ToLower() + eState._GET_2.ToString().ToLower() + direction.ToString().ToLower();
		Utils.Play(animator, name, 1f);
	}

	public void SetTouchEnable(bool enable)
	{
		touch.SetEnabled(enable);
	}
}
