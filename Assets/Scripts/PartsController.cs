using System;
using System.Collections.Generic;
using UnityEngine;

public class PartsController : MonoBehaviour
{
	public enum eCharacter
	{
		MAIN,
		WORKER,
		CUSTOMER
	}

	public enum eCloth
	{
		TYPE_1,
		TYPE_2,
		TYPE_3,
		TYPE_4,
		TYPE_5,
		TYPE_6,
		TYPE_7,
		TYPE_8,
		TYPE_9,
		TYPE_10,
		TYPE_11,
		TYPE_12,
		TYPE_13,
		TYPE_14,
		TYPE_15,
		TYPE_16,
		TYPE_17,
		TYPE_18,
		TYPE_19,
		TYPE_20,
		TYPE_21,
		TYPE_22,
		TYPE_23,
		TYPE_24,
		TYPE_25,
		MAX
	}

	public enum eHat
	{
		NONE,
		TYPE_1,
		TYPE_2,
		TYPE_3,
		TYPE_4,
		TYPE_5,
		TYPE_6,
		TYPE_7,
		TYPE_8,
		TYPE_9,
		TYPE_10,
		TYPE_11,
		TYPE_12,
		TYPE_13,
		TYPE_14,
		TYPE_15
	}

	public enum eHair
	{
		TYPE_1,
		TYPE_2,
		TYPE_3,
		TYPE_4,
		TYPE_5,
		TYPE_6,
		TYPE_7,
		TYPE_8,
		TYPE_9,
		TYPE_10,
		TYPE_11,
		TYPE_12,
		TYPE_13,
		TYPE_14,
		TYPE_15,
		TYPE_16,
		TYPE_17,
		TYPE_18,
		TYPE_19,
		TYPE_20,
		TYPE_21,
		TYPE_22,
		TYPE_23,
		TYPE_24,
		TYPE_25,
		TYPE_26,
		TYPE_27,
		TYPE_28,
		TYPE_29,
		TYPE_30,
		TYPE_31,
		TYPE_32,
		TYPE_33,
		TYPE_34,
		TYPE_35,
		TYPE_36,
		TYPE_37,
		TYPE_38,
		TYPE_39,
		TYPE_40
	}

	public enum eFace
	{
		TYPE_1,
		TYPE_2,
		TYPE_3,
		TYPE_4
	}

	public enum eEye
	{
		TYPE_1,
		TYPE_2,
		TYPE_3,
		TYPE_4,
		TYPE_5,
		TYPE_6,
		TYPE_7,
		TYPE_8,
		TYPE_9,
		TYPE_10,
		TYPE_11,
		TYPE_12,
		TYPE_13,
		TYPE_14,
		TYPE_15,
		TYPE_16,
		TYPE_17,
		TYPE_18,
		TYPE_19,
		TYPE_20,
		TYPE_21,
		TYPE_22,
		TYPE_23
	}

	public class Style
	{
		public eCharacter character;

		public eCloth cloth;

		public eHat hat;

		public eHair hair;

		public eFace face;

		public eEye eye;

		public Style(eCharacter character, eCloth cloth, eHat hat, eHair hair, eFace face, eEye eye)
		{
			this.character = character;
			this.cloth = cloth;
			this.hat = hat;
			this.hair = hair;
			this.face = face;
			this.eye = eye;
		}
	}

	public enum ePartsItem
	{
		NONE,
		BAG_1,
		BAG_2,
		BINO,
		CUT,
		HUM_1,
		HUM_2,
		ROD_1,
		ROD_2,
		SWEAT,
		WOOD,
		FORK,
		KART,
		GOG_1,
		GOG_2,
		PAUL,
		MOP,
		KART_2
	}

	public enum ePartsCloth
	{
		NONE = 0,
		BODY_DOWN_1 = 1,
		BODY_DOWN_2 = 2,
		BODY_SIDE_1 = 3,
		BODY_SIDE_2 = 4,
		BODY_SIDE_3 = 5,
		BODY_SIDE_4 = 6,
		BODY_UP_1 = 7,
		BODY_UP_2 = 8,
		ARM_DOWN_1 = 9,
		ARM_DOWN_2 = 10,
		ARM_DOWN_3 = 11,
		ARM_DOWN_4 = 12,
		ARM_DOWN_5 = 13,
		ARM_DOWN_6 = 14,
		ARM_DOWN_7 = 0xF,
		ARM_SIDE_1 = 0x10,
		ARM_SIDE_2 = 17,
		ARM_SIDE_3 = 18,
		ARM_SIDE_4 = 19,
		ARM_SIDE_5 = 20,
		ARM_SIDE_6 = 21,
		ARM_SIDE_7 = 22,
		ARM_SIDE_8 = 23,
		ARM_SIDE_9 = 24,
		ARM_UP_1 = 25,
		ARM_UP_2 = 26,
		ARM_UP_3 = 27,
		ARM_UP_4 = 28,
		LEG_DOWN_1 = 29,
		LEG_DOWN_2 = 30,
		LEG_DOWN_3 = 0x1F,
		LEG_DOWN_4 = 0x20,
		LEG_SIDE_1 = 33,
		LEG_SIDE_2 = 34,
		LEG_SIDE_3 = 35,
		LEG_SIDE_4 = 36,
		LEG_SIDE_5 = 37,
		LEG_UP_1 = 38,
		LEG_UP_2 = 39,
		LEG_UP_3 = 40,
		MAX = 40
	}

	public enum ePartsHat
	{
		NONE = 0,
		DOWN_1 = 1,
		SIDE_1 = 2,
		UP_1 = 3,
		MAX = 3
	}

	public enum ePartsHair
	{
		NONE = 0,
		DOWN_1 = 1,
		SIDE_1 = 2,
		UP_1 = 3,
		MAX = 3
	}

	public enum ePartsFace
	{
		NONE = 0,
		DOWN_1 = 1,
		SIDE_1 = 2,
		MAX = 2
	}

	public enum eAnimType
	{
		_STAY_1_DOWN,
		_WALK_1_DOWN,
		_STAY_1_SIDE,
		_STAY_2_SIDE,
		_WALK_1_SIDE,
		_WALK_2_SIDE,
		_STAY_1_UP,
		_WALK_1_UP,
		_CUT_1_DOWN,
		_CUT_2_DOWN,
		_GET_1_UP,
		_GET_2_SIDE,
		_FISHING_1_SIDE,
		_CONST_1_SIDE,
		_CONST_2_SIDE,
		_SUPRISE_1_UP,
		_POSE_1_DOWN,
		_HUG_1_UP,
		_BINOCULARS_1_DOWN,
		MAX
	}

	[Serializable]
	public class Parts
	{
		public enum eType
		{
			HAT,
			HAIR,
			FACE,
			EYE,
			BODY,
			ARM_R,
			ARM_L,
			LEG_R,
			LEG_L,
			ITEM1,
			ITEM2,
			ITEM3,
			SHADOW,
			MAX
		}

		public static List<string> type2string = new List<string>
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			"BODY_",
			"ARM_",
			"ARM_",
			"LEG_",
			"LEG_",
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		private const int ITEM_MAX = 3;

		public SpriteRenderer[] rens = new SpriteRenderer[13];

		public int[] base_order = new int[13];

		public bool[] base_flipX = new bool[13];

		public eCharacter character;

		public eCloth cloth;

		public eHat hat;

		public eHair hair;

		public eFace face;

		public eEye eye;

		public eAnimType anim;

		public bool auto_set_item = true;

		public ePartsItem[] set_item = new ePartsItem[3];

		public Parts(Transform parent, eCharacter character)
		{
			this.character = character;
			auto_set_item = true;
			for (int i = 0; i < 13; i++)
			{
				SpriteRenderer[] array = rens;
				int num = i;
				eType eType = (eType)i;
				array[num] = parent.Find(eType.ToString().ToLower()).GetComponent<SpriteRenderer>();
				base_order[i] = rens[i].sortingOrder;
				base_flipX[i] = rens[i].flipX;
			}
			for (int j = 0; j < 3; j++)
			{
				set_item[j] = ePartsItem.NONE;
			}
		}

		public Parts(Transform parent, Style style)
			: this(parent, style.character)
		{
			SetStyle(style.cloth, style.hat, style.hair, style.face, style.eye);
		}

		public void SetStyle(eCloth cloth, eHat hat, eHair hair, eFace face, eEye eye)
		{
			SetCloth(cloth);
			SetHat(hat);
			SetHair(hair);
			this.face = face;
			this.eye = eye;
		}

		public void SetHat(eHat hat)
		{
			this.hat = hat;
		}

		public void SetHair(eHair hair)
		{
			this.hair = hair;
		}

		public void SetCloth(eCloth cloth)
		{
			this.cloth = cloth;
		}

		public Parts SetAnimation(eAnimType anim)
		{
			this.anim = anim;
			return this;
		}

		public void SetSortingOrder(int order)
		{
			for (int i = 0; i < 12; i++)
			{
				rens[i].sortingOrder = base_order[i] + order;
			}
			rens[12].sortingOrder = 1;
		}

		public void SetSortingOrderAll(int order)
		{
			for (int i = 0; i < 13; i++)
			{
				rens[i].sortingOrder = base_order[i] + order;
			}
		}

		public void ApplyBaseFlipX()
		{
			for (int i = 0; i < 13; i++)
			{
				if (null != rens[i])
				{
					rens[i].flipX = base_flipX[i];
				}
			}
		}

		public void SetFlipX(bool flipX)
		{
			for (int i = 0; i < 13; i++)
			{
				if (null != rens[i])
				{
					rens[i].flipX = (flipX != base_flipX[i]);
				}
			}
		}

		public void SetSprite(eType type, Sprite sprite)
		{
			rens[(int)type].sprite = sprite;
		}

		public void SetItem(eType type, ePartsItem item)
		{
			set_item[(int)(type - 9)] = item;
		}

		public ePartsItem GetItem(eType type)
		{
			return set_item[(int)(type - 9)];
		}

		public void SetColorAll(Color color)
		{
			for (int i = 0; i < 13; i++)
			{
				rens[i].color = color;
			}
		}
	}

	public static Color[] eye_color = new Color[23]
	{
		new Color(137f / 255f, 163f / 255f, 91f / 255f),
		new Color(14f / 85f, 184f / 255f, 59f / 85f),
		new Color(29f / 51f, 13f / 51f, 12f / 85f),
		new Color(0f, 88f / 255f, 137f / 255f),
		new Color(166f / 255f, 0.5882353f, 128f / 255f),
		new Color(0.294117659f, 146f / 255f, 166f / 255f),
		new Color(39f / 85f, 24f / 85f, 0.1764706f),
		new Color(46f / 85f, 25f / 51f, 106f / 255f),
		new Color(25f / 51f, 25f / 51f, 25f / 51f),
		new Color(0.235294119f, 23f / 85f, 59f / 255f),
		new Color(112f / 255f, 0.4f, 86f / 255f),
		new Color(23f / 51f, 0.5882353f, 0.3529412f),
		new Color(163f / 255f, 121f / 255f, 10f / 51f),
		new Color(0.470588237f, 22f / 51f, 19f / 51f),
		new Color(124f / 255f, 163f / 255f, 53f / 85f),
		new Color(133f / 255f, 63f / 85f, 128f / 255f),
		new Color(92f / 255f, 28f / 85f, 0.294117659f),
		new Color(48f / 85f, 41f / 85f, 29f / 85f),
		new Color(148f / 255f, 113f / 255f, 36f / 85f),
		new Color(112f / 255f, 26f / 51f, 24f / 85f),
		new Color(247f / 255f, 172f / 255f, 101f / 255f),
		new Color(178f / 255f, 29f / 51f, 12f / 85f),
		new Color(3.17647052f, 57f / 85f, 13f / 51f)
	};

	private const string NONE = "NONE";

	private const string DOWN_1 = "DOWN_1";

	private const string DOWN_2 = "DOWN_2";

	private const string DOWN_3 = "DOWN_3";

	private const string DOWN_4 = "DOWN_4";

	private const string DOWN_5 = "DOWN_5";

	private const string DOWN_6 = "DOWN_6";

	private const string DOWN_7 = "DOWN_7";

	private const string SIDE_1 = "SIDE_1";

	private const string SIDE_2 = "SIDE_2";

	private const string SIDE_3 = "SIDE_3";

	private const string SIDE_4 = "SIDE_4";

	private const string SIDE_5 = "SIDE_5";

	private const string SIDE_6 = "SIDE_6";

	private const string SIDE_7 = "SIDE_7";

	private const string SIDE_8 = "SIDE_8";

	private const string SIDE_9 = "SIDE_9";

	private const string UP_1 = "UP_1";

	private const string UP_2 = "UP_2";

	private const string UP_3 = "UP_3";

	private const string UP_4 = "UP_4";

	private static List<List<List<string>>> parts_list = new List<List<List<string>>>
	{
		new List<List<string>>
		{
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_2",
				"DOWN_3",
				"DOWN_2",
				"DOWN_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_3",
				"DOWN_2",
				"DOWN_1",
				"DOWN_2",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_2",
				"DOWN_3",
				"DOWN_2",
				"DOWN_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_2",
				"NONE",
				"SIDE_2",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_3",
				"NONE",
				"SIDE_3",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_2",
				"NONE",
				"SIDE_2",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_2",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_3",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_8",
				"NONE",
				"SIDE_2",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_1",
				"DOWN_1",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_2",
				"DOWN_3",
				"UP_2",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_1",
				"DOWN_1",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_3",
				"DOWN_2",
				"UP_1",
				"UP_2",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_1",
				"DOWN_1",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"DOWN_2",
				"DOWN_3",
				"UP_2",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_2",
				"DOWN_1",
				ePartsItem.CUT.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				ePartsItem.CUT.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_2",
				ePartsItem.CUT.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				ePartsItem.CUT.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_2",
				"DOWN_1",
				ePartsItem.CUT.ToString(),
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_2",
				"DOWN_6",
				"DOWN_6",
				"NONE",
				"NONE",
				ePartsItem.GOG_1.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_2",
				"DOWN_7",
				"DOWN_7",
				"NONE",
				"NONE",
				ePartsItem.GOG_1.ToString(),
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_2",
				"DOWN_6",
				"DOWN_6",
				"NONE",
				"NONE",
				ePartsItem.GOG_1.ToString(),
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"UP_2",
				"UP_2",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"UP_1",
				"UP_1",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"UP_2",
				"UP_2",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_1",
				"NONE",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_2",
				"SIDE_6",
				"NONE",
				"SIDE_5",
				"NONE",
				"NONE",
				ePartsItem.ROD_1.ToString(),
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_3",
				"SIDE_7",
				"NONE",
				"SIDE_5",
				"NONE",
				"NONE",
				ePartsItem.ROD_2.ToString(),
				"NONE"
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_2",
				"SIDE_6",
				"NONE",
				"SIDE_5",
				"NONE",
				"NONE",
				ePartsItem.ROD_1.ToString(),
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_2",
				"SIDE_4",
				"NONE",
				"SIDE_4",
				"NONE",
				"NONE",
				ePartsItem.HUM_1.ToString(),
				ePartsItem.WOOD.ToString()
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_2",
				"SIDE_5",
				"NONE",
				"SIDE_4",
				"NONE",
				"NONE",
				ePartsItem.HUM_2.ToString(),
				ePartsItem.WOOD.ToString()
			},
			new List<string>
			{
				"SIDE_1",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_2",
				"SIDE_4",
				"NONE",
				"SIDE_4",
				"NONE",
				"NONE",
				ePartsItem.HUM_1.ToString(),
				ePartsItem.WOOD.ToString()
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_4",
				"SIDE_9",
				"NONE",
				"NONE",
				"NONE",
				ePartsItem.GOG_2.ToString(),
				ePartsItem.HUM_1.ToString(),
				ePartsItem.PAUL.ToString()
			},
			new List<string>
			{
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_4",
				"SIDE_9",
				"NONE",
				"NONE",
				"NONE",
				ePartsItem.GOG_2.ToString(),
				ePartsItem.HUM_2.ToString(),
				ePartsItem.PAUL.ToString()
			},
			new List<string>
			{
				"NONE",
				"SIDE_1",
				"SIDE_1",
				"NONE",
				"SIDE_4",
				"SIDE_9",
				"NONE",
				"NONE",
				"NONE",
				ePartsItem.GOG_2.ToString(),
				ePartsItem.HUM_1.ToString(),
				ePartsItem.PAUL.ToString()
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_1",
				"UP_4",
				"UP_4",
				"UP_3",
				"UP_3",
				"NONE",
				ePartsItem.SWEAT.ToString(),
				ePartsItem.SWEAT.ToString()
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_4",
				"DOWN_4",
				"DOWN_3",
				"DOWN_3",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_5",
				"DOWN_5",
				"DOWN_4",
				"DOWN_4",
				"NONE",
				"NONE",
				"NONE"
			},
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_4",
				"DOWN_4",
				"DOWN_3",
				"DOWN_3",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"UP_1",
				"UP_1",
				"DOWN_1",
				"NONE",
				"UP_2",
				"UP_1",
				"UP_3",
				"UP_1",
				"UP_1",
				"NONE",
				"NONE",
				"NONE"
			}
		},
		new List<List<string>>
		{
			new List<string>
			{
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"NONE",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				"DOWN_1",
				ePartsItem.BINO.ToString(),
				"NONE",
				"NONE"
			}
		}
	};

	private Animator animator;

	public Parts parts;

	public static void PlayAnimation(Animator animator, Parts parts)
	{
		SetParts(0, parts);
		Utils.Play(animator, "human" + parts.anim.ToString().ToLower(), 1f);
	}

	public static void PlayAnimation(Animator animator, Parts parts, float speed)
	{
		SetParts(0, parts);
		Utils.Play(animator, "human" + parts.anim.ToString().ToLower(), speed);
	}

	public static void PlayAnimation(Animator animator, Parts parts, float speed, float normalized_time)
	{
		SetParts(0, parts);
		Utils.Play(animator, "human" + parts.anim.ToString().ToLower(), speed, normalized_time);
	}

	public static void SetParts(int frame, Parts parts)
	{
		for (int i = 0; i < 12; i++)
		{
			if (i == 3)
			{
				SetEye(parts, parts.eye);
			}
			else if (i == 0)
			{
				parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterHat(3 * (int)parts.hat + (int)Enum.Parse(typeof(ePartsHat), parts_list[(int)parts.anim][frame][i])));
			}
			else if (i == 1)
			{
				parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterHair(3 * (int)parts.hair + (int)Enum.Parse(typeof(ePartsHair), parts_list[(int)parts.anim][frame][i])));
			}
			else if (i == 2)
			{
				parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterFace(2 * (int)parts.face + (int)Enum.Parse(typeof(ePartsFace), parts_list[(int)parts.anim][frame][i])));
			}
			else if (i >= 9)
			{
				if (parts.GetItem((Parts.eType)i) == ePartsItem.NONE)
				{
					parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterItem((int)Enum.Parse(typeof(ePartsItem), parts_list[(int)parts.anim][frame][i])));
				}
				else
				{
					parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterItem((int)parts.GetItem((Parts.eType)i)));
				}
			}
			else
			{
				parts.SetSprite((Parts.eType)i, SpriteManager.GetCharacterCloth(parts.character, (!(parts_list[(int)parts.anim][frame][i] == "NONE")) ? (40 * (int)parts.cloth + (int)Enum.Parse(typeof(ePartsCloth), Parts.type2string[i] + parts_list[(int)parts.anim][frame][i])) : 0));
			}
		}
		ApplyHairHatSortOrder(parts, frame);
	}

	private static void ApplyHairHatSortOrder(Parts parts, int frame)
	{
		SpriteRenderer hat = parts.rens[(int)Parts.eType.HAT];
		SpriteRenderer hair = parts.rens[(int)Parts.eType.HAIR];
		if (null == hat || null == hair)
		{
			return;
		}
		if (parts_list[(int)parts.anim][frame][1] == "UP_1")
		{
			hair.sortingOrder = hat.sortingOrder + 1;
		}
		else
		{
			hair.sortingOrder = hat.sortingOrder - 1;
		}
	}

	private static void SetEye(Parts parts, eEye eye)
	{
		parts.rens[3].color = eye_color[(int)eye];
	}

	public void Init(eCharacter character, eCloth cloth, eHat hat, eHair hair, eFace face, eEye eye)
	{
		animator = GetComponent<Animator>();
		parts = new Parts(base.transform, character);
		parts.SetStyle(cloth, hat, hair, face, eye);
	}

	public void Init(Style style)
	{
		animator = GetComponent<Animator>();
		parts = new Parts(base.transform, style);
		SetStyle(style);
	}

	public void SetStyle(Style style)
	{
		parts.SetStyle(style.cloth, style.hat, style.hair, style.face, style.eye);
	}

	public void SetStyle(eCloth cloth, eHat hat, eHair hair, eFace face, eEye eye)
	{
		parts.SetStyle(cloth, hat, hair, face, eye);
	}

	public void Play(eAnimType type)
	{
		PlayAnimation(animator, parts.SetAnimation(type));
	}

	public void Play(eAnimType type, float speed)
	{
		PlayAnimation(animator, parts.SetAnimation(type), speed);
	}

	public void Play(eAnimType type, float speed, float normalized_time)
	{
		PlayAnimation(animator, parts.SetAnimation(type), speed, normalized_time);
	}

	public void Stop()
	{
		Utils.Stop(animator);
	}

	public bool IsPlaying()
	{
		return Utils.IsPlaying(animator);
	}

	public void SetSortingOrder(int order)
	{
		parts.SetSortingOrder(order);
	}

	public void SetSortingOrderAll(int order)
	{
		parts.SetSortingOrderAll(order);
	}

	public void SetFlipX(bool flipX)
	{
		if (parts != null)
		{
			parts.ApplyBaseFlipX();
		}
		Vector3 localScale = base.transform.localScale;
		float absX = Mathf.Abs(localScale.x);
		if (absX < 0.0001f)
		{
			absX = 1f;
		}
		base.transform.localScale = new Vector3(flipX ? (0f - absX) : absX, localScale.y, localScale.z);
	}

	public void SetHat(eHat type)
	{
		parts.SetHat(type);
	}

	public void SetItem(Parts.eType type, ePartsItem item)
	{
		parts.SetItem(type, item);
		parts.SetSprite(type, SpriteManager.GetCharacterItem((int)item));
	}

	public void SetColor(Color color)
	{
		parts.SetColorAll(color);
	}

	private void FrameEvent(int frame_no)
	{
		SetParts(frame_no, parts);
	}
}
