using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MainCharacter
{
	public enum eWorkPlace
	{
		NONE = -1,
		FACILITY_0,
		FACILITY_1,
		FACILITY_2,
		FACILITY_3,
		FACILITY_4,
		FACILITY_5,
		FACILITY_6,
		FACILITY_7,
		FACILITY_8,
		FACILITY_9,
		FACILITY_10,
		HOTEL,
		SAILO,
		MAX
	}

	public enum eType
	{
		WORKER_1 = 0,
		WORKER_2 = 1,
		WORKER_3 = 2,
		WORKER_4 = 3,
		WORKER_5 = 4,
		WORKER_6 = 5,
		WORKER_7 = 6,
		WORKER_8 = 7,
		WORKER_9 = 8,
		WORKER_10 = 9,
		WORKER_11 = 10,
		WORKER_12 = 11,
		WORKER_13 = 12,
		WORKER_14 = 13,
		WORKER_15 = 14,
		WORKER_16 = 0xF,
		WORKER_MAX = 0x10,
		SPECIAL_1 = 0x10,
		SPECIAL_2 = 17,
		SPECIAL_3 = 18,
		SPECIAL_4 = 19,
		SPECIAL_5 = 20,
		SPECIAL_6 = 21,
		SPECIAL_7 = 22,
		SPECIAL_8 = 23,
		SPECIAL_9 = 24,
		MAX = 25
	}

	public enum eLevel
	{
		UNREG,
		STAR_1,
		STAR_2,
		STAR_3,
		STAR_4,
		MAX
	}

	[Serializable]
	public class MapInfo
	{
		public List<Vector2> pos;

		public List<bool> keep_out;

		public MapInfo()
		{
			pos = new List<Vector2>();
			keep_out = new List<bool>();
		}
	}

	private const int MOTION_FOLK = 1;

	private const int MOTION_CART = 2;

	private int motion_no = 1;

	private int motion_play = -1;

	public new static List<PartsController.Style> style = new List<PartsController.Style>
	{
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_1, PartsController.eHat.NONE, PartsController.eHair.TYPE_3, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_3),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_2, PartsController.eHat.NONE, PartsController.eHair.TYPE_4, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_4),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_3, PartsController.eHat.NONE, PartsController.eHair.TYPE_5, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_5),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_4, PartsController.eHat.NONE, PartsController.eHair.TYPE_6, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_6),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_5, PartsController.eHat.TYPE_2, PartsController.eHair.TYPE_7, PartsController.eFace.TYPE_2, PartsController.eEye.TYPE_4),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_6, PartsController.eHat.NONE, PartsController.eHair.TYPE_8, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_7),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_7, PartsController.eHat.NONE, PartsController.eHair.TYPE_9, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_8),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_8, PartsController.eHat.NONE, PartsController.eHair.TYPE_10, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_9),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_9, PartsController.eHat.NONE, PartsController.eHair.TYPE_26, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_10, PartsController.eHat.NONE, PartsController.eHair.TYPE_27, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_17),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_11, PartsController.eHat.NONE, PartsController.eHair.TYPE_28, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_18),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_12, PartsController.eHat.NONE, PartsController.eHair.TYPE_29, PartsController.eFace.TYPE_3, PartsController.eEye.TYPE_14),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_13, PartsController.eHat.NONE, PartsController.eHair.TYPE_30, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_19),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_14, PartsController.eHat.NONE, PartsController.eHair.TYPE_31, PartsController.eFace.TYPE_3, PartsController.eEye.TYPE_20),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_15, PartsController.eHat.NONE, PartsController.eHair.TYPE_32, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_21),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_16, PartsController.eHat.NONE, PartsController.eHair.TYPE_33, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_22),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_17, PartsController.eHat.NONE, PartsController.eHair.TYPE_34, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_23),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_18, PartsController.eHat.TYPE_9, PartsController.eHair.TYPE_35, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_4),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_19, PartsController.eHat.TYPE_10, PartsController.eHair.TYPE_36, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_3),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_20, PartsController.eHat.TYPE_15, PartsController.eHair.TYPE_21, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_6),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_21, PartsController.eHat.TYPE_11, PartsController.eHair.TYPE_31, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_3),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_22, PartsController.eHat.TYPE_12, PartsController.eHair.TYPE_37, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_15),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_23, PartsController.eHat.NONE, PartsController.eHair.TYPE_38, PartsController.eFace.TYPE_3, PartsController.eEye.TYPE_4),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_24, PartsController.eHat.TYPE_13, PartsController.eHair.TYPE_39, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_4),
		new PartsController.Style(PartsController.eCharacter.WORKER, PartsController.eCloth.TYPE_25, PartsController.eHat.TYPE_14, PartsController.eHair.TYPE_40, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1)
	};

	public const int MAP_FACILITY = 0;

	public const int MAP_HOTEL = 1;

	public const int MAP_SAILO = 2;

	public static int[] WORKPLACE_TO_MAPINDEX = new int[13]
	{
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		1,
		2
	};

	public readonly int[] LINE_MAX = new int[3]
	{
		3,
		1,
		1
	};

	public readonly int[] COLM_MAX = new int[3]
	{
		5,
		4,
		4
	};

	public float[] BETWEEN_LINE = new float[3]
	{
		0.2f,
		0.068f,
		0.2f
	};

	public readonly float[] BETWEEN_COLM = new float[3]
	{
		0.21f,
		0.34f,
		0.2f
	};

	public readonly float[] START_POS_X = new float[3]
	{
		-0.42f,
		-0.6f,
		0f
	};

	public readonly float[] START_POS_Y = new float[3]
	{
		0.02f,
		-0.27f,
		0.48f
	};

	public MapInfo my_map;

	public eType type;

	public eWorkPlace place;

	private bool initial;

	private int base_order;

	public List<int> coin_bag_list = new List<int>();

	private float grass_cut_time;

	private const float GRASS_CUT_TIMER = 2.5f;

	private float harvest_time;

	private const float HARVEST_TIMER = 1f;

	private float harvest_to = 1f;

	public static Worker Create(Manager manager, eType type, eWorkPlace space, Transform parent)
	{
		Worker worker = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/human"), parent).AddComponent<Worker>();
		worker.Open(manager, -1, Vector2.zero, eState.FIELD, (int)type, (int)space);
		return worker;
	}

	public override void Open(Manager m, int idx, Vector2 pos, eState sts, int type, int id)
	{
		manager = m;
		map = manager.map;
		state = sts;
		direction = eDIRECTION.DOWN;
		renderers = new CharRenderers();
		controller = GetComponent<PartsController>();
		controller.Init(style[type]);
		SetPos(idx, pos);
		Initialize(type, id);
		move = eMove.WAIT;
	}

	public override void Initialize(int type, int place)
	{
		this.type = (eType)type;
		this.place = (eWorkPlace)place;
		destination = -1;
		grass_cut_info = new GrassCutInfo();
		construct_info = new ConstructInfo(3f);
		List<int> list = new List<int>();
		my_map = new MapInfo();
		if (Data.farm_type == Data.eFarmType.RESORT)
		{
			BETWEEN_LINE[0] = 0.16f;
		}
		int num = WORKPLACE_TO_MAPINDEX[place];
		for (int i = 0; i < LINE_MAX[num] * COLM_MAX[num]; i++)
		{
			my_map.pos.Add(new Vector2(START_POS_X[num] + BETWEEN_COLM[num] * (float)(i % COLM_MAX[num]), START_POS_Y[num] - BETWEEN_LINE[num] * (float)(i / COLM_MAX[num])));
			bool flag = false;
			if (num == 0)
			{
				if (i / COLM_MAX[num] != LINE_MAX[num] - 1 && i % COLM_MAX[num] != 0 && i % COLM_MAX[num] != COLM_MAX[num] - 1)
				{
					flag = true;
				}
				else if (place == 10 && (i % COLM_MAX[num] == 0 || i % COLM_MAX[num] == COLM_MAX[num] - 1))
				{
					flag = true;
				}
				else if (place % 2 == 0 && i % COLM_MAX[num] == COLM_MAX[num] - 1)
				{
					flag = true;
				}
				else if (place % 2 == 1 && i % COLM_MAX[num] == 0)
				{
					flag = true;
				}
				if (Data.farm_type == Data.eFarmType.RESORT && place % 2 == 0 && i % COLM_MAX[num] == 0)
				{
					flag = true;
				}
				if (Data.farm_type == Data.eFarmType.RESORT && place % 2 == 1 && i % COLM_MAX[num] == COLM_MAX[num] - 1)
				{
					flag = true;
				}
				my_map.keep_out.Add(flag);
			}
			else
			{
				my_map.keep_out.Add(flag);
			}
			if (!flag)
			{
				list.Add(i);
			}
		}
		if (WORKPLACE_TO_MAPINDEX[place] == 0)
		{
			base_order = manager.map.facility_list[place].GetBaseOrder() + 105;
		}
		else if (WORKPLACE_TO_MAPINDEX[place] == 1)
		{
			base_order = 20005;
		}
		else
		{
			base_order = 25000;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		SetPos(list[index], my_map.pos[list[index]]);
		initial = true;
		SetSortingOrder(base_order);
		NextTurn();
	}

	public override void PlayTurnAnimation()
	{
		PartsController.eAnimType eAnimType = (PartsController.eAnimType)Enum.Parse(typeof(PartsController.eAnimType), motion.ToString() + motion_no + submotion[(int)direction].ToUpper());
		controller.Play(eAnimType);
	}

	public override void PlayWorkAnimation()
	{
		PartsController.eAnimType eAnimType = (PartsController.eAnimType)Enum.Parse(typeof(PartsController.eAnimType), work_motion.ToString() + submotion[(int)WorkDir[(int)work_motion]].ToUpper());
		controller.Play(eAnimType);
	}

	public override void SetSortingOrder()
	{
		controller.SetSortingOrder(base_order);
	}

	public int GetNext(int prev)
	{
		int result = prev;
		int num = WORKPLACE_TO_MAPINDEX[(int)place];
		List<int> list = new List<int>();
		if (prev % COLM_MAX[num] > 0)
		{
			list.Add(-1);
		}
		if (prev % COLM_MAX[num] < COLM_MAX[num] - 1)
		{
			list.Add(1);
		}
		if (prev / COLM_MAX[num] > 0)
		{
			list.Add(-COLM_MAX[num]);
		}
		if (prev / COLM_MAX[num] < LINE_MAX[num] - 1)
		{
			list.Add(COLM_MAX[num]);
		}
		list.Add(0);
		while (list.Count != 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			int num2 = prev + list[index];
			if (num2 == prev)
			{
				break;
			}
			if (my_map.keep_out[num2])
			{
				list.RemoveAt(index);
				continue;
			}
			result = num2;
			break;
		}
		return result;
	}

	private int GetRoute(int prev, int target)
	{
		int num = prev;
		int num2 = WORKPLACE_TO_MAPINDEX[(int)place];
		if (target != num)
		{
			num = ((prev / COLM_MAX[num2] != target / COLM_MAX[num2]) ? (prev + ((target <= prev) ? (-COLM_MAX[num2]) : COLM_MAX[num2])) : (prev + ((target > prev) ? 1 : (-1))));
		}
		return num;
	}

	public override void NextTurn()
	{
		CheckCoinBag();
		prev_index = current_index;
		if (destination == -1)
		{
			current_index = GetNext(prev_index);
		}
		else
		{
			current_index = GetRoute(prev_index, destination);
		}
		move = eMove.ACTIVE;
		Vector2 move_pos = my_map.pos[current_index];
		SetMotionAndDirection();
		DoMove(move_pos, Map.eType.FIELD);
	}

	public override void DoMove(Vector2 move_pos, Map.eType place)
	{
		SetFlipX((direction == eDIRECTION.RIGHT) ? true : false);
		PlayTurnAnimation();
		base.transform.DOKill();
		float duration = (destination != -1) ? 1f : 2f;
		base.transform.DOLocalMove(move_pos, duration).SetEase(Ease.Linear).OnComplete(((Character)this).OnMoveComplete);
	}

	private void SetMotionAndDirection()
	{
		int num = WORKPLACE_TO_MAPINDEX[(int)place];
		List<int> list = new List<int>();
		list.Add(-map.COLM_MAX[num]);
		list.Add(map.COLM_MAX[num]);
		list.Add(1);
		list.Add(-1);
		List<int> list2 = list;
		if (prev_index == current_index)
		{
			motion = eMOTIONTYPE._STAY_;
		}
		else
		{
			motion = eMOTIONTYPE._WALK_;
			direction = (eDIRECTION)list2.IndexOf(current_index - prev_index);
		}
		SetSortingOrder(base_order);
		if (direction == eDIRECTION.LEFT || direction == eDIRECTION.RIGHT)
		{
			if (motion_play == -1)
			{
				GetRandomMotion();
			}
			else
			{
				motion_play++;
				if (motion_play >= 3)
				{
					GetRandomMotion();
				}
			}
		}
		else
		{
			motion_no = 1;
			motion_play = -1;
		}
		if (WORKPLACE_TO_MAPINDEX[(int)place] == 1)
		{
			motion_no = 1;
			SetWorkItem(PartsController.ePartsItem.NONE, PartsController.ePartsItem.NONE);
		}
		else if (motion_no == 1)
		{
			if (direction == eDIRECTION.DOWN)
			{
				SetWorkItem((Data.farm_type != Data.eFarmType.RESORT) ? PartsController.ePartsItem.FORK : PartsController.ePartsItem.MOP, PartsController.ePartsItem.NONE);
			}
			else
			{
				SetWorkItem(PartsController.ePartsItem.NONE, (Data.farm_type != Data.eFarmType.RESORT) ? PartsController.ePartsItem.FORK : PartsController.ePartsItem.MOP);
			}
		}
		else
		{
			SetWorkItem(PartsController.ePartsItem.NONE, (Data.farm_type != Data.eFarmType.RESORT) ? PartsController.ePartsItem.KART : PartsController.ePartsItem.KART_2);
		}
	}

	private void SetWorkItem(PartsController.ePartsItem item1, PartsController.ePartsItem item2)
	{
		controller.SetItem(PartsController.Parts.eType.ITEM1, item1);
		controller.SetItem(PartsController.Parts.eType.ITEM2, item2);
	}

	private void GetRandomMotion()
	{
		motion_play = 0;
		motion_no = UnityEngine.Random.Range(1, 3);
	}

	public override void OnMoveComplete()
	{
		if (destination == current_index)
		{
			CollectCoinBag();
			destination = -1;
		}
		else
		{
			NextTurn();
		}
	}

	public void RegistCoinBagQueue(int i)
	{
		coin_bag_list.Add(i);
	}

	private void CheckCoinBag()
	{
		if (destination == -1 && WORKPLACE_TO_MAPINDEX[(int)place] == 1 && coin_bag_list.Count != 0)
		{
			destination = coin_bag_list[0];
		}
	}

	private void CollectCoinBag()
	{
		if (WORKPLACE_TO_MAPINDEX[(int)place] == 1 && current_index == destination)
		{
			move = eMove.EVENT;
			base.transform.DOKill();
			harvest = true;
			work_motion = eWorkMotion._GET_1;
			PlayWorkAnimation();
			manager.hotel.CollectCoinBag(destination);
			coin_bag_list.Remove(destination);
		}
	}

	private void ReturnField()
	{
		move = eMove.WAIT;
		SetPos(current_index, my_map.pos[current_index]);
		motion = eMOTIONTYPE._STAY_;
		NextTurn();
	}

	public override void RegistGrassCutQueue(Grass grass)
	{
		if (grass.IsLong())
		{
			grass_cut_info.queue.Add(grass);
			grass.Cut(((MainCharacter)this).NotifyGrassCut);
			if (grass_cut_info.queue.Count == 1)
			{
				grass_cut_info.prev_id = grass.facility.my_id;
			}
		}
	}

	public override void NotifyGrassCut(Grass grass)
	{
		grass_cut_info.queue.Remove(grass);
		if (grass_cut_info.queue.Count == 0 && manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)type]] >= 2)
		{
			manager.map.facility_list[(int)place].WorkerGetCoinExp(delegate
			{
			});
		}
	}

	private void CheckCutGrassTime()
	{
		if (WORKPLACE_TO_MAPINDEX[(int)place] == 0 && manager.map.facility_list[(int)place].state == Facility.eState.GRASS)
		{
			grass_cut_time += Time.deltaTime;
			if (grass_cut_time >= 2.5f)
			{
				manager.map.facility_list[(int)place].CheckGrass();
				grass_cut_time = 0f;
			}
		}
	}

	public void GetCoinExp(GameObject coin_exp)
	{
		move = eMove.EVENT;
		coin_exp.GetComponent<TouchEvent>().ClickDown.Invoke();
	}

	public override void RegistConstructQueue(List<Data.ConstructPosition> list)
	{
		construct_info.queue.AddRange(list);
		construct_info.isConst = true;
		construct_info.current = 0;
		construct_info.hammer = 0f;
		DoConstruct();
	}

	public override void DoConstruct()
	{
		move = eMove.EVENT;
		base.transform.DOKill();
		work_motion = (construct_info.queue[construct_info.current].land ? eWorkMotion._CONST_1 : eWorkMotion._CONST_2);
		SetWorkItem(PartsController.ePartsItem.NONE, PartsController.ePartsItem.NONE);
		PlayWorkAnimation();
		SetSortingOrder(construct_info.queue[construct_info.current].order_in_layer);
		SetPos(construct_info.queue[construct_info.current].position);
		SetFlipX(construct_info.queue[construct_info.current].flipX);
		construct_info.monitor.Start();
	}

	public override void NotifyConstruct(List<Data.ConstructPosition> list)
	{
		Data.ConstructPosition constructPosition = construct_info.queue[construct_info.current];
		int index = construct_info.queue.IndexOf(list[0]);
		construct_info.queue.RemoveRange(index, list.Count);
		construct_info.isConst = false;
		construct_info.monitor.Stop();
		ReturnField();
	}

	public override bool DoHarvest(HarvestInfo info, bool interval_disp)
	{
		SetWorkItem(PartsController.ePartsItem.NONE, PartsController.ePartsItem.NONE);
		bool flag = base.DoHarvest(info, interval_disp);
		if (!flag)
		{
			harvest_to = 5f;
		}
		else
		{
			harvest_to = 1f;
		}
		return flag;
	}

	public override void RegistHarvestQueue(HarvestInfo info)
	{
		harvest_info.Add(info);
		if (harvest_info.Count == 1)
		{
			base.transform.parent = manager.transform;
			DoHarvest(info, interval_disp: true);
		}
	}

	public override void FinishHarvest(bool interval_disp)
	{
		if (harvest_info.Count != 0)
		{
			harvest_info[0].Destroy();
			harvest_info.RemoveAt(0);
		}
		if (harvest_info.Count != 0)
		{
			base.transform.parent = manager.transform;
			DoHarvest(harvest_info[0], interval_disp);
		}
		else
		{
			base.transform.parent = manager.map.facility_list[(int)place].transform;
			ReturnField();
		}
	}

	private void CheckHarvestTime()
	{
		if (WORKPLACE_TO_MAPINDEX[(int)place] == 0 && manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)type]] >= 3 && (manager.map.facility_list[(int)place].state == Facility.eState.ACTIVE || manager.map.facility_list[(int)place].tree_info.plant != Facility.TreeInfo.ePlant.NOTING))
		{
			harvest_time += Time.deltaTime;
			if (harvest_time >= harvest_to)
			{
				manager.map.facility_list[(int)place].CheckHarvest();
				harvest_time = 0f;
			}
		}
	}

	public override void CheckHarvest()
	{
		if (!harvest || controller.IsPlaying())
		{
			return;
		}
		harvest = false;
		if (WORKPLACE_TO_MAPINDEX[(int)place] == 0)
		{
			if (work_motion == eWorkMotion._FISHING_1 && success_fishing)
			{
				harvest_info[0].facility.NotifyFishng();
			}
			SetFlipX(flipX: false);
			FinishHarvest(interval_disp: true);
		}
		else if (WORKPLACE_TO_MAPINDEX[(int)place] == 1)
		{
			ReturnField();
		}
	}

	public void Close()
	{
		foreach (HarvestInfo item in harvest_info)
		{
			if (item.animal != null)
			{
				item.animal.SetTouchEnable(enable: true);
			}
			else
			{
				item.facility.SetTouchEnabled(enable: true);
			}
		}
		harvest_info.Clear();
	}

	private void Update()
	{
		if (initial)
		{
			CheckCutGrassTime();
			CheckHarvestTime();
			CheckConstruct();
			CheckHarvest();
		}
	}
}
