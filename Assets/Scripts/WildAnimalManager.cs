using System;
using System.Collections.Generic;
using UnityEngine;

public class WildAnimalManager : MonoBehaviour
{
	[Serializable]
	public class VisitAnimal
	{
		public WildAnimal.eCATEGORY category;

		public float elapsed_time;

		private static readonly int[] VISIT_INTERVAL = new int[5]
		{
			30,
			50,
			45,
			55,
			50
		};

		public int Interval;

		public VisitAnimal(WildAnimal.eCATEGORY _category)
		{
			category = _category;
			Interval = VISIT_INTERVAL[(int)category];
		}
	}

	private Manager manager;

	private Data.WildAnimalData wad;

	public VisitAnimal[] visit_animals;

	public const int STAY_TIME = 300;

	private List<Timer.TimeData>[] timer_list = new List<Timer.TimeData>[5];

	public void Init(Manager m)
	{
		manager = m;
		wad = manager.data.wild_animal_data;
		visit_animals = new VisitAnimal[5];
		for (int i = 0; i < wad.visit.Length; i++)
		{
			visit_animals[i] = new VisitAnimal((WildAnimal.eCATEGORY)i);
			timer_list[i] = new List<Timer.TimeData>();
			ulong num = (ulong)(DateTime.Now.Ticks - (long)wad.visit_time[i]);
			visit_animals[i].elapsed_time = (float)(double)(num / 10000000uL) * 1f % (float)visit_animals[i].Interval;
			for (int j = 0; j < wad.visit[i].areas.Count; j++)
			{
				timer_list[i].Add(null);
				WildAnimal.eType type = wad.visit[i].areas[j].type;
				if (type != WildAnimal.eType.NONE)
				{
					manager.map.GoToFarm(type, j, entry: false);
					WildAnimal.eCATEGORY category = (WildAnimal.eCATEGORY)i;
					int area_id = j;
					timer_list[i][j] = Timer.Create(wad.visit[i].areas[j].time, 300, delegate
					{
						GoBack(category, type, area_id);
					});
				}
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < visit_animals.Length; i++)
		{
			Visit(visit_animals[i]);
		}
	}

	private void Visit(VisitAnimal visit_animal)
	{
		Common.ConditionCompare conditionCompare = null;
		visit_animal.elapsed_time += Time.deltaTime;
		if (!(visit_animal.elapsed_time > (float)visit_animal.Interval))
		{
			return;
		}
		visit_animal.elapsed_time = 0f;
		ulong ticks = (ulong)DateTime.Now.Ticks;
		wad.SetVisitTime(ticks, visit_animal.category);
		int area_id = VacantArea(visit_animal);
		if (area_id == -1)
		{
			return;
		}
		if (conditionCompare == null)
		{
			conditionCompare = Common.GetCompareData(manager, wild_animal: true);
		}
		WildAnimal.eType type = DecideAnimal(visit_animal.category, conditionCompare);
		if (type != WildAnimal.eType.NONE)
		{
			wad.Visiting(visit_animal.category, area_id, type, ticks);
			manager.map.GoToFarm(type, area_id, entry: true);
			if (type == WildAnimal.eType.SEAGULL_1)
			{
				Manager.sound.PlaySe(Sound.eSe.SEAGULL);
			}
			if (type == WildAnimal.eType.OWL_1)
			{
				Manager.sound.PlaySe(Sound.eSe.OWL);
			}
			WildAnimal.eCATEGORY category = visit_animal.category;
			timer_list[(int)category][area_id] = Timer.Create(ticks, 300, delegate
			{
				GoBack(category, type, area_id);
			});
			if (manager.data.GetReg(Data.CharacterData.eType.WILDANIMAL, (int)type) == 0)
			{
				manager.data.SetReg(Data.CharacterData.eType.WILDANIMAL, (int)type);
			}
		}
	}

	private WildAnimal.eType DecideAnimal(WildAnimal.eCATEGORY category, Common.ConditionCompare compare)
	{
		List<WildAnimal.eType> list = WildAnimal.GetList(category);
		List<WildAnimal.eType> list2 = new List<WildAnimal.eType>();
		for (int i = 0; i < list.Count; i++)
		{
			if (manager.map.IsGoToFarm(list[i]) && Common.IsMeetConditions(list[i], compare))
			{
				list2.Add(list[i]);
			}
		}
		if (list2.Count == 0)
		{
			return WildAnimal.eType.NONE;
		}
		return list2[UnityEngine.Random.Range(0, list2.Count)];
	}

	private int VacantArea(VisitAnimal visit_animal)
	{
		Data.WildAnimalData.Visit visit = wad.visit[(int)visit_animal.category];
		int num = manager.data.level / 4 + 1;
		for (int i = 0; i < visit.areas.Count && i < num; i++)
		{
			if (visit.areas[i].type == WildAnimal.eType.NONE)
			{
				return i;
			}
		}
		return -1;
	}

	private void GoBack(WildAnimal.eCATEGORY category, WildAnimal.eType type, int area_id)
	{
		manager.map.ReturnFromFarm(type, area_id);
		wad.Visiting(category, area_id, WildAnimal.eType.NONE, 0uL);
		timer_list[(int)category][area_id] = null;
	}

	public void ForceAnimalDestroy(WildAnimal.eType type, int area_id)
	{
		WildAnimal.eCATEGORY eCATEGORY = WildAnimal.TypeToCategory(type);
		GoBack(eCATEGORY, type, area_id);
		if (timer_list[(int)eCATEGORY][area_id] != null)
		{
			Timer.Remove(timer_list[(int)eCATEGORY][area_id]);
		}
	}
}
