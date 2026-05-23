public class Price
{
	public static Data data;

	private static readonly int[] tOPEN_ROOM_LEVEL = new int[4]
	{
		4,
		10,
		20,
		30
	};

	private static readonly int[] tOPEN_ROOM_PRICE = new int[4]
	{
		500,
		3000,
		30000,
		60000
	};

	private static readonly int[] tOPEN_TABLE_LEVEL = new int[4]
	{
		1,
		9,
		16,
		25
	};

	private static readonly int[] tOPEN_TABLE_PRICE = new int[4]
	{
		0,
		2000,
		12000,
		30000
	};

	private static readonly int[] tOPEN_FARM_ANIMAL_PRICE = new int[36]
	{
		50,
		50,
		50,
		100,
		50,
		100,
		100,
		200,
		200,
		200,
		200,
		200,
		300,
		200,
		200,
		300,
		500,
		1000,
		100,
		200,
		100,
		100,
		100,
		100,
		100,
		100,
		100,
		100,
		200,
		300,
		100,
		100,
		500,
		1000,
		1500,
		100
	};

	private static readonly int[] tOPEN_SAILO_LEVEL = new int[4]
	{
		1,
		8,
		15,
		25
	};

	private static readonly int[] tOPEN_SAILO_PRICE = new int[4]
	{
		0,
		3000,
		15000,
		40000
	};

	private static readonly int[] tOPEN_FACILITY_PRICE = new int[22]
	{
		100,
		100,
		100,
		100,
		200,
		200,
		200,
		200,
		300,
		300,
		300,
		100,
		100,
		100,
		100,
		300,
		500,
		300,
		300,
		300,
		500,
		500
	};

	private static readonly int[] tOPEN_FACILITY_BUY_UNLOCK_LEVEL = new int[22]
	{
		1,
		2,
		4,
		6,
		10,
		12,
		14,
		16,
		20,
		25,
		30,
		1,
		1,
		2,
		2,
		10,
		20,
		15,
		21,
		28,
		22,
		32
	};

	private static readonly int[] tOPEN_FACILITY_ITEM_PRICE = new int[6]
	{
		200,
		1000,
		3000,
		200,
		1000,
		3000
	};

	private static readonly int[] tOPEN_FACILITY_ITEM_BUY_UNLOCK_LEVEL = new int[6]
	{
		3,
		15,
		30,
		3,
		15,
		30
	};

	private static readonly int[] tOPEN_FACILITY_UNLOCK_LEVEL = new int[11]
	{
		1,
		1,
		4,
		4,
		8,
		8,
		16,
		16,
		25,
		25,
		30
	};

	private static readonly int[] tOPEN_FACILITY_UNLOCK_PRICE = new int[11]
	{
		0,
		0,
		1000,
		1000,
		5000,
		5000,
		10000,
		10000,
		40000,
		40000,
		100000
	};

	private static readonly int[] tOPEN_WORKER_PRICE = new int[13]
	{
		10,
		30,
		50,
		200,
		500,
		1000,
		5000,
		10000,
		10000,
		10000,
		10000,
		10000,
		10000
	};

	private const int tOPEN_WORKER_LVUP_PRICE_BASE = 40;

	private const int total_level_boundary = 24;

	private const int total_level_boundary_2 = 40;

	private const int WORKER_MENU_LEVEL = 2;

	private const int RESORT_MENU_LEVEL = 20;

	private const int FARM_BASE_COIN = 5;

	private const int FISH_BASE_COIN = 4;

	private const int CUSTOMER_BASE_COIN = 50;

	public const int GRASS_CUT_COIN = 1;

	public const int CUSTOMER_TIP_COIN = 1;

	public const int EGG_COIN = 50;

	public const int SUGOROKU_COIN_BAG = 20;

	public const int SUGOROKU_COIN_FARM = 30;

	public const int GRASS_CUT_EXP = 1;

	public const int WILD_ANIMAL_EXP = 2;

	public const int LODGING_EXP = 1;

	public const int ADD_FACILITY_EXP = 10;

	public const int ADD_ANIMAL_EXP = 2;

	public const int HARVEST_EXP = 2;

	public const int SILO_GRASS_CUT = 1;

	public const int SUGOROKU_EXP_BAG = 20;

	public const int SUGOROKU_EXP_FARM = 30;

	public static void Init(Data _data)
	{
		data = _data;
	}

	public static int OpenRoomLevel(int room_id)
	{
		return tOPEN_ROOM_LEVEL[room_id];
	}

	public static int OpenRoomPrice(int room_id)
	{
		return tOPEN_ROOM_PRICE[room_id];
	}

	public static int OpenTableLevel(int table_id)
	{
		return tOPEN_TABLE_LEVEL[table_id];
	}

	public static int OpenTablePrice(int table_id)
	{
		return tOPEN_TABLE_PRICE[table_id];
	}

	public static int OpenFarmAnimalLevel(FarmAnimal.eType type)
	{
		for (int i = 0; i < FarmAnimal.GetConditionLength(); i++)
		{
			Data.Condition conditions = FarmAnimal.GetConditions(type, i);
			if (conditions.category == Data.Condition.eCATEGORY.LEVEL)
			{
				return conditions.type;
			}
		}
		return 0;
	}

	public static int OpenFarmAnimalPrice(FarmAnimal.eType type)
	{
		int level = data.character_data[0].contents[(int)type].level;
		return tOPEN_FARM_ANIMAL_PRICE[(int)type] + level - 1;
	}

	public static int OpenSailoLevel(int sailo_id)
	{
		return tOPEN_SAILO_LEVEL[sailo_id];
	}

	public static int OpenSailoPrice(int sailo_id)
	{
		return tOPEN_SAILO_PRICE[sailo_id];
	}

	public static int OpenFacilityPrice(Facility.eType type)
	{
		return tOPEN_FACILITY_PRICE[(int)type];
	}

	public static int OpenFacilityBuyUnlockLevel(Facility.eType type)
	{
		return tOPEN_FACILITY_BUY_UNLOCK_LEVEL[(int)type];
	}

	public static int OpenFacilityItemPrice(Facility.eItem item)
	{
		return tOPEN_FACILITY_ITEM_PRICE[(int)item];
	}

	public static int OpenFacilityItemBuyUnlockLevel(Facility.eItem item)
	{
		return tOPEN_FACILITY_ITEM_BUY_UNLOCK_LEVEL[(int)item];
	}

	public static int OpenFacilityUnlockLevel(int faility_id)
	{
		return tOPEN_FACILITY_UNLOCK_LEVEL[faility_id];
	}

	public static int OpenFacilityUnlockPrice(int faility_id)
	{
		return tOPEN_FACILITY_UNLOCK_PRICE[faility_id];
	}

	public static int OpenWorkerPrice(int count)
	{
		return tOPEN_WORKER_PRICE[count];
	}

	public static int LvUpWorkerPrice()
	{
		int num = 0;
		for (int i = 0; i < data.worker_data.worker_level.Length; i++)
		{
			num += data.worker_data.worker_level[i];
		}
		if (num <= 24)
		{
			return 40 * num;
		}
		if (num <= 40)
		{
			return 1000 * (num - 24);
		}
		return 3000 * (num - 24);
	}

	public static int OpenWorkerMenu()
	{
		return 2;
	}

	public static int OpenFarmMenu()
	{
		return 20;
	}

	public static int HarvestPrice(FarmAnimal.eType type)
	{
		int level = data.character_data[0].contents[(int)type].level;
		return 5 + level - 1 + (int)((float)level * 0.3f);
	}

	public static int HarvestPrice(Fish.eType type)
	{
		int level = data.character_data[2].contents[(int)type].level;
		return 4 + level - 1 + (int)((float)level * 0.3f);
	}

	public static int CustomerPrice(Customer.eType type)
	{
		int level = data.character_data[3].contents[(int)type].level;
		return 50 + (level - 1) * 2;
	}

	public static int OpenAnimalReleasePrice(FarmAnimal.eType type)
	{
		return (int)((float)OpenFarmAnimalPrice(type) * 0.8f);
	}

	public static int CoinGrassCut()
	{
		return Utils.Round(data.level, 2);
	}

	public static int CoinCustomerTip(Customer.eType type)
	{
		return data.character_data[3].contents[(int)type].level / 3 + 1;
	}

	public static int CoinEgg()
	{
		return 50 * (data.level / 3 + 1);
	}

	public static int CoinInterstitial()
	{
		return data.level * 5;
	}

	public static int CoinSugorokuBag()
	{
		return 20 * Utils.Round(data.level, 2);
	}

	public static int CoinSugorokuFarm()
	{
		return 30 * Utils.Round(data.level, 2);
	}

	public static int ExpGrassCut()
	{
		return data.level / 5 + 1;
	}

	public static int ExpWildAnimalTip(WildAnimal.eType type)
	{
		return data.character_data[1].contents[(int)type].level * 2 / 20 + 1;
	}

	public static int ExpWildAnimalHeart(WildAnimal.eType type)
	{
		return data.character_data[1].contents[(int)type].level * 2 / 20 + 1;
	}

	public static int ExpLodging(Customer.eType type)
	{
		return data.character_data[3].contents[(int)type].level / 3 + 1;
	}

	public static int ExpAddFacility(Facility.eType type)
	{
		return 10 * (data.level / 3 + 1);
	}

	public static int ExpAddFacility(Facility.eItem item)
	{
		return 10 * (data.level / 5 + 1);
	}

	public static int ExpAddAnimal(FarmAnimal.eType type)
	{
		return data.character_data[0].contents[(int)type].level * 2 + 1;
	}

	public static int ExpHarvest(FarmAnimal.eType type)
	{
		return data.character_data[0].contents[(int)type].level * 2 / 5 + 1;
	}

	public static int ExpHarvest(Fish.eType type)
	{
		return data.character_data[2].contents[(int)type].level * 2 / 5 + 1;
	}

	public static int ExpSiloGrass()
	{
		return data.level / 4 + 1;
	}

	public static int ExpEgg()
	{
		return (int)((float)Data.tFARM_LEVEL[data.level] * 0.3f) + 1;
	}

	public static int ExpSugorokuBag()
	{
		return 20 * Utils.Round(data.level, 2);
	}

	public static int ExpSugorokuFarm()
	{
		return 30 * Utils.Round(data.level, 2);
	}
}
