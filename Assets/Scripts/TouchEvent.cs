using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchEvent : MonoBehaviour
{
	[Serializable]
	public class Param
	{
		public int value1;

		public int value2;

		public int value3;

		public GameObject value_obj1;

		public UnityAction call1;

		[HideInInspector]
		public Vector2 pos;
	}

	public Param param = new Param();

	public UnityEvent ClickDown;

	public UnityEvent ClickMove;

	public UnityEvent ClickUp;

	public UnityEvent Swipe;

	private BoxCollider2D col;

	private void SetCollider()
	{
		if (col == null)
		{
			col = GetComponent<BoxCollider2D>();
		}
	}

	public void SetEnabled(bool enabled)
	{
		SetCollider();
		col.enabled = enabled;
	}

	public bool GetEnabled()
	{
		SetCollider();
		return col.enabled;
	}

	public void ChangeColliderPos(Vector2 offset, Vector2 size)
	{
		SetCollider();
		col.offset = offset;
		col.size = size;
	}
}
