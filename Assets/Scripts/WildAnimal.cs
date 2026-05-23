using System;
using System.Collections.Generic;
using UnityEngine;

public class WildAnimal : Character
{
	public enum eType
	{
		NONE = -1,
		WOLF_1,
		WOODPECKER_1,
		KINGFISHER_1,
		RABBIT_1,
		RABBIT_2,
		FOX_1,
		RACCOON_1,
		DEER_1,
		SQUIRRE_1,
		CHICK_1,
		DUCK_1,
		SWAN_1,
		RABBIT_3,
		KANGAROO_1,
		WOMBAT_1,
		WOMBAT_2,
		KOALA_1,
		KOALA_2,
		SLOTH_1,
		SLOTH_2,
		PLATYPUS_1,
		OWL_1,
		SEAGULL_1,
		MONKEY_1,
		FLAMINGO_1,
		PELICAN_1,
		MAX
	}

	public enum eCATEGORY
	{
		NORMAL,
		BIRD,
		SMALL,
		CHILD,
		FOWL,
		MAX
	}

	public static List<List<eType>> animal_list = new List<List<eType>>(5)
	{
		new List<eType>
		{
			eType.WOLF_1,
			eType.RABBIT_1,
			eType.RABBIT_2,
			eType.FOX_1,
			eType.RACCOON_1,
			eType.DEER_1,
			eType.RABBIT_3,
			eType.KANGAROO_1,
			eType.WOMBAT_1,
			eType.WOMBAT_2,
			eType.KOALA_1,
			eType.KOALA_2,
			eType.SLOTH_1,
			eType.SLOTH_2,
			eType.PLATYPUS_1
		},
		new List<eType>
		{
			eType.WOODPECKER_1,
			eType.KINGFISHER_1,
			eType.OWL_1,
			eType.SEAGULL_1
		},
		new List<eType>
		{
			eType.SQUIRRE_1,
			eType.MONKEY_1
		},
		new List<eType>
		{
			eType.CHICK_1
		},
		new List<eType>
		{
			eType.SWAN_1,
			eType.DUCK_1,
			eType.FLAMINGO_1,
			eType.PELICAN_1
		}
	};

	public eType type;

	public eCATEGORY category;

	public int my_id;

	protected Timer.TimeData heart_timer;

	private Map.eType place;

	protected int[,] motion_pattarn = new int[26, 2]
	{
		{
			1,
			1
		},
		{
			2,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			2,
			1
		},
		{
			1,
			0
		},
		{
			1,
			0
		},
		{
			1,
			0
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			1,
			1
		},
		{
			2,
			1
		},
		{
			1,
			0
		},
		{
			1,
			0
		}
	};

	public static readonly Data.Condition[,] tCONDITION = new Data.Condition[26, 6]
	{
		{
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2),
			null,
			null,
			null,
			null,
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 0),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 0),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 0),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 4),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 4),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 16),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 17),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 11),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 7),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 0),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 13),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 5),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 5),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 0),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 7),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 12),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 27),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 5),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 22),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 23),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 27),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 28),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 29),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 24),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 19),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 26),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 28),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 21),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 24),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 19),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 18),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 33),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITYITEM, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 27),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 18),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 19),
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		}
	};

	public static readonly eType[,] tWILDANIMAL_ORDER = new eType[2, 13]
	{
		{
			eType.WOLF_1,
			eType.RABBIT_1,
			eType.WOODPECKER_1,
			eType.SWAN_1,
			eType.SQUIRRE_1,
			eType.KINGFISHER_1,
			eType.RABBIT_2,
			eType.CHICK_1,
			eType.DEER_1,
			eType.RABBIT_3,
			eType.DUCK_1,
			eType.FOX_1,
			eType.RACCOON_1
		},
		{
			eType.KANGAROO_1,
			eType.WOMBAT_1,
			eType.WOMBAT_2,
			eType.KOALA_1,
			eType.KOALA_2,
			eType.SLOTH_1,
			eType.SLOTH_2,
			eType.PLATYPUS_1,
			eType.OWL_1,
			eType.SEAGULL_1,
			eType.MONKEY_1,
			eType.FLAMINGO_1,
			eType.PELICAN_1
		}
	};

	public static int GetConditionLength()
	{
		return tCONDITION.GetLength(1);
	}

	public static Data.Condition GetConditions(eType t, int index)
	{
		return tCONDITION[(int)t, index];
	}

	public static List<eType> GetList(eCATEGORY category)
	{
		return animal_list[(int)category];
	}

	public static eCATEGORY TypeToCategory(eType t)
	{
		bool flag = false;
		eCATEGORY result = eCATEGORY.NORMAL;
		for (int i = 0; i < 5; i++)
		{
			if (flag)
			{
				break;
			}
			flag = animal_list[i].Contains(t);
			result = (eCATEGORY)i;
		}
		return result;
	}

	public override void Initialize(int t, int id)
	{
		my_id = id;
		type = (eType)t;
		category = TypeToCategory(type);
		renderers = new CharRenderers(base.transform);
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = (RuntimeAnimatorController)UnityEngine.Object.Instantiate(Resources.Load("Animation/" + GetType().FullName.ToLower() + "/" + type.ToString().ToLower()));
		motion = eMOTIONTYPE._STAY_;
		direction = eDIRECTION.DOWN;
		PlayTurnAnimation();
		SetHeartTimer();
	}

	protected void SetHeartTimer()
	{
		if (heart_timer != null)
		{
			Timer.Remove(heart_timer);
		}
		if (manager.data.character_data[1].contents[(int)type].reg == 0)
		{
			heart_timer = Timer.Create((ulong)DateTime.Now.Ticks, 10, OccurHeartTime, chk_owner: true, base.transform);
		}
		else
		{
			heart_timer = Timer.Create((ulong)DateTime.Now.Ticks, UnityEngine.Random.Range(30, 60), OccurHeartTime, chk_owner: true, base.transform);
		}
	}

	protected void OccurHeartTime()
	{
		Common.OccurHeart(type, base.transform, renderers.sprite[0].sortingOrder + 100, SetHeartTimer);
	}

	public override void SetSortingOrder()
	{
		base.SetSortingOrder();
		if (place == Map.eType.W_ENTRY || place == Map.eType.W_EXIT)
		{
			renderers.SetSortingOrder(40 + map.COLM_MAX[0] * map.LINE_MAX[0] * 2 + current_index);
		}
	}

	public override void NextTurn()
	{
		move = eMove.ACTIVE;
		prev_index = current_index;
		place = Map.eType.FIELD;
		if (state == eState.PREEXIT)
		{
			map.RemoveTurnCharactor(this);
			SetDestination(map.FIELD_GATEWAY[(int)Data.farm_type, 5]);
			map.ReseveSquare(prev_index, reserve: false, place);
			state = eState.EXIT;
			move = eMove.EVENT;
		}
		Vector2 move_pos;
		if (state == eState.FIELD)
		{
			move_pos = ((destination == -1) ? RandomMoveInField(place) : ((destination != current_index) ? ForcedMoveInField(place) : ArriveDestinationInField()));
			OccurExp();
		}
		else if (state == eState.ENTRY)
		{
			place = Map.eType.W_ENTRY;
			current_index = map.GetNextEntryRoute(prev_index, ref place);
			if (place == Map.eType.W_ENTRY)
			{
				move_pos = map.GetRoutePos(current_index, place);
			}
			else
			{
				state = eState.FIELD;
				move_pos = map.GetFieldPos(current_index, place);
			}
			motion = ((prev_index != current_index) ? eMOTIONTYPE._WALK_ : eMOTIONTYPE._STAY_);
			direction = eDIRECTION.UP;
		}
		else
		{
			place = Map.eType.W_EXIT;
			if (destination == current_index)
			{
				destination = 500;
				current_index = map.GetNextExitRoute(-1, -1, place);
				move_pos = map.GetRoutePos(current_index, place);
				direction = eDIRECTION.DOWN;
			}
			else if (destination == 500)
			{
				if (current_index == map.ENTRY[(int)place])
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				current_index = map.GetNextExitRoute(current_index, map.ENTRY[(int)place], place);
				move_pos = map.GetRoutePos(current_index, place);
				direction = eDIRECTION.DOWN;
			}
			else
			{
				place = Map.eType.FIELD;
				current_index = map.GetNextExit(current_index, destination, place);
				move_pos = map.GetFieldPos(current_index, place);
				SetMotionAndDirection(place);
			}
			motion = eMOTIONTYPE._WALK_;
		}
		DoMove(move_pos, place);
	}

	public override void PlayTurnAnimation()
	{
		int num = UnityEngine.Random.Range(0, motion_pattarn[(int)type, (int)motion]) + 1;
		if (type == eType.WOODPECKER_1 && motion == eMOTIONTYPE._STAY_ && num == 2)
		{
			Manager.sound.PlaySe(Sound.eSe.WOODPECKER);
		}
		string name = type.ToString().ToLower() + motion.ToString().ToLower() + num + submotion[(int)direction];
		Utils.Play(animator, name, 1f);
	}

	public override void ReturnBase()
	{
		state = eState.PREEXIT;
		if (heart_timer != null)
		{
			Timer.Remove(heart_timer);
			heart_timer = null;
		}
	}

	private void OccurExp()
	{
		if (category == eCATEGORY.NORMAL && UnityEngine.Random.Range(0, 15) == 5)
		{
			Vector3 position = base.transform.position;
			float x = position.x + 0.1f;
			Vector3 position2 = base.transform.position;
			Vector2 pos = new Vector2(x, position2.y - 0.01f);
			GameObject obj = Common.OccurMapExp(Price.ExpWildAnimalTip(type), pos, renderers.sprite[0].sortingOrder - 3, delegate
			{
			});
			Timer.Create((ulong)DateTime.Now.Ticks, 10, delegate
			{
				if (obj != null)
				{
					UnityEngine.Object.Destroy(obj);
				}
			});
		}
	}
}
