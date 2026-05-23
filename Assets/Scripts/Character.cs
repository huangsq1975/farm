using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum eMove
	{
		WAIT,
		ACTIVE,
		STOP,
		EVENT
	}

	public enum eState
	{
		ENTRY,
		EXIT,
		FIELD,
		FACILITY,
		PREEXIT
	}

	public enum eDIRECTION
	{
		UP,
		DOWN,
		RIGHT,
		LEFT
	}

	public enum eMOTIONTYPE
	{
		_STAY_,
		_WALK_
	}

	[Serializable]
	public class CharRenderers
	{
		public enum eType
		{
			SPRITE,
			SHADOW,
			HAT,
			MAX
		}

		public List<SpriteRenderer> sprite;

		private List<int> base_order;

		public Transform trans;

		public bool isActive;

		public CharRenderers()
		{
			isActive = false;
		}

		public CharRenderers(Transform self)
		{
			isActive = true;
			trans = self;
			sprite = new List<SpriteRenderer>();
			base_order = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				Transform transform = trans;
				eType eType = (eType)i;
				Transform transform2 = transform.Find(eType.ToString().ToLower());
				if (null != transform2)
				{
					SpriteRenderer component = transform2.gameObject.GetComponent<SpriteRenderer>();
					sprite.Add(component);
					base_order.Add(component.sortingOrder);
				}
			}
		}

		public void SetFlipX(bool flip)
		{
			foreach (SpriteRenderer item in sprite)
			{
				item.flipX = flip;
			}
		}

		public void SetFlipY(bool flip)
		{
			foreach (SpriteRenderer item in sprite)
			{
				item.flipY = flip;
			}
		}

		public void SetSortingOrder(int order)
		{
			sprite[0].sortingOrder = order;
			sprite[1].sortingOrder = 1;
			if (sprite.Count == 3)
			{
				sprite[2].sortingOrder = order + 1;
			}
		}

		public void SetSortingOrderAll(int order)
		{
			sprite[0].sortingOrder = order;
			sprite[1].sortingOrder = order - 1;
			if (sprite.Count == 3)
			{
				sprite[2].sortingOrder = order + 1;
			}
		}

		public void SetSelect(bool select)
		{
			SpriteRenderer spriteRenderer = sprite[0];
			Color color = sprite[0].color;
			float r = color.r;
			Color color2 = sprite[0].color;
			float g = color2.g;
			Color color3 = sprite[0].color;
			spriteRenderer.color = new Color(r, g, color3.b, (!select) ? 1f : 0.7f);
			SpriteRenderer spriteRenderer2 = sprite[1];
			Color color4 = sprite[1].color;
			float r2 = color4.r;
			Color color5 = sprite[1].color;
			float g2 = color5.g;
			Color color6 = sprite[1].color;
			spriteRenderer2.color = new Color(r2, g2, color6.b, (!select) ? 0.5882353f : 0f);
			if (sprite.Count == 3)
			{
				SpriteRenderer spriteRenderer3 = sprite[2];
				Color color7 = sprite[2].color;
				float r3 = color7.r;
				Color color8 = sprite[2].color;
				float g3 = color8.g;
				Color color9 = sprite[2].color;
				spriteRenderer3.color = new Color(r3, g3, color9.b, (!select) ? 1f : 0.7f);
			}
		}
	}

	public eMove move;

	public const int SORTING_ORDER_CHARACTER = 40;

	public const float SPEED_NORMAL = 2f;

	protected const float SPEED_RETURN = 0.5f;

	protected const int DEST_NON = -1;

	protected const int DEST_BASE = 500;

	protected const int DEST_VISIT_AREA = 501;

	protected const int VISIT_STAY_COUNT = 3;

	protected List<string> submotion = new List<string>
	{
		"_up",
		"_down",
		"_side",
		"_side"
	};

	[SerializeField]
	protected Map map;

	[SerializeField]
	protected eState state;

	[SerializeField]
	protected int prev_index;

	[SerializeField]
	protected int current_index;

	[SerializeField]
	protected int visit_index;

	[SerializeField]
	protected int stay_count;

	[SerializeField]
	protected int destination = -1;

	[SerializeField]
	protected eDIRECTION direction;

	[SerializeField]
	protected eMOTIONTYPE motion;

	[SerializeField]
	protected Animator animator;

	public PartsController controller;

	protected Manager manager;

	public CharRenderers renderers;

	public virtual void Open(Manager m, int idx, Vector2 pos, eState sts, int type, int id)
	{
		manager = m;
		map = manager.map;
		state = sts;
		direction = eDIRECTION.DOWN;
		destination = -1;
		current_index = idx;
		renderers = new CharRenderers();
		SetPos(pos);
		Initialize(type, id);
		move = eMove.WAIT;
		SetSortingOrder();
	}

	public virtual void Initialize(int type, int id)
	{
	}

	public virtual void SetSortingOrder()
	{
		int num = 40 + current_index * 10;
		if (renderers.isActive)
		{
			renderers.SetSortingOrder(num + 9);
		}
		else
		{
			controller.SetSortingOrder(num);
		}
	}

	public virtual void SetSortingOrder(int order)
	{
		if (renderers.isActive)
		{
			renderers.SetSortingOrder(order + 9);
		}
		else
		{
			controller.SetSortingOrder(order);
		}
	}

	public void SetPos(int idx, Vector2 pos)
	{
		destination = -1;
		current_index = idx;
		base.transform.localPosition = pos;
		SetSortingOrder();
	}

	public void SetPos(Vector2 pos)
	{
		base.transform.localPosition = pos;
	}

	public virtual void SetMotionAndDirection(Map.eType place)
	{
		List<int> list = new List<int>();
		list.Add(-map.COLM_MAX[(int)place]);
		list.Add(map.COLM_MAX[(int)place]);
		list.Add(1);
		list.Add(-1);
		List<int> list2 = list;
		if (prev_index == current_index)
		{
			motion = eMOTIONTYPE._STAY_;
			return;
		}
		motion = eMOTIONTYPE._WALK_;
		direction = (eDIRECTION)list2.IndexOf(current_index - prev_index);
	}

	public virtual void SetFlipX(bool flipX)
	{
		if (controller != null)
		{
			controller.SetFlipX(flipX);
			return;
		}
		if (renderers.isActive)
		{
			renderers.SetFlipX(flipX);
			return;
		}
		Vector3 localScale = base.transform.localScale;
		float x = localScale.x;
		x = ((!flipX) ? ((!(x > 0f)) ? (0f - x) : x) : ((!(x > 0f)) ? x : (0f - x)));
		Transform transform = base.transform;
		float x2 = x;
		Vector3 localScale2 = base.transform.localScale;
		float y = localScale2.y;
		Vector3 localScale3 = base.transform.localScale;
		transform.localScale = new Vector3(x2, y, localScale3.z);
	}

	public virtual Vector2 RandomMoveInField(Map.eType place)
	{
		current_index = map.GetFreeField(current_index, place);
		SetMotionAndDirection(place);
		return map.GetFieldPos(current_index, place);
	}

	public virtual Vector2 ForcedMoveInField(Map.eType place)
	{
		current_index = map.GetNextField(current_index, destination, place);
		SetMotionAndDirection(place);
		return map.GetFieldPos(current_index, place);
	}

	public virtual Vector2 StayInVisitArea()
	{
		Vector2 result;
		if (stay_count < 3)
		{
			result = base.transform.position;
			motion = eMOTIONTYPE._STAY_;
			direction = eDIRECTION.UP;
		}
		else
		{
			result = map.GetFieldPos(map.visit_area_list[visit_index].exit_square, Map.eType.FIELD);
			map.ReleaseVisitArea(visit_index);
			destination = -1;
			motion = eMOTIONTYPE._WALK_;
			direction = ((visit_index % 2 != 0) ? eDIRECTION.LEFT : eDIRECTION.RIGHT);
		}
		return result;
	}

	public virtual Vector2 ArriveDestinationInField()
	{
		destination = 501;
		motion = eMOTIONTYPE._WALK_;
		direction = ((visit_index % 2 != 0) ? eDIRECTION.RIGHT : eDIRECTION.LEFT);
		return map.visit_area_list[visit_index].pos;
	}

	public virtual void NextTurn()
	{
		prev_index = current_index;
		move = eMove.ACTIVE;
		Vector2 move_pos = (destination == -1) ? RandomMoveInField(Map.eType.FIELD) : ((destination == current_index) ? ArriveDestinationInField() : ((destination != 501) ? ForcedMoveInField(Map.eType.FIELD) : StayInVisitArea()));
		DoMove(move_pos, Map.eType.FIELD);
	}

	public virtual void PlayTurnAnimation()
	{
	}

	public virtual void ReturnBase()
	{
		state = eState.PREEXIT;
	}

	public virtual void DoMove(Vector2 move_pos, Map.eType place)
	{
		SetFlipX((direction == eDIRECTION.RIGHT) ? true : false);
		PlayTurnAnimation();
		base.transform.DOKill();
		base.transform.DOLocalMove(move_pos, (state != eState.EXIT) ? 2f : 0.5f).SetEase(Ease.Linear).OnComplete(OnMoveComplete);
	}

	public virtual void OnMoveComplete()
	{
		move = eMove.WAIT;
		SetSortingOrder();
		if (state == eState.EXIT)
		{
			NextTurn();
		}
		else
		{
			map.TurnEndNotify();
		}
	}

	public void SetDestination(int t)
	{
		destination = t;
	}

	public void SetPause(bool pause)
	{
		if (pause)
		{
			base.transform.DOPause();
			Utils.Stop(animator);
		}
		else
		{
			base.transform.DOPlay();
			Utils.Restart(animator);
		}
	}
}
