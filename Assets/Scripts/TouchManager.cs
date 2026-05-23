using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
	[Serializable]
	public class Specific
	{
		public bool enabled;

		public HashSet<TouchEvent> hash = new HashSet<TouchEvent>();
	}

	[SerializeField]
	private Camera target_camera;

	private GameObject objDown;

	private TouchEvent evt;

	private GameObject obj;

	private Vector2 current_pos;

	private Vector2 start_pos;

	public Specific specific = new Specific();

	public static GameObject GetObject(Camera camera, Vector2 pos)
	{
		GameObject result = null;
		Ray ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
		if (raycastHit2D.collider != null)
		{
			result = raycastHit2D.collider.gameObject;
		}
		return result;
	}

	private bool DoTouch()
	{
		bool result = false;
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		float x = mousePosition.x;
		Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
		Vector2 vector = new Vector2(x, mousePosition2.y);
		current_pos = target_camera.ScreenToWorldPoint(vector);
		obj = GetObject(target_camera, vector);
		if (obj != null)
		{
			evt = obj.GetComponent<TouchEvent>();
			if (evt != null && (!specific.enabled || specific.hash.Contains(evt)))
			{
				evt.param.pos = target_camera.ScreenToWorldPoint(vector);
				result = true;
			}
		}
		return result;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (DoTouch())
			{
				objDown = obj;
				evt.ClickDown.Invoke();
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (DoTouch())
			{
				evt.ClickMove.Invoke();
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (DoTouch() && objDown == obj)
			{
				evt.ClickUp.Invoke();
			}
			objDown = null;
		}
	}

	public void SetSpecificEnabled(bool enabled)
	{
		specific.enabled = enabled;
	}

	public void AddSpecific(TouchEvent touch)
	{
		specific.hash.Add(touch);
	}

	public void DelSpecific(TouchEvent touch)
	{
		specific.hash.Remove(touch);
	}

	public void ClearSpecific()
	{
		specific.hash.Clear();
	}
}
