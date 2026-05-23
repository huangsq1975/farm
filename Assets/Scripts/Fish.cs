using UnityEngine;

public class Fish : MonoBehaviour
{
	public enum eType
	{
		NONE = -1,
		AYU_1,
		TROUT_1,
		RAINBOW_TROUT_1,
		SMELT_1,
		BUS_FISH_1,
		SALMON_1,
		CARP_1,
		EEL_1,
		ANGEL_FISH_1,
		AROWANA_1,
		CRAB_1,
		SHRIMP_1,
		OYSTER_1,
		SQUID_1,
		OCTOPUS_1,
		YELLOW_TANG_1,
		CONVICT_SURGEONFISH_1,
		CLAM_1,
		CORAL_1,
		CORAL_2,
		MAX
	}

	public const int NORMAL_FARM_MAX = 10;

	public const int RESORT_FARM_MAX = 10;

	public static readonly Data.Condition[,] tCONDITION = new Data.Condition[20, 3]
	{
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 6),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 8),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 12),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 3),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 40),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 0)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 1),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 2),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 5),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 10),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 15),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 20),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 25),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 30),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 35),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		},
		{
			new Data.Condition(Data.Condition.eCATEGORY.FACILITY, 14),
			new Data.Condition(Data.Condition.eCATEGORY.LEVEL, 40),
			new Data.Condition(Data.Condition.eCATEGORY.FARM_TYPE, 1)
		}
	};

	public static readonly eType[,] tFISH_ORDER = new eType[2, 10]
	{
		{
			eType.AYU_1,
			eType.TROUT_1,
			eType.RAINBOW_TROUT_1,
			eType.SMELT_1,
			eType.BUS_FISH_1,
			eType.SALMON_1,
			eType.CARP_1,
			eType.EEL_1,
			eType.ANGEL_FISH_1,
			eType.AROWANA_1
		},
		{
			eType.CRAB_1,
			eType.SHRIMP_1,
			eType.OYSTER_1,
			eType.SQUID_1,
			eType.OCTOPUS_1,
			eType.YELLOW_TANG_1,
			eType.CONVICT_SURGEONFISH_1,
			eType.CLAM_1,
			eType.CORAL_1,
			eType.CORAL_2
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
}
