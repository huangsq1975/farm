using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
	[Serializable]
	public class TimeData
	{
		public ulong start_time;

		public ulong progress_time;

		public int sec;

		public bool fix;

		public UnityAction call;

		public float elapsed_time;

		public bool chk_owner;

		public Transform owner;
	}

	private static Timer self;

	public List<TimeData> time_data = new List<TimeData>();

	public void Init()
	{
		self = this;
	}

	public static TimeData Create(ulong _start_time, int time, UnityAction call_fix, bool chk_owner = false, Transform owner = null)
	{
		TimeData timeData = new TimeData();
		timeData.start_time = _start_time;
		timeData.sec = time;
		timeData.progress_time = (ulong)((long)timeData.sec * 10000000L);
		timeData.call = call_fix;
		timeData.fix = false;
		timeData.chk_owner = chk_owner;
		timeData.owner = owner;
		self.time_data.Add(timeData);
		return timeData;
	}

	public static void Remove(TimeData data)
	{
		self.time_data.Remove(data);
	}

	private void Update()
	{
		int num = 0;
		while (num < time_data.Count)
		{
			TimeData timeData = time_data[num];
			if (Action(timeData))
			{
				time_data.Remove(timeData);
			}
			else
			{
				num++;
			}
		}
	}

	private bool Action(TimeData data)
	{
		if (!data.fix && (ulong)DateTime.Now.Ticks >= data.progress_time + data.start_time)
		{
			data.fix = true;
			if (data.call != null && (!data.chk_owner || data.owner != null))
			{
				data.call();
			}
			return true;
		}
		return false;
	}
}
