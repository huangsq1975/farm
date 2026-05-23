using System;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Character
{
	public enum eType
	{
		NONE = -1,
		CUSTOMER_1,
		CUSTOMER_2,
		CUSTOMER_3,
		CUSTOMER_4,
		CUSTOMER_5,
		CUSTOMER_6,
		CUSTOMER_7,
		CUSTOMER_8,
		CUSTOMER_9,
		CUSTOMER_RESORT_1,
		CUSTOMER_RESORT_2,
		CUSTOMER_RESORT_3,
		CUSTOMER_RESORT_4,
		CUSTOMER_RESORT_5,
		CUSTOMER_RESORT_6,
		CUSTOMER_RESORT_7,
		CUSTOMER_RESORT_8,
		MAX
	}

	private int[,] motion_pattarn = new int[17, 2]
	{
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
		}
	};

	public eType type;

	private Map.eType place;

	public int my_id;

	public static readonly Data.Condition[,] tCONDITION = new Data.Condition[17, 6]
	{
		{
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 4),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 0),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 5),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 14),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 11),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 12),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.SEASON_EVENT, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 7),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 4),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.SEASON_EVENT, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null,
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 21),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 4),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null,
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 24),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 12),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1),
			null
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 22),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 28),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 18),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 27),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 31),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 26),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 19),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 29),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 32),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.SEASON_EVENT, 3),
			new Data.Condition(Data.Condition.eCATEGORY.HOTEL, 3),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 23),
			new Data.Condition(Data.Condition.eCATEGORY.FARMANIMAL, 24),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		}
	};

	public static List<PartsController.Style> style = new List<PartsController.Style>
	{
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_1, PartsController.eHat.NONE, PartsController.eHair.TYPE_11, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_5),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_2, PartsController.eHat.NONE, PartsController.eHair.TYPE_12, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_3, PartsController.eHat.NONE, PartsController.eHair.TYPE_13, PartsController.eFace.TYPE_3, PartsController.eEye.TYPE_10),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_4, PartsController.eHat.TYPE_3, PartsController.eHair.TYPE_14, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_5),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_5, PartsController.eHat.NONE, PartsController.eHair.TYPE_15, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_11),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_7, PartsController.eHat.NONE, PartsController.eHair.TYPE_16, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_5),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_8, PartsController.eHat.TYPE_4, PartsController.eHair.TYPE_17, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_9, PartsController.eHat.TYPE_5, PartsController.eHair.TYPE_18, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_12),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_10, PartsController.eHat.TYPE_6, PartsController.eHair.TYPE_1, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_11, PartsController.eHat.NONE, PartsController.eHair.TYPE_19, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_13),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_12, PartsController.eHat.NONE, PartsController.eHair.TYPE_20, PartsController.eFace.TYPE_3, PartsController.eEye.TYPE_14),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_13, PartsController.eHat.NONE, PartsController.eHair.TYPE_21, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_14, PartsController.eHat.NONE, PartsController.eHair.TYPE_22, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_15, PartsController.eHat.TYPE_7, PartsController.eHair.TYPE_23, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_16, PartsController.eHat.NONE, PartsController.eHair.TYPE_24, PartsController.eFace.TYPE_2, PartsController.eEye.TYPE_16),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_17, PartsController.eHat.TYPE_8, PartsController.eHair.TYPE_21, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.CUSTOMER, PartsController.eCloth.TYPE_18, PartsController.eHat.NONE, PartsController.eHair.TYPE_25, PartsController.eFace.TYPE_4, PartsController.eEye.TYPE_15)
	};

	public static readonly eType[,] tCUSTOMER_ORDER = new eType[2, 9]
	{
		{
			eType.CUSTOMER_1,
			eType.CUSTOMER_2,
			eType.CUSTOMER_3,
			eType.CUSTOMER_4,
			eType.CUSTOMER_5,
			eType.CUSTOMER_6,
			eType.CUSTOMER_7,
			eType.CUSTOMER_8,
			eType.CUSTOMER_9
		},
		{
			eType.CUSTOMER_RESORT_1,
			eType.CUSTOMER_RESORT_2,
			eType.CUSTOMER_RESORT_3,
			eType.CUSTOMER_RESORT_4,
			eType.CUSTOMER_RESORT_5,
			eType.CUSTOMER_RESORT_6,
			eType.CUSTOMER_RESORT_7,
			eType.CUSTOMER_RESORT_8,
			eType.NONE
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

	public override void Initialize(int type, int my_id)
	{
		this.my_id = my_id;
		this.type = (eType)type;
		controller = GetComponent<PartsController>();
		controller.Init(style[type]);
		motion = eMOTIONTYPE._STAY_;
		direction = eDIRECTION.DOWN;
		PlayTurnAnimation();
	}

	public override void SetSortingOrder()
	{
		base.SetSortingOrder();
		if (place == Map.eType.CUSTOMER)
		{
			controller.SetSortingOrder(40 - (map.COLM_MAX[3] - 1 - current_index) * 10);
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
			SetDestination(map.FIELD_GATEWAY[(int)Data.farm_type, 3]);
			map.ReseveSquare(prev_index, reserve: false, place);
			state = eState.EXIT;
			move = eMove.EVENT;
		}
		Vector2 move_pos;
		if (state == eState.FIELD)
		{
			move_pos = ((destination == -1) ? RandomMoveInField(place) : ((destination == current_index) ? ArriveDestinationInField() : ((destination != 501) ? ForcedMoveInField(place) : StayInVisitArea())));
			OccurTip();
		}
		else if (state == eState.ENTRY)
		{
			place = Map.eType.CUSTOMER;
			current_index = map.GetNextEntryRoute(prev_index, ref place);
			if (place == Map.eType.CUSTOMER)
			{
				move_pos = map.GetRoutePos(current_index, place);
				direction = eDIRECTION.RIGHT;
			}
			else
			{
				state = eState.FIELD;
				move_pos = map.GetFieldPos(current_index, place);
				direction = eDIRECTION.DOWN;
			}
			motion = ((prev_index != current_index) ? eMOTIONTYPE._WALK_ : eMOTIONTYPE._STAY_);
		}
		else
		{
			place = Map.eType.CUSTOMER;
			if (destination == current_index)
			{
				destination = 500;
				current_index = map.GetNextExitRoute(-1, -1, place);
				move_pos = map.GetRoutePos(current_index, place);
				direction = eDIRECTION.UP;
			}
			else if (destination == 500)
			{
				if (current_index != 0)
				{
					current_index = map.GetNextExitRoute(current_index, map.ENTRY[(int)place], place);
					direction = eDIRECTION.LEFT;
				}
				else
				{
					if (current_index != 0 || direction != eDIRECTION.LEFT)
					{
						manager.hotel.CustomerReturnRoom(type, my_id);
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
					direction = eDIRECTION.UP;
				}
				move_pos = map.GetRoutePos(current_index, place);
			}
			else
			{
				place = Map.eType.FIELD;
				current_index = map.GetNextExit(current_index, destination, place);
				SetMotionAndDirection(place);
				move_pos = map.GetFieldPos(current_index, place);
				SetMotionAndDirection(place);
			}
			motion = eMOTIONTYPE._WALK_;
		}
		DoMove(move_pos, place);
	}

	public override void PlayTurnAnimation()
	{
		PartsController.eAnimType eAnimType = (PartsController.eAnimType)Enum.Parse(typeof(PartsController.eAnimType), motion.ToString() + (UnityEngine.Random.Range(0, motion_pattarn[(int)type, (int)motion]) + 1) + submotion[(int)direction].ToUpper());
		controller.Play(eAnimType);
	}

	private void OccurTip()
	{
		if (UnityEngine.Random.Range(0, 5) == 1)
		{
			Vector3 position = base.transform.position;
			float x = position.x + 0.1f;
			Vector3 position2 = base.transform.position;
			Vector2 pos = new Vector2(x, position2.y - 0.01f);
			GameObject obj = Common.OccurMapCoin(Price.CoinCustomerTip(type), pos, controller.parts.rens[4].sortingOrder - 3, delegate
			{
			});
			Timer.Create((ulong)DateTime.Now.Ticks, 15, delegate
			{
				if (obj != null)
				{
					UnityEngine.Object.Destroy(obj);
				}
			});
		}
	}
}
