using System;
using UnityEngine;
using UnityEngine.Events;

public class Progress : MonoBehaviour
{
	public enum eType
	{
		NONE = -1,
		LOOP,
		ONCE
	}

	public enum eSTATE
	{
		NONE = -1,
		RUN,
		SHOW,
		PRE_HIDE,
		HIDE,
		EMERGENCY,
		MAX
	}

	public class Parts
	{
		public SpriteRenderer sr;

		public Color color;

		public Parts(SpriteRenderer s)
		{
			sr = s;
			color = s.color;
		}
	}

	public class LoopData
	{
		public ulong start_time;

		public ulong progress_time;

		public int sec;

		public int reminder;

		public bool emergency;

		public bool fix;

		public float hide_delay;

		public float emergency_time;

		public UnityAction call;

		public TextMesh text_mesh;
	}

	public class OnceData
	{
		public float s_time;

		public float e_time;

		public float dulation;

		public float life_time;
	}

	public AnimationClip progress_clip;

	public AnimationClip progress_emergency_clip;

	public AnimationClip progress_end_clip;

	public Sprite bar_sprite;

	public Sprite fix_bar_sprite;

	public const float Persistence = -1f;

	private Animation anim;

	public eType type = eType.NONE;

	public eSTATE state = eSTATE.NONE;

	private Parts frame;

	private Parts bar;

	private Parts shadow;

	private LoopData loop;

	private OnceData once;

	private float elapsed_time;

	public ulong Loop(ulong _start_time, int time, UnityAction call_fix)
	{
		Init();
		loop = new LoopData();
		loop.start_time = _start_time;
		loop.sec = time;
		loop.progress_time = (ulong)((long)loop.sec * 10000000L);
		loop.call = call_fix;
		loop.hide_delay = -1f;
		loop.emergency_time = 0f;
		loop.fix = false;
		elapsed_time = 0f;
		loop.reminder = time;
		loop.text_mesh = base.transform.Find("text").GetComponent<TextMesh>();
		loop.text_mesh.gameObject.SetActive(value: false);
		state = eSTATE.RUN;
		type = eType.LOOP;
		Hide(0f);
		return loop.progress_time;
	}

	public void Once(float start_normalized_time, float end_normalized_time, float dulation, float life_time)
	{
		Init();
		once = new OnceData();
		once.dulation = dulation;
		once.life_time = life_time;
		elapsed_time = 0f;
		once.s_time = start_normalized_time;
		once.e_time = end_normalized_time;
		Utils.Play(anim, progress_clip.name, 0f, start_normalized_time);
		TextMesh component = base.transform.Find("text").GetComponent<TextMesh>();
		component.gameObject.SetActive(value: false);
		state = eSTATE.RUN;
		type = eType.ONCE;
	}

	public void Once(float normalized_time, float life_time)
	{
		Init();
		once = new OnceData();
		once.life_time = life_time;
		elapsed_time = 0f;
		once.s_time = normalized_time;
		once.e_time = -1f;
		if (normalized_time >= 1f)
		{
			bar.sr.sprite = fix_bar_sprite;
		}
		TextMesh component = base.transform.Find("text").GetComponent<TextMesh>();
		component.gameObject.SetActive(value: false);
		Utils.Play(anim, progress_clip.name, 0f, normalized_time);
		state = eSTATE.RUN;
		type = eType.ONCE;
	}

	public void SetTextVisibility(bool visibility, Vector2 pos, Color color, int order_in_layer = -1)
	{
		if (type == eType.LOOP)
		{
			loop.text_mesh.gameObject.SetActive(visibility);
			if (visibility)
			{
				SetReminderTimeText();
				loop.text_mesh.transform.localPosition = pos;
				loop.text_mesh.color = color;
				if (order_in_layer != -1)
				{
					CustomText component = loop.text_mesh.GetComponent<CustomText>();
					component.SetOrderInLayer(order_in_layer);
				}
			}
		}
		else
		{
			UnityEngine.Debug.unityLogger.LogError("Error", "只能在LOOP模式下使用。");
		}
	}

	public void ChangeEndTime(int time)
	{
		if (type == eType.LOOP)
		{
			loop.sec = time;
			loop.progress_time = (ulong)((long)loop.sec * 10000000L);
		}
	}

	private void SetReminderTimeText()
	{
		if (loop.text_mesh.gameObject.activeSelf && loop.sec > loop.reminder)
		{
			loop.text_mesh.text = (loop.reminder / 60).ToString("00") + ":" + (loop.reminder % 60).ToString("00");
		}
	}

	private void Init()
	{
		if (anim == null)
		{
			anim = GetComponent<Animation>();
			frame = new Parts(base.transform.Find("frame").GetComponent<SpriteRenderer>());
			bar = new Parts(base.transform.Find("frame/bar").GetComponent<SpriteRenderer>());
			shadow = new Parts(base.transform.Find("frame/shadow").GetComponent<SpriteRenderer>());
		}
		bar.sr.sprite = bar_sprite;
	}

	public void SetOrderInLayer(int order_in_layer)
	{
		Init();
		bar.sr.sortingOrder += order_in_layer;
		frame.sr.sortingOrder += order_in_layer;
		shadow.sr.sortingOrder += order_in_layer;
	}

	private void Update()
	{
		if (type == eType.LOOP)
		{
			LoopAction();
		}
		else if (type == eType.ONCE)
		{
			OnceAction();
		}
	}

	private void LoopAction()
	{
		if (state == eSTATE.PRE_HIDE)
		{
			elapsed_time += Time.deltaTime;
			if (elapsed_time > loop.hide_delay)
			{
				state = eSTATE.HIDE;
				Utils.Play(anim, progress_end_clip.name, 1f, 0f);
			}
		}
		else if (state == eSTATE.HIDE && !anim.isPlaying)
		{
			Hide(0f);
		}
		if (state == eSTATE.RUN || state == eSTATE.SHOW || state == eSTATE.PRE_HIDE)
		{
			if (loop.fix)
			{
				return;
			}
			int num = (int)((ulong)((long)(loop.progress_time + loop.start_time) - DateTime.Now.Ticks) / 10000000uL);
			loop.reminder = num;
			SetReminderTimeText();
			float num2 = (num <= loop.sec) ? ((float)(loop.sec - num) * 1f / (float)loop.sec) : 1f;
			if (num2 >= 1f)
			{
				num2 = 1f;
				bar.sr.sprite = fix_bar_sprite;
				loop.fix = true;
				loop.call();
				if (loop.text_mesh.gameObject.activeSelf)
				{
					loop.text_mesh.gameObject.SetActive(value: false);
				}
			}
			Utils.Play(anim, progress_clip.name, 0f, num2);
		}
		else if (state == eSTATE.EMERGENCY)
		{
			loop.emergency_time += Time.deltaTime;
		}
	}

	private void OnceAction()
	{
		if (state == eSTATE.RUN)
		{
			elapsed_time += Time.deltaTime;
			if (once.e_time != -1f)
			{
				float num = elapsed_time / once.dulation;
				float num2 = once.e_time - once.s_time;
				num2 = ((!(num > 1f)) ? (num2 * num) : num2);
				Utils.Play(anim, progress_clip.name, 0f, once.s_time + num2);
				if (elapsed_time > once.dulation && once.e_time >= 1f)
				{
					bar.sr.sprite = fix_bar_sprite;
				}
				if (elapsed_time > once.life_time + 1f)
				{
					state = eSTATE.HIDE;
					Utils.Play(anim, progress_end_clip.name, 1f, 0f);
				}
			}
			else if (elapsed_time > once.life_time)
			{
				state = eSTATE.HIDE;
				Utils.Play(anim, progress_end_clip.name, 1f, 0f);
			}
		}
		else if (state == eSTATE.HIDE)
		{
			if (!anim.isPlaying)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (state == eSTATE.EMERGENCY && anim.clip != progress_emergency_clip)
		{
			Utils.Play(anim, progress_emergency_clip.name, 1f, 0f);
			anim.clip = progress_emergency_clip;
		}
	}

	public bool Emergency(bool onoff)
	{
		if (type == eType.LOOP)
		{
			if (onoff)
			{
				if (!loop.fix)
				{
					Show();
					loop.emergency_time = 0f;
					if (progress_emergency_clip != null)
					{
						Utils.Play(anim, progress_emergency_clip.name, 1f, 0f);
					}
					state = eSTATE.EMERGENCY;
				}
			}
			else if (state == eSTATE.EMERGENCY)
			{
				loop.progress_time += (ulong)(loop.emergency_time * 1E+07f);
				Show();
				state = eSTATE.SHOW;
			}
			return true;
		}
		return false;
	}

	public bool EmergencyOnce(float normalized_time)
	{
		if (type == eType.NONE)
		{
			Init();
			TextMesh component = base.transform.Find("text").GetComponent<TextMesh>();
			component.gameObject.SetActive(value: false);
			Utils.Play(anim, progress_clip.name, 0f, normalized_time);
			type = eType.ONCE;
			state = eSTATE.EMERGENCY;
			return true;
		}
		return false;
	}

	public bool EmergencyOnceStop()
	{
		if (type == eType.ONCE && state == eSTATE.EMERGENCY)
		{
			Utils.Play(anim, progress_end_clip.name, 1f, 0f);
			state = eSTATE.HIDE;
			return true;
		}
		return false;
	}

	public bool Show()
	{
		if (type == eType.LOOP)
		{
			SetVisibility(frame, visibility: true);
			SetVisibility(bar, visibility: true);
			SetVisibility(shadow, visibility: true);
			loop.hide_delay = -1f;
			if (state != eSTATE.EMERGENCY)
			{
				state = eSTATE.SHOW;
			}
			return true;
		}
		return false;
	}

	public bool Hide(float time)
	{
		if (type == eType.LOOP)
		{
			elapsed_time = 0f;
			loop.hide_delay = time;
			if (loop.hide_delay > 0f)
			{
				state = eSTATE.PRE_HIDE;
			}
			else
			{
				SetVisibility(frame, visibility: false);
				SetVisibility(bar, visibility: false);
				SetVisibility(shadow, visibility: false);
				state = eSTATE.RUN;
			}
			return true;
		}
		return false;
	}

	private void SetVisibility(Parts parts, bool visibility)
	{
		parts.sr.enabled = visibility;
		parts.sr.color = parts.color;
	}
}
