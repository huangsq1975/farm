using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class Facility : MonoBehaviour
{
	public enum eState
	{
		NONE = -1,
		GRASS,
		EMPTY,
		CONSTRUCT,
		FIX,
		ACTIVE,
		MAX
	}

	public enum eType
	{
		GRAZE_1 = 0,
		POULTRY_1 = 1,
		STABLE_1 = 2,
		RIVER_1 = 3,
		GRAZE_2 = 4,
		POULTRY_2 = 5,
		STABLE_2 = 6,
		RIVER_2 = 7,
		GRAZE_3 = 8,
		POULTRY_3 = 9,
		STABLE_3 = 10,
		NET_1 = 11,
		LANDSHOW_1 = 12,
		UNDERWATERSHOW_1 = 13,
		ROCKYSPOT_1 = 14,
		NET_2 = 0xF,
		NET_3 = 0x10,
		LANDSHOW_2 = 17,
		UNDERWATERSHOW_2 = 18,
		ROCKYSPOT_2 = 19,
		LANDSHOW_3 = 20,
		UNDERWATERSHOW_3 = 21,
		MAX = 22,
		NONE = -1
	}

	public enum ePrefix
	{
		GRAZE,
		POULTRY,
		STABLE,
		RIVER,
		NET,
		LANDSHOW,
		UNDERWATERSHOW,
		ROCKYSPOT,
		MAX
	}

	public enum eItem
	{
		TREE_1 = 0,
		TREE_2 = 1,
		TREE_3 = 2,
		TREE_4 = 3,
		TREE_5 = 4,
		TREE_6 = 5,
		MAX = 6,
		NONE = -1
	}

	[Serializable]
	public class BusInfo
	{
		public Common.Bus bus;

		public int call_num;

		private List<UnityAction> pending;

		public BusInfo()
		{
			bus = null;
			call_num = 0;
			pending = new List<UnityAction>();
		}

		public void Come(Vector2 start_pos, Vector2 end_pos, bool flipX, float dulation, float interval, int order_in_layer, UnityAction arrival_call, UnityAction leave_call)
		{
			if (bus == null)
			{
				bus = Common.ComeBus(start_pos, end_pos, flipX, dulation, interval, order_in_layer, arrival_call, leave_call);
				call_num++;
			}
			else if (Common.RegistCallbackBus(bus, arrival_call, leave_call))
			{
				call_num++;
			}
			else
			{
				SetNext(delegate
				{
					Come(start_pos, end_pos, flipX, dulation, interval, order_in_layer, arrival_call, leave_call);
				});
			}
		}

		public void Leave(Vector2 end_pos, bool flipX, float dulation)
		{
			if (isFinish())
			{
				Common.LeaveBus(bus, end_pos, flipX, dulation, Next);
			}
		}

		private void SetNext(UnityAction func)
		{
			pending.Add(func);
		}

		private void Next()
		{
			bus.animator = null;
			bus.audio = null;
			bus = null;
			if (pending.Count != 0)
			{
				foreach (UnityAction item in pending)
				{
					item();
				}
				pending.Clear();
			}
		}

		private bool isFinish()
		{
			bool result = false;
			call_num--;
			if (call_num <= 0)
			{
				result = true;
			}
			return result;
		}
	}

	[Serializable]
	public class Structures
	{
		public Facility f;

		public GameObject obj;

		public Grass[] grass = new Grass[6];

		public Animation anim;

		public Animator fish;

		public Vector2 fish_pos;

		public SpriteRenderer[] fish_ren = new SpriteRenderer[2];

		private const int FISH_MAX = 2;

		public Sequence fish_sequence;

		public Structures(Facility self)
		{
			f = self;
			obj = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/" + ChangeGrassFarmType(Data.farm_type, f.state) + f.GetType().FullName.ToLower() + MakeFacilityName(f.state, f.type)), f.transform);
			if (f.type == eType.NONE)
			{
				obj.transform.localPosition = Vector2.zero;
			}
			else
			{
				obj.transform.localPosition = tFACILITY_POS[(int)f.type];
			}
			anim = obj.GetComponent<Animation>();
			if (f.state == eState.ACTIVE && (f.prefix == ePrefix.RIVER || f.prefix == ePrefix.ROCKYSPOT))
			{
				fish = obj.transform.Find("contents/fish").gameObject.GetComponent<Animator>();
				fish_pos = fish.transform.localPosition;
				for (int i = 0; i < 2; i++)
				{
					fish_ren[i] = fish.transform.Find("sprite_" + (i + 1)).gameObject.GetComponent<SpriteRenderer>();
					fish_ren[i].color = Utils.Alpha(fish_ren[i].color, 0f);
				}
				Utils.Play(fish, "river_fish", 1f);
			}
			ControlSortingOrder(obj.transform, eSortMode.FIELD, f.GetBaseOrder());
			if (f.state == eState.GRASS || f.state == eState.CONSTRUCT)
			{
				obj.transform.localPosition = Vector2.zero;
				CreateGrass((f.state == eState.CONSTRUCT) ? true : false);
			}
		}

		private string ChangeGrassFarmType(Data.eFarmType ftype, eState state)
		{
			if (state == eState.GRASS || state == eState.EMPTY || state == eState.CONSTRUCT)
			{
				switch (ftype)
				{
				case Data.eFarmType.NORMAL:
					return "farm_0/";
				case Data.eFarmType.RESORT:
					return "farm_1/";
				}
			}
			return string.Empty;
		}

		public void Fish(bool set)
		{
			if (!(fish != null))
			{
				return;
			}
			if (fish_sequence != null)
			{
				fish_sequence.Kill();
			}
			fish_sequence = DOTween.Sequence();
			if (set)
			{
				fish_sequence.AppendCallback(delegate
				{
					fish.transform.localPosition = fish_pos;
				});
				for (int i = 0; i < 2; i++)
				{
					SpriteRenderer ren = fish_ren[i];
					fish_sequence.Append(DOTween.ToAlpha(() => ren.color, delegate(Color color)
					{
						ren.color = color;
					}, 1f, 0.3f));
				}
			}
			else
			{
				Sequence s = fish_sequence;
				Transform transform = fish.transform;
				Vector3 localPosition = fish.transform.localPosition;
				s.Append(transform.DOLocalMoveX(localPosition.x - 0.2f, 0.3f));
				for (int j = 0; j < 2; j++)
				{
					SpriteRenderer ren2 = fish_ren[j];
					fish_sequence.Append(DOTween.ToAlpha(() => ren2.color, delegate(Color color)
					{
						ren2.color = color;
					}, 0f, 0.2f));
				}
			}
			fish_sequence.AppendCallback(delegate
			{
				fish_sequence = null;
			});
			fish_sequence.Play();
		}

		public void Destroy()
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj);
				f = null;
				obj = null;
				anim = null;
				for (int i = 0; i < 6; i++)
				{
					grass[i] = null;
				}
			}
		}

		private void CreateGrass(bool empty)
		{
			Transform transform = obj.transform.Find("contents");
			for (int i = 0; i < 6; i++)
			{
				grass[i] = transform.Find("grass" + i).gameObject.GetComponent<Grass>();
				grass[i].Open(f, f.main, empty);
			}
		}
	}

	[Serializable]
	public class Selector
	{
		public GameObject obj;

		public Selector(Facility f, string name)
		{
			obj = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/" + f.GetType().FullName.ToLower() + name), f.transform);
			obj.transform.localPosition = Vector2.zero;
			ControlSortingOrder(obj.transform, eSortMode.SELECT, 10100);
		}

		public void Destroy()
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj);
				obj = null;
			}
		}
	}

	[Serializable]
	public class AnimalInfo
	{
		public enum ePos
		{
			LEFT,
			RIGHT,
			TREE
		}

		private Facility f;

		public FarmAnimal[] animals = new FarmAnimal[3];

		public FarmAnimal.eType[] types = new FarmAnimal.eType[3];

		public bool[] shoping = new bool[3];

		public const int GIVE_GRASS_MAX = 2;

		public const int GIVE_GRASS_HORSE_MAX = 3;

		public int[] give_grass = new int[2];

		public long honey_time;

		public FarmAnimal selector;

		public AnimalInfo(Facility self)
		{
			f = self;
			for (int i = 0; i < 3; i++)
			{
				animals[i] = null;
				types[i] = FarmAnimal.eType.NONE;
				shoping[i] = false;
			}
			selector = null;
		}

		public void Create(FarmAnimal.eType type, FarmAnimal.eState state, GameObject parent, int index, int order)
		{
			types[index] = type;
			f.manager.data.SetFacilityFarmAnimal(f.my_id, index, type);
			animals[index] = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/farmanimal"), parent.transform).AddComponent<FarmAnimal>();
			if (type == FarmAnimal.eType.BLUE_WHALE_1 || type == FarmAnimal.eType.SPERM_WHALE_1)
			{
				animals[index].Open(f, type, state, (index != 0) ? new Vector3(0.068f, -0.092f, -1f) : new Vector3(-0.079f, 0.055f, -1f), index, order, select: false);
			}
			else
			{
				animals[index].Open(f, type, state, (index != 2) ? f.ANIMAL_POS[(int)f.prefix][index] : HONEY_LPOS, index, order, select: false);
			}
			shoping[index] = false;
		}

		public void Delete(int index)
		{
			shoping[index] = false;
			types[index] = f.manager.data.SetFacilityFarmAnimal(f.my_id, index, FarmAnimal.eType.NONE);
			Destroy(index);
		}

		public void Selector(FarmAnimal.eType type, FarmAnimal.eState state, GameObject parent, int index, int order)
		{
			selector = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/farmanimal"), parent.transform).AddComponent<FarmAnimal>();
			if (type == FarmAnimal.eType.BLUE_WHALE_1 || type == FarmAnimal.eType.SPERM_WHALE_1)
			{
				selector.Open(f, type, state, (index != 0) ? new Vector3(0.068f, -0.092f, -1f) : new Vector3(-0.079f, 0.055f, -1f), index, order, select: true);
			}
			else
			{
				selector.Open(f, type, state, (index != 2) ? f.ANIMAL_POS[(int)f.prefix][index] : HONEY_LPOS, index, order, select: true);
			}
		}

		public void DelSelector()
		{
			UnityEngine.Object.Destroy(selector.gameObject);
		}

		public void Shoping(int index)
		{
			shoping[index] = true;
		}

		public void ShopingOff(int index)
		{
			shoping[index] = false;
		}

		public void Destroy(int index)
		{
			if (animals[index] != null)
			{
				UnityEngine.Object.Destroy(animals[index].gameObject);
				animals[index] = null;
				types[index] = FarmAnimal.eType.NONE;
			}
		}

		public void DestroyAll()
		{
			for (int i = 0; i < 3; i++)
			{
				Destroy(i);
			}
			f = null;
		}
	}

	[Serializable]
	public class TreeInfo
	{
		public enum ePlant
		{
			TOP = 0,
			CENTER = 1,
			BOTTOM = 2,
			MAX = 3,
			NOTING = -1
		}

		public enum eAnimal
		{
			BIRD,
			SMALL,
			MAX
		}

		private Facility f;

		public const int PLANT_MAX = 8;

		public const int BASE_ORDER = 15;

		public GameObject obj;

		public GameObject selector;

		public SpriteRenderer sprite;

		public eItem type;

		public ePlant plant;

		[HideInInspector]
		public Vector2[] plant_pos = new Vector2[3];

		private int[,] PLANT_MAP = new int[2, 2]
		{
			{
				3,
				7
			},
			{
				0,
				8
			}
		};

		public WildAnimal[] reserve = new WildAnimal[2];

		public int[] map_index = new int[2];

		[HideInInspector]
		public Vector2[] animal_pos = new Vector2[2];

		public Animation anim;

		public TreeInfo(Facility self)
		{
			f = self;
			plant = ePlant.NOTING;
			for (int i = 0; i < 3; i++)
			{
				plant_pos[i] = new Vector2((f.my_id % 2 != 0) ? (-0.39f) : 0.39f, 0.04f - (float)i * 0.12f);
			}
			animal_pos[0] = ((f.my_id % 2 != 0) ? BIRD_LPOS_R : BIRD_LPOS_L);
			animal_pos[1] = ((f.my_id % 2 != 0) ? SMALL_LPOS_R : SMALL_LPOS_L);
		}

		public void Create(eItem t, ePlant p, Transform parent)
		{
			type = f.manager.data.SetFacilityTree(f.my_id, t);
			plant = f.manager.data.SetFacilityTreePlant(f.my_id, p);
			obj = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/facility_" + type.ToString().ToLower()), parent);
			obj.gameObject.transform.localPosition = plant_pos[(int)plant];
			sprite = obj.transform.Find("contents").Find("sprite").gameObject.GetComponent<SpriteRenderer>();
			anim = obj.GetComponent<Animation>();
			ControlSortingOrder(obj.transform, eSortMode.FIELD, f.GetBaseOrder());
			map_index[0] = f.my_id / 2 * 5 * f.manager.map.COLM_MAX[1] + PLANT_MAP[0, f.my_id % 2] + f.manager.map.COLM_MAX[1] * (int)plant;
			map_index[1] = f.my_id / 2 * 5 * f.manager.map.COLM_MAX[2] + PLANT_MAP[1, f.my_id % 2] + f.manager.map.COLM_MAX[2] * (int)plant;
		}

		public void Selector(eItem t, ePlant p, Transform parent)
		{
			selector = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/facility_" + t.ToString().ToLower()), parent);
			selector.gameObject.transform.localPosition = plant_pos[(int)p];
			ControlSortingOrder(selector.transform, eSortMode.SELECT, 10100);
		}

		public void Delete()
		{
			type = f.manager.data.SetFacilityTree(f.my_id, eItem.NONE);
			plant = f.manager.data.SetFacilityTreePlant(f.my_id, ePlant.NOTING);
			Destroy();
		}

		public void Destroy()
		{
			if (selector != null)
			{
				UnityEngine.Object.Destroy(selector);
				selector = null;
			}
			if (obj != null)
			{
				for (int i = 0; i < 2; i++)
				{
					reserve[i] = null;
					map_index[i] = -1;
				}
				UnityEngine.Object.Destroy(obj);
				obj = null;
				sprite = null;
				type = eItem.NONE;
				plant = ePlant.NOTING;
			}
		}
	}

	[Serializable]
	public class IconIfo
	{
		public Icon icon;

		public TouchEvent touch;

		public Data.Condition.eCATEGORY category;

		public IconIfo(List<Sprite> list, Transform parent, Vector3 pos, Icon.ePos set, Data.Condition.eCATEGORY _category)
		{
			icon = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/icon"), parent).GetComponent<Icon>();
			icon.transform.localPosition = pos;
			icon.Open(list, set);
			touch = icon.GetComponent<TouchEvent>();
			category = _category;
		}

		public void Destroy()
		{
			touch = null;
			UnityEngine.Object.Destroy(icon.gameObject);
			icon = null;
		}

		public void SetEnabled(bool enable)
		{
			touch.SetEnabled(enable);
		}

		public void SetActive(bool active)
		{
			icon.gameObject.SetActive(active);
		}
	}

	[Serializable]
	public class ProgressInfo
	{
		public Progress[] bars = new Progress[3];

		public Progress Create(int index, Transform parent, Vector2 pos, int order)
		{
			Destroy(index);
			bars[index] = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/progress_1"), parent).GetComponent<Progress>();
			bars[index].gameObject.name = "progress_1";
			bars[index].transform.localPosition = pos;
			bars[index].SetOrderInLayer(order);
			return bars[index];
		}

		public Progress Get(int index)
		{
			return bars[index];
		}

		public void AtOnce(int index)
		{
			bars[index] = null;
		}

		public void ForcedDestroy(GameObject parent, int index)
		{
			if (bars[index] != null)
			{
				UnityEngine.Object.Destroy(bars[index].gameObject);
				return;
			}
			Transform transform = parent.transform.Find("progress_1");
			if (transform != null)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}

		public void Destroy(int index)
		{
			if (bars[index] != null)
			{
				UnityEngine.Object.Destroy(bars[index].gameObject);
			}
			bars[index] = null;
		}

		public void DestroyAll()
		{
			for (int i = 0; i < 3; i++)
			{
				Destroy(i);
			}
		}
	}

	public enum eSortMode
	{
		SELECT,
		FIELD,
		MENU
	}

	public const int ANIMAL_MAX = 3;

	public const int ANIMAL_GAUGE_MAX = 2;

	public const int COLM = 2;

	public const int ADD_ORDER = 150;

	public readonly int[] ADD_ANIMAL_ORDER = new int[3]
	{
		70,
		75,
		60
	};

	public const int TIMER_MAX = 3;

	public const int CONSTRUCT_TIMER = 0;

	public const int FISH_TIMER = 0;

	public const int GRASS_MAX = 6;

	public const int CONSTRUCT_TIME = 600;

	public const int RIVER_HARVEST_TIME = 20;

	public const int HONEY_HARVEST_TIME = 30;

	public const int BUS_BASE_ORDER = 175;

	public const int HARVEST_BASE_ORDER = 120;

	public const int NORMAL_FACILITY_MAX = 11;

	public const int RESORT_FACILITY_MAX = 11;

	private const int SELECT_ORDER = 10100;

	public static readonly List<List<eType>> base_type_list = new List<List<eType>>
	{
		new List<eType>
		{
			eType.GRAZE_1,
			eType.POULTRY_1,
			eType.STABLE_1,
			eType.RIVER_1
		},
		new List<eType>
		{
			eType.NET_1,
			eType.LANDSHOW_1,
			eType.UNDERWATERSHOW_1,
			eType.ROCKYSPOT_1
		}
	};

	public static readonly List<List<eType>> type_list = new List<List<eType>>(8)
	{
		new List<eType>
		{
			eType.GRAZE_1,
			eType.GRAZE_2,
			eType.GRAZE_3
		},
		new List<eType>
		{
			eType.POULTRY_1,
			eType.POULTRY_2,
			eType.POULTRY_3
		},
		new List<eType>
		{
			eType.STABLE_1,
			eType.STABLE_2,
			eType.STABLE_3
		},
		new List<eType>
		{
			eType.RIVER_1,
			eType.RIVER_2
		},
		new List<eType>
		{
			eType.NET_1,
			eType.NET_2,
			eType.NET_3
		},
		new List<eType>
		{
			eType.LANDSHOW_1,
			eType.LANDSHOW_2,
			eType.LANDSHOW_3
		},
		new List<eType>
		{
			eType.UNDERWATERSHOW_1,
			eType.UNDERWATERSHOW_2,
			eType.UNDERWATERSHOW_3
		},
		new List<eType>
		{
			eType.ROCKYSPOT_1,
			eType.ROCKYSPOT_2
		}
	};

	private readonly List<List<FarmAnimal.ePrefix>> facility_animal_prefix = new List<List<FarmAnimal.ePrefix>>(22)
	{
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.SHEEP,
			FarmAnimal.ePrefix.COW,
			FarmAnimal.ePrefix.PIG,
			FarmAnimal.ePrefix.ALPACA,
			FarmAnimal.ePrefix.GOAT
		},
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.CHICKEN,
			FarmAnimal.ePrefix.TURKEY
		},
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.HORSE
		},
		new List<FarmAnimal.ePrefix>(),
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.WATER_BUFFALO,
			FarmAnimal.ePrefix.BLUE_WHALE,
			FarmAnimal.ePrefix.SPERM_WHALE,
			FarmAnimal.ePrefix.SEA_OTTER,
			FarmAnimal.ePrefix.SHARK,
			FarmAnimal.ePrefix.CROCODILE,
			FarmAnimal.ePrefix.TURTLE,
			FarmAnimal.ePrefix.GIANT_SQUID
		},
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.SEA_LION,
			FarmAnimal.ePrefix.WALRUS
		},
		new List<FarmAnimal.ePrefix>
		{
			FarmAnimal.ePrefix.DOLPHIN,
			FarmAnimal.ePrefix.KILLER_WHALE
		},
		new List<FarmAnimal.ePrefix>()
	};

	private readonly string[] facility_animal_prefix_to_string = new string[22]
	{
		"SHEEP_",
		"COW_",
		"CHICKEN_",
		"HORSE_",
		"PIG_",
		"ALPACA_",
		"GOAT_",
		"TURKEY_",
		"HONEY_",
		"WATER_BUFFALO_",
		"BLUE_WHALE_",
		"SPERM_WHALE_",
		"SEA_OTTER_",
		"SHARK_",
		"CROCODILE_",
		"TURTLE_",
		"GIANT_SQUID_",
		"DOLPHIN_",
		"SEA_LION_",
		"WALRUS_",
		"KILLER_WHALE_",
		"FRUIT_"
	};

	private readonly List<List<eItem>> tree_prefix = new List<List<eItem>>
	{
		new List<eItem>
		{
			eItem.TREE_1,
			eItem.TREE_2,
			eItem.TREE_3
		},
		new List<eItem>
		{
			eItem.TREE_4,
			eItem.TREE_5,
			eItem.TREE_6
		}
	};

	private readonly List<List<FarmAnimal.eType>> tree_animal_prefix = new List<List<FarmAnimal.eType>>
	{
		new List<FarmAnimal.eType>
		{
			FarmAnimal.eType.HONEY_1,
			FarmAnimal.eType.HONEY_2
		},
		new List<FarmAnimal.eType>
		{
			FarmAnimal.eType.FRUIT_1,
			FarmAnimal.eType.FRUIT_2,
			FarmAnimal.eType.FRUIT_3
		}
	};

	private const int WILD_ANIMAL_MAX = 2;

	private readonly List<List<Vector2>> wild_animal_pos = new List<List<Vector2>>
	{
		new List<Vector2>
		{
			new Vector2(0f, 0f)
		},
		new List<Vector2>
		{
			new Vector2(-0.23f, -0.09f),
			new Vector2(-0.04f, 0.06f)
		},
		new List<Vector2>
		{
			new Vector2(0f, 0f)
		},
		new List<Vector2>
		{
			new Vector2(0.128f, -0.124f)
		},
		new List<Vector2>
		{
			new Vector2(0f, 0f)
		},
		new List<Vector2>
		{
			new Vector2(0f, 0f)
		},
		new List<Vector2>
		{
			new Vector2(0f, 0f)
		},
		new List<Vector2>
		{
			new Vector2(0.08f, -0.124f)
		}
	};

	public WildAnimal[] wild_animal_reserve = new WildAnimal[2];

	public Manager manager;

	public MainCharacter main;

	private Casher casher;

	public eType type;

	public ePrefix prefix;

	public int my_id;

	public eState state;

	public ulong[] timer = new ulong[3];

	private TouchEvent touch_event;

	private Animator river_fish_animator;

	private GameObject present_video;

	private Timer.TimeData present_video_timer;

	public Worker worker;

	public Effect flower;

	private readonly List<List<Vector3>> ANIMAL_POS = new List<List<Vector3>>(8)
	{
		new List<Vector3>
		{
			new Vector3(-0.135f, -0.088f, -1f),
			new Vector3(0.135f, -0.12f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(-0.205f, -0.015f, -1f),
			new Vector3(-0.05f, -0.11f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(-0.145f, -0.21f, -1f),
			new Vector3(0.135f, -0.23f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(0.15f, -0.1f, -1f),
			new Vector3(0.15f, -0.1f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(-0.135f, -0.08f, -1f),
			new Vector3(0.135f, -0.095f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(-0.135f, -0.028f, -1f),
			new Vector3(0.135f, -0.06f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(-0.135f, -0.044f, -1f),
			new Vector3(0.135f, -0.076f, -1f),
			HONEY_LPOS
		},
		new List<Vector3>
		{
			new Vector3(0.15f, -0.1f, -1f),
			new Vector3(0.15f, -0.1f, -1f),
			HONEY_LPOS
		}
	};

	public BusInfo bus_info;

	public Structures structures;

	public Selector selector;

	private SpriteRenderer keep_out;

	public AnimalInfo animal_info;

	private const float TREE_BASE_LOCAL_X = 0.39f;

	private const float TREE_BASE_LOCAL_Y = 0.04f;

	private const float TREE_LOCAL_MARGIN = 0.12f;

	private static readonly Vector2 BIRD_LPOS_L = new Vector2(0.091f, 0.233f);

	private static readonly Vector2 BIRD_LPOS_R = new Vector2(-0.091f, 0.233f);

	private static readonly Vector3 HONEY_LPOS = new Vector3(0f, 0.225f, -1f);

	private static readonly Vector2 SMALL_LPOS_L = new Vector2(0.04f, -0.05f);

	private static readonly Vector2 SMALL_LPOS_R = new Vector2(-0.04f, -0.05f);

	public TreeInfo tree_info;

	private const float ICON_DEPTH = -12f;

	private const string SALE_ICON = "icon";

	private static readonly Vector3 ICON_FACILITY_LPOS = new Vector3(0f, -0.17f, -12f);

	private static readonly List<List<Vector3>> ICON_FARMANIMAL_LPOS = new List<List<Vector3>>
	{
		new List<Vector3>(2)
		{
			new Vector3(-0.14f, 0.04f, -12f),
			new Vector3(0.14f, 0.04f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(-0.23f, 0.152f, -12f),
			new Vector3(-0.028f, 0.055f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(-0.14f, 0.04f, -12f),
			new Vector3(0.14f, 0.04f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(0f, 0f, 0f)
		},
		new List<Vector3>(2)
		{
			new Vector3(-0.14f, 0.04f, -12f),
			new Vector3(0.14f, 0.04f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(-0.14f, 0.04f, -12f),
			new Vector3(0.14f, 0.04f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(-0.14f, 0.04f, -12f),
			new Vector3(0.14f, 0.04f, -12f)
		},
		new List<Vector3>(2)
		{
			new Vector3(0f, 0f, 0f)
		}
	};

	private static readonly Vector3[] ICON_BIRD_LPOS = new Vector3[2]
	{
		new Vector3(-0.2f, 0.1f, -12f),
		new Vector3(-0.05f, -0.02f, -12f)
	};

	private const float ICON_TREE_LPOS_X = 0.42f;

	private const float ICON_TREE_LPOS_Y = 0.17f;

	private static readonly Vector3 ICON_HONEY_LPOS = new Vector3(0f, 0.22f, -12f);

	private static readonly Vector3 ICON_DELTREE_LPOS = new Vector3(0f, 0.026f, -12f);

	[SerializeField]
	private List<IconIfo> icon_list = new List<IconIfo>();

	private const string PROGRESS_NAME = "progress_1";

	private const int PROGRESS_ORDER = 800;

	private const float PROGRESS_NOTIFY_TIME = 3f;

	private const float PROGRESS_DULATION = 0.3f;

	private const float PROGRESS_TOUCH_TIME = 2f;

	private static readonly Vector2 CONST_BAR_POS = new Vector2(0f, -0.045f);

	private static readonly Vector2 ANIMAL_BAR_POS = new Vector2(0f, 0.33f);

	private static readonly Vector2 HONEY_BAR_POS = new Vector2(0f, 0.106f);

	private static readonly Vector2 RIVER_BAR_POS = new Vector2(0f, 0.185f);

	private static readonly Vector2[] BAR_POS = new Vector2[3]
	{
		ANIMAL_BAR_POS,
		ANIMAL_BAR_POS,
		HONEY_BAR_POS
	};

	[SerializeField]
	private ProgressInfo progress;

	private const int CONSTRUCT_AREA = 0;

	private const int WORK_MAX = 4;

	private Vector2[] const_pos = new Vector2[4]
	{
		new Vector2(-0.18f, 0f),
		new Vector2(0.18f, 0f),
		new Vector2(-0.18f, -0.2f),
		new Vector2(0.18f, -0.2f)
	};

	private bool[] flipX = new bool[4]
	{
		true,
		false,
		true,
		false
	};

	public List<Data.ConstructPosition> const_positions = new List<Data.ConstructPosition>(4);

	public List<Data.ConstructPosition> const_worker_positions = new List<Data.ConstructPosition>(4);

	private TouchEvent lock_icon;

	private int select_index;

	private const float BUS_POS_MARGIN_Y = 0.4f;

	private Progress construct_prog;

	private GameObject[] give_grass = new GameObject[2];

	public static readonly Vector2[] tFACILITY_POS = new Vector2[22]
	{
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		new Vector2(0f, -0.02f),
		new Vector2(0f, 0.03f),
		Vector2.zero,
		new Vector2(0f, 0.02f),
		new Vector2(0f, -0.02f),
		new Vector2(0f, -0.055f),
		new Vector2(0f, 0.03f),
		new Vector2(0f, -0.015f),
		new Vector2(0f, 0.02f),
		new Vector2(0f, 0.03f),
		new Vector2(0f, -0.025f)
	};

	[CompilerGenerated]
	private static Comparison<FarmAnimal.ePrefix> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Comparison<FarmAnimal.ePrefix> _003C_003Ef__mg_0024cache1;

	private void SetKeepOut(bool create)
	{
		if (create)
		{
			if (manager.data.facility_data[my_id].enabled == 0)
			{
				GameObject original = Resources.Load("Prefab/facility_keep_out") as GameObject;
				keep_out = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<SpriteRenderer>();
				keep_out.sortingOrder = GetBaseOrder() + 75;
				keep_out.transform.Find("shadow").GetComponent<SpriteRenderer>().sortingOrder = keep_out.sortingOrder;
			}
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.SMOKE);
			CreateEffectTrash(base.transform.position);
			UnityEngine.Object.Destroy(keep_out.gameObject);
			keep_out = null;
		}
	}

	public void Init(int id, Manager m, MainCharacter mc)
	{
		manager = m;
		main = mc;
		my_id = id;
		bus_info = new BusInfo();
		this.progress = new ProgressInfo();
		state = manager.data.facility_data[my_id].state;
		type = manager.data.facility_data[my_id].type;
		prefix = TypeToPrefix(type);
		for (int i = 0; i < 3; i++)
		{
			timer[i] = (ulong)manager.data.facility_data[my_id].timer[i];
		}
		tree_info = new TreeInfo(this);
		eItem tree = manager.data.facility_data[my_id].tree;
		if (tree != eItem.NONE)
		{
			tree_info.Create(tree, manager.data.facility_data[my_id].plant, base.transform);
		}
		if (state != eState.NONE)
		{
			structures = new Structures(this);
		}
		if (state == eState.FIX)
		{
			Progress progress = this.progress.Create(0, base.transform, CONST_BAR_POS, 800);
			progress.Once(1f, 300f);
			this.progress.AtOnce(0);
		}
		animal_info = new AnimalInfo(this);
		for (int j = 0; j < 2; j++)
		{
			animal_info.give_grass[j] = manager.data.facility_data[my_id].give_grass[j];
		}
		for (int k = 0; k < 3; k++)
		{
			if (manager.data.facility_data[my_id].farm_animals[k] != FarmAnimal.eType.NONE)
			{
				FarmAnimal.eState eState;
				GameObject parent;
				if (k < 2)
				{
					eState = ((FarmAnimal.TypeToPrefix(manager.data.facility_data[my_id].farm_animals[k]) != FarmAnimal.ePrefix.HORSE && FarmAnimal.TypeToPrefix(manager.data.facility_data[my_id].farm_animals[k]) != FarmAnimal.ePrefix.DOLPHIN && FarmAnimal.TypeToPrefix(manager.data.facility_data[my_id].farm_animals[k]) != FarmAnimal.ePrefix.SEA_LION && FarmAnimal.TypeToPrefix(manager.data.facility_data[my_id].farm_animals[k]) != FarmAnimal.ePrefix.WALRUS && FarmAnimal.TypeToPrefix(manager.data.facility_data[my_id].farm_animals[k]) != FarmAnimal.ePrefix.KILLER_WHALE) ? ((animal_info.give_grass[k] == 2) ? FarmAnimal.eState._STAY_1 : FarmAnimal.eState._STAY_2) : ((animal_info.give_grass[k] == 3) ? FarmAnimal.eState._STAY_1 : FarmAnimal.eState._STAY_2));
					parent = base.gameObject;
				}
				else
				{
					animal_info.honey_time = (long)timer[2];
					eState = ((animal_info.honey_time == 0) ? FarmAnimal.eState._STAY_1 : FarmAnimal.eState._STAY_2);
					parent = tree_info.obj;
				}
				animal_info.Create(manager.data.facility_data[my_id].farm_animals[k], eState, parent, k, ADD_ANIMAL_ORDER[k] + GetBaseOrder());
			}
		}
		animal_info.honey_time = (long)timer[2];
		if (animal_info.animals[2] != null)
		{
			if (animal_info.honey_time != 0)
			{
				Progress progress2 = this.progress.Create(2, animal_info.animals[2].transform, BAR_POS[2], 800);
				progress2.Loop((ulong)animal_info.honey_time, 30, HoneyHarvestTime);
			}
			else
			{
				HoneyHarvestTime();
			}
		}
		if (state == eState.ACTIVE && (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT))
		{
			if (timer[0] != 0)
			{
				Progress progress3 = this.progress.Create(0, base.transform, RIVER_BAR_POS, 800);
				progress3.Loop(timer[0], 20, FishingTime);
			}
			else
			{
				FishingTime();
			}
		}
		touch_event = GetComponent<TouchEvent>();
		touch_event.ClickDown.AddListener(delegate
		{
			TouchRiver(touch_event);
		});
		touch_event.ClickMove.AddListener(delegate
		{
			TouchRiver(touch_event);
		});
		touch_event.ClickUp.AddListener(delegate
		{
			Touch(touch_event);
		});
		touch_event.SetEnabled((state == eState.FIX || (state == eState.ACTIVE && (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT))) ? true : false);
		for (int l = 0; l < 4; l++)
		{
			const_positions.Add(new Data.ConstructPosition(flipX[l], base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(const_pos[l])), (Data.farm_type != Data.eFarmType.RESORT) ? true : false, GetBaseOrder() + 70));
			const_worker_positions.Add(new Data.ConstructPosition(flipX[l], const_pos[l], (Data.farm_type != Data.eFarmType.RESORT) ? true : false, GetBaseOrder() + 70));
		}
		if (state == eState.CONSTRUCT)
		{
			StartConstruct(timer[0]);
		}
		SetConstructVideo();
		int baseOrder = GetBaseOrder();
		if (Data.farm_type != Data.eFarmType.RESORT || my_id != 10)
		{
			flower = Effect.Flower(base.transform, manager.data.level, baseOrder + 100, baseOrder + 48);
		}
		SetKeepOut(create: true);
	}

	public int GetBaseOrder()
	{
		return 150 * (((my_id == 10) ? 9 : my_id) / 2);
	}

	public static void ControlSortingOrder(Transform trans, eSortMode mode, int margin_order)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				SpriteRenderer component = transform.gameObject.GetComponent<SpriteRenderer>();
				if (null != component)
				{
					if (mode == eSortMode.MENU)
					{
						component.sortingOrder += margin_order;
					}
					else
					{
						component.sortingOrder = ((0 <= transform.gameObject.name.IndexOf("shadow")) ? component.sortingOrder : (component.sortingOrder + margin_order));
						if (mode == eSortMode.SELECT)
						{
							SpriteRenderer spriteRenderer = component;
							Color color = component.color;
							float r = color.r;
							Color color2 = component.color;
							float g = color2.g;
							Color color3 = component.color;
							spriteRenderer.color = new Color(r, g, color3.b, (0 <= transform.gameObject.name.IndexOf("shadow")) ? 0f : 0.7f);
						}
					}
				}
				if (transform.childCount != 0)
				{
					ControlSortingOrder(transform, mode, margin_order);
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

	public static ePrefix TypeToPrefix(eType type)
	{
		ePrefix result = ePrefix.GRAZE;
		if (type != eType.NONE)
		{
			result = (ePrefix)Enum.Parse(typeof(ePrefix), type.ToString().Substring(0, type.ToString().IndexOf("_")));
		}
		return result;
	}

	private static string MakeFacilityName(eState state, eType type)
	{
		switch (state)
		{
		case eState.GRASS:
		case eState.EMPTY:
		case eState.CONSTRUCT:
			return "_" + eState.GRASS.ToString().ToLower();
		case eState.ACTIVE:
			return "_" + type.ToString().ToLower();
		default:
			return "_" + state.ToString().ToLower();
		}
	}

	public void Shoping(Casher c)
	{
		casher = c;
		PausePlant();
		SetIcons();
	}

	public void FinishShoping()
	{
		foreach (IconIfo item in icon_list)
		{
			item.Destroy();
		}
		if (lock_icon != null)
		{
			UnityEngine.Object.Destroy(lock_icon.gameObject);
			lock_icon = null;
		}
		icon_list.Clear();
		RestartPlant();
	}

	public void PauseShoping()
	{
		foreach (IconIfo item in icon_list)
		{
			item.Destroy();
		}
		if (lock_icon != null)
		{
			UnityEngine.Object.Destroy(lock_icon.gameObject);
		}
		icon_list.Clear();
	}

	public void RestartShoping()
	{
		SetIcons();
		if ((state == eState.GRASS || state == eState.EMPTY) && manager.data.facility_data[my_id].enabled == 0)
		{
			CreateLockIcon();
		}
	}

	private void SetIcons()
	{
		if (state == eState.EMPTY)
		{
			if (manager.data.facility_data[my_id].enabled == 0)
			{
				CreateLockIcon();
			}
			else
			{
				CreateIcon(GetFacilityIconList(), base.transform, ICON_FACILITY_LPOS, Icon.ePos.UP, AddFacility, -1, Data.Condition.eCATEGORY.FACILITY);
			}
		}
		else if (state == eState.GRASS)
		{
			if (manager.data.facility_data[my_id].enabled == 0)
			{
				CreateLockIcon();
			}
			else
			{
				CreateGrassCutIcon();
			}
		}
		else if (state == eState.ACTIVE)
		{
			if (animal_info.animals[0] == null && animal_info.animals[1] == null && animal_info.types[0] == FarmAnimal.eType.NONE && animal_info.types[1] == FarmAnimal.eType.NONE)
			{
				CreateIcon(GetSaleIconList(1), base.transform, ICON_FACILITY_LPOS, Icon.ePos.UP, DelFacility, -1, Data.Condition.eCATEGORY.CASHER);
			}
			for (int i = 0; i < 2; i++)
			{
				if (prefix == ePrefix.RIVER)
				{
					break;
				}
				if (prefix == ePrefix.ROCKYSPOT)
				{
					break;
				}
				if (!animal_info.shoping[i])
				{
					if (animal_info.animals[i] == null)
					{
						int num = (i == 0) ? 1 : 0;
						FarmAnimal.ePrefix animal_prefix = (animal_info.types[num] != FarmAnimal.eType.NONE) ? FarmAnimal.TypeToPrefix(animal_info.types[num]) : FarmAnimal.ePrefix.NONE;
						CreateIcon(GetAnimalIconList(prefix, animal_prefix), base.transform, ICON_FARMANIMAL_LPOS[(int)prefix][i], Icon.ePos.CENTER, AddAnimal, i, Data.Condition.eCATEGORY.FARMANIMAL);
					}
					else
					{
						CreateIcon(GetSaleIconList(0), base.transform, ICON_FARMANIMAL_LPOS[(int)prefix][i], Icon.ePos.UP, DelAnimal, i, Data.Condition.eCATEGORY.CASHER);
					}
				}
			}
		}
		if (manager.data.tree_plant != 1 || manager.data.facility_data[my_id].enabled != 1)
		{
			return;
		}
		if (tree_info.obj == null)
		{
			for (int j = 0; j < 3; j++)
			{
				if (my_id >= 8)
				{
					break;
				}
				CreateIcon(GetItemIconList(), base.transform, new Vector3((my_id % 2 != 0) ? (-0.42f) : 0.42f, 0.17f - 0.17f * (float)j, -12f), Icon.ePos.UP, AddItemFacility, j, Data.Condition.eCATEGORY.FACILITYITEM);
			}
		}
		else if (!animal_info.shoping[2])
		{
			if (animal_info.animals[2] == null)
			{
				CreateIcon(GetTreeAnimalIconList(), tree_info.obj.transform, ICON_HONEY_LPOS, Icon.ePos.CENTER, AddAnimal, 2, Data.Condition.eCATEGORY.FARMANIMAL);
			}
			else
			{
				CreateIcon(GetSaleIconList(0), tree_info.obj.transform, ICON_HONEY_LPOS, Icon.ePos.UP, DelAnimal, 2, Data.Condition.eCATEGORY.CASHER);
			}
			if (animal_info.animals[2] == null)
			{
				CreateIcon(GetSaleIconList(1), tree_info.obj.transform, ICON_DELTREE_LPOS, Icon.ePos.UP, DelItemFacility, -1, Data.Condition.eCATEGORY.CASHER);
			}
		}
	}

	private void CreateIcon(List<Sprite> list, Transform parent, Vector3 pos, Icon.ePos set, UnityAction<TouchEvent> func, int param, Data.Condition.eCATEGORY category)
	{
		IconIfo info = new IconIfo(list, parent, pos, set, category);
		icon_list.Add(info);
		info.touch.ClickDown.AddListener(ClickIcon);
		info.touch.ClickUp.AddListener(delegate
		{
			func(info.touch);
		});
		info.touch.param.value1 = param;
	}

	public IconIfo GetIconInfo(Data.Condition.eCATEGORY category)
	{
		for (int i = 0; i < icon_list.Count; i++)
		{
			if (icon_list[i].category == category)
			{
				return icon_list[i];
			}
		}
		return null;
	}

	private void ClickIcon()
	{
		Manager.sound.PlaySe(Sound.eSe.CLICK);
	}

	private List<Sprite> GetFacilityIconList()
	{
		List<Sprite> list = new List<Sprite>();
		for (int i = 0; i < Convert.FacilityLength(Data.farm_type); i++)
		{
			string[] array = ((eType)Convert.FacilityTypePlus(Data.farm_type, i)).ToString().Split('_');
			if (array.Length > 1 && int.Parse(array[1]) == 1)
			{
				list.Add(SpriteManager.GetFacilityIcon((eType)Convert.FacilityTypePlus(Data.farm_type, i)));
			}
		}
		return list;
	}

	private List<Sprite> GetItemIconList()
	{
		List<Sprite> list = new List<Sprite>();
		for (int i = 0; i < 6; i++)
		{
			eItem eItem = (eItem)i;
			string[] array = eItem.ToString().Split('_');
			if (array.Length > 1 && int.Parse(array[1]) == 1)
			{
				list.Add(SpriteManager.GetItemIcon((eItem)i));
			}
		}
		return list;
	}

	public List<Sprite> GetAnimalIconList(ePrefix type, FarmAnimal.ePrefix animal_prefix)
	{
		List<Sprite> list = new List<Sprite>();
		if (animal_prefix == FarmAnimal.ePrefix.NONE)
		{
			for (int i = 0; i < facility_animal_prefix[(int)type].Count; i++)
			{
				FarmAnimal.eType eType = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), facility_animal_prefix[(int)type][i].ToString() + "_1");
				list.Add(SpriteManager.GetAnimalIcon(eType));
			}
		}
		else
		{
			FarmAnimal.eType eType = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), animal_prefix.ToString() + "_1");
			list.Add(SpriteManager.GetAnimalIcon(eType));
		}
		return list;
	}

	private List<Sprite> GetTreeAnimalIconList()
	{
		List<Sprite> list = new List<Sprite>();
		for (int i = 0; i < tree_animal_prefix[(int)Data.farm_type].Count; i++)
		{
			list.Add(SpriteManager.GetAnimalIcon(tree_animal_prefix[(int)Data.farm_type][i]));
		}
		return list;
	}

	private List<Sprite> GetSaleIconList(int trash_onoff)
	{
		List<Sprite> list = new List<Sprite>();
		list.Add(SpriteManager.GetSaleIcon(trash_onoff));
		return list;
	}

	private List<FarmAnimal.eType> CreateAnimalList(ePrefix prefix)
	{
		List<FarmAnimal.ePrefix> list = new List<FarmAnimal.ePrefix>(facility_animal_prefix[(int)prefix]);
		List<FarmAnimal.eType> list2 = new List<FarmAnimal.eType>();
		List<FarmAnimal.eType> list3 = new List<FarmAnimal.eType>();
		list.Sort(Utils.CompareFarmPrefix);
		foreach (FarmAnimal.ePrefix item2 in list)
		{
			for (int i = 1; i < 10; i++)
			{
				try
				{
					FarmAnimal.eType item = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), facility_animal_prefix_to_string[(int)item2] + i);
					if (manager.data.level < Price.OpenFarmAnimalLevel(item))
					{
						list3.Add(item);
					}
					else
					{
						list2.Add(item);
					}
				}
				catch (ArgumentException)
				{
					goto IL_00d3;
				}
			}
			IL_00d3:;
		}
		list2.AddRange(list3);
		return list2;
	}

	private List<FarmAnimal.eType> CreateSelectAnimalList(ePrefix prefix, FarmAnimal.ePrefix animal_prefix)
	{
		List<FarmAnimal.ePrefix> list = new List<FarmAnimal.ePrefix>(facility_animal_prefix[(int)prefix]);
		List<FarmAnimal.eType> list2 = new List<FarmAnimal.eType>();
		List<FarmAnimal.eType> list3 = new List<FarmAnimal.eType>();
		list.Sort(Utils.CompareFarmPrefix);
		for (int i = 1; i < 10; i++)
		{
			try
			{
				FarmAnimal.eType item = (FarmAnimal.eType)Enum.Parse(typeof(FarmAnimal.eType), facility_animal_prefix_to_string[(int)animal_prefix] + i);
				if (manager.data.level < Price.OpenFarmAnimalLevel(item))
				{
					list3.Add(item);
				}
				else
				{
					list2.Add(item);
				}
			}
			catch (ArgumentException)
			{
				break;
			}
		}
		list2.AddRange(list3);
		return list2;
	}

	public void CreateLockIcon()
	{
		if (lock_icon != null)
		{
			UnityEngine.Object.Destroy(lock_icon.gameObject);
		}
		lock_icon = Common.CreateLockIcon(base.transform, manager.data.level, Price.OpenFacilityUnlockLevel(my_id), 0, delegate
		{
			casher.OpenFacility(this);
		}, 10020);
		Transform transform = lock_icon.transform;
		Vector3 localPosition = lock_icon.transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = lock_icon.transform.localPosition;
		float y = localPosition2.y - 0.1f;
		Vector3 localPosition3 = lock_icon.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition3.z);
	}

	public void CreateGrassCutIcon()
	{
		if (lock_icon != null)
		{
			UnityEngine.Object.Destroy(lock_icon.gameObject);
		}
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/grass_cut_icon") as GameObject;
		lock_icon = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<TouchEvent>();
		Utils.SetOrderInLayer(lock_icon.gameObject, GetBaseOrder() + 10010);
		lock_icon.ClickDown.AddListener(delegate
		{
			lock_icon.GetComponent<Animation>().Play();
		});
		lock_icon.ClickUp.AddListener(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
		});
		lock_icon.transform.Find("contents/bg/boy").GetComponent<SpriteRenderer>().sprite = SpriteManager.GetCasherNG(manager.data.main_type + 2 * (int)Data.farm_type);
	}

	public void FixOpenFacility()
	{
		int num = Price.OpenFacilityUnlockPrice(my_id);
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		CreateEffectTrash(base.transform.position);
		UnityEngine.Object.Destroy(keep_out.gameObject);
		manager.data.EnableFacility(my_id);
		manager.data.SetCoinCount(manager.data.coin - num);
		if (lock_icon != null)
		{
			UnityEngine.Object.Destroy(lock_icon.gameObject);
			lock_icon = null;
		}
	}

	public void AddAnimal(TouchEvent touch)
	{
		select_index = touch.param.value1;
		casher.AddAnimal(this, GetAnimalList());
	}

	public void SelectAnimal(FarmAnimal.eType animal_type)
	{
		animal_info.Selector(animal_type, (select_index == 2) ? FarmAnimal.eState._STAY_2 : FarmAnimal.eState._ALBUM_1, (select_index == 2) ? tree_info.obj : base.gameObject, select_index, 10100);
	}

	public void FixAnimal(FarmAnimal.eType animal_type)
	{
		CancelAnimal(animal_type);
		int index = select_index;
		animal_info.Shoping(index);
		animal_info.types[index] = animal_type;
		CreateBusMovePos(out Vector2 start_pos, out Vector2 end_pos);
		bus_info.Come(start_pos, end_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.3f : 2f, (my_id >= 10) ? 2f : 1f, GetBaseOrder() + 175, delegate
		{
			BuyAnimal(animal_type, index);
		}, null);
	}

	private void CreateBusMovePos(out Vector2 start_pos, out Vector2 end_pos)
	{
		Vector2 vector = base.transform.localPosition;
		float num = (Data.farm_type != 0) ? 0f : 0.2f;
		if (my_id < 10)
		{
			start_pos = new Vector2(vector.x + ((my_id % 2 != 0) ? 1f : (-1f)), vector.y - 0.4f);
			end_pos = new Vector2(vector.x + ((my_id % 2 != 0) ? num : (0f - num)), vector.y - 0.4f);
		}
		else
		{
			start_pos = new Vector2(vector.x - 0.1f, vector.y - 0.4f);
			end_pos = new Vector2(vector.x, vector.y - 0.4f);
		}
	}

	public void CancelAnimal(FarmAnimal.eType animal_type)
	{
		animal_info.DelSelector();
	}

	private void BuyAnimal(FarmAnimal.eType animal_type, int index)
	{
		CreateBusMovePos(out Vector2 start_pos, out Vector2 _);
		bus_info.Leave(start_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.3f : 2f);
		animal_info.Create(animal_type, FarmAnimal.eState._STAY_1, (index == 2) ? tree_info.obj : base.gameObject, index, ADD_ANIMAL_ORDER[index] + GetBaseOrder());
		Manager.sound.PlaySe(Sound.eSe.APPEAR);
		CreateEffectTrash(animal_info.animals[index].transform.position);
		Manager.sound.PlaySe(Sound.eSe.EXP);
		Effect.Exp(Price.ExpAddAnimal(animal_type), 3, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
		});
		if (index != 2)
		{
			int num = (animal_info.animals[index].prefix != FarmAnimal.ePrefix.HORSE && animal_info.animals[index].prefix != FarmAnimal.ePrefix.DOLPHIN && animal_info.animals[index].prefix != FarmAnimal.ePrefix.SEA_LION && animal_info.animals[index].prefix != FarmAnimal.ePrefix.WALRUS && animal_info.animals[index].prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? 2 : 3;
			animal_info.give_grass[index] = manager.data.SetFacilityGiveGrass(my_id, index, num);
		}
		else
		{
			animal_info.honey_time = manager.data.SetFacilityTime(my_id, 2, 0L);
		}
		Progress progress = (index >= 3) ? this.progress.Create(index, base.transform, RIVER_BAR_POS, 800) : this.progress.Create(index, animal_info.animals[index].transform, BAR_POS[index], 800);
		progress.Once(1f, 3f);
		this.progress.AtOnce(index);
		if (manager.data.GetReg(Data.CharacterData.eType.FARMANIMAL, (int)animal_type) == 0)
		{
			manager.data.SetReg(Data.CharacterData.eType.FARMANIMAL, (int)animal_type);
		}
		int num2 = Price.OpenFarmAnimalPrice(animal_type);
		manager.data.SetCoinCount(manager.data.coin - num2);
	}

	public List<FarmAnimal.eType> GetAnimalList()
	{
		if (select_index == 2)
		{
			return tree_animal_prefix[(int)Data.farm_type];
		}
		if (animal_info.types[0] == FarmAnimal.eType.NONE && animal_info.types[1] == FarmAnimal.eType.NONE)
		{
			return CreateAnimalList(prefix);
		}
		int num = (animal_info.types[0] == FarmAnimal.eType.NONE) ? 1 : 0;
		return CreateSelectAnimalList(prefix, FarmAnimal.TypeToPrefix(animal_info.types[num]));
	}

	public void AddItem(TouchEvent touch)
	{
	}

	public void SelectItem(eType item)
	{
	}

	public void FixItem(eType item)
	{
	}

	public void CancelItem(eType item)
	{
	}

	public void AddFacility(TouchEvent touch)
	{
		casher.AddFacility(this, base_type_list[(int)Data.farm_type]);
	}

	public void SelectFacility(eType facility)
	{
		if (selector != null)
		{
			selector.Destroy();
		}
		selector = new Selector(this, MakeFacilityName(eState.ACTIVE, facility));
	}

	public void FixFacility(eType facility)
	{
		CancelFacility(facility);
		type = manager.data.SetFacilityType(my_id, facility);
		prefix = TypeToPrefix(type);
		StartConstruct((ulong)DateTime.Now.Ticks);
	}

	public void CancelFacility(eType facility)
	{
		selector.Destroy();
		selector = null;
	}

	public void AddItemFacility(TouchEvent touch)
	{
		select_index = touch.param.value1;
		List<eItem> list = new List<eItem>();
		for (int i = 0; i < tree_prefix[(int)Data.farm_type].Count; i++)
		{
			list.Add(tree_prefix[(int)Data.farm_type][i]);
		}
		casher.AddFacility(this, list);
	}

	public void SelectFacility(eItem item)
	{
		tree_info.Selector(item, (TreeInfo.ePlant)select_index, base.transform);
	}

	public void FixFacility(eItem item)
	{
		tree_info.Destroy();
		tree_info.Create(item, (TreeInfo.ePlant)select_index, base.transform);
		tree_info.anim.Play();
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		CreateEffectTrash(tree_info.obj.transform.position);
	}

	public void CancelFacility(eItem item)
	{
		tree_info.Destroy();
	}

	public void DelFacility(TouchEvent touch)
	{
		casher.DelFacility(this, type);
	}

	public void FixDelFacility(eType facility_type)
	{
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		CreateEffectTrash(base.transform.position);
		if (prefix == ePrefix.RIVER || prefix == ePrefix.POULTRY || prefix == ePrefix.ROCKYSPOT)
		{
			for (int i = 0; i < 2; i++)
			{
				if (wild_animal_reserve[i] != null)
				{
					manager.wild.ForceAnimalDestroy(wild_animal_reserve[i].type, wild_animal_reserve[i].my_id);
				}
			}
		}
		if (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT)
		{
			progress.ForcedDestroy(base.gameObject, 0);
		}
		ChangeState(eState.GRASS);
	}

	public void CancelDellFacility(eType facility_type)
	{
	}

	public void DelItemFacility(TouchEvent touch)
	{
		casher.DelFacility(this, tree_info.type);
	}

	public void FixDelFacility(eItem item)
	{
		CreateEffectTrash(tree_info.obj.transform.position);
		for (int i = 0; i < 2; i++)
		{
			if (tree_info.reserve[i] != null)
			{
				manager.wild.ForceAnimalDestroy(tree_info.reserve[i].type, tree_info.reserve[i].my_id);
			}
		}
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		tree_info.Delete();
	}

	public void CancelDelFacility(eItem item)
	{
	}

	public void DelAnimal(TouchEvent touch)
	{
		select_index = touch.param.value1;
		casher.DelAnimal(this, animal_info.types[select_index]);
	}

	public void FixDelAnimal(FarmAnimal.eType type)
	{
		main.DelHarvestQueue(animal_info.animals[select_index]);
		if (worker != null)
		{
			worker.DelHarvestQueue(animal_info.animals[select_index]);
		}
		progress.ForcedDestroy(animal_info.animals[select_index].gameObject, select_index);
		int index = select_index;
		animal_info.Shoping(index);
		animal_info.animals[index].SetTouchEnable(enable: false);
		animal_info.types[index] = FarmAnimal.eType.NONE;
		CreateBusMovePos(out Vector2 start_pos, out Vector2 end_pos);
		bus_info.Come(start_pos, end_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.1f : 1f, (my_id >= 10) ? 0.5f : 0.5f, GetBaseOrder() + 175, delegate
		{
			BuyOutAnimal(index);
		}, null);
	}

	public void CancelDelAnimal()
	{
	}

	private void CreateEffectTrash(Vector2 pos)
	{
		Effect.Run(Resources.Load("Prefab/effect_trash") as GameObject, pos, base.transform);
	}

	public void BuyOutAnimal(int index)
	{
		CreateBusMovePos(out Vector2 start_pos, out Vector2 _);
		bus_info.Leave(start_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.1f : 1f);
		int num = Price.OpenAnimalReleasePrice(animal_info.animals[index].type);
		manager.data.coin = manager.data.coin + num;
		manager.data.SetCoinCount(manager.data.coin);
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		CreateEffectTrash(animal_info.animals[index].transform.position);
		Manager.sound.PlaySe(Sound.eSe.COIN);
		Effect.Coin(num, 3, animal_info.animals[index].transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
		});
		animal_info.Delete(index);
	}

	public void ChangeState(eState next)
	{
		state = manager.data.SetFacilitySate(my_id, next);
		if (structures != null)
		{
			structures.Destroy();
		}
		structures = new Structures(this);
		touch_event.SetEnabled((state == eState.FIX) ? true : false);
		if (state == eState.FIX || state == eState.ACTIVE)
		{
			structures.anim.Play();
		}
	}

	public void NotifyGrassCut()
	{
		int num = 0;
		for (int i = 0; i < 6 && structures.grass[i].state == Grass.eState.SHORT; i++)
		{
			num++;
		}
		state = ((num == 6) ? eState.EMPTY : eState.GRASS);
	}

	public void NotifyGrassPlant()
	{
		state = eState.GRASS;
	}

	public void PausePlant()
	{
		if (state == eState.GRASS || state == eState.EMPTY)
		{
			for (int i = 0; i < 6; i++)
			{
				structures.grass[i].Pause();
			}
		}
	}

	public void RestartPlant()
	{
		if (state == eState.GRASS || state == eState.EMPTY)
		{
			for (int i = 0; i < 6; i++)
			{
				structures.grass[i].Restart();
			}
		}
	}

	public void CheckGrass()
	{
		if (state == eState.GRASS && worker != null)
		{
			WorkerStartGrassCut();
		}
	}

	private void WorkerStartGrassCut()
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < 6; i++)
		{
			int grass_id = i;
			sequence.AppendCallback(delegate
			{
				structures.grass[grass_id].AutoCut();
			});
			sequence.AppendInterval(0.1f);
		}
		sequence.Play();
	}

	public void WorkerGetCoinExp(UnityAction call)
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < 6; i++)
		{
			GameObject obj = structures.grass[i].GetCoinExp();
			if (obj != null)
			{
				sequence.AppendCallback(delegate
				{
					worker.GetCoinExp(obj);
				});
				sequence.AppendInterval(0.1f);
			}
		}
		sequence.AppendCallback(delegate
		{
			call();
		});
		sequence.Play();
	}

	private void StartConstruct(ulong time)
	{
		construct_prog = progress.Create(0, base.transform, CONST_BAR_POS, 800);
		construct_prog.Loop((ulong)manager.data.SetFacilityTime(my_id, 0, (long)time), 600, FinishConstruct);
		construct_prog.SetTextVisibility(visibility: true, new Vector2(0f, -0.088f), Color.white, 801);
		construct_prog.Show();
		if (worker != null)
		{
			worker.RegistConstructQueue(const_worker_positions);
		}
		else
		{
			main.RegistConstructQueue(const_positions);
		}
		ChangeState(eState.CONSTRUCT);
		SetConstructVideo();
	}

	private void FinishConstruct()
	{
		construct_prog = null;
		RemovePresentBox();
		if (present_video_timer != null)
		{
			Timer.Remove(present_video_timer);
			present_video_timer = null;
		}
		manager.data.SetFacilityTime(my_id, 0, 0L);
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		CreateEffectPaper();
		ChangeState(eState.FIX);
		manager.data.Save();
		if (worker != null)
		{
			worker.NotifyConstruct(const_worker_positions);
		}
		else
		{
			main.NotifyConstruct(const_positions);
		}
	}

	public void Touch(TouchEvent touch)
	{
		if (state == eState.FIX)
		{
			CreateFacility();
		}
		else if (state == eState.ACTIVE && (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT))
		{
			Fishing(main);
		}
	}

	private void CreateEffectPaper()
	{
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		Vector2 pos = new Vector2(x, position2.y + 0.3f);
		Effect.Run(Resources.Load("Prefab/effect_paper") as GameObject, pos, base.transform);
	}

	private void SetConstructVideo()
	{
		if (state == eState.CONSTRUCT && Manager.events.state == Event.eState.NONE)
		{
			RemovePresentBox();
			if (present_video_timer != null)
			{
				Timer.Remove(present_video_timer);
			}
			long num = DateTime.Now.Ticks - 6000000000L;
			if (manager.LoadVideo())
			{
				present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 3, ShowConstructPresent);
			}
		}
	}

	private void ShowConstructPresent()
	{
		if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			int num = 600 - (int)((ulong)(DateTime.Now.Ticks - (long)timer[0]) / 10000000uL);
			if ((float)num >= 0f)
			{
				RemovePresentBox();
				Vector3 pos = new Vector3(0f, 0.1f, -5f);
				int base_order_in_layer = GetBaseOrder() + 100;
				present_video = Common.CreatePresentConstructBox(base.transform, pos, base_order_in_layer, delegate
				{
					manager.PlayVideo(PlayVideoCompleted, PlayVideoFailed);
				}, VideoCancel);
				present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, HideConstructPresent);
			}
		}
		else
		{
			present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 2, ShowConstructPresent);
			if (manager.load_video.state == Manager.VideoRwd.eState.NONE)
			{
				manager.LoadVideo();
			}
		}
	}

	public void SetPresentConstructBox()
	{
		present_video = Common.CreatePresentConstructBoxEarly(base.transform, new Vector3(0.237f, 0.4f, -5f), GetBaseOrder() + 100, delegate
		{
			progress.bars[0].ChangeEndTime(1);
		});
	}

	public void SetDefaultLayer()
	{
		int mask = LayerMask.GetMask("Default");
		if (present_video != null)
		{
			Utils.SetLayer(present_video, mask);
		}
		if (construct_prog != null)
		{
			Utils.SetLayer(construct_prog.gameObject, mask);
		}
		if (worker != null)
		{
			Utils.SetLayer(worker.gameObject, mask);
		}
	}

	private void HideConstructPresent()
	{
		RemovePresentBox();
		present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, ShowConstructPresent);
	}

	public void RemovePresentBox()
	{
		if (present_video != null)
		{
			UnityEngine.Object.Destroy(present_video);
		}
	}

	public void PlayVideoCompleted()
	{
		Utils.Log("Facility(id=" + my_id + ") PlayVideoCompleted");
		if (present_video_timer != null)
		{
			Timer.Remove(present_video_timer);
			present_video_timer = null;
		}
		progress.bars[0].ChangeEndTime(1);
		manager.LoadVideo();
	}

	public void PlayVideoFailed()
	{
		Utils.Log("Facility(id=" + my_id + ") PlayVideoFailed");
		manager.LoadVideo();
		if (present_video_timer != null)
		{
			Timer.Remove(present_video_timer);
			present_video_timer = null;
		}
		present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, ShowConstructPresent);
	}

	public void VideoCancel()
	{
		Utils.Log("Facility(id=" + my_id + ") VideoCancel");
		if (present_video_timer != null)
		{
			Timer.Remove(present_video_timer);
			present_video_timer = null;
		}
		present_video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, ShowConstructPresent);
	}

	private void CreateFacility()
	{
		progress.ForcedDestroy(base.gameObject, 0);
		Manager.sound.PlaySe(Sound.eSe.APPEAR);
		CreateEffectPaper();
		ChangeState(eState.ACTIVE);
		if (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT)
		{
			touch_event.SetEnabled(enabled: true);
			FishingTime();
		}
		Manager.sound.PlaySe(Sound.eSe.EXP);
		Effect.Exp(Price.ExpAddFacility(type), 3, base.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
		});
	}

	private void TouchRiver(TouchEvent touchEvent)
	{
		if (state == eState.ACTIVE && (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT))
		{
			Fishing(main);
		}
	}

	public bool HarvestAnimal(FarmAnimal animal, MainCharacter harvester)
	{
		bool result = false;
		int num = (animal.prefix != FarmAnimal.ePrefix.HORSE && animal.prefix != FarmAnimal.ePrefix.DOLPHIN && animal.prefix != FarmAnimal.ePrefix.SEA_LION && animal.prefix != FarmAnimal.ePrefix.WALRUS && animal.prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? 2 : 3;
		if (animal.my_id < 2 && animal_info.give_grass[animal.my_id] == num)
		{
			MainCharacter.HarvestInfo info;
			if (animal.prefix == FarmAnimal.ePrefix.HORSE)
			{
				Vector2 harvest_pos = default(Vector2);
				if (my_id == 10)
				{
					Vector3 position = base.transform.position;
					float x = position.x + 0.57f;
					Vector3 position2 = animal.transform.position;
					harvest_pos = new Vector2(x, position2.y - 0.12f);
				}
				else
				{
					Vector3 position3 = base.transform.position;
					float x2 = position3.x + ((my_id % 2 != 0) ? (-0.4f) : 0.4f);
					Vector3 position4 = animal.transform.position;
					harvest_pos = new Vector2(x2, position4.y - 0.12f);
				}
				info = new MainCharacter.HarvestInfo(harvest_pos, (my_id % 2 != 0) ? true : false, this, animal);
				CreateBusMovePos(out Vector2 start_pos, out Vector2 end_pos);
				bus_info.Come(start_pos, end_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.3f : 1f, (my_id >= 10) ? 1.2f : 0.5f, GetBaseOrder() + 175, delegate
				{
					HarvestHorse(harvester, info);
				}, delegate
				{
					FinishRace(animal);
				});
			}
			else
			{
				Vector3 position5 = animal.transform.position;
				float x3 = position5.x;
				Vector3 position6 = animal.transform.position;
				info = new MainCharacter.HarvestInfo(new Vector2(x3, position6.y - 0.26f), flip_x: false, this, animal);
				harvester.RegistHarvestQueue(info);
			}
			result = true;
		}
		else if (animal.my_id == 2 && animal_info.honey_time == 0)
		{
			Vector3 position7 = animal.transform.position;
			float x4 = position7.x;
			Vector3 position8 = animal.transform.position;
			MainCharacter.HarvestInfo info2 = new MainCharacter.HarvestInfo(new Vector2(x4, position8.y - 0.26f), flip_x: false, this, animal);
			harvester.RegistHarvestQueue(info2);
			result = true;
		}
		return result;
	}

	public void HarvestHorse(MainCharacter harvester, MainCharacter.HarvestInfo info)
	{
		harvester.RegistHarvestQueue(info);
	}

	public void GoToRace(FarmAnimal animal)
	{
		CreateBusMovePos(out Vector2 start_pos, out Vector2 end_pos);
		start_pos = end_pos;
		start_pos.x += ((my_id % 2 != 0) ? (-0.4f) : 0.4f);
		start_pos.y += 0.05f;
		end_pos.y += 0.05f;
		progress.ForcedDestroy(animal.gameObject, animal.my_id);
		animal.RideBus(start_pos, end_pos, 1.2f, (my_id % 2 != 0) ? true : false, RidedBus);
	}

	public void RidedBus()
	{
		CreateBusMovePos(out Vector2 start_pos, out Vector2 _);
		bus_info.Leave(start_pos, (my_id % 2 != 0) ? true : false, (my_id >= 10) ? 0.3f : 1f);
	}

	public void FinishRace(FarmAnimal animal)
	{
		animal.ReturnBase();
	}

	public void DelGrassGauge(FarmAnimal animal)
	{
		int index;
		GameObject gameObject;
		if (animal != null)
		{
			index = animal.my_id;
			gameObject = animal.gameObject;
		}
		else
		{
			index = 0;
			gameObject = base.gameObject;
			structures.Fish(set: false);
		}
		progress.ForcedDestroy(gameObject, index);
	}

	public void TouchAnimal(FarmAnimal animal)
	{
		if (animal.my_id < 2)
		{
			this.progress.ForcedDestroy(animal.gameObject, animal.my_id);
			Progress progress = this.progress.Create(animal.my_id, animal.transform, BAR_POS[animal.my_id], 800);
			int num = (animal.prefix != FarmAnimal.ePrefix.HORSE && animal.prefix != FarmAnimal.ePrefix.DOLPHIN && animal.prefix != FarmAnimal.ePrefix.SEA_LION && animal.prefix != FarmAnimal.ePrefix.WALRUS && animal.prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? 2 : 3;
			progress.Once((float)animal_info.give_grass[animal.my_id] / (float)num, 2f);
			this.progress.AtOnce(animal.my_id);
		}
		else
		{
			this.progress.Get(animal.my_id).Show();
			this.progress.Get(animal.my_id).Hide(2f);
		}
	}

	public void GiveGrass(int id, bool grass, bool immediate)
	{
		if (grass)
		{
			if (!immediate)
			{
				if (animal_info.animals[id] != null)
				{
					Common.OccurGrass(base.transform, animal_info.animals[id].transform.localPosition, delegate
					{
						Eating(id);
					});
				}
			}
			else
			{
				Eating(id);
			}
		}
		else if (give_grass[id] == null)
		{
			give_grass[id] = Common.OccurGiveGrassBalloon(animal_info.animals[id].transform, new Vector2(0.05f, 0.13f), GetBaseOrder() + 80, flipX: false);
		}
	}

	private void Eating(int id)
	{
		if (give_grass[id] != null)
		{
			UnityEngine.Object.Destroy(give_grass[id]);
		}
		int num = animal_info.give_grass[id];
		animal_info.give_grass[id] = manager.data.SetFacilityGiveGrass(my_id, id, animal_info.give_grass[id] + 1);
		if (animal_info.animals[id] != null && Manager.events.state == Event.eState.NONE)
		{
			Manager.sound.PlayEat();
			this.progress.ForcedDestroy(animal_info.animals[id].gameObject, id);
			Progress progress = this.progress.Create(id, animal_info.animals[id].transform, BAR_POS[id], 800);
			int num2 = (animal_info.animals[id].prefix != FarmAnimal.ePrefix.HORSE && animal_info.animals[id].prefix != FarmAnimal.ePrefix.DOLPHIN && animal_info.animals[id].prefix != FarmAnimal.ePrefix.SEA_LION && animal_info.animals[id].prefix != FarmAnimal.ePrefix.WALRUS && animal_info.animals[id].prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? 2 : 3;
			progress.Once((float)num / 2f, (float)animal_info.give_grass[id] / (float)num2, 0.3f, 3f);
			this.progress.AtOnce(id);
		}
		if (animal_info.animals[id] != null)
		{
			int num3 = (animal_info.animals[id].prefix != FarmAnimal.ePrefix.HORSE && animal_info.animals[id].prefix != FarmAnimal.ePrefix.DOLPHIN && animal_info.animals[id].prefix != FarmAnimal.ePrefix.SEA_LION && animal_info.animals[id].prefix != FarmAnimal.ePrefix.WALRUS && animal_info.animals[id].prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? 2 : 3;
			if (animal_info.give_grass[id] == num3)
			{
				animal_info.animals[id].ChangeSate(FarmAnimal.eState._STAY_1);
			}
		}
	}

	public void NotifyHarvest(FarmAnimal animal)
	{
		if (animal.my_id < 2)
		{
			animal_info.give_grass[animal.my_id] = manager.data.SetFacilityGiveGrass(my_id, animal.my_id, 0);
		}
		else
		{
			this.progress.ForcedDestroy(animal_info.animals[2].gameObject, animal.my_id);
			Progress progress = this.progress.Create(animal.my_id, animal.transform, BAR_POS[2], 800);
			animal_info.honey_time = manager.data.SetFacilityTime(my_id, 2, DateTime.Now.Ticks);
			progress.Loop((ulong)animal_info.honey_time, 30, HoneyHarvestTime);
			progress.Show();
			progress.Hide(1f);
		}
		animal.ChangeSate(FarmAnimal.eState._STAY_2);
		animal_info.ShopingOff(animal.my_id);
	}

	public void HoneyHarvestTime()
	{
		int num = 2;
		animal_info.honey_time = manager.data.SetFacilityTime(my_id, num, 0L);
		animal_info.animals[num].ChangeSate(FarmAnimal.eState._STAY_1);
		this.progress.ForcedDestroy(animal_info.animals[num].gameObject, num);
		Progress progress = this.progress.Create(num, animal_info.animals[num].transform, BAR_POS[num], 800);
		progress.Once(1f, 3f);
		this.progress.AtOnce(num);
	}

	public void CheckHarvest()
	{
		if ((state == eState.ACTIVE || tree_info.plant != TreeInfo.ePlant.NOTING) && worker != null)
		{
			if ((prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT) && timer[0] == 0)
			{
				Fishing(worker);
			}
			WorkerStartHarvest();
		}
	}

	private void WorkerStartHarvest()
	{
		for (int i = 0; i < 2; i++)
		{
			if (manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)worker.type]] < 3)
			{
				break;
			}
			if (animal_info.animals[i] != null)
			{
				animal_info.animals[i].AutoHarvest();
			}
		}
		if (manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)worker.type]] >= 4 && animal_info.animals[2] != null)
		{
			animal_info.animals[2].AutoHarvest();
		}
	}

	public void NotifyFishng()
	{
		this.progress.ForcedDestroy(base.gameObject, 0);
		Progress progress = this.progress.Create(0, base.transform, RIVER_BAR_POS, 800);
		timer[0] = (ulong)manager.data.SetFacilityTime(my_id, 0, DateTime.Now.Ticks);
		progress.Loop(timer[0], 20, FishingTime);
		progress.Show();
		progress.Hide(1f);
		touch_event.SetEnabled(enabled: true);
	}

	public void FishingTime()
	{
		timer[0] = (ulong)manager.data.SetFacilityTime(my_id, 0, 0L);
		this.progress.ForcedDestroy(base.gameObject, 0);
		Progress progress = this.progress.Create(0, base.transform, RIVER_BAR_POS, 800);
		progress.Once(1f, 3f);
		this.progress.AtOnce(0);
		structures.Fish(set: true);
	}

	public void Fishing(MainCharacter fisher)
	{
		if (timer[0] == 0)
		{
			touch_event.SetEnabled(enabled: false);
			Vector2 v = new Vector2(-0.22f, 0f);
			MainCharacter.HarvestInfo info = new MainCharacter.HarvestInfo(base.transform.TransformPoint(v), flip_x: true, this, null);
			fisher.RegistHarvestQueue(info);
		}
		else
		{
			progress.Get(0).Show();
			progress.Get(0).Hide(2f);
		}
	}

	public void FailedFishng(bool balloon)
	{
		if (balloon)
		{
			Common.OccurStoreMaxBalloon(base.transform, 1000, delegate
			{
				touch_event.SetEnabled(enabled: true);
			});
			return;
		}
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			touch_event.SetEnabled(enabled: true);
		});
		sequence.Play();
	}

	public bool ReseveTree(TreeInfo.eAnimal animal, WildAnimal sa)
	{
		bool result = false;
		if (tree_info.plant != TreeInfo.ePlant.NOTING && tree_info.reserve[(int)animal] == null)
		{
			tree_info.reserve[(int)animal] = sa;
			result = true;
		}
		return result;
	}

	public void ReleaseTree(TreeInfo.eAnimal animal)
	{
		tree_info.reserve[(int)animal] = null;
	}

	public bool IsFreeTree(TreeInfo.eAnimal animal)
	{
		bool result = false;
		if (tree_info.plant != TreeInfo.ePlant.NOTING && tree_info.reserve[(int)animal] == null)
		{
			result = true;
		}
		return result;
	}

	public Vector2 GetTreePos(TreeInfo.eAnimal animal)
	{
		return tree_info.sprite.transform.TransformPoint(tree_info.animal_pos[(int)animal]);
	}

	public int IsFreeWild(WildAnimal.eCATEGORY category)
	{
		int num = -1;
		if (state == eState.ACTIVE && prefix == ePrefix.POULTRY && category == WildAnimal.eCATEGORY.CHILD)
		{
			num = ((!(wild_animal_reserve[0] == null)) ? (-1) : 0);
			if (num == -1)
			{
				num = ((wild_animal_reserve[1] == null) ? 1 : (-1));
			}
		}
		else if (state == eState.ACTIVE && (prefix == ePrefix.RIVER || prefix == ePrefix.ROCKYSPOT) && category == WildAnimal.eCATEGORY.FOWL)
		{
			num = ((!(wild_animal_reserve[0] == null)) ? (-1) : 0);
		}
		return num;
	}

	public void ReserveWildAnimal(WildAnimal animal, int index)
	{
		wild_animal_reserve[index] = animal;
		animal.transform.parent = base.transform;
		animal.transform.localPosition = wild_animal_pos[(int)prefix][index];
	}

	public void ReleaseWildAnimal(int index)
	{
		if (wild_animal_reserve[index] != null)
		{
			UnityEngine.Object.Destroy(wild_animal_reserve[index].gameObject);
			wild_animal_reserve[index] = null;
		}
	}

	public eState State()
	{
		return state;
	}

	public eType Type()
	{
		return type;
	}

	public bool IsThereAnimals(int id)
	{
		bool result = false;
		if (animal_info.animals[id] != null)
		{
			result = true;
		}
		return result;
	}

	public bool IsHungry(int id)
	{
		int num = 2;
		if (animal_info.animals[id] != null)
		{
			num = ((animal_info.animals[id].prefix != FarmAnimal.ePrefix.HORSE && animal_info.animals[id].prefix != FarmAnimal.ePrefix.DOLPHIN && animal_info.animals[id].prefix != FarmAnimal.ePrefix.SEA_LION && animal_info.animals[id].prefix != FarmAnimal.ePrefix.WALRUS && animal_info.animals[id].prefix != FarmAnimal.ePrefix.KILLER_WHALE) ? num : 3);
		}
		return animal_info.give_grass[id] < num;
	}

	public int AnimalCount()
	{
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			if (animal_info.animals[i] != null)
			{
				num++;
			}
		}
		return num;
	}

	public FarmAnimal.eType Animal(int id)
	{
		return animal_info.types[id];
	}

	public eItem Tree()
	{
		return tree_info.type;
	}

	public void SetTouchEnabled(bool enable)
	{
		touch_event.SetEnabled(enable);
	}

	public void AssignWorker(Worker.eType type)
	{
		worker = Worker.Create(manager, type, (Worker.eWorkPlace)my_id, base.transform);
		if (state == eState.CONSTRUCT)
		{
			main.NotifyConstruct(const_positions);
			worker.RegistConstructQueue(const_worker_positions);
		}
	}

	public void FreeWorker()
	{
		if (!(worker == null))
		{
			if (state == eState.CONSTRUCT)
			{
				worker.NotifyConstruct(const_worker_positions);
				main.RegistConstructQueue(const_positions);
			}
			worker.Close();
			UnityEngine.Object.Destroy(worker.gameObject);
			worker = null;
		}
	}
}
