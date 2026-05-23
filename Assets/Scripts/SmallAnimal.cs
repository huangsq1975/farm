using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SmallAnimal : WildAnimal
{
	private const int SORTING_ORDER_BIRD = 780;

	private new const float SPEED_NORMAL = 0.5f;

	private new const float SPEED_RETURN = 0.25f;

	[SerializeField]
	private int random_count;

	[SerializeField]
	private Vector2 target;

	[SerializeField]
	private int tree;

	private bool IsBird(eType t)
	{
		return WildAnimal.animal_list[1].Contains(t);
	}

	private bool IsSamll(eType t)
	{
		return WildAnimal.animal_list[2].Contains(t);
	}

	public override void SetSortingOrder()
	{
		if (IsBird(type))
		{
			renderers.SetSortingOrder(780);
		}
		else if (current_index == destination && tree != -1)
		{
			renderers.SetSortingOrder(map.facility_list[tree].tree_info.sprite.sortingOrder + 1);
		}
		else if (prev_index % map.COLM_MAX[2] != map.KEEPOUT_L[2] && prev_index % map.COLM_MAX[2] != map.KEEPOUT_R[2])
		{
			renderers.SetSortingOrder(40);
		}
	}

	public override void SetMotionAndDirection(Map.eType place)
	{
		base.SetMotionAndDirection(place);
		List<int> list = new List<int>();
		list.Add(-map.COLM_MAX[(int)place]);
		list.Add(map.COLM_MAX[(int)place]);
		list.Add(1);
		list.Add(-1);
		List<int> list2 = list;
		if (prev_index == current_index && tree != -1 && type != eType.SQUIRRE_1 && type != eType.MONKEY_1)
		{
			direction = ((tree % 2 != 0) ? eDIRECTION.RIGHT : eDIRECTION.LEFT);
			motion = eMOTIONTYPE._STAY_;
		}
		else if (prev_index == current_index && tree != -1 && (type == eType.SQUIRRE_1 || type == eType.MONKEY_1))
		{
			direction = eDIRECTION.DOWN;
			motion = eMOTIONTYPE._STAY_;
		}
	}

	public override void Initialize(int t, int id)
	{
		my_id = id;
		type = (eType)t;
		category = WildAnimal.TypeToCategory(type);
		tree = -1;
		destination = current_index;
		renderers = new CharRenderers(base.transform);
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = (RuntimeAnimatorController)Object.Instantiate(Resources.Load("Animation/wildanimal/" + type.ToString().ToLower()));
		NextTurn();
		SetHeartTimer();
	}

	public override void NextTurn()
	{
		prev_index = current_index;
		move = eMove.ACTIVE;
		Map.eType place = (!WildAnimal.animal_list[2].Contains(type)) ? Map.eType.AIR : Map.eType.SMALL;
		if (state == eState.PREEXIT)
		{
			SetDestination(map.FIELD_GATEWAY[(int)Data.farm_type, (!IsSamll(type)) ? 6 : 7]);
			state = eState.EXIT;
			move = eMove.EVENT;
		}
		Vector2 move_pos;
		if (state == eState.FIELD)
		{
			if (destination == prev_index && tree != -1)
			{
				state = eState.FACILITY;
				move_pos = base.transform.localPosition;
				SetMotionAndDirection(place);
			}
			else if (destination == prev_index)
			{
				SelectNextFieldAction(place);
				move_pos = ForcedMoveInField(place);
			}
			else
			{
				move_pos = ForcedMoveInField(place);
				if (Random.Range(0, 35) == 5 && category == eCATEGORY.BIRD)
				{
					if (Data.farm_type == Data.eFarmType.NORMAL)
					{
						Manager.sound.PlaySe(Sound.eSe.BIRD);
					}
					else if (Data.farm_type == Data.eFarmType.RESORT)
					{
						if (type == eType.SEAGULL_1)
						{
							Manager.sound.PlaySe(Sound.eSe.SEAGULL);
						}
						else if (type == eType.OWL_1)
						{
							Manager.sound.PlaySe(Sound.eSe.OWL);
						}
					}
				}
			}
		}
		else if (state == eState.FACILITY)
		{
			SelectNextFacilityAction(place);
			move_pos = map.GetFieldPos(current_index, place);
			SetMotionAndDirection(place);
		}
		else if (state == eState.ENTRY)
		{
			place = ((!IsSamll(type)) ? Map.eType.AIR_GATE : Map.eType.SMALL_GATE);
			current_index = map.GetNextEntryRoute(prev_index, ref place);
			if (place == Map.eType.SMALL_GATE || place == Map.eType.AIR_GATE)
			{
				move_pos = map.GetRoutePos(current_index, place);
			}
			else
			{
				state = eState.FIELD;
				destination = current_index;
				move_pos = map.GetFieldPos(current_index, place);
			}
			direction = ((!IsSamll(type)) ? eDIRECTION.LEFT : eDIRECTION.RIGHT);
			motion = eMOTIONTYPE._WALK_;
		}
		else
		{
			place = ((!IsSamll(type)) ? Map.eType.AIR_GATE : Map.eType.SMALL_GATE);
			if (destination == current_index)
			{
				destination = 500;
				current_index = map.GetNextExitRoute(-1, -1, place);
				move_pos = map.GetRoutePos(current_index, place);
				direction = ((!IsSamll(type)) ? eDIRECTION.RIGHT : eDIRECTION.LEFT);
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
				direction = ((!IsSamll(type)) ? eDIRECTION.RIGHT : eDIRECTION.LEFT);
			}
			else
			{
				place = ((!IsSamll(type)) ? Map.eType.AIR : Map.eType.SMALL);
				current_index = map.GetNextExit(current_index, destination, place);
				move_pos = map.GetFieldPos(current_index, place);
				SetMotionAndDirection(place);
			}
			motion = eMOTIONTYPE._WALK_;
		}
		DoMove(move_pos, place);
	}

	public void SelectNextFieldAction(Map.eType place)
	{
		random_count++;
		if ((random_count > 3 && Random.Range(0, 3) == 0) || place == Map.eType.SMALL)
		{
			destination = map.GetTreePos((type == eType.SQUIRRE_1 || type == eType.MONKEY_1) ? Facility.TreeInfo.eAnimal.SMALL : Facility.TreeInfo.eAnimal.BIRD, this, out tree);
			random_count = ((destination == -1) ? random_count : 0);
		}
		if (tree == -1)
		{
			destination = ((place != Map.eType.SMALL) ? map.GetNextTarget(prev_index, place) : current_index);
		}
	}

	public void SelectNextFacilityAction(Map.eType place)
	{
		random_count++;
		if (random_count > 15 && Random.Range(0, 2) == 0)
		{
			map.ReleaseTree(tree, (type == eType.SQUIRRE_1 || type == eType.MONKEY_1) ? Facility.TreeInfo.eAnimal.SMALL : Facility.TreeInfo.eAnimal.BIRD);
			random_count = 0;
			tree = -1;
			state = eState.FIELD;
			if (place == Map.eType.SMALL)
			{
				SelectNextFieldAction(place);
			}
			else
			{
				destination = map.GetNextTarget(prev_index, place);
			}
		}
	}

	public override void OnMoveComplete()
	{
		move = eMove.WAIT;
		NextTurn();
	}

	public override void DoMove(Vector2 move_pos, Map.eType place)
	{
		if (motion == eMOTIONTYPE._STAY_ && !IsSamll(type))
		{
			renderers.SetFlipX((direction == eDIRECTION.RIGHT) ? true : false);
			renderers.SetFlipY(flip: false);
		}
		else if (motion == eMOTIONTYPE._WALK_ && !IsSamll(type))
		{
			renderers.SetFlipY((direction == eDIRECTION.RIGHT) ? true : false);
			renderers.SetFlipX(flip: false);
		}
		else
		{
			renderers.SetFlipX((direction == eDIRECTION.RIGHT) ? true : false);
		}
		PlayTurnAnimation();
		SetSortingOrder();
		base.transform.DOLocalMove(move_pos, 0.25f).SetEase(Ease.Linear).OnComplete(((Character)this).OnMoveComplete);
	}

	public override void ReturnBase()
	{
		if (heart_timer != null)
		{
			Timer.Remove(heart_timer);
			heart_timer = null;
		}
		tree = -1;
		state = eState.PREEXIT;
	}
}
