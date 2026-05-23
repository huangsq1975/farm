using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
	public enum eSAVE
	{
		OPENING,
		ENDING,
		MAIN_TYPE,
		FARM_TYPE,
		VOLUME,
		INTERSTITIAL_COUNT,
		FIRST_COIN,
		PREV_SYSTEM_TIME,
		CHARACTER_REG,
		CHARACTER_LEVEL,
		CHARACTER_NEW_REG,
		CHARACTER_WATCHED_VIDEO,
		LEVEL_CONDITION_COUNT,
		LEVEL,
		EXP,
		COIN,
		STORE_LEVEL,
		STORE_BASKET,
		STORE_BASKET_ITEM,
		STORE_NEXT_CUSTOMER_TIME,
		STORE_STOCK_EARNINGS,
		STORE_UPDATE_TIME,
		SAILO_LEVEL,
		SAILO_GRASS_STOCK,
		SAILO_GRASS_TIME,
		SAILO_SPOUT_OUT_TIME,
		SAILO_UPDATE_TIME,
		HOTEL_LEVEL,
		HOTEL_CUSTOMER_TIME,
		HOTEL_CUSTOMER_TYPE,
		HOTEL_CUSTOMER_VISIT_TIME,
		HOTEL_UPDATE_TIME,
		TREE_PLANT,
		FACILITY_STATE,
		FACILITY_TYPE,
		FACILITY_TIME,
		FACILITY_FARM_ANIMAL,
		FACILITY_GIVE_GRASS,
		FACILITY_TREE,
		FACILITY_TREE_PLANT,
		FACILITY_ENABLE,
		WILD_ANIMAL_VISIT_TIME,
		WILD_ANIMAL_TIME,
		WILD_ANIMAL_TYPE,
		SUGOROKU_DICE_VALUE,
		SUGOROKU_MOVE_COUNT,
		SUGOROKU_HISTORY,
		SUGOROKU_DICE_REMIND_COUNT,
		SUGOROKU_CURRENT,
		SUGOROKU_TIMER,
		SUGOROKU_CAR_DIR,
		SUGOROKU_MAP,
		SUGOROKU_FARM,
		SUGOROKU_COIN,
		SUGOROKU_EXP,
		SUGOROKU_EVENT,
		ALBUM_DISP,
		WORKER_LEVEL,
		WORKER_PLACE,
		WORKER_RELEASE,
		PURCHASE,
		REVIEW,
		RESORT_EVENT,
		CHK_SPECIAL_WORKER,
		DEV_PLAYER_NAME
	}

	public enum eLang
	{
		JP,
		EN,
		MAX
	}

	public enum eFarmType
	{
		NORMAL = 0,
		RESORT = 1,
		MAX = 2,
		IGNORE = -1
	}

	public enum eMainType
	{
		MAIN_1,
		MAIN_2,
		MAX
	}

	[Serializable]
	public class CharacterData
	{
		public enum eType
		{
			FARMANIMAL,
			WILDANIMAL,
			FISH,
			CUSTOMER,
			MAX
		}

		[Serializable]
		public class Contents
		{
			public int reg;

			public int level;

			public int new_reg;

			public int watched_video;

			public int level_condition_count;
		}

		public List<Contents> contents = new List<Contents>();

		public int album;
	}

	[Serializable]
	public class Condition
	{
		public enum eCATEGORY
		{
			FARMANIMAL,
			WILDANIMAL,
			FISH,
			CUSTOMER,
			FACILITY,
			LEVEL,
			CASHER,
			FACILITYITEM,
			SEASON_EVENT,
			HOTEL,
			FARM_TYPE
		}

		public eCATEGORY category;

		public int type = -1;

		public Condition(eCATEGORY _category, int _type = -1)
		{
			category = _category;
			type = _type;
		}
	}

	public enum eHARVEST
	{
		NONE = -1,
		MILK,
		FUR,
		MEAT,
		FISH,
		EGG,
		HONEY,
		FRUIT,
		MAX
	}

	[Serializable]
	public class StoreData
	{
		[Serializable]
		public class Table
		{
			[Serializable]
			public class Basket
			{
				[Serializable]
				public class Item
				{
					[HideInInspector]
					public int id;

					public int value = -1;

					public Item(int _id, int _value)
					{
						id = _id;
						value = _value;
					}
				}

				[HideInInspector]
				public int id;

				public eHARVEST type = eHARVEST.NONE;

				public int item_count;

				public List<Item> items = new List<Item>();

				public static readonly int[] tBASKET_ITEM_MAX = new int[7]
				{
					8,
					8,
					8,
					5,
					8,
					9,
					9
				};

				public Basket(int table, int basket_pos)
				{
					id = basket_pos;
					type = (eHARVEST)PlayerPrefs.GetInt(eSAVE.STORE_BASKET.ToString() + "_" + table + "_" + basket_pos + suf, -1);
					if (type == eHARVEST.NONE)
					{
						return;
					}
					for (int i = 0; i < tBASKET_ITEM_MAX[(int)type]; i++)
					{
						int @int = PlayerPrefs.GetInt(eSAVE.STORE_BASKET_ITEM.ToString() + "_" + table + "_" + basket_pos + "_" + i + suf, -1);
						items.Add(new Item(i, @int));
						if (@int != -1)
						{
							item_count++;
						}
					}
				}

				public int GetItemMax()
				{
					return tBASKET_ITEM_MAX[(int)type];
				}

				public static int GetItemMax(eHARVEST harvest)
				{
					return tBASKET_ITEM_MAX[(int)harvest];
				}
			}

			public const int BASKET_MAX = 6;

			[HideInInspector]
			public int id;

			public List<Basket> baskets = new List<Basket>();

			public Table(int table)
			{
				id = table;
				for (int i = 0; i < 6; i++)
				{
					baskets.Add(new Basket(table, i));
				}
			}
		}

		public const int LEVEL_MAX = 4;

		public int level;

		public ulong next_customer_time;

		public ulong store_update_time;

		public int stock_earnings;

		public List<Table> tables = new List<Table>();

		public void Init()
		{
			level = PlayerPrefs.GetInt(eSAVE.STORE_LEVEL.ToString() + suf, 1);
			for (int i = 0; i < level; i++)
			{
				tables.Add(new Table(i));
			}
			stock_earnings = PlayerPrefs.GetInt(eSAVE.STORE_STOCK_EARNINGS.ToString() + suf, 0);
			next_customer_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.STORE_NEXT_CUSTOMER_TIME.ToString() + suf, DateTime.Now.Ticks.ToString()));
			store_update_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.STORE_UPDATE_TIME.ToString() + suf, "0"));
		}

		public void AddBasket(int table, int basket_pos, eHARVEST basket_type)
		{
			Table.Basket basket = tables[table].baskets[basket_pos];
			basket.type = basket_type;
			PlayerPrefs.SetInt(eSAVE.STORE_BASKET.ToString() + "_" + table + "_" + basket_pos + suf, (int)basket_type);
			int itemMax = basket.GetItemMax();
			for (int i = 0; i < itemMax; i++)
			{
				basket.items.Add(new Table.Basket.Item(i, -1));
			}
		}

		public void AddItem(int table, int basket_pos, int item_pos, int item)
		{
			Table.Basket basket = tables[table].baskets[basket_pos];
			basket.items[item_pos].value = item;
			basket.item_count++;
			if (basket.item_count > basket.GetItemMax())
			{
				UnityEngine.Debug.unityLogger.LogError("Error", "バスケットのアイテム数が異常です。" + basket.type + " item_count=" + basket.item_count);
				basket.item_count = basket.GetItemMax();
			}
			PlayerPrefs.SetInt(eSAVE.STORE_BASKET_ITEM.ToString() + "_" + table + "_" + basket_pos + "_" + item_pos + suf, item);
		}

		public void DelItem(int table, int basket_pos, int item_pos)
		{
			Table.Basket basket = tables[table].baskets[basket_pos];
			basket.items[item_pos].value = -1;
			basket.item_count--;
			if (basket.item_count < 0)
			{
				UnityEngine.Debug.unityLogger.LogError("Error", "バスケットのアイテム数が異常です。" + basket.type + " item_count=" + basket.item_count);
				basket.item_count = 0;
			}
			if (basket.item_count == 0)
			{
				for (int i = 0; i < basket.GetItemMax(); i++)
				{
					PlayerPrefs.SetInt(eSAVE.STORE_BASKET_ITEM.ToString() + "_" + table + "_" + basket_pos + "_" + i + suf, -1);
				}
				basket.type = eHARVEST.NONE;
				PlayerPrefs.SetInt(eSAVE.STORE_BASKET.ToString() + "_" + table + "_" + basket_pos + suf, -1);
				basket.items.Clear();
			}
			else
			{
				PlayerPrefs.SetInt(eSAVE.STORE_BASKET_ITEM.ToString() + "_" + table + "_" + basket_pos + "_" + item_pos + suf, -1);
			}
		}

		public void AddTable()
		{
			if (level < 4)
			{
				tables.Add(new Table(level));
				level++;
				PlayerPrefs.SetInt(eSAVE.STORE_LEVEL.ToString() + suf, level);
			}
		}

		public void UpdateStore(ulong time)
		{
			store_update_time = time;
			PlayerPrefs.SetString(eSAVE.STORE_UPDATE_TIME.ToString() + suf, store_update_time.ToString());
		}

		public int ValidItemCount()
		{
			return level * 48;
		}

		public void AddStockEarnings(int coin)
		{
			stock_earnings += coin;
			PlayerPrefs.SetInt(eSAVE.STORE_STOCK_EARNINGS.ToString() + suf, stock_earnings);
		}

		public void ClearStockEarnings()
		{
			stock_earnings = 0;
			PlayerPrefs.SetInt(eSAVE.STORE_STOCK_EARNINGS.ToString() + suf, 0);
		}

		public void SetNextCustomerTime(ulong time)
		{
			next_customer_time = time;
			PlayerPrefs.SetString(eSAVE.STORE_NEXT_CUSTOMER_TIME.ToString() + suf, next_customer_time.ToString());
		}
	}

	[Serializable]
	public class HotelData
	{
		[Serializable]
		public class Room
		{
			public Customer.eType type = Customer.eType.NONE;

			public ulong time;
		}

		public const int LEVEL_MAX = 4;

		public int level = 1;

		public ulong hotel_update_time;

		public ulong customer_visit_time;

		public Room[] rooms = new Room[4];

		public void Init()
		{
			level = PlayerPrefs.GetInt(eSAVE.HOTEL_LEVEL.ToString() + suf, hotel_init_level[(int)farm_type]);
			hotel_update_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.HOTEL_UPDATE_TIME.ToString() + suf, "0"));
			customer_visit_time = ulong.Parse(PlayerPrefs.GetString(defaultValue: ((ulong)(DateTime.Now.Ticks - 2900000000u)).ToString(), key: eSAVE.HOTEL_CUSTOMER_VISIT_TIME.ToString() + suf));
			for (int i = 0; i < 4; i++)
			{
				rooms[i] = new Room();
				rooms[i].type = (Customer.eType)PlayerPrefs.GetInt(eSAVE.HOTEL_CUSTOMER_TYPE.ToString() + "_" + i + suf, -1);
				rooms[i].time = ulong.Parse(PlayerPrefs.GetString(eSAVE.HOTEL_CUSTOMER_TIME.ToString() + "_" + i + suf, "0"));
			}
		}

		public void UpdateHotel(ulong time)
		{
			hotel_update_time = time;
			PlayerPrefs.SetString(eSAVE.HOTEL_UPDATE_TIME.ToString() + suf, hotel_update_time.ToString());
		}

		public void SetVisit(ulong time)
		{
			customer_visit_time = time;
			PlayerPrefs.SetString(eSAVE.HOTEL_CUSTOMER_VISIT_TIME.ToString() + suf, time.ToString());
		}

		public void AddRoom()
		{
			level++;
			PlayerPrefs.SetInt(eSAVE.HOTEL_LEVEL.ToString() + suf, level);
		}

		public void SetCustomer(ulong time, Customer.eType type, int room_id)
		{
			rooms[room_id].type = type;
			rooms[room_id].time = time;
			PlayerPrefs.SetInt(eSAVE.HOTEL_CUSTOMER_TYPE.ToString() + "_" + room_id + suf, (int)type);
			PlayerPrefs.SetString(eSAVE.HOTEL_CUSTOMER_TIME.ToString() + "_" + room_id + suf, time.ToString());
		}
	}

	[Serializable]
	public class SailoData
	{
		[Serializable]
		public class Area
		{
			[Serializable]
			public class Grass
			{
				public int area;

				public int id;

				public ulong time;

				public Grass(int area_pos, int pos)
				{
					area = area_pos;
					id = pos;
					time = ulong.Parse(PlayerPrefs.GetString(eSAVE.SAILO_GRASS_TIME.ToString() + "_" + area + "_" + id + suf, "0"));
				}
			}

			public const int GRASS_MAX = 15;

			public Grass[] grasses = new Grass[15];
		}

		public const int LEVEL_MAX = 4;

		public static int[] GRASS_STOCK_MAX = new int[5]
		{
			0,
			30,
			60,
			90,
			150
		};

		public int level = 1;

		public int grass_stock;

		public ulong spout_out_time;

		public ulong sailo_update_time;

		public Area[] areas = new Area[4];

		public void Init()
		{
			level = PlayerPrefs.GetInt(eSAVE.SAILO_LEVEL.ToString() + suf, 1);
			grass_stock = PlayerPrefs.GetInt(eSAVE.SAILO_GRASS_STOCK.ToString() + suf, 2);
			spout_out_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.SAILO_SPOUT_OUT_TIME.ToString() + suf, DateTime.Now.Ticks.ToString()));
			sailo_update_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.SAILO_UPDATE_TIME.ToString() + suf, "0"));
			for (int i = 0; i < 4; i++)
			{
				areas[i] = new Area();
				for (int j = 0; j < 15; j++)
				{
					areas[i].grasses[j] = new Area.Grass(i, j);
				}
			}
		}

		public void SpoutOut()
		{
			spout_out_time = (ulong)DateTime.Now.Ticks;
			PlayerPrefs.SetString(eSAVE.SAILO_SPOUT_OUT_TIME.ToString() + suf, spout_out_time.ToString());
		}

		public void GrassCut(int area, int grass)
		{
			areas[area].grasses[grass].time = (ulong)DateTime.Now.Ticks;
			PlayerPrefs.SetString(eSAVE.SAILO_GRASS_TIME.ToString() + "_" + area + "_" + grass + suf, areas[area].grasses[grass].time.ToString());
		}

		public void GrassGrow(int area, int grass)
		{
			areas[area].grasses[grass].time = 0uL;
			PlayerPrefs.SetString(eSAVE.SAILO_GRASS_TIME.ToString() + "_" + area + "_" + grass + suf, "0");
		}

		public void AddGrassStock(int grass)
		{
			grass_stock += grass;
			PlayerPrefs.SetInt(eSAVE.SAILO_GRASS_STOCK.ToString() + suf, grass_stock);
		}

		public void UpdateSairo(ulong time)
		{
			sailo_update_time = time;
			PlayerPrefs.SetString(eSAVE.SAILO_UPDATE_TIME.ToString() + suf, sailo_update_time.ToString());
		}

		public void AddGrassArea(int area)
		{
			level++;
			PlayerPrefs.SetInt(eSAVE.SAILO_LEVEL.ToString() + suf, level);
		}
	}

	[Serializable]
	public class FacilityData
	{
		public Facility.eState state;

		public Facility.eType type;

		public long[] timer = new long[3];

		public FarmAnimal.eType[] farm_animals = new FarmAnimal.eType[3];

		public int[] give_grass = new int[2];

		public Facility.eItem tree;

		public Facility.TreeInfo.ePlant plant;

		public int enabled;
	}

	[Serializable]
	public class ConstructPosition
	{
		public bool current;

		public bool flipX;

		public Vector2 position;

		public int order_in_layer = 200;

		public bool land;

		public ConstructPosition(bool flip, Vector2 pos, bool land, int oil = 200)
		{
			flipX = flip;
			position = pos;
			order_in_layer = oil;
			this.land = land;
		}
	}

	[Serializable]
	public class WildAnimalData
	{
		[Serializable]
		public class Area
		{
			public WildAnimal.eType type = WildAnimal.eType.NONE;

			public ulong time;
		}

		[Serializable]
		public class Visit
		{
			public WildAnimal.eCATEGORY category;

			public List<Area> areas = new List<Area>();

			public Visit(WildAnimal.eCATEGORY _category)
			{
				category = _category;
			}
		}

		public static readonly int[] tVISIT_MAX = new int[5]
		{
			4,
			2,
			2,
			2,
			2
		};

		public ulong[] visit_time = new ulong[5];

		public Visit[] visit = new Visit[5];

		public void Init()
		{
			ulong num = (ulong)(DateTime.Now.Ticks - 2900000000u);
			for (int i = 0; i < visit.Length; i++)
			{
				visit_time[i] = ulong.Parse(PlayerPrefs.GetString(eSAVE.WILD_ANIMAL_VISIT_TIME.ToString() + "_" + i + suf, num.ToString()));
				visit[i] = new Visit((WildAnimal.eCATEGORY)i);
				for (int j = 0; j < tVISIT_MAX[i]; j++)
				{
					Area area = new Area();
					area.type = (WildAnimal.eType)PlayerPrefs.GetInt(eSAVE.WILD_ANIMAL_TYPE.ToString() + "_" + i + "_" + j + suf, -1);
					area.time = ulong.Parse(PlayerPrefs.GetString(eSAVE.WILD_ANIMAL_TIME.ToString() + "_" + i + "_" + j + suf, "0"));
					visit[i].areas.Add(area);
				}
			}
		}

		public void Visiting(WildAnimal.eCATEGORY category, int area, WildAnimal.eType type, ulong time)
		{
			visit[(int)category].areas[area].type = type;
			visit[(int)category].areas[area].time = time;
			PlayerPrefs.SetInt(eSAVE.WILD_ANIMAL_TYPE.ToString() + "_" + (int)category + "_" + area + suf, (int)type);
			PlayerPrefs.SetString(eSAVE.WILD_ANIMAL_TIME.ToString() + "_" + (int)category + "_" + area + suf, time.ToString());
		}

		public void SetVisitTime(ulong time, WildAnimal.eCATEGORY category)
		{
			visit_time[(int)category] = time;
			PlayerPrefs.SetString(eSAVE.WILD_ANIMAL_VISIT_TIME.ToString() + "_" + (int)category + suf, time.ToString());
		}
	}

	[Serializable]
	public class SugorokuData
	{
		public enum eDir
		{
			NONE = -1,
			UP,
			DOWN,
			RIGHT,
			LEFT,
			MAX
		}

		public const int HISTORY_MAX = 7;

		public const int DICE_MAX = 3;

		public int map_id;

		public int farm_mass;

		public int coin_mass;

		public int exp_mass;

		public int current;

		public int move_count;

		public int[] history = new int[7];

		public int dice_value;

		public int dice_remind_count;

		public ulong restore_time;

		public eDir car_dir;

		public int mass_event;

		public void Init()
		{
			dice_remind_count = PlayerPrefs.GetInt(eSAVE.SUGOROKU_DICE_REMIND_COUNT.ToString(), 3);
			dice_value = PlayerPrefs.GetInt(eSAVE.SUGOROKU_DICE_VALUE.ToString(), 0);
			current = PlayerPrefs.GetInt(eSAVE.SUGOROKU_CURRENT.ToString(), -1);
			move_count = PlayerPrefs.GetInt(eSAVE.SUGOROKU_MOVE_COUNT.ToString(), 0);
			for (int i = 0; i < 7; i++)
			{
				history[i] = PlayerPrefs.GetInt(eSAVE.SUGOROKU_HISTORY.ToString() + i, -1);
			}
			car_dir = (eDir)PlayerPrefs.GetInt(eSAVE.SUGOROKU_CAR_DIR.ToString(), 3);
			restore_time = ulong.Parse(PlayerPrefs.GetString(eSAVE.SUGOROKU_TIMER.ToString(), "0"));
			map_id = PlayerPrefs.GetInt(eSAVE.SUGOROKU_MAP.ToString(), 0);
			farm_mass = PlayerPrefs.GetInt(eSAVE.SUGOROKU_FARM.ToString(), -1);
			coin_mass = PlayerPrefs.GetInt(eSAVE.SUGOROKU_COIN.ToString(), -1);
			exp_mass = PlayerPrefs.GetInt(eSAVE.SUGOROKU_EXP.ToString(), -1);
			mass_event = PlayerPrefs.GetInt(eSAVE.SUGOROKU_EVENT.ToString(), 0);
		}

		public void SetDiceValue(int dice_value)
		{
			this.dice_value = dice_value;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_DICE_VALUE.ToString(), dice_value);
		}

		public void SetDiceRemindCount(int dice_remind_count)
		{
			this.dice_remind_count = dice_remind_count;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_DICE_REMIND_COUNT.ToString(), dice_remind_count);
		}

		public void SetMoveCount(int move_count)
		{
			this.move_count = move_count;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_MOVE_COUNT.ToString(), move_count);
		}

		public void SetHistry(int id, int prev_mass)
		{
			history[id] = prev_mass;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_HISTORY.ToString() + id, prev_mass);
		}

		public void SetCurrent(int mass)
		{
			current = mass;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_CURRENT.ToString(), mass);
		}

		public void SetCarDir(eDir dir)
		{
			car_dir = dir;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_CAR_DIR.ToString(), (int)dir);
		}

		public void SetRestoreTime(ulong time)
		{
			restore_time = time;
			PlayerPrefs.SetString(eSAVE.SUGOROKU_TIMER.ToString(), time.ToString());
		}

		public void SetFarmMass(int mass)
		{
			farm_mass = mass;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_FARM.ToString(), mass);
		}

		public void SetCoinMass(int mass)
		{
			coin_mass = mass;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_COIN.ToString(), mass);
		}

		public void SetExpMass(int mass)
		{
			exp_mass = mass;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_EXP.ToString(), mass);
		}

		public void SetMassEvent(int onoff)
		{
			mass_event = onoff;
			PlayerPrefs.SetInt(eSAVE.SUGOROKU_EVENT.ToString(), onoff);
		}

		public void Reset()
		{
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_DICE_VALUE.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_MOVE_COUNT.ToString());
			for (int i = 0; i < 7; i++)
			{
				PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_HISTORY.ToString() + i);
			}
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_DICE_REMIND_COUNT.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_CURRENT.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_TIMER.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_FARM.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_COIN.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_EXP.ToString());
			PlayerPrefs.DeleteKey(eSAVE.SUGOROKU_EVENT.ToString());
		}
	}

	[Serializable]
	public class WorkerData
	{
		public const int WORKER_MAX = 8;

		public const int WORKER_LEVEL_MAX = 4;

		public const int SPECIAL_MAX = 5;

		public int[] worker_level = new int[13];

		public int[] worker_release = new int[13];

		public int[] worker_place = new int[13];

		public void Init()
		{
			for (int i = 0; i < worker_level.Length; i++)
			{
				worker_level[i] = PlayerPrefs.GetInt(eSAVE.WORKER_LEVEL.ToString() + i + suf, 0);
				worker_place[i] = PlayerPrefs.GetInt(eSAVE.WORKER_PLACE.ToString() + i + suf, -1);
				worker_release[i] = PlayerPrefs.GetInt(eSAVE.WORKER_RELEASE.ToString() + i + suf, -1);
			}
		}

		public void SetWorkerLvUP(TouchEvent touch)
		{
			worker_level[touch.param.value1]++;
			if (worker_level[touch.param.value1] > 5)
			{
				worker_level[touch.param.value1] = 4;
			}
			PlayerPrefs.SetInt(eSAVE.WORKER_LEVEL.ToString() + touch.param.value1 + suf, worker_level[touch.param.value1]);
		}

		public void SetWorkerPlace(int i, Worker.eWorkPlace place)
		{
			worker_place[i] = (int)place;
			PlayerPrefs.SetInt(eSAVE.WORKER_PLACE.ToString() + i + suf, worker_place[i]);
		}

		public void SetWorkerRelease(TouchEvent touch, int count)
		{
			worker_release[touch.param.value1] = count;
			PlayerPrefs.SetInt(eSAVE.WORKER_RELEASE.ToString() + touch.param.value1 + suf, worker_release[touch.param.value1]);
		}

		public static int SpecialMax(eFarmType type)
		{
			return 5 + ((type != 0) ? (-1) : 0);
		}

		public bool IsExist()
		{
			bool flag = false;
			for (int i = 0; i < 8 + SpecialMax(farm_type); i++)
			{
				if (flag)
				{
					break;
				}
				flag = (worker_level[i] > 0);
			}
			return flag;
		}
	}

	[NamedArray(new string[]
	{
		"Ads",
		"Donation_1",
		"Donation_2",
		"Donation_3",
		"Worker(Farm)",
		"Worker(Resort)",
		"Store(Farm)",
		"Store(Resort)"
	})]
	public int[] purchase;

	public const int LEVEL_MAX = 99;

	public const int CHARACTER_LEVEL_MAX = 99;

	public const long BALANCE_MAX = 99999999L;

	public const int ROOM_MAX = 4;

	public const int LAYER_UP = 10000;

	public const float ALUBUM_RIGHT_MAX = 0.85f;

	public const int FACILITY_MAX = 11;

	public const int INTERSTITIAL_COUNT_LIMIT = 8;

	public const int YES = 1;

	public const int NO = 0;

	public const int ON = 1;

	public const int OFF = 0;

	public const int IGNORE = -1;

	public int opening;

	public int ending;

	public int resort_event;

	public static eFarmType farm_type;

	public eMainType main_type;

	public int first_coin;

	public static string suf = string.Empty;

	public const string DEV_PLAYER_NAME_DEFAULT = "dev";

	public eLang lang;

	public int interstitial_count;

	public bool interstitial_ON;

	public int review_flag;

	public int chk_special_worker;

	public int game_time;

	public int level_condition_count;

	public bool[] room_unlock = new bool[4];

	public bool[] room_reservation = new bool[4];

	public float[] room_reminder_time = new float[4];

	public int level;

	public int coin;

	public int exp;

	private Manager manager;

	public CharacterData[] character_data = new CharacterData[4];

	public StoreData store_data;

	public HotelData hotel_data;

	public SailoData sailo_data;

	public FacilityData[] facility_data = new FacilityData[11];

	public int tree_plant;

	public WildAnimalData wild_animal_data;

	public SugorokuData sugoroku_data;

	public WorkerData worker_data;

	private static readonly int[] tID_TO_INDEX = new int[4]
	{
		-1,
		0,
		1,
		2
	};

	private static readonly Purchaser.eTYPE[] tINDEX_TO_ID = new Purchaser.eTYPE[3]
	{
		Purchaser.eTYPE.DONATION_1,
		Purchaser.eTYPE.DONATION_2,
		Purchaser.eTYPE.DONATION_3
	};

	public bool get_granpa_coin;

	private int[] coin_init_value = new int[2]
	{
		2000,
		2000
	};

	private int[] off_on_init = new int[2]
	{
		0,
		1
	};

	public static readonly int[] hotel_init_level = new int[2]
	{
		0,
		1
	};

	public static readonly int[] tFARM_LEVEL = new int[100]
	{
		0,
		50,
		85,
		100,
		150,
		200,
		300,
		350,
		400,
		450,
		550,
		650,
		750,
		850,
		950,
		1100,
		1250,
		1400,
		1550,
		1600,
		1800,
		2000,
		2200,
		2400,
		2600,
		2900,
		3200,
		3500,
		3800,
		4100,
		4500,
		5000,
		5500,
		6000,
		6500,
		7000,
		7500,
		8000,
		8500,
		9000,
		10000,
		11000,
		12000,
		13000,
		14000,
		15000,
		16000,
		17000,
		18000,
		19000,
		20000,
		21000,
		22000,
		23000,
		24000,
		25000,
		26000,
		27000,
		28000,
		29000,
		30000,
		31000,
		32000,
		33000,
		34000,
		35000,
		36000,
		37000,
		38000,
		39000,
		40000,
		41000,
		42000,
		43000,
		44000,
		45000,
		46000,
		47000,
		48000,
		49000,
		50000,
		51000,
		52000,
		53000,
		54000,
		55000,
		56000,
		57000,
		58000,
		59000,
		60000,
		61000,
		62000,
		63000,
		64000,
		65000,
		66000,
		67000,
		68000,
		69000
	};

	public static readonly int[,] tCHARACTER_LEVEL = new int[4, 99]
	{
		{
			2,
			2,
			3,
			5,
			5,
			10,
			10,
			10,
			10,
			10,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99
		},
		{
			2,
			2,
			3,
			5,
			5,
			10,
			10,
			10,
			10,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99
		},
		{
			2,
			2,
			3,
			5,
			5,
			10,
			10,
			10,
			10,
			10,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			20,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99
		},
		{
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2,
			2
		}
	};

	public static readonly float[,] SetIconX = new float[6, 6]
	{
		{
			0f,
			0f,
			0f,
			0f,
			0f,
			0f
		},
		{
			-0.07f,
			0.07f,
			0f,
			0f,
			0f,
			0f
		},
		{
			-0.14f,
			0f,
			0.14f,
			0f,
			0f,
			0f
		},
		{
			-0.21f,
			-0.07f,
			0.07f,
			0.21f,
			0f,
			0f
		},
		{
			-0.28f,
			-0.14f,
			0f,
			0.14f,
			0.28f,
			0f
		},
		{
			-0.35f,
			-0.21f,
			-0.07f,
			0.07f,
			0.21f,
			0.35f
		}
	};

	public void SetTreePlant(int yesno)
	{
		tree_plant = yesno;
		PlayerPrefs.SetInt(eSAVE.TREE_PLANT.ToString() + suf, yesno);
	}

	public Facility.eState SetFacilitySate(int f_id, Facility.eState state)
	{
		facility_data[f_id].state = state;
		PlayerPrefs.SetInt(eSAVE.FACILITY_STATE.ToString() + f_id + suf, (int)state);
		return state;
	}

	public Facility.eType SetFacilityType(int f_id, Facility.eType type)
	{
		facility_data[f_id].type = type;
		PlayerPrefs.SetInt(eSAVE.FACILITY_TYPE.ToString() + f_id + suf, (int)type);
		return type;
	}

	public long SetFacilityTime(int f_id, int timer_index, long timeout)
	{
		facility_data[f_id].timer[timer_index] = timeout;
		PlayerPrefs.SetString(eSAVE.FACILITY_TIME.ToString() + timer_index + f_id + suf, timeout.ToString());
		return timeout;
	}

	public FarmAnimal.eType SetFacilityFarmAnimal(int f_id, int animal_index, FarmAnimal.eType animal)
	{
		facility_data[f_id].farm_animals[animal_index] = animal;
		PlayerPrefs.SetInt(eSAVE.FACILITY_FARM_ANIMAL.ToString() + animal_index + f_id + suf, (int)animal);
		return animal;
	}

	public int SetFacilityGiveGrass(int f_id, int index, int give_grass)
	{
		facility_data[f_id].give_grass[index] = give_grass;
		PlayerPrefs.SetInt(eSAVE.FACILITY_GIVE_GRASS.ToString() + index + f_id + suf, give_grass);
		return give_grass;
	}

	public Facility.eItem SetFacilityTree(int f_id, Facility.eItem tree)
	{
		facility_data[f_id].tree = tree;
		PlayerPrefs.SetInt(eSAVE.FACILITY_TREE.ToString() + f_id + suf, (int)tree);
		return tree;
	}

	public Facility.TreeInfo.ePlant SetFacilityTreePlant(int f_id, Facility.TreeInfo.ePlant plant)
	{
		facility_data[f_id].plant = plant;
		PlayerPrefs.SetInt(eSAVE.FACILITY_TREE_PLANT.ToString() + f_id + suf, (int)plant);
		return plant;
	}

	public void EnableFacility(int f_id)
	{
		facility_data[f_id].enabled = 1;
		PlayerPrefs.SetInt(eSAVE.FACILITY_ENABLE.ToString() + f_id + suf, 1);
	}

	public void Init(Manager m, bool delete_all)
	{
		manager = m;
		if (delete_all)
		{
			PlayerPrefs.DeleteAll();
		}
		farm_type = (eFarmType)PlayerPrefs.GetInt(eSAVE.FARM_TYPE.ToString(), 0);
		suf = SetSuffix(farm_type);
		opening = PlayerPrefs.GetInt(eSAVE.OPENING.ToString(), 0);
		if (opening == 0 && !HasSavedPlayerName() && !HasAnyPersistedProgress())
		{
			Reset();
		}
		ending = PlayerPrefs.GetInt(eSAVE.ENDING.ToString(), 0);
		review_flag = PlayerPrefs.GetInt(eSAVE.REVIEW.ToString(), 0);
		chk_special_worker = PlayerPrefs.GetInt(eSAVE.CHK_SPECIAL_WORKER.ToString(), 0);
		resort_event = PlayerPrefs.GetInt(eSAVE.RESORT_EVENT.ToString(), 0);
		lang = ((Application.systemLanguage != SystemLanguage.Japanese) ? eLang.EN : eLang.JP);
		int[] array = new int[4]
		{
			36,
			26,
			20,
			17
		};
		for (int i = 0; i < 4; i++)
		{
			character_data[i] = new CharacterData();
			CharacterData.Contents contents;
			for (int j = 0; j < array[i]; character_data[i].contents.Add(contents), j++)
			{
				contents = new CharacterData.Contents();
				contents.reg = PlayerPrefs.GetInt(eSAVE.CHARACTER_REG.ToString() + i + string.Empty + j, 0);
				contents.level = PlayerPrefs.GetInt(eSAVE.CHARACTER_LEVEL.ToString() + i + string.Empty + j, 1);
				contents.new_reg = PlayerPrefs.GetInt(eSAVE.CHARACTER_NEW_REG.ToString() + i + string.Empty + j, 0);
				contents.level_condition_count = PlayerPrefs.GetInt(eSAVE.LEVEL_CONDITION_COUNT.ToString() + i + string.Empty + j, 0);
				switch (i)
				{
				case 0:
				case 2:
					contents.watched_video = PlayerPrefs.GetInt(eSAVE.CHARACTER_WATCHED_VIDEO.ToString() + i + string.Empty + j, 1);
					continue;
				case 1:
					if (j == 0 || j == 3)
					{
						contents.watched_video = PlayerPrefs.GetInt(eSAVE.CHARACTER_WATCHED_VIDEO.ToString() + i + string.Empty + j, 1);
						continue;
					}
					break;
				}
				if (i == 3 && j < 2)
				{
					contents.watched_video = PlayerPrefs.GetInt(eSAVE.CHARACTER_WATCHED_VIDEO.ToString() + i + string.Empty + j, 1);
				}
				else
				{
					contents.watched_video = PlayerPrefs.GetInt(eSAVE.CHARACTER_WATCHED_VIDEO.ToString() + i + string.Empty + j, 0);
				}
			}
		}
		for (int k = 0; k < 4; k++)
		{
			for (int l = 0; l < 2; l++)
			{
				if (GetAlbumDisp(k, l) != 0)
				{
					continue;
				}
				if (l == 0)
				{
					int charaDataLength = Convert.GetCharaDataLength((CharacterData.eType)k, eFarmType.NORMAL);
					for (int n = 0; n < charaDataLength; n++)
					{
						if (character_data[k].contents[n].reg == 1)
						{
							SetAlbumDisp(1, k, eFarmType.NORMAL);
							break;
						}
					}
				}
				else
				{
					SetAlbumDisp(1, k, (eFarmType)l);
				}
			}
		}
		main_type = (eMainType)PlayerPrefs.GetInt(eSAVE.MAIN_TYPE.ToString(), 0);
		first_coin = PlayerPrefs.GetInt(eSAVE.FIRST_COIN.ToString(), 0);
		level = PlayerPrefs.GetInt(eSAVE.LEVEL.ToString() + suf, 1);
		exp = PlayerPrefs.GetInt(eSAVE.EXP.ToString() + suf, 0);
		coin = PlayerPrefs.GetInt(eSAVE.COIN.ToString() + suf, coin_init_value[(int)farm_type]);
        AudioListener.volume = PlayerPrefs.GetInt(eSAVE.VOLUME.ToString(), (int)AudioListener.volume);
		interstitial_count = PlayerPrefs.GetInt(eSAVE.INTERSTITIAL_COUNT.ToString(), 0);
		store_data = new StoreData();
		store_data.Init();
		hotel_data = new HotelData();
		hotel_data.Init();
		sailo_data = new SailoData();
		sailo_data.Init();
		wild_animal_data = new WildAnimalData();
		wild_animal_data.Init();
		sugoroku_data = new SugorokuData();
		sugoroku_data.Init();
		worker_data = new WorkerData();
		worker_data.Init();
		purchase = new int[6];
		for (int num = 0; num < 6; num++)
		{
			Purchaser.eTYPE eTYPE = (Purchaser.eTYPE)num;
			if (eTYPE <= Purchaser.eTYPE.DONATION_3)
			{
				purchase[num] = PlayerPrefs.GetInt(eSAVE.PURCHASE.ToString() + num, 0);
			}
			else
			{
				purchase[num] = PlayerPrefs.GetInt(eSAVE.PURCHASE.ToString() + "_" + eTYPE.ToString(), 0);
			}
		}
		tree_plant = PlayerPrefs.GetInt(eSAVE.TREE_PLANT.ToString() + suf, off_on_init[(int)farm_type]);
		for (int num2 = 0; num2 < 11; num2++)
		{
			facility_data[num2] = new FacilityData();
			facility_data[num2].state = (Facility.eState)PlayerPrefs.GetInt(eSAVE.FACILITY_STATE.ToString() + num2 + suf, (num2 == 10 && farm_type == eFarmType.RESORT) ? (-1) : 0);
			facility_data[num2].type = (Facility.eType)PlayerPrefs.GetInt(eSAVE.FACILITY_TYPE.ToString() + num2 + suf, -1);
			for (int num3 = 0; num3 < 3; num3++)
			{
				long.TryParse(PlayerPrefs.GetString(eSAVE.FACILITY_TIME.ToString() + num3 + num2 + suf, "0"), out facility_data[num2].timer[num3]);
			}
			for (int num4 = 0; num4 < 3; num4++)
			{
				facility_data[num2].farm_animals[num4] = (FarmAnimal.eType)PlayerPrefs.GetInt(eSAVE.FACILITY_FARM_ANIMAL.ToString() + num4 + num2 + suf, -1);
			}
			for (int num5 = 0; num5 < 2; num5++)
			{
				facility_data[num2].give_grass[num5] = PlayerPrefs.GetInt(eSAVE.FACILITY_GIVE_GRASS.ToString() + num5 + num2 + suf, 0);
			}
			facility_data[num2].tree = (Facility.eItem)PlayerPrefs.GetInt(eSAVE.FACILITY_TREE.ToString() + num2 + suf, -1);
			facility_data[num2].plant = (Facility.TreeInfo.ePlant)PlayerPrefs.GetInt(eSAVE.FACILITY_TREE_PLANT.ToString() + num2 + suf, -1);
			facility_data[num2].enabled = PlayerPrefs.GetInt(eSAVE.FACILITY_ENABLE.ToString() + num2 + suf, facility_init_enabled(num2));
		}
		get_granpa_coin = false;
		if (opening == 0 && ShouldSkipOpeningFlow())
		{
			SetOpening();
		}
	}

	private string SetSuffix(eFarmType ftype)
	{
		switch (ftype)
		{
		case eFarmType.NORMAL:
			return string.Empty;
		case eFarmType.RESORT:
			return "_1";
		default:
			return "error";
		}
	}

	/// <summary>
	/// 是否已有可辨識的存檔進度（用於避免誤觸新手選角／開場）。
	/// </summary>
	private static bool HasAnyPersistedProgress()
	{
		if (PlayerPrefs.GetInt(eSAVE.FIRST_COIN.ToString(), 0) != 0)
		{
			return true;
		}
		if (PlayerPrefs.GetInt(eSAVE.ENDING.ToString(), 0) != 0)
		{
			return true;
		}
		if (PlayerPrefs.GetInt(eSAVE.RESORT_EVENT.ToString(), 0) != 0)
		{
			return true;
		}
		foreach (string farmSuf in new string[2]
		{
			string.Empty,
			"_1"
		})
		{
			string levelKey = eSAVE.LEVEL.ToString() + farmSuf;
			if (PlayerPrefs.HasKey(levelKey) && PlayerPrefs.GetInt(levelKey, 1) > 1)
			{
				return true;
			}
			string expKey = eSAVE.EXP.ToString() + farmSuf;
			if (PlayerPrefs.HasKey(expKey) && PlayerPrefs.GetInt(expKey, 0) > 0)
			{
				return true;
			}
			string coinKey = eSAVE.COIN.ToString() + farmSuf;
			if (PlayerPrefs.HasKey(coinKey) && PlayerPrefs.GetInt(coinKey, 2000) != 2000)
			{
				return true;
			}
			for (int i = 0; i < 11; i++)
			{
				string typeKey = eSAVE.FACILITY_TYPE.ToString() + i + farmSuf;
				if (PlayerPrefs.HasKey(typeKey) && PlayerPrefs.GetInt(typeKey, -1) != -1)
				{
					return true;
				}
			}
		}
		int[] charaCounts = new int[4]
		{
			36,
			26,
			20,
			17
		};
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < charaCounts[i]; j++)
			{
				if (PlayerPrefs.GetInt(eSAVE.CHARACTER_REG.ToString() + i + j, 0) == 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool HasExistingGameProgress()
	{
		if (first_coin != 0 || ending != 0 || resort_event != 0)
		{
			return true;
		}
		if (level > 1 || exp > 0)
		{
			return true;
		}
		if (PlayerPrefs.HasKey(eSAVE.COIN.ToString() + suf) && coin != coin_init_value[(int)farm_type])
		{
			return true;
		}
		for (int i = 0; i < 11; i++)
		{
			if (facility_data[i].type != (Facility.eType)(-1))
			{
				return true;
			}
			int defaultState = (i == 10 && farm_type == eFarmType.RESORT) ? (-1) : 0;
			if ((int)facility_data[i].state != defaultState)
			{
				return true;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < character_data[i].contents.Count; j++)
			{
				if (character_data[i].contents[j].reg == 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool ShouldSkipOpeningFlow()
	{
		return HasSavedPlayerName() || HasExistingGameProgress();
	}

	public void SetOpening()
	{
		opening = 1;
		PlayerPrefs.SetInt(eSAVE.OPENING.ToString(), 1);
	}

	public void SetEnding()
	{
		ending = 1;
		PlayerPrefs.SetInt(eSAVE.ENDING.ToString(), 1);
	}

	public void SetReview()
	{
		review_flag = 1;
		PlayerPrefs.SetInt(eSAVE.REVIEW.ToString(), 1);
	}

	public bool ChkSpecialWorker()
	{
		return chk_special_worker == 1;
	}

	public void SetSpecialWorker()
	{
		chk_special_worker = 1;
		PlayerPrefs.SetInt(eSAVE.CHK_SPECIAL_WORKER.ToString(), 1);
	}

	public void SetResortEvent()
	{
		resort_event = 1;
		PlayerPrefs.SetInt(eSAVE.RESORT_EVENT.ToString(), 1);
	}

	public void SetMainType(eMainType type)
	{
		main_type = type;
		PlayerPrefs.SetInt(eSAVE.MAIN_TYPE.ToString(), (int)type);
	}

	public void SetFarmType(eFarmType type)
	{
		farm_type = type;
		PlayerPrefs.SetInt(eSAVE.FARM_TYPE.ToString(), (int)type);
	}

	public void SetFirstCoin()
	{
		first_coin = 1;
		PlayerPrefs.SetInt(eSAVE.FIRST_COIN.ToString(), 1);
	}

	public void SetCoinCount(int _coin)
	{
		coin = _coin;
		PlayerPrefs.SetInt(eSAVE.COIN.ToString() + suf, coin);
	}

	public void SetExpCount(int _exp)
	{
		exp = _exp;
		if (exp > tFARM_LEVEL[level])
		{
			exp = tFARM_LEVEL[level];
		}
		PlayerPrefs.SetInt(eSAVE.EXP.ToString() + suf, exp);
	}

	public void SetLevelUp()
	{
		level++;
		PlayerPrefs.SetInt(eSAVE.LEVEL.ToString() + suf, level);
	}

	public int GetReg(CharacterData.eType type, int id)
	{
		return character_data[(int)type].contents[id].reg;
	}

	public void SetReg(CharacterData.eType type, int id)
	{
		if (GetAlbumDisp((int)type, (int)farm_type) == 0 && farm_type == eFarmType.NORMAL)
		{
			SetAlbumDisp(-1, (int)type, farm_type);
		}
		character_data[(int)type].contents[id].reg = 1;
		PlayerPrefs.SetInt(eSAVE.CHARACTER_REG.ToString() + (int)type + string.Empty + id, 1);
		SetNewRegFlag((int)type, id, 1);
	}

	public void SetNewRegFlag(int type, int id, int flag)
	{
		character_data[type].contents[id].new_reg = flag;
		PlayerPrefs.SetInt(eSAVE.CHARACTER_NEW_REG.ToString() + type + id, flag);
	}

	public bool AddLevelCondCount(CharacterData.eType type, int id)
	{
		CharacterData.Contents contents = character_data[(int)type].contents[id];
		bool result = false;
		int num = contents.level;
		if (num < 99)
		{
			contents.level_condition_count++;
			if (contents.level_condition_count >= tCHARACTER_LEVEL[(int)type, num - 1])
			{
				contents.level_condition_count = 0;
				contents.level++;
				result = true;
				PlayerPrefs.SetInt(eSAVE.CHARACTER_LEVEL.ToString() + (int)type + string.Empty + id, contents.level);
			}
			PlayerPrefs.SetInt(eSAVE.LEVEL_CONDITION_COUNT.ToString() + (int)type + string.Empty + id, contents.level_condition_count);
		}
		return result;
	}

	public void SetVolume(int onoff)
	{
		AudioListener.volume = onoff;
		PlayerPrefs.SetInt(eSAVE.VOLUME.ToString(), (int)AudioListener.volume);
	}

	public void GameTimeReset()
	{
		game_time = 0;
	}

	public int GetSystemDiffTime()
	{
		long num = long.Parse(PlayerPrefs.GetString(eSAVE.PREV_SYSTEM_TIME.ToString(), (DateTime.Now.Ticks / 10000000).ToString()));
		long num2 = DateTime.Now.Ticks / 10000000;
		return (int)(num2 - num);
	}

	public static int DonationIndex(Purchaser.eTYPE type)
	{
		return tID_TO_INDEX[(int)type];
	}

	public static Purchaser.eTYPE DonationId(int index)
	{
		return tINDEX_TO_ID[index];
	}

	public int Purchase(Purchaser.eTYPE type)
	{
		return purchase[(int)type];
	}

	public void SetPurchase(Purchaser.eTYPE type, int onoff)
	{
		purchase[(int)type] = onoff;
		if (type <= Purchaser.eTYPE.DONATION_3)
		{
			PlayerPrefs.SetInt(eSAVE.PURCHASE.ToString() + (int)type, onoff);
		}
		else
		{
			PlayerPrefs.SetInt(eSAVE.PURCHASE.ToString() + "_" + type.ToString(), onoff);
		}
		PlayerPrefs.Save();
	}

	public void InterstitialCount()
	{
		interstitial_count++;
		PlayerPrefs.SetInt(eSAVE.INTERSTITIAL_COUNT.ToString(), interstitial_count);
	}

	public void InterstitialCountReset()
	{
		interstitial_count = 0;
		PlayerPrefs.SetInt(eSAVE.INTERSTITIAL_COUNT.ToString(), interstitial_count);
	}

	public void SetWatchedVideo(int yesno, int type = 0, int id = 0)
	{
		character_data[type].contents[id].watched_video = yesno;
		PlayerPrefs.SetInt(eSAVE.CHARACTER_WATCHED_VIDEO.ToString() + type + string.Empty + id, yesno);
	}

	public void SetAlbumDisp(int yesno, int type, eFarmType farmtype)
	{
		character_data[type].album = yesno;
		PlayerPrefs.SetInt(eSAVE.ALBUM_DISP.ToString() + type + SetSuffix(farmtype), yesno);
	}

	public int GetAlbumDisp(int type, int album_mode)
	{
		int result = 0;
		switch (album_mode)
		{
		case 0:
			result = PlayerPrefs.GetInt(eSAVE.ALBUM_DISP.ToString() + type);
			break;
		case 1:
			result = PlayerPrefs.GetInt(eSAVE.ALBUM_DISP.ToString() + type + "_1");
			break;
		}
		return result;
	}

	public static bool IsDevelopmentBuild()
	{
		return Debug.isDebugBuild;
	}

	public static bool HasSavedPlayerName()
	{
		return PlayerPrefs.HasKey(eSAVE.DEV_PLAYER_NAME.ToString());
	}

	public static string GetSavedPlayerName()
	{
		return PlayerPrefs.GetString(eSAVE.DEV_PLAYER_NAME.ToString(), DEV_PLAYER_NAME_DEFAULT);
	}

	public static string GetSavedDevPlayerName()
	{
		return GetSavedPlayerName();
	}

	public static string NormalizeDevPlayerName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return DEV_PLAYER_NAME_DEFAULT;
		}
		return name.Trim();
	}

	public static string GenerateRandomPlayerName()
	{
		const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		char[] id = new char[8];
		for (int i = 0; i < id.Length; i++)
		{
			id[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
		}
		return "p_" + new string(id);
	}

	/// <summary>
	/// 開發版用輸入名稱；發佈版取得或建立隨機名稱。
	/// </summary>
	public static string ResolvePlayerNameForConfirm(string inputName)
	{
		if (IsDevelopmentBuild())
		{
			return NormalizeDevPlayerName(inputName);
		}
		if (HasSavedPlayerName())
		{
			return GetSavedPlayerName();
		}
		return GenerateRandomPlayerName();
	}

	/// <summary>
	/// 確認角色名：與已存名稱相同則保留進度；不同則清除存檔並以新名稱重新載入。
	/// </summary>
	/// <returns>是否已清除並重新載入進度</returns>
	public static bool ConfirmPlayerName(string inputName, Manager manager)
	{
		string name = ResolvePlayerNameForConfirm(inputName);
		string saved = HasSavedPlayerName() ? GetSavedPlayerName() : string.Empty;
		if (name == saved)
		{
			return false;
		}
		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetString(eSAVE.DEV_PLAYER_NAME.ToString(), name);
		PlayerPrefs.Save();
		manager.ReloadProgressData();
		return true;
	}

	public static bool ConfirmDevPlayerName(string inputName, Manager manager)
	{
		return ConfirmPlayerName(inputName, manager);
	}

	public static void ClearAllSaveData()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public void Reset()
	{
		PlayerPrefs.DeleteAll();
	}

	public void Save()
	{
		PlayerPrefs.Save();
	}

	public static int facility_init_enabled(int id)
	{
		if (id < 2)
		{
			return 1;
		}
		if (farm_type == eFarmType.RESORT && id == 10)
		{
			return -1;
		}
		return 0;
	}

	public static int on_off_init(CharacterData.eType type, int i)
	{
		if (type == CharacterData.eType.FARMANIMAL && i < 18)
		{
			return 1;
		}
		if (type == CharacterData.eType.FISH && i < 10)
		{
			return 1;
		}
		return 0;
	}
}
