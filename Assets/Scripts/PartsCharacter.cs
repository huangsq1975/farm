using System;
using UnityEngine;

public class PartsCharacter : MonoBehaviour
{
	public enum eSTATE
	{
		STAY,
		WALK,
		MAX
	}

	public enum eDIR
	{
		DOWN,
		SIDE,
		UP,
		MAX
	}

	public enum ePARTS
	{
		HAIR,
		FACE,
		TOPS,
		BOTTOMS,
		SHADOW,
		MAX
	}

	[Serializable]
	public class ClothesSet
	{
		[Serializable]
		public class Clothes
		{
			[Serializable]
			public class Set
			{
				public static readonly int[] MAX = new int[2]
				{
					2,
					4
				};

				public Sprite[] tops;

				public Sprite[] bottoms;

				public Set(eSTATE state)
				{
					tops = new Sprite[MAX[(int)state]];
					bottoms = new Sprite[MAX[(int)state]];
				}
			}

			public Set[] set = new Set[2];
		}

		public Clothes[] clothes = new Clothes[3];
	}

	public eSTATE state;

	public eDIR dir;

	public bool flipX;

	protected SpriteRenderer[] parts = new SpriteRenderer[5];

	public Sprite[] hair_side_sprite = new Sprite[1];

	public Sprite[] hair_up_sprite = new Sprite[1];

	public Sprite[] face_side_sprite = new Sprite[1];

	public Sprite[] face_up_sprite = new Sprite[1];

	public Sprite shadow_sprite;

	public Sprite shadow_side_sprite;

	[HideInInspector]
	public ClothesSet[] clothes_set = new ClothesSet[1];

	public int hair_no;

	public int face_no;

	public int tops_no;

	public int bottoms_no;

	public float elapsed_time;

	public const float LIMIT = 0.25f;

	protected int frame;

	private static readonly float[,,] tSTAY_POS_Y = new float[5, 3, 2]
	{
		{
			{
				0f,
				0f
			},
			{
				0.113f,
				0.103f
			},
			{
				0.103f,
				0.093f
			}
		},
		{
			{
				0f,
				0f
			},
			{
				0.113f,
				0.103f
			},
			{
				0.103f,
				0.093f
			}
		},
		{
			{
				0f,
				0f
			},
			{
				0.043f,
				0.033f
			},
			{
				0.043f,
				0.033f
			}
		},
		{
			{
				0f,
				0f
			},
			{
				0.003f,
				0.003f
			},
			{
				0.003f,
				0.003f
			}
		},
		{
			{
				0f,
				0f
			},
			{
				0.03f,
				0.03f
			},
			{
				0.02f,
				0.02f
			}
		}
	};

	private static readonly float[,,] tWALK_POS_Y = new float[5, 3, 4]
	{
		{
			{
				0f,
				0f,
				0f,
				0f
			},
			{
				0.113f,
				0.103f,
				0.113f,
				0.103f
			},
			{
				0f,
				0f,
				0f,
				0f
			}
		},
		{
			{
				0f,
				0f,
				0f,
				0f
			},
			{
				0.113f,
				0.103f,
				0.113f,
				0.103f
			},
			{
				0f,
				0f,
				0f,
				0f
			}
		},
		{
			{
				0f,
				0f,
				0f,
				0f
			},
			{
				0.043f,
				0.033f,
				0.043f,
				0.033f
			},
			{
				0f,
				0f,
				0f,
				0f
			}
		},
		{
			{
				0f,
				0f,
				0f,
				0f
			},
			{
				0.003f,
				0.003f,
				0.003f,
				0.003f
			},
			{
				0f,
				0f,
				0f,
				0f
			}
		},
		{
			{
				0f,
				0f,
				0f,
				0f
			},
			{
				0.03f,
				0.03f,
				0.03f,
				0.03f
			},
			{
				0f,
				0f,
				0f,
				0f
			}
		}
	};

	public void SetState(eSTATE new_state, eDIR new_dir, bool _flipX)
	{
		if (state != new_state || dir != new_dir || flipX != _flipX)
		{
			state = new_state;
			dir = new_dir;
			flipX = _flipX;
			if (flipX)
			{
				Transform transform = base.transform;
				Vector3 localScale = base.transform.localScale;
				float x = 0f - Mathf.Abs(localScale.x);
				Vector3 localScale2 = base.transform.localScale;
				transform.localScale = new Vector2(x, localScale2.y);
			}
			else
			{
				Transform transform2 = base.transform;
				Vector3 localScale3 = base.transform.localScale;
				float x2 = Mathf.Abs(localScale3.x);
				Vector3 localScale4 = base.transform.localScale;
				transform2.localScale = new Vector2(x2, localScale4.y);
			}
			frame = 0;
		}
	}

	protected void UpdateSprite()
	{
		elapsed_time += Time.deltaTime;
		if (elapsed_time >= 0.25f)
		{
			UpdateSpriteImmediate();
		}
	}

	protected void UpdateSpriteImmediate()
	{
		elapsed_time = 0f;
		SetSprite(state, dir, frame);
		frame++;
		if (frame >= GetFrameMax(state, dir))
		{
			frame = 0;
		}
	}

	public void SetOrderInLayer(int base_order_in_layer)
	{
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i].sortingOrder += base_order_in_layer + 1;
		}
	}

	private int GetFrameMax(eSTATE state, eDIR dir)
	{
		return clothes_set[tops_no].clothes[(int)dir].set[(int)state].tops.Length;
	}

	protected void SetSprite(eSTATE state, eDIR dir, int index)
	{
		switch (dir)
		{
		case eDIR.SIDE:
			parts[0].sprite = hair_side_sprite[hair_no];
			parts[1].sprite = face_side_sprite[face_no];
			break;
		case eDIR.UP:
			parts[0].sprite = hair_up_sprite[hair_no];
			parts[1].sprite = face_up_sprite[face_no];
			break;
		case eDIR.DOWN:
			parts[0].sprite = hair_up_sprite[hair_no];
			parts[1].sprite = face_up_sprite[face_no];
			break;
		}
		parts[2].sprite = clothes_set[tops_no].clothes[(int)dir].set[(int)state].tops[index];
		parts[3].sprite = clothes_set[bottoms_no].clothes[(int)dir].set[(int)state].bottoms[index];
		parts[4].sprite = shadow_side_sprite;
		SetPosition(state, dir, index);
	}

	private void SetPosition(eSTATE state, eDIR dir, int index)
	{
		for (int i = 0; i < parts.Length; i++)
		{
			Transform transform = parts[i].transform;
			Transform transform2 = transform;
			Vector3 localPosition = transform.localPosition;
			transform2.localPosition = new Vector2(localPosition.x, GetPosY((ePARTS)i, dir, index));
		}
	}

	private float GetPosY(ePARTS parts, eDIR dir, int index)
	{
		if (state == eSTATE.STAY)
		{
			return tSTAY_POS_Y[(int)parts, (int)dir, index];
		}
		if (state == eSTATE.WALK)
		{
			return tWALK_POS_Y[(int)parts, (int)dir, index];
		}
		return 0f;
	}
}
