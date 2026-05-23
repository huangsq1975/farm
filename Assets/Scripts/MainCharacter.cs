using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCharacter : Character
{
	protected enum eWorkMotion
	{
		_CUT_1,
		_CONST_1,
		_FISHING_1,
		_GET_1,
		_GET_2,
		_CUT_2,
		_CONST_2,
		MAX
	}

	[Serializable]
	public class GrassCutInfo
	{
		public List<Grass> queue = new List<Grass>();

		public int prev_id = -1;
	}

	[Serializable]
	public class ConstructInfo
	{
		public bool isConst;

		public List<Data.ConstructPosition> queue = new List<Data.ConstructPosition>();

		public int current = -1;

		public Map.Monitor monitor;

		public float hammer;

		public ConstructInfo(float move_time)
		{
			monitor = new Map.Monitor(move_time);
		}

		public void Destroy()
		{
			queue.Clear();
			queue = null;
			monitor = null;
		}
	}

	[Serializable]
	public class HarvestInfo
	{
		public Vector2 pos;

		public Facility facility;

		public FarmAnimal animal;

		public bool flipX;

		public HarvestInfo(Vector2 harvest_pos, bool flip_x, Facility f, FarmAnimal a)
		{
			pos = harvest_pos;
			facility = f;
			animal = a;
			flipX = flip_x;
		}

		public void Destroy()
		{
			facility = null;
			animal = null;
		}
	}

	private int[,] motion_pattarn = new int[2, 2]
	{
		{
			1,
			1
		},
		{
			1,
			1
		}
	};

	protected eWorkMotion work_motion;

	protected eDIRECTION[] WorkDir = new eDIRECTION[7]
	{
		eDIRECTION.DOWN,
		eDIRECTION.LEFT,
		eDIRECTION.LEFT,
		eDIRECTION.UP,
		eDIRECTION.LEFT,
		eDIRECTION.DOWN,
		eDIRECTION.LEFT
	};

	public static List<PartsController.Style> style = new List<PartsController.Style>
	{
		new PartsController.Style(PartsController.eCharacter.MAIN, PartsController.eCloth.TYPE_1, PartsController.eHat.NONE, PartsController.eHair.TYPE_1, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1),
		new PartsController.Style(PartsController.eCharacter.MAIN, PartsController.eCloth.TYPE_2, PartsController.eHat.NONE, PartsController.eHair.TYPE_2, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_2),
		new PartsController.Style(PartsController.eCharacter.MAIN, PartsController.eCloth.TYPE_3, PartsController.eHat.NONE, PartsController.eHair.TYPE_1, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_1),
		new PartsController.Style(PartsController.eCharacter.MAIN, PartsController.eCloth.TYPE_4, PartsController.eHat.NONE, PartsController.eHair.TYPE_2, PartsController.eFace.TYPE_1, PartsController.eEye.TYPE_2)
	};

	[SerializeField]
	protected GrassCutInfo grass_cut_info;

	public const float HAMMER_SOUND_TIME = 0.5f;

	public const float MOVE_TIME = 3f;

	[SerializeField]
	protected ConstructInfo construct_info;

	[SerializeField]
	protected List<HarvestInfo> harvest_info = new List<HarvestInfo>();

	[SerializeField]
	protected bool harvest;

	[SerializeField]
	protected bool success_fishing;

	private Data.eMainType type;

	public int interval;

	private const int MAX_INTERVAL = 2;

	public override void Initialize(int t, int id)
	{
		type = (Data.eMainType)t;
		controller = GetComponent<PartsController>();
		controller.Init(style[t + (int)Data.farm_type * 2]);
		grass_cut_info = new GrassCutInfo();
		construct_info = new ConstructInfo(3f);
	}

	public override void NextTurn()
	{
		move = eMove.ACTIVE;
		prev_index = current_index;
		Vector2 move_pos = (destination == -1) ? RandomMoveInField(Map.eType.FIELD) : ((destination != current_index) ? ForcedMoveInField(Map.eType.FIELD) : ArriveDestinationInField());
		DoMove(move_pos, Map.eType.FIELD);
	}

	public override void PlayTurnAnimation()
	{
		PartsController.eAnimType eAnimType = (PartsController.eAnimType)Enum.Parse(typeof(PartsController.eAnimType), motion.ToString() + (UnityEngine.Random.Range(0, motion_pattarn[(int)type, (int)motion]) + 1) + submotion[(int)direction].ToUpper());
		controller.Play(eAnimType);
	}

	public virtual void PlayWorkAnimation()
	{
		PartsController.eAnimType eAnimType = (PartsController.eAnimType)Enum.Parse(typeof(PartsController.eAnimType), work_motion.ToString() + submotion[(int)WorkDir[(int)work_motion]].ToUpper());
		controller.Play(eAnimType);
	}

	private void SetHat(bool wear)
	{
	}

	private void ReturnField()
	{
		if (Manager.events.state == Event.eState.NONE)
		{
			map.AddTurnCharactor(this);
			move = eMove.WAIT;
		}
		SetPos(current_index, map.GetFieldPos(current_index, Map.eType.FIELD));
		motion = eMOTIONTYPE._STAY_;
		PlayTurnAnimation();
	}

	public virtual void RegistGrassCutQueue(Grass grass)
	{
		if (!grass.IsLong())
		{
			return;
		}
		grass_cut_info.queue.Add(grass);
		grass.Cut(NotifyGrassCut);
		if (construct_info.isConst)
		{
			construct_info.monitor.Stop();
		}
		if (harvest_info.Count == 0)
		{
			move = eMove.EVENT;
			base.transform.DOKill();
			work_motion = ((Data.farm_type == Data.eFarmType.RESORT) ? eWorkMotion._CUT_2 : eWorkMotion._CUT_1);
			PlayWorkAnimation();
			if (grass_cut_info.queue.Count == 1)
			{
				SetPos(manager.transform.InverseTransformPoint(grass.facility.transform.TransformPoint(grass.transform.localPosition)));
				map.RemoveTurnCharactor(this);
				grass_cut_info.prev_id = grass.facility.my_id;
			}
			else if (grass_cut_info.prev_id == grass.facility.my_id)
			{
				base.transform.DOMove(grass.transform.position, 0.1f).SetEase(Ease.Linear);
			}
			else
			{
				SetPos(grass.transform.position);
				grass_cut_info.prev_id = grass.facility.my_id;
			}
			SetSortingOrder(50 + 150 * (((grass_cut_info.prev_id == 10) ? 9 : grass_cut_info.prev_id) / 2));
		}
	}

	public virtual void NotifyGrassCut(Grass grass)
	{
		grass_cut_info.queue.Remove(grass);
		if (grass_cut_info.queue.Count == 0)
		{
			if (harvest_info.Count != 0)
			{
				harvest = true;
			}
			else if (construct_info.isConst)
			{
				DoConstruct();
			}
			else
			{
				ReturnField();
			}
		}
	}

	public virtual void RegistConstructQueue(List<Data.ConstructPosition> list)
	{
		construct_info.queue.AddRange(list);
		map.RemoveTurnCharactor(this);
		if (!construct_info.isConst)
		{
			construct_info.isConst = true;
			construct_info.current = 0;
			construct_info.hammer = 0f;
			if (Manager.events.state != Event.eState.ENDING)
			{
				Manager.sound.PlaySe(Sound.eSe.CONSTRUCT);
			}
			DoConstruct();
		}
	}

	public virtual void DoConstruct()
	{
		move = eMove.EVENT;
		base.transform.DOKill();
		work_motion = (construct_info.queue[construct_info.current].land ? eWorkMotion._CONST_1 : eWorkMotion._CONST_2);
		PlayWorkAnimation();
		SetSortingOrder(construct_info.queue[construct_info.current].order_in_layer);
		SetPos(construct_info.queue[construct_info.current].position);
		SetFlipX(construct_info.queue[construct_info.current].flipX);
		construct_info.monitor.Start();
	}

	public virtual void NotifyConstruct(List<Data.ConstructPosition> list)
	{
		Data.ConstructPosition item = construct_info.queue[construct_info.current];
		int num = construct_info.queue.IndexOf(list[0]);
		bool flag = false;
		if (num <= construct_info.current && construct_info.current < num + list.Count)
		{
			item = ((construct_info.queue.Count <= num + list.Count) ? construct_info.queue[0] : construct_info.queue[num + list.Count]);
			flag = true;
		}
		construct_info.queue.RemoveRange(num, list.Count);
		if (construct_info.queue.Count == 0)
		{
			construct_info.isConst = false;
			construct_info.monitor.Stop();
			if (grass_cut_info.queue.Count == 0)
			{
				ReturnField();
			}
		}
		else
		{
			construct_info.current = construct_info.queue.IndexOf(item);
			if (flag)
			{
				construct_info.monitor.Stop();
				DoConstruct();
			}
		}
	}

	public void CheckConstruct()
	{
		if (construct_info == null)
		{
			return;
		}
		if (construct_info.isConst && construct_info.monitor.TimeOut())
		{
			construct_info.current = ((construct_info.queue.Count > construct_info.current + 1) ? (construct_info.current + 1) : 0);
			DoConstruct();
		}
		else
		{
			if (!construct_info.isConst || controller.IsPlaying() || work_motion != eWorkMotion._CONST_1)
			{
				return;
			}
			construct_info.hammer += Time.deltaTime;
			if (construct_info.hammer >= 0.5f)
			{
				construct_info.hammer = 0f;
				if (Manager.events.state != Event.eState.ENDING)
				{
					Manager.sound.PlaySe(Sound.eSe.CONSTRUCT);
				}
			}
		}
	}

	public virtual void RegistHarvestQueue(HarvestInfo info)
	{
		if (construct_info.isConst)
		{
			construct_info.monitor.Stop();
		}
		harvest_info.Add(info);
		map.RemoveTurnCharactor(this);
		if (harvest_info.Count > 1)
		{
			controller.Stop();
		}
		else if (harvest_info.Count == 1)
		{
			DoHarvest(info, interval_disp: false);
		}
	}

	public void DelHarvestQueue(FarmAnimal farm_animal)
	{
		if (harvest_info.Count != 0)
		{
			HarvestInfo harvestInfo = harvest_info.Find((HarvestInfo item) => item.animal == farm_animal);
			if (harvestInfo != null)
			{
				harvest_info.Remove(harvestInfo);
				harvestInfo.Destroy();
			}
		}
	}

	public virtual bool DoHarvest(HarvestInfo info, bool interval_disp)
	{
		bool result = true;
		move = eMove.EVENT;
		harvest = true;
		base.transform.DOKill();
		if (info.animal == null)
		{
			work_motion = eWorkMotion._FISHING_1;
		}
		else
		{
			work_motion = ((info.animal.prefix == FarmAnimal.ePrefix.HORSE) ? eWorkMotion._GET_2 : eWorkMotion._GET_1);
		}
		SetFlipX(info.flipX);
		PlayWorkAnimation();
		SetSortingOrder(info.facility.GetBaseOrder() + 120);
		SetPos(manager.transform.InverseTransformPoint(info.pos));
		Fish.eType fish;
		if (info.animal != null && info.animal.prefix == FarmAnimal.ePrefix.HORSE)
		{
			Manager.sound.PlaySe(FarmAnimal.PrefixToSound[(int)info.animal.prefix]);
			info.facility.GoToRace(info.animal);
			interval = 0;
		}
		else if (info.animal != null && (info.animal.prefix == FarmAnimal.ePrefix.DOLPHIN || info.animal.prefix == FarmAnimal.ePrefix.SEA_LION || info.animal.prefix == FarmAnimal.ePrefix.WALRUS || info.animal.prefix == FarmAnimal.ePrefix.KILLER_WHALE))
		{
			if (info.animal.my_id < 2)
			{
				Manager.sound.PlaySe(FarmAnimal.PrefixToSound[(int)info.animal.prefix]);
			}
			Manager.sound.PlayGet();
			info.facility.DelGrassGauge(info.animal);
			info.animal.ChangeSate(FarmAnimal.eState._GET_1);
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			Vector2 pos = new Vector2(x, position2.y + 0.3f);
			Common.OccurGetHarvest(info.animal.type, pos, manager.store.GetStorePos(), 1999);
			int num = Price.ExpHarvest(info.animal.type);
			if (manager.data.AddLevelCondCount(Data.CharacterData.eType.FARMANIMAL, (int)info.animal.type))
			{
				int level = manager.data.character_data[0].contents[(int)info.animal.type].level;
				pos = info.animal.transform.TransformPoint(new Vector2(0f, 0.1f));
				Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
				Effect.LevelupSmall(level, pos, info.animal.transform, Color.white);
				num *= 2;
			}
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(num, 1, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			interval = 0;
		}
		else if (info.animal != null && manager.store.AddHarvest(info.animal.type))
		{
			if (info.animal.my_id < 2)
			{
				Manager.sound.PlaySe(FarmAnimal.PrefixToSound[(int)info.animal.prefix]);
			}
			Manager.sound.PlayGet();
			info.facility.DelGrassGauge(info.animal);
			info.animal.ChangeSate(FarmAnimal.eState._GET_1);
			Vector3 position3 = base.transform.position;
			float x2 = position3.x;
			Vector3 position4 = base.transform.position;
			Vector2 pos2 = new Vector2(x2, position4.y + 0.3f);
			Common.OccurGetHarvest(info.animal.type, pos2, manager.store.GetStorePos(), 1999);
			int num2 = Price.ExpHarvest(info.animal.type);
			if (manager.data.AddLevelCondCount(Data.CharacterData.eType.FARMANIMAL, (int)info.animal.type))
			{
				int level2 = manager.data.character_data[0].contents[(int)info.animal.type].level;
				pos2 = info.animal.transform.TransformPoint(new Vector2(0f, 0.1f));
				Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
				Effect.LevelupSmall(level2, pos2, info.animal.transform, Color.white);
				num2 *= 2;
			}
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(num2, 1, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			interval = 0;
		}
		else if (info.animal == null && manager.store.AddHarvest(fish = Fishing()))
		{
			success_fishing = true;
			Manager.sound.PlaySe(Sound.eSe.FISHING);
			info.facility.DelGrassGauge(null);
			Vector3 position5 = base.transform.position;
			float x3 = position5.x;
			Vector3 position6 = base.transform.position;
			Vector2 pos3 = new Vector2(x3, position6.y + 0.2f);
			Common.OccurGetHarvest(fish, pos3, manager.store.GetStorePos(), 1999);
			pos3.y += 0.5f;
			NotifyFishType(fish, pos3, info.facility.transform);
			interval = 0;
		}
		else
		{
			if (interval == 0)
			{
				manager.store.StoreMax();
			}
			bool balloon = (interval == 0) ? true : false;
			if (info.animal != null)
			{
				info.animal.FailedHarvest(balloon);
			}
			else
			{
				success_fishing = false;
				info.facility.FailedFishng(balloon);
			}
			interval = (interval_disp ? (interval + 1) : 0);
			if (2 < interval)
			{
				interval = 0;
			}
			result = false;
		}
		return result;
	}

	public void NotifyFishType(Fish.eType fish, Vector2 pos, Transform parent)
	{
		if (manager.data.GetReg(Data.CharacterData.eType.FISH, (int)fish) == 0)
		{
			manager.data.SetReg(Data.CharacterData.eType.FISH, (int)fish);
		}
		int num = Price.ExpHarvest(fish);
		if (manager.data.AddLevelCondCount(Data.CharacterData.eType.FISH, (int)fish))
		{
			int level = manager.data.character_data[2].contents[(int)fish].level;
			Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
			Effect.LevelupSmall(level, pos, parent, Color.white);
			num *= 2;
		}
		Manager.sound.PlaySe(Sound.eSe.EXP);
		Effect.Exp(num, 1, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
		});
	}

	private Fish.eType Fishing()
	{
		Common.ConditionCompare conditionCompare = null;
		conditionCompare = Common.GetCompareData(manager);
		List<Fish.eType> list = Enum.GetValues(typeof(Fish.eType)).Cast<Fish.eType>().ToList();
		List<Fish.eType> list2 = new List<Fish.eType>();
		list.Remove(Fish.eType.MAX);
		list.Remove(Fish.eType.NONE);
		for (int i = 0; i < list.Count; i++)
		{
			if (Common.IsMeetConditions(list[i], conditionCompare))
			{
				list2.Add(list[i]);
			}
		}
		if (list2.Count == 0)
		{
			return Fish.eType.AYU_1;
		}
		return list2[UnityEngine.Random.Range(0, list2.Count)];
	}

	public virtual void FinishHarvest(bool intaval_disp)
	{
		if (harvest_info.Count != 0)
		{
			harvest_info[0].Destroy();
			harvest_info.RemoveAt(0);
		}
		if (harvest_info.Count != 0)
		{
			DoHarvest(harvest_info[0], intaval_disp);
		}
		else if (construct_info.isConst)
		{
			DoConstruct();
		}
		else
		{
			ReturnField();
		}
	}

	public virtual void CheckHarvest()
	{
		if (harvest_info.Count != 0 && harvest && !controller.IsPlaying())
		{
			harvest = false;
			if (work_motion == eWorkMotion._FISHING_1 && success_fishing)
			{
				harvest_info[0].facility.NotifyFishng();
			}
			SetFlipX(flipX: false);
			FinishHarvest(intaval_disp: false);
		}
	}

	public void EventTutorialGrassCut()
	{
		base.transform.DOKill();
		map.RemoveTurnCharactor(this);
		SetPos(6, map.GetFieldPos(6, Map.eType.FIELD));
		direction = eDIRECTION.LEFT;
		motion = eMOTIONTYPE._STAY_;
		move = eMove.EVENT;
		SetFlipX(flipX: false);
		PlayTurnAnimation();
	}

	public void FinishEvent()
	{
		ReturnField();
	}

	private void Update()
	{
		CheckConstruct();
		CheckHarvest();
	}
}
