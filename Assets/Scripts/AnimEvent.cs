using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
	public bool auto_destroy;

	public List<UnityEvent> event_list;

	public UnityAction finish_call;

	private Animation anim;

	private Animator animator;

	private void Start()
	{
		anim = GetComponent<Animation>();
		animator = GetComponent<Animator>();
	}

	public void SetFinishCallback(UnityAction call)
	{
		finish_call = call;
	}

	public void SetEventCallback(UnityAction call, int event_list_index)
	{
		event_list[event_list_index].AddListener(call);
	}

	public void Event(int event_list_index)
	{
		event_list[event_list_index].Invoke();
	}

	private void Update()
	{
		if ((animator != null && !Utils.IsPlaying(animator)) || (anim != null && !anim.isPlaying))
		{
			if (finish_call != null)
			{
				finish_call();
			}
			if (auto_destroy)
			{
				Finish();
			}
		}
	}

	private void Finish()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		animator = null;
		anim = null;
	}
}
