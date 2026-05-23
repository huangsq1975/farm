using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ContentsScroller : MonoBehaviour
{
	private new bool enabled = true;

	private RectTransform rect;

	[HideInInspector]
	public TouchEventRect touch;

	[HideInInspector]
	public Vector3 right_end_pos;

	[HideInInspector]
	public Vector3 parts_right;

	private float content_scroll_end;

	private UnityAction cleanup_call;

	private Vector3 down_world_pos;

	private Vector3 move_world_pos;

	private Vector3 down_local_pos;

	private const float MOVE_RIGHT_MAX = 0.3f;

	private float display_right_max;

	private bool touch_down_flag;

	private float diffrence_posx;

	private float dotween_speed;

	private float init_posX;

	public void Init()
	{
		rect = GetComponent<RectTransform>();
		touch = GetComponent<TouchEventRect>();
		touch.SetCamera(Camera.main);
		Vector3 localPosition = rect.transform.localPosition;
		init_posX = localPosition.x;
	}

	public void SetCleanup(UnityAction call)
	{
		cleanup_call = call;
	}

	public void SetTouchEventEnabled(bool enabled)
	{
		touch.SetEnabled(enabled);
	}

	public void SetEnabled(bool _enabled)
	{
		enabled = _enabled;
	}

	public void SetContentsPosition()
	{
		display_right_max = right_end_pos.x;
		touch_down_flag = true;
		content_scroll_end = parts_right.x - display_right_max - init_posX;
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		down_world_pos = Camera.main.ScreenToWorldPoint(mousePosition);
		down_local_pos = rect.transform.localPosition;
	}

	public void MoveContentsPosition()
	{
		if (!enabled || !touch_down_flag)
		{
			return;
		}
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		move_world_pos = Camera.main.ScreenToWorldPoint(mousePosition);
		bool flag = false;
		diffrence_posx = move_world_pos.x - down_world_pos.x;
		if (diffrence_posx == 0f)
		{
			return;
		}
		float num = down_local_pos.x + diffrence_posx;
		if (num > 0.3f)
		{
			num = 0.3f;
			flag = true;
		}
		if (num < 0f - (content_scroll_end + 0.3f))
		{
			num = 0f - (content_scroll_end + 0.3f);
			flag = true;
		}
		rect.DOKill();
		if (flag)
		{
			dotween_speed = 0.1f;
		}
		else
		{
			Vector2 sizeDelta = rect.sizeDelta;
			if (sizeDelta.x > 12f)
			{
				dotween_speed = 0.5f;
			}
			else
			{
				dotween_speed = 0.1f;
			}
		}
		rect.DOLocalMoveX(num, dotween_speed).SetEase(Ease.OutQuint).OnComplete(delegate
		{
			AlbumBothEndsControll(rect.transform.localPosition);
		});
		if (cleanup_call != null)
		{
			cleanup_call();
		}
	}

	public void EndContentsScroll()
	{
		touch_down_flag = false;
	}

	private void AlbumBothEndsControll(Vector3 pos)
	{
		dotween_speed = 0.6f;
		if (pos.x > init_posX)
		{
			rect.DOLocalMoveX(init_posX, dotween_speed).SetEase(Ease.OutBounce);
		}
		else if (pos.x < 0f - content_scroll_end)
		{
			rect.DOLocalMoveX(0f - content_scroll_end, dotween_speed).SetEase(Ease.OutBounce);
		}
	}
}
