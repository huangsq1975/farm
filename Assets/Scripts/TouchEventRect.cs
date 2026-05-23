using UnityEngine;
using UnityEngine.Events;

public class TouchEventRect : MonoBehaviour
{
	public Camera camera;

	public UnityEvent ClickDown;

	public UnityEvent ClickMove;

	public UnityEvent ClickUp;

	public UnityEvent ClickCancel;

	private RectTransform rect_transform;

	private bool down_enable;

	private bool touching;

	private bool touch_enabled = true;

	public TouchEvent.Param param = new TouchEvent.Param();

	private void Awake()
	{
		rect_transform = GetComponent<RectTransform>();
		if (rect_transform == null)
		{
			UnityEngine.Debug.unityLogger.LogError("Error", "需要RectTransform组件。");
		}
	}

	public void SetCamera(Camera _camera)
	{
		camera = _camera;
	}

	private void Update()
	{
		if (!touch_enabled)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (DoTouchEvent(ClickDown))
			{
				down_enable = true;
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (!DoTouchEvent(ClickMove))
			{
				Cancel();
				touching = false;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (down_enable)
			{
				if (!DoTouchEvent(ClickUp))
				{
					Cancel();
					touching = false;
				}
				down_enable = false;
			}
		}
		else
		{
			Cancel();
			touching = false;
			down_enable = false;
		}
	}

	private Vector2 TouchPos()
	{
		Camera obj = camera;
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		float x = mousePosition.x;
		Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
		Vector2 vector = obj.ScreenToWorldPoint(new Vector2(x, mousePosition2.y));
		if (base.transform.parent != null)
		{
			vector = base.transform.InverseTransformPoint(vector);
		}
		return vector;
	}

	private bool DoTouchEvent(UnityEvent unity_event)
	{
		Vector2 vector = TouchPos();
		if (rect_transform.rect.Contains(vector))
		{
			touching = true;
			param.pos = vector;
			unity_event.Invoke();
			return true;
		}
		return false;
	}

	private void Cancel()
	{
		if (touching && ClickCancel != null)
		{
			ClickCancel.Invoke();
		}
	}

	public void SetEnabled(bool enabled)
	{
		touch_enabled = enabled;
	}
}
