using UnityEngine;

public class Convert
{
	private static Manager manager;

	public static int[] WorkerTypeToIndex = new int[25]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11,
		12,
		8,
		9,
		10,
		11
	};

	public static void Init()
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
	}

	public static int FarmAnimalInitValue(int ftype)
	{
		if (ftype != 0)
		{
			int num = 0;
			for (int i = 0; i < ftype; i++)
			{
				num += FarmAnimalLength(i);
			}
			return num;
		}
		return 0;
	}

	public static int WildAnimalInitValue(int ftype)
	{
		if (ftype != 0)
		{
			int num = 0;
			for (int i = 0; i < ftype; i++)
			{
				num += WildAnimalLength(i);
			}
			return num;
		}
		return 0;
	}

	public static int FishInitValue(int ftype)
	{
		if (ftype != 0)
		{
			int num = 0;
			for (int i = 0; i < ftype; i++)
			{
				num += FishLength(i);
			}
			return num;
		}
		return 0;
	}

	public static int CustomerInitValue(int ftype)
	{
		if (ftype != 0)
		{
			int num = 0;
			for (int i = 0; i < ftype; i++)
			{
				num += CustomerLength(i);
			}
			return num;
		}
		return 0;
	}

	public static int FarmAnimalLength(int ftype)
	{
		int num = 0;
		for (int i = 0; i < FarmAnimal.tFARMANIMAL_ORDER.GetLength(1); i++)
		{
			if (FarmAnimal.tFARMANIMAL_ORDER[ftype, i] != FarmAnimal.eType.NONE)
			{
				num++;
			}
		}
		return num;
	}

	public static int WildAnimalLength(int ftype)
	{
		int num = 0;
		for (int i = 0; i < WildAnimal.tWILDANIMAL_ORDER.GetLength(1); i++)
		{
			if (WildAnimal.tWILDANIMAL_ORDER[ftype, i] != WildAnimal.eType.NONE)
			{
				num++;
			}
		}
		return num;
	}

	public static int FishLength(int ftype)
	{
		int num = 0;
		for (int i = 0; i < Fish.tFISH_ORDER.GetLength(1); i++)
		{
			if (Fish.tFISH_ORDER[ftype, i] != Fish.eType.NONE)
			{
				num++;
			}
		}
		return num;
	}

	public static int CustomerLength(int ftype)
	{
		int num = 0;
		for (int i = 0; i < Customer.tCUSTOMER_ORDER.GetLength(1); i++)
		{
			if (Customer.tCUSTOMER_ORDER[ftype, i] != Customer.eType.NONE)
			{
				num++;
			}
		}
		return num;
	}

	public static int GetCharaDataLength(Data.CharacterData.eType ctype, Data.eFarmType ftype)
	{
		int result = 0;
		switch (ctype)
		{
		case Data.CharacterData.eType.FARMANIMAL:
			result = FarmAnimalLength((int)ftype);
			break;
		case Data.CharacterData.eType.WILDANIMAL:
			result = WildAnimalLength((int)ftype);
			break;
		case Data.CharacterData.eType.FISH:
			result = FishLength((int)ftype);
			break;
		case Data.CharacterData.eType.CUSTOMER:
			result = CustomerLength((int)ftype);
			break;
		}
		return result;
	}

	public static int FacilityLength(Data.eFarmType ftype)
	{
		switch (ftype)
		{
		case Data.eFarmType.NORMAL:
			return 11;
		case Data.eFarmType.RESORT:
			return 11;
		default:
			return -1;
		}
	}

	public static int FacilityTypePlus(Data.eFarmType ftype, int i)
	{
		switch (ftype)
		{
		case Data.eFarmType.NORMAL:
			return i;
		case Data.eFarmType.RESORT:
			return i + 11;
		default:
			return -1;
		}
	}

	public static Worker.eType ToWorkerType(int worker_index)
	{
		if (worker_index < 7)
		{
			return (Worker.eType)(worker_index + 8 * (int)Data.farm_type);
		}
		worker_index -= 8;
		return (Worker.eType)(((Data.farm_type != 0) ? 21 : 16) + worker_index);
	}
}
