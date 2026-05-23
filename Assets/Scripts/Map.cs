using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
	public enum eType
	{
		FIELD,
		AIR,
		SMALL,
		CUSTOMER,
		W_ENTRY,
		W_EXIT,
		AIR_GATE,
		SMALL_GATE,
		MAX
	}

	[Serializable]
	public class SquareInfo
	{
		public bool reserve;

		public Vector2 pos;

		public Character exist_char;

		public SquareInfo(float x, float y, bool r)
		{
			pos = new Vector2(x, y);
			reserve = r;
			exist_char = null;
		}
	}

	[Serializable]
	public class VisitAreaInfo : SquareInfo
	{
		public bool arrival;

		public int exit_square;

		public VisitAreaInfo(float x, float y, bool r, int exit)
			: base(x, y, r)
		{
			arrival = false;
			exit_square = exit;
		}
	}

	[Serializable]
	public class MapInfo
	{
		[Serializable]
		public class SquareList
		{
			public List<SquareInfo> list = new List<SquareInfo>();
		}

		public List<SquareList> squares = new List<SquareList>(8);

		public MapInfo()
		{
			for (int i = 0; i < 8; i++)
			{
				squares.Add(new SquareList());
			}
		}
	}

	[Serializable]
	public class Egg
	{
		public const int LIMIT = 300;

		public int place = -1;

		public Vector2 pos;

		public float elapsed_time;

		public int order_in_layer;

		public GameObject obj;

		public Timer.TimeData timer;

		public bool request_video;

		public void Init()
		{
			elapsed_time = UnityEngine.Random.Range(10, 290);
		}
	}

	[Serializable]
	public class Monitor
	{
		public float TIMEOUT;

		public bool set;

		public float time;

		public Monitor(float to)
		{
			TIMEOUT = to;
		}

		public bool IsSet()
		{
			return set;
		}

		public void Start()
		{
			set = true;
			time = 0f;
		}

		public void Stop()
		{
			set = false;
		}

		public bool TimeOut()
		{
			bool result = false;
			if (IsSet())
			{
				time += Time.deltaTime;
				if (time > TIMEOUT)
				{
					result = true;
					Stop();
				}
			}
			return result;
		}
	}

	private const float MONITOR_TIME = 0.2f;

	private Camera cam;

	public readonly int[] LINE_MAX = new int[8]
	{
		12,
		25,
		20,
		1,
		6,
		6,
		1,
		1
	};

	public readonly int[] COLM_MAX = new int[8]
	{
		5,
		11,
		9,
		4,
		1,
		1,
		6,
		12
	};

	public readonly float[] BETWEEN_LINE = new float[8]
	{
		0.2f,
		0.12f,
		0.12f,
		0.2f,
		0.2f,
		0.2f,
		0.12f,
		0.12f
	};

	public readonly float[] BETWEEN_COLM = new float[8]
	{
		0.21f,
		0.1845f,
		0.115f,
		0.21f,
		0.21f,
		0.21f,
		0.1845f,
		0.115f
	};

	public readonly float[,] START_POS_X = new float[2, 8]
	{
		{
			-0.42f,
			-0.9225f,
			-0.46f,
			-0.84f,
			-0.42f,
			0.42f,
			1.107f,
			-1.84f
		},
		{
			-0.42f,
			-0.9225f,
			-0.46f,
			-0.84f,
			-0.21f,
			0.21f,
			1.107f,
			-1.84f
		}
	};

	public readonly float[] START_POS_Y = new float[8]
	{
		0.95f,
		1.213f,
		0.94f,
		1.15f,
		-1.45f,
		-1.45f,
		0.253f,
		-0.02f
	};

	public readonly int[] KEEPOUT_L = new int[8]
	{
		0,
		1000,
		0,
		1000,
		1000,
		1000,
		1000,
		1000
	};

	public readonly int[] KEEPOUT_R = new int[8]
	{
		4,
		1000,
		8,
		1000,
		1000,
		1000,
		1000,
		1000
	};

	public readonly int[,] FIELD_GATEWAY = new int[2, 8]
	{
		{
			-1,
			-1,
			-1,
			1,
			55,
			59,
			98,
			72
		},
		{
			-1,
			-1,
			-1,
			1,
			56,
			58,
			98,
			72
		}
	};

	public readonly int[] ROUTE_GATEWAY = new int[8]
	{
		-1,
		-1,
		-1,
		3,
		0,
		0,
		0,
		11
	};

	public readonly int[] ENTRY = new int[8]
	{
		-1,
		-1,
		-1,
		0,
		5,
		5,
		5,
		0
	};

	private static float[] WANIMAL_POSX_IN = new float[2]
	{
		-0.42f,
		-0.21f
	};

	private static float[] WANIMAL_POSX_OUT = new float[2]
	{
		0.42f,
		0.21f
	};

	private static int[] WANIMAL_FIELD_IN = new int[2]
	{
		55,
		56
	};

	private static int[] WANIMAL_FIELD_OUT = new int[2]
	{
		59,
		58
	};

	private const int VISIT_AREA_LINE = 3;

	public const int VISIT_AREA_NUM_LINE = 2;

	private const int VISIT_AREA_MAX = 8;

	private const float VISIT_L_POS_X = 0.84f;

	private Manager manager;

	[SerializeField]
	private MapInfo mapinfo;

	public List<VisitAreaInfo> visit_area_list = new List<VisitAreaInfo>(8);

	public Egg egg = new Egg();

	private const int FACILITY_MAX = 11;

	private const int TREE_MAX = 8;

	private int[] TREE_ID = new int[8];

	public List<Facility> facility_list = new List<Facility>(11);

	public List<Character> character_list = new List<Character>();

	public List<Customer> customer_list = new List<Customer>();

	public List<WildAnimal> wild_animal_list = new List<WildAnimal>();

	public List<SmallAnimal> small_animal_list = new List<SmallAnimal>();

	public List<Fowl> fowl_list = new List<Fowl>();

	[SerializeField]
	private MainCharacter main;

	[SerializeField]
	private Monitor char_monitor;

	private const float MAGIN_SCREEN = 0.2f;

	private List<int> egg_place_list = new List<int>
	{
		21,
		22,
		23,
		27,
		31,
		32,
		33
	};

	private void Update()
	{
		CharacterMonitor();
		EggMonitor();
	}

	private void CharacterMonitor()
	{
		if (char_monitor != null && char_monitor.TimeOut())
		{
			NextTurn();
		}
	}

	public void Init(Manager m)
	{
		manager = m;
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		CreateMapInfo();
		CreateMain(m.data.main_type);
		InitFacility();
		char_monitor = new Monitor(0.2f);
		egg.Init();
		NextTurn();
	}

	private void CreateMapInfo()
	{
		mapinfo = new MapInfo();
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < LINE_MAX[i] * COLM_MAX[i]; j++)
			{
				bool r = false;
				switch (i)
				{
				case 0:
					r = (((j % COLM_MAX[i] == KEEPOUT_L[i] || j % COLM_MAX[i] == KEEPOUT_R[i]) && j / COLM_MAX[i] % 3 != 2) ? true : false);
					break;
				case 2:
					r = ((j % COLM_MAX[i] == KEEPOUT_L[i] || j % COLM_MAX[i] == KEEPOUT_R[i]) ? true : false);
					break;
				}
				mapinfo.squares[i].list.Add(new SquareInfo(START_POS_X[(int)Data.farm_type, i] + BETWEEN_COLM[i] * (float)(j % COLM_MAX[i]), START_POS_Y[i] - BETWEEN_LINE[i] * (float)(j / COLM_MAX[i]), r));
			}
		}
		for (int k = 0; k < 8; k++)
		{
			visit_area_list.Add(new VisitAreaInfo((k % 2 != 0) ? 0.84f : (-0.84f), START_POS_Y[0] - BETWEEN_LINE[0] * (float)(3 * (k / 2 + 1) - 1), r: false, COLM_MAX[0] * (3 * (k / 2 + 1) - 1) + (COLM_MAX[0] - 1) * (k % 2)));
		}
	}

	private void InitFacility()
	{
		for (int i = 0; i < 11; i++)
		{
			facility_list.Add(GameObject.Find("facility" + i).GetComponent<Facility>());
			facility_list[i].Init(i, manager, main);
		}
		for (int j = 0; j < 8; j++)
		{
			TREE_ID[j] = j;
		}
	}

	private void CreateMain(Data.eMainType type)
	{
		main = manager.main;
		SetRandom(main, (int)type, eType.FIELD, -1);
		AddTurnCharactor(main);
	}

	private Character CreateCustomer()
	{
		Customer customer = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/human"), base.transform).AddComponent<Customer>();
		customer_list.Add(customer);
		AddTurnCharactor(customer);
		return customer;
	}

	private Character CreateWildAnimal()
	{
		WildAnimal wildAnimal = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/character"), base.transform).AddComponent<WildAnimal>();
		wild_animal_list.Add(wildAnimal);
		AddTurnCharactor(wildAnimal);
		return wildAnimal;
	}

	private Character CreateSmallAnimal()
	{
		SmallAnimal smallAnimal = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/character"), base.transform).AddComponent<SmallAnimal>();
		small_animal_list.Add(smallAnimal);
		return smallAnimal;
	}

	private Fowl CreateFowl()
	{
		Fowl fowl = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/character"), base.transform).AddComponent<Fowl>();
		fowl_list.Add(fowl);
		return fowl;
	}

	public bool GoToFarm(Customer.eType type, int id, bool entry)
	{
		bool flag = true;
		if (entry)
		{
			flag = ReserveEntryPos(eType.CUSTOMER);
			if (flag)
			{
				SetEntry(CreateCustomer(), (int)type, eType.CUSTOMER, id);
			}
		}
		else
		{
			SetRandom(CreateCustomer(), (int)type, eType.FIELD, id);
		}
		return flag;
	}

	public void ReturnFromFarm(Customer.eType type)
	{
		Customer customer = customer_list.Find((Customer item) => item.type == type);
		if (customer != null)
		{
			SetExit(customer);
			customer_list.Remove(customer);
		}
	}

	public bool GoToFarm(WildAnimal.eType type, int id, bool entry)
	{
		WildAnimal.eCATEGORY eCATEGORY = WildAnimal.TypeToCategory(type);
		bool flag = true;
		switch (eCATEGORY)
		{
		case WildAnimal.eCATEGORY.NORMAL:
			return WildEntry(type, id, entry);
		case WildAnimal.eCATEGORY.CHILD:
		case WildAnimal.eCATEGORY.FOWL:
			return FowlEntry(type, id, eCATEGORY);
		default:
			return SmallEntry(type, id, entry, eCATEGORY);
		}
	}

	public void ReturnFromFarm(WildAnimal.eType type, int id)
	{
		WildAnimal.eCATEGORY eCATEGORY = WildAnimal.TypeToCategory(type);
		switch (eCATEGORY)
		{
		case WildAnimal.eCATEGORY.NORMAL:
			WildExit(type, id, eCATEGORY);
			break;
		case WildAnimal.eCATEGORY.CHILD:
		case WildAnimal.eCATEGORY.FOWL:
			FowlExit(type, id, eCATEGORY);
			break;
		default:
			SmallExit(type, id, eCATEGORY);
			break;
		}
	}

	public bool IsGoToFarm(WildAnimal.eType type)
	{
		bool flag = false;
		WildAnimal.eCATEGORY eCATEGORY = WildAnimal.TypeToCategory(type);
		switch (eCATEGORY)
		{
		case WildAnimal.eCATEGORY.NORMAL:
			flag = true;
			break;
		case WildAnimal.eCATEGORY.CHILD:
		case WildAnimal.eCATEGORY.FOWL:
			for (int j = 0; j < 11; j++)
			{
				if (flag)
				{
					break;
				}
				int num = facility_list[j].IsFreeWild(eCATEGORY);
				flag = ((num != -1) ? true : false);
			}
			break;
		default:
			for (int i = 0; i < 8; i++)
			{
				if (flag)
				{
					break;
				}
				flag = facility_list[i].IsFreeTree((eCATEGORY != WildAnimal.eCATEGORY.BIRD) ? Facility.TreeInfo.eAnimal.SMALL : Facility.TreeInfo.eAnimal.BIRD);
			}
			break;
		}
		return flag;
	}

	private bool WildEntry(WildAnimal.eType type, int id, bool entry)
	{
		bool flag = true;
		if (entry)
		{
			flag = ReserveEntryPos(eType.W_ENTRY);
			if (flag)
			{
				SetEntry(CreateWildAnimal(), (int)type, eType.W_ENTRY, id);
			}
		}
		else
		{
			SetRandom(CreateWildAnimal(), (int)type, eType.FIELD, id);
		}
		return flag;
	}

	private bool SmallEntry(WildAnimal.eType type, int id, bool entry, WildAnimal.eCATEGORY category)
	{
		bool result = true;
		if (entry)
		{
			SetEntry(CreateSmallAnimal(), (int)type, (category != WildAnimal.eCATEGORY.BIRD) ? eType.SMALL_GATE : eType.AIR_GATE, id);
		}
		else
		{
			SetRandom(CreateSmallAnimal(), (int)type, (category == WildAnimal.eCATEGORY.BIRD) ? eType.AIR : eType.SMALL, id);
		}
		return result;
	}

	private bool FowlEntry(WildAnimal.eType type, int id, WildAnimal.eCATEGORY category)
	{
		bool flag = false;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 11; i++)
		{
			if (flag)
			{
				break;
			}
			num2 = facility_list[i].IsFreeWild(category);
			if (num2 != -1)
			{
				flag = true;
				num = i;
			}
		}
		if (flag)
		{
			Fowl fowl = CreateFowl();
			fowl.Open(manager, num, num2, Character.eState.ENTRY, (int)type, id);
			manager.map.facility_list[num].ReserveWildAnimal(fowl, num2);
		}
		return flag;
	}

	private void WildExit(WildAnimal.eType type, int id, WildAnimal.eCATEGORY category)
	{
		WildAnimal wildAnimal = wild_animal_list.Find((WildAnimal item) => item.type == type && item.my_id == id && item.category == category);
		if (wildAnimal != null)
		{
			SetExit(wildAnimal);
			wild_animal_list.Remove(wildAnimal);
		}
	}

	private void SmallExit(WildAnimal.eType type, int id, WildAnimal.eCATEGORY category)
	{
		SmallAnimal smallAnimal = small_animal_list.Find((SmallAnimal item) => item.type == type && item.my_id == id && item.category == category);
		if (smallAnimal != null)
		{
			SetExit(smallAnimal);
			small_animal_list.Remove(smallAnimal);
		}
	}

	private void FowlExit(WildAnimal.eType type, int id, WildAnimal.eCATEGORY category)
	{
		Fowl fowl = fowl_list.Find((Fowl item) => item.type == type && item.my_id == id && item.category == category);
		if (fowl != null)
		{
			SetExit(fowl);
			fowl_list.Remove(fowl);
			manager.map.facility_list[fowl.f_id].ReleaseWildAnimal(fowl.f_space);
		}
	}

	public void AddTurnCharactor(Character c)
	{
		if (!character_list.Contains(c))
		{
			character_list.Add(c);
		}
	}

	public void RemoveTurnCharactor(Character c)
	{
		if (character_list.Contains(c))
		{
			character_list.Remove(c);
		}
	}

	private void NextTurn()
	{
		if (character_list.Count != 0 && character_list.FindAll((Character item) => item.move == Character.eMove.ACTIVE).Count == 0)
		{
			for (int i = 0; i < character_list.Count; i++)
			{
				if (character_list[i].move == Character.eMove.WAIT)
				{
					character_list[i].NextTurn();
				}
			}
		}
		char_monitor.Start();
	}

	public void TurnEndNotify()
	{
	}

	public void SetCharactersPause(bool pause)
	{
		foreach (Character item in character_list)
		{
			item.SetPause(pause);
		}
	}

	public void ReseveSquare(int prev, int next, eType place)
	{
		if (place != eType.AIR && place != eType.SMALL && place != eType.AIR_GATE && place != eType.SMALL_GATE)
		{
			mapinfo.squares[(int)place].list[prev].reserve = false;
			mapinfo.squares[(int)place].list[next].reserve = true;
		}
	}

	public void ReseveSquare(int square, bool reserve, eType place)
	{
		if (place != eType.AIR && place != eType.SMALL && place != eType.AIR_GATE && place != eType.SMALL_GATE)
		{
			mapinfo.squares[(int)place].list[square].reserve = reserve;
		}
	}

	public int ArriveField(int area_id)
	{
		int i;
		for (i = 0; i < visit_area_list.Count && visit_area_list[i].exit_square != area_id; i++)
		{
		}
		return i;
	}

	public Vector2 GetFieldPos(int index, eType place)
	{
		return mapinfo.squares[(int)place].list[index].pos;
	}

	public int GetFreeField(int prev, eType place)
	{
		int num = prev;
		List<int> list = new List<int>();
		if (prev % COLM_MAX[(int)place] > 0)
		{
			list.Add(-1);
		}
		if (prev % COLM_MAX[(int)place] < COLM_MAX[(int)place] - 1)
		{
			list.Add(1);
		}
		if (prev / COLM_MAX[(int)place] > 0)
		{
			list.Add(-COLM_MAX[(int)place]);
		}
		if (prev / COLM_MAX[(int)place] < LINE_MAX[(int)place] - 1)
		{
			list.Add(COLM_MAX[(int)place]);
		}
		for (int i = 0; i < 2; i++)
		{
			if (place != 0)
			{
				break;
			}
			list.Add(0);
		}
		while (list.Count != 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			int num2 = prev + list[index];
			if (num2 == prev)
			{
				break;
			}
			if (num2 % COLM_MAX[(int)place] != KEEPOUT_L[(int)place] && num2 % COLM_MAX[(int)place] != KEEPOUT_R[(int)place])
			{
				if (!mapinfo.squares[(int)place].list[num2].reserve)
				{
					num = num2;
					ReseveSquare(prev, num, place);
					break;
				}
				list.RemoveAt(index);
			}
		}
		return num;
	}

	private int GetShortRoute(int prev, int target, eType place)
	{
		if (prev / COLM_MAX[(int)place] == target / COLM_MAX[(int)place])
		{
			return prev + ((target > prev) ? 1 : (-1));
		}
		return prev + ((target <= prev) ? (-COLM_MAX[(int)place]) : COLM_MAX[(int)place]);
	}

	public int GetNextField(int prev, int target, eType place)
	{
		int num = GetShortRoute(prev, target, place);
		if (num != target || (place != eType.AIR && place != eType.SMALL))
		{
			if (mapinfo.squares[(int)place].list[num].reserve)
			{
				num = GetFreeField(prev, place);
			}
			else
			{
				ReseveSquare(prev, num, place);
			}
		}
		return num;
	}

	public int GetNextExit(int prev, int target, eType place)
	{
		int num = prev % COLM_MAX[(int)place];
		int num2;
		if (num == 0 || num == COLM_MAX[(int)place] - 1)
		{
			num2 = ((num != 0) ? (prev - 1) : (prev + 1));
		}
		else
		{
			num2 = GetShortRoute(prev, target, place);
			if (target != num2 && mapinfo.squares[(int)place].list[num2].reserve)
			{
				object list2;
				if (prev / COLM_MAX[(int)place] == target / COLM_MAX[(int)place])
				{
					List<int> list = new List<int>();
					list.Add(-COLM_MAX[(int)place]);
					list.Add(COLM_MAX[(int)place]);
					list2 = list;
				}
				else
				{
					List<int> list = new List<int>();
					list.Add(-1);
					list.Add(1);
					list2 = list;
				}
				List<int> list3 = (List<int>)list2;
				for (int i = 0; i < list3.Count; i++)
				{
					int num3 = prev + list3[i];
					if (0 <= num3 && num3 < LINE_MAX[(int)place] * COLM_MAX[(int)place] && num3 % COLM_MAX[(int)place] != KEEPOUT_L[(int)place] && num3 % COLM_MAX[(int)place] != KEEPOUT_R[(int)place] && !mapinfo.squares[(int)place].list[num3].reserve)
					{
						num2 = num3;
						break;
					}
				}
			}
		}
		return num2;
	}

	private void SetEntry(Character c, int type, eType place, int id)
	{
		c.Open(manager, ENTRY[(int)place], mapinfo.squares[(int)place].list[ENTRY[(int)place]].pos, Character.eState.ENTRY, type, id);
	}

	private void SetRandom(Character c, int type, eType place, int id)
	{
		int num = ReseveRandomPos(place);
		c.Open(manager, num, mapinfo.squares[(int)place].list[num].pos, Character.eState.FIELD, type, id);
	}

	private bool ReserveEntryPos(eType place)
	{
		if (!mapinfo.squares[(int)place].list[ENTRY[(int)place]].reserve)
		{
			ReseveSquare(ENTRY[(int)place], reserve: true, place);
		}
		return mapinfo.squares[(int)place].list[ENTRY[(int)place]].reserve;
	}

	private int ReseveRandomPos(eType place)
	{
		int num = 0;
		bool flag = false;
		while (!flag)
		{
			num = UnityEngine.Random.Range(1, LINE_MAX[(int)place] * COLM_MAX[(int)place]);
			if (!mapinfo.squares[(int)place].list[num].reserve && num % COLM_MAX[(int)place] != KEEPOUT_L[(int)place] && num % COLM_MAX[(int)place] != KEEPOUT_R[(int)place])
			{
				if (place != eType.AIR && place != eType.SMALL && place != eType.AIR_GATE && place != eType.SMALL_GATE)
				{
					mapinfo.squares[(int)place].list[num].reserve = true;
				}
				flag = true;
			}
		}
		return num;
	}

	public int GetNextEntryRoute(int prev, ref eType place)
	{
		int num = prev;
		eType eType = (place == eType.AIR_GATE) ? eType.AIR : ((place == eType.SMALL_GATE) ? eType.SMALL : eType.FIELD);
		if (prev == ROUTE_GATEWAY[(int)place])
		{
			int num2 = FIELD_GATEWAY[(int)Data.farm_type, (int)place];
			if (!mapinfo.squares[(int)eType].list[num2].reserve || eType == eType.AIR || eType == eType.SMALL)
			{
				num = num2;
				ReseveSquare(prev, reserve: false, place);
				place = eType;
				ReseveSquare(num, reserve: true, place);
			}
		}
		else
		{
			int num2 = ROUTE_GATEWAY[(int)place];
			int shortRoute = GetShortRoute(prev, num2, place);
			if (!mapinfo.squares[(int)place].list[shortRoute].reserve)
			{
				num = shortRoute;
				ReseveSquare(prev, num, place);
			}
		}
		return num;
	}

	public void SetExit(Character c)
	{
		c.ReturnBase();
	}

	public int GetNextExitRoute(int prev, int target, eType place)
	{
		int result = ROUTE_GATEWAY[(int)place];
		if (prev != -1)
		{
			result = GetShortRoute(prev, target, place);
		}
		return result;
	}

	public Vector2 GetRoutePos(int index, eType place)
	{
		return mapinfo.squares[(int)place].list[index].pos;
	}

	public bool ReserveVisitArea(int area_id, Character c)
	{
		bool result = false;
		if (!visit_area_list[area_id].reserve)
		{
			visit_area_list[area_id].reserve = true;
			c.SetDestination(visit_area_list[area_id].exit_square);
			result = true;
		}
		return result;
	}

	public void ReleaseVisitArea(int area_id)
	{
		visit_area_list[area_id].reserve = false;
		visit_area_list[area_id].exist_char = null;
	}

	public void AddSmallAnimal(SmallAnimal c)
	{
		small_animal_list.Add(c);
	}

	public void RemoveSmallAnimal(SmallAnimal c)
	{
		small_animal_list.Remove(c);
	}

	private void SetEntrySmallAnimal(Character c, int type, int id)
	{
		c.Open(manager, -1, GetSmallAnimalEntryExitPos(), Character.eState.ENTRY, type, id);
	}

	public void SetExitSmallAnimal(Character c)
	{
		c.ReturnBase();
	}

	public int GetTreePos(Facility.TreeInfo.eAnimal animal, SmallAnimal sa, out int tree_index)
	{
		int[] array = (from i in TREE_ID
			orderby Guid.NewGuid()
			select i).ToArray();
		int num = -1;
		tree_index = -1;
		for (int j = 0; j < 8; j++)
		{
			if (num != -1)
			{
				break;
			}
			if (facility_list[array[j]].ReseveTree(animal, sa))
			{
				tree_index = array[j];
				num = facility_list[tree_index].tree_info.map_index[(int)animal];
			}
		}
		return num;
	}

	public void ReleaseTree(int idex, Facility.TreeInfo.eAnimal animal)
	{
		facility_list[idex].ReleaseTree(animal);
	}

	private Vector2 GetSmallAnimalEntryExitPos()
	{
		int num = UnityEngine.Random.Range(0, visit_area_list.Count);
		Vector2 result = (num % 2 != 0) ? cam.ScreenToWorldPoint(new Vector2(Screen.width, 0f)) : cam.ScreenToWorldPoint(Vector2.zero);
		result.x = ((num % 2 != 0) ? (result.x + 0.2f) : (result.x - 0.2f));
		result.y = mapinfo.squares[0].list[visit_area_list[num].exit_square].pos.y;
		return result;
	}

	public Vector2 GetNextEntry(Vector2 prev, eType place)
	{
		return new Vector2((!(prev.x < 0f)) ? mapinfo.squares[0].list[COLM_MAX[(int)place] - 1].pos.x : mapinfo.squares[0].list[0].pos.x, prev.y);
	}

	public Vector2 GetNextExit()
	{
		return GetSmallAnimalEntryExitPos();
	}

	public int GetNextTarget(int prev, eType place)
	{
		bool flag = false;
		int num = prev;
		while (!flag)
		{
			num = UnityEngine.Random.Range(0, LINE_MAX[(int)place] * COLM_MAX[(int)place]);
			flag = ((!mapinfo.squares[(int)place].list[num].reserve) ? true : false);
		}
		return num;
	}

	public int ReserveEggPlace(out Vector2 pos, out int order)
	{
		int num = -1;
		pos = Vector2.zero;
		order = 0;
		List<int> list = new List<int>(egg_place_list);
		while (list.Count != 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			if (mapinfo.squares[0].list[index].reserve)
			{
				list.RemoveAt(index);
				continue;
			}
			ReseveSquare(list[index], reserve: true, eType.FIELD);
			num = list[index];
			pos = mapinfo.squares[0].list[num].pos;
			order = 40 + num * 2;
			break;
		}
		return num;
	}

	public void ReleaseEggPlace(int place)
	{
		ReseveSquare(place, reserve: false, eType.FIELD);
	}

	public void UpdateFacilityFlower()
	{
		for (int i = 0; i < facility_list.Count; i++)
		{
			if (Data.farm_type != Data.eFarmType.RESORT || i != 10)
			{
				facility_list[i].flower.FlowerLevelup(manager.data.level);
			}
		}
	}

	private void EggMonitor()
	{
		if (manager.data.level <= 3)
		{
			return;
		}
		if (!egg.request_video)
		{
			egg.elapsed_time += Time.deltaTime;
			if (!(egg.elapsed_time >= 300f))
			{
				return;
			}
			egg.elapsed_time = 0f;
			if (egg.place != -1)
			{
				return;
			}
			int num = UnityEngine.Random.Range(0, 2);
			if (num == 1)
			{
				egg.place = ReserveEggPlace(out egg.pos, out egg.order_in_layer);
				if (egg.place != -1 && manager.LoadVideo())
				{
					egg.request_video = true;
				}
			}
		}
		else if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			egg.request_video = false;
			egg.obj = SetEgg(egg.pos, egg.order_in_layer);
			egg.timer = Timer.Create((ulong)DateTime.Now.Ticks, 60, TouchEgg);
		}
		else if (manager.load_video.state == Manager.VideoRwd.eState.NONE)
		{
			egg.request_video = false;
			TouchEgg();
		}
	}

	private GameObject SetEgg(Vector2 pos, int order_in_layer)
	{
		if (UnityEngine.Random.Range(0, 3) == 0)
		{
			return Common.CreatePresentCoinEggBox(base.transform, pos, order_in_layer, TouchEgg);
		}
		return Common.CreatePresentExpEggBox(base.transform, pos, order_in_layer, TouchEgg);
	}

	public void TouchEgg()
	{
		if (egg.place != -1)
		{
			ReleaseEggPlace(egg.place);
			egg.place = -1;
			if (egg.obj != null)
			{
				UnityEngine.Object.Destroy(egg.obj);
				egg.obj = null;
			}
		}
		if (egg.timer != null)
		{
			Timer.Remove(egg.timer);
		}
	}
}
