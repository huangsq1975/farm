using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixUI : MonoBehaviour
{
	[Serializable]
	public class FixScale
	{
		public Vector2 ReferenceResultion;

		public List<GameObject> ObjList = new List<GameObject>();
	}

	[Serializable]
	public class FixPos
	{
		public float posx;

		public float posy;

		public GameObject obj;

		[HideInInspector]
		public float default_z;
	}

	public List<FixScale> FixScaleList = new List<FixScale>();

	public Camera target;

	public List<FixPos> FixPosList = new List<FixPos>();

	public void Init()
	{
		for (int i = 0; i < FixPosList.Count; i++)
		{
			FixPos fixPos = FixPosList[i];
			Vector3 position = FixPosList[i].obj.transform.position;
			fixPos.default_z = position.z;
		}
		Update();
	}

	private void Update()
	{
		SetFixPosList();
		SetScale();
	}

	private void SetFixPosList()
	{
		WebMediator.ScreenInfo screenInfo = WebMediator.GetScreenInfo();
		for (int i = 0; i < FixPosList.Count; i++)
		{
			SetFixPos(FixPosList[i], screenInfo);
		}
	}

	private void SetFixPos(FixPos fix_pos, WebMediator.ScreenInfo screen)
	{
		float x = screen.Xper + screen.Width * fix_pos.posx;
		float y = screen.Yper + screen.Height * fix_pos.posy;
		Vector2 v = new Vector2(x, y);
		v = target.ScreenToWorldPoint(v);
		fix_pos.obj.transform.position = new Vector3(v.x, v.y, fix_pos.default_z);
	}

	private void SetScale()
	{
		for (int i = 0; i < FixScaleList.Count; i++)
		{
			float num = 1f;
			float num2 = (float)Screen.width / (float)Screen.height;
			float num3 = FixScaleList[i].ReferenceResultion.x / FixScaleList[i].ReferenceResultion.y;
			float num4 = 0f;
			if (num2 < num3)
			{
				num4 = 0f;
			}
			else if (num2 > num3)
			{
				num4 = 1f;
			}
			num = num2 / num3;
			num = num * (1f - num4) + num4;
			for (int j = 0; j < FixScaleList[i].ObjList.Count; j++)
			{
				FixScaleList[i].ObjList[j].transform.localScale = new Vector3(num, num, 1f);
			}
		}
	}

	public void AddList(GameObject obj)
	{
	}
}
