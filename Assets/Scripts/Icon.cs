using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
	public enum ePos
	{
		UP,
		CENTER,
		DOWN,
		MAX
	}

	private const float SWITCH_TIME = 1f;

	private Vector3[] icon_pos = new Vector3[3]
	{
		new Vector3(0f, 0.16f, 0f),
		new Vector3(0f, 0.13f, 0f),
		new Vector3(0f, 0.1f, 0f)
	};

	[SerializeField]
	private Animation anim;

	[SerializeField]
	private SpriteRenderer ren;

	[SerializeField]
	private List<Sprite> sprite_list = new List<Sprite>();

	[SerializeField]
	private int list_index;

	[SerializeField]
	private float switch_time;

	[SerializeField]
	private bool Switch;

	public void Open(List<Sprite> list, ePos pos)
	{
		anim = GetComponent<Animation>();
		ren = base.transform.Find("contents").gameObject.transform.Find("icon").gameObject.GetComponent<SpriteRenderer>();
		ren.transform.localPosition = icon_pos[(int)pos];
		sprite_list = list;
		list_index = 0;
		switch_time = 0f;
		if (list.Count > 1)
		{
			Switch = true;
			list_index = UnityEngine.Random.Range(0, list.Count);
		}
		SwitchSprite();
	}

	private void OnDestroy()
	{
		icon_pos = null;
		anim = null;
		ren = null;
		sprite_list = null;
	}

	private void Update()
	{
		if (Switch)
		{
			switch_time += Time.deltaTime;
			if (switch_time >= 1f)
			{
				list_index = ((sprite_list.Count != list_index + 1) ? (list_index + 1) : 0);
				SwitchSprite();
				switch_time = 0f;
			}
		}
	}

	private void SwitchSprite()
	{
		ren.sprite = sprite_list[list_index];
	}

	public void Push()
	{
		anim.Play();
	}
}
