using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Sailo : MonoBehaviour
{
	[Serializable]
	public class SailoOpen
	{
		[Serializable]
		public class Area
		{
			public Transform root;

			public Animation area_anim;

			public Animation[] grass_anim = new Animation[15];

			public Sequence[] sequence = new Sequence[15];
		}

		public const string SAILO_EMERGENCY = "sailo_emergency";

		public const string SAILO_SPOUT_OUT = "sailo_spout_out";

		public const string SAILO_OPEN_ANIM = "sailo_open";

		public const string PROGRESS_ANIM = "sailo_progress";

		public const string PROGRESS_FULL_ANIM = "sailo_progress_full";

		public const string AREA_APPEAR_ANIM = "sailo_area_appear";

		public const string AREA_LOCK_ANIM = "sailo_area_lock";

		public const string GRASS_ANIM = "sailo_grass_grow";

		public const string GRASS_FISH_ANIM = "sailo_grass";

		public const string GRASS_FISH_ANIM_GROW = "sailo_grass_grow";

		public const string GRASS_GET_ANIM = "sailo_grass_get";

		public const string ANIMALS_STAY = "silo_animals_stay";

		public const string ANIMALS_EAT = "silo_animals_eat";

		public const string ANIMALS_SHOCK = "silo_animals_shock";

		public const string ANIMALS_FULL_STAY = "silo_animals_full_stay";

		public const string ANIMALS_FULL_EAT = "silo_animals_full_eat";

		public Animation anim;

		public Animator animals;

		public AnimEvent animals_events;

		public Animation progress;

		public TextMesh stock_text;

		public Transform grass_target;

		public Area[] areas = new Area[4];

		public Vector3[] default_grass_pos = new Vector3[15];
	}

	[Serializable]
	public class SailoWorker
	{
		private Manager manager;

		public bool regist;

		public Worker obj;

		public Worker.eType type;

		public Transform parent;

		public SailoWorker(Manager manager, bool regist)
		{
			this.manager = manager;
			this.regist = regist;
			obj = null;
		}

		public void Create()
		{
			if (regist)
			{
				obj = Worker.Create(manager, type, Worker.eWorkPlace.SAILO, parent);
			}
		}

		public void Destroy()
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj.gameObject);
				obj = null;
			}
		}
	}

	public class SpoutOut
	{
		public int facility;

		public int animal;

		public bool grass;

		public SpoutOut(int f, int a, bool g = false)
		{
			facility = f;
			animal = a;
			grass = g;
		}
	}

	public class SiloBoy
	{
		public Animator animator;

		public int count;

		private SpriteRenderer sr;

		private SpriteRenderer grass;

		public bool spout_outing;

		public const float X = -0.6f;

		public const float Y = 0.08f;

		public float flipX = 1f;

		private int default_oil;

		public void SetAnimator(Animator _animator)
		{
			animator = _animator;
			sr = animator.GetComponent<SpriteRenderer>();
			grass = sr.transform.Find("grass").GetComponent<SpriteRenderer>();
			default_oil = sr.sortingOrder;
			spout_outing = false;
		}

		public void SetVisibility(bool visibility)
		{
			sr.enabled = visibility;
			grass.gameObject.SetActive(visibility);
		}

		public void SetFlipX(bool flip)
		{
			flipX = ((!flip) ? 1 : (-1));
		}

		public void SetOrderInLayer(int oil)
		{
			sr.sortingOrder = default_oil + oil;
			grass.sortingOrder = default_oil - 1 + oil;
		}
	}

	public const int SEC = 4;

	public const ulong GROW_INTERVAL = 40000000uL;

	public const int SPOUT_OUT_SEC = 7;

	public const ulong SPOUT_OUT_INTERVAL = 70000000uL;

	public const int UPDATE_TIME = 600;

	private Manager manager;

	private Data.SailoData sailo_data;

	private Map map;

	private Animation sailo_anim;

	private TouchEvent work_icon;

	private Progress progress;

	private GameObject present_video;

	public SailoOpen open;

	public List<Data.SailoData.Area.Grass> grow_list = new List<Data.SailoData.Area.Grass>();

	public SailoWorker worker;

	public const int GRASS_CUT_NUM = 6;

	private bool isHungry;

	private List<SpoutOut> spout_out = new List<SpoutOut>();

	private List<SpoutOut> give_grass_list = new List<SpoutOut>();

	private Timer.TimeData video_timer;

	private int prev_stock = -1;

	private Prompt prompt;

	private bool fix_sailo_udpate;

	private SiloBoy silo_boy;

	private static readonly string[] tANIM_PREFIX = new string[2]
	{
		"silo_boy",
		"silo_girl"
	};

	public void Init()
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		map = manager.GetComponent<Map>();
		sailo_anim = GetSilo(base.transform);
		sailo_data = manager.data.sailo_data;
		worker = new SailoWorker(manager, regist: false);
		for (int i = 0; i < sailo_data.areas.Length; i++)
		{
			for (int j = 0; j < sailo_data.areas[i].grasses.Length; j++)
			{
				if (sailo_data.areas[i].grasses[j].time != 0)
				{
					grow_list.Add(sailo_data.areas[i].grasses[j]);
				}
			}
		}
		open = new SailoOpen();
		silo_boy = new SiloBoy();
		CreateSiloBoy(flip: false, 0);
		SpriteRenderer component = GameObject.Find("UI/silo/icon/sprite").GetComponent<SpriteRenderer>();
		component.sprite = SpriteManager.GetSiloMiniIcon(Data.farm_type);
	}

	private void Update()
	{
		if (sailo_data.grass_stock == 0 && IsHungry())
		{
			Emergency();
		}
		SpoutOuting();
		Growing();
		SetProgress();
	}

	private void SpoutOuting()
	{
		if (Manager.events.state != Event.eState.NONE || silo_boy.spout_outing)
		{
			return;
		}
		ulong ticks = (ulong)DateTime.Now.Ticks;
		ulong num = ticks - sailo_data.spout_out_time;
		int num2 = (int)(num / 70000000uL);
		if (num2 > 0)
		{
			StartAutoCut();
			if (silo_boy.animator != null && IsThereAnimal())
			{
				silo_boy.count = ((num2 >= 10) ? 10 : num2);
				BoyGotoSilo();
			}
		}
	}

	private bool IsThereAnimal()
	{
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			if (map.facility_list[i].AnimalCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsHungry()
	{
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			for (int j = 0; j < map.facility_list[i].AnimalCount(); j++)
			{
				if (map.facility_list[i].IsThereAnimals(j) && map.facility_list[i].IsHungry(j))
				{
					return true;
				}
			}
		}
		return false;
	}

	private int DoSpoutOut(bool immediate)
	{
		for (int i = 0; i < 11; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (map.facility_list[i].IsThereAnimals(j))
				{
					spout_out.Add(new SpoutOut(i, j));
				}
			}
		}
		int num = 0;
		int num2 = 0;
		while (num < sailo_data.grass_stock && spout_out.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, spout_out.Count);
			SpoutOut spoutOut = spout_out[index];
			if (map.facility_list[spoutOut.facility].IsHungry(spoutOut.animal))
			{
				if (immediate)
				{
					map.facility_list[spoutOut.facility].GiveGrass(spoutOut.animal, grass: true, immediate);
				}
				else
				{
					give_grass_list.Add(new SpoutOut(spoutOut.facility, spoutOut.animal, g: true));
				}
				num++;
				num2++;
			}
			spout_out.RemoveAt(index);
		}
		sailo_data.AddGrassStock(-num);
		for (int k = 0; k < spout_out.Count; k++)
		{
			if (map.facility_list[spout_out[k].facility].IsHungry(spout_out[k].animal))
			{
				if (immediate)
				{
					map.facility_list[spout_out[k].facility].GiveGrass(spout_out[k].animal, grass: false, immediate);
				}
				else
				{
					give_grass_list.Add(new SpoutOut(spout_out[k].facility, spout_out[k].animal));
				}
				num2++;
			}
		}
		if (spout_out.Count > 0 && num2 > 0)
		{
			Emergency();
		}
		spout_out.Clear();
		sailo_data.SpoutOut();
		if (num2 > 0 && num > 0)
		{
			Utils.Play(sailo_anim, "sailo_spout_out", 1f, 0f);
		}
		return num2;
	}

	private void Growing()
	{
		if (grow_list.Count <= 0)
		{
			return;
		}
		ulong ticks = (ulong)DateTime.Now.Ticks;
		int num = 0;
		while (num < grow_list.Count)
		{
			Data.SailoData.Area.Grass grass = grow_list[num];
			ulong num2 = grass.time + 40000000;
			if (num2 < ticks)
			{
				sailo_data.GrassGrow(grass.area, grass.id);
				grow_list.Remove(grass);
				if (open.anim != null)
				{
					Utils.Play(open.areas[grass.area].grass_anim[grass.id], "sailo_grass_grow", 1f, 0f);
					Manager.sound.PlaySe(Sound.eSe.GRASS_GROW);
				}
			}
			else
			{
				num++;
			}
		}
	}

	private Animation GetSilo(Transform t_parent)
	{
		return t_parent.Find("icon").GetComponent<Animation>();
	}

	public void Open()
	{
		if (!(open.anim == null))
		{
			return;
		}
		prev_stock = -1;
		sailo_anim.Stop();
		open = new SailoOpen();
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/silo_open") as GameObject;
		open.anim = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<Animation>();
		open.progress = open.anim.transform.Find("knob/progress").GetComponent<Animation>();
		open.stock_text = open.anim.transform.Find("knob/stock_text").GetComponent<TextMesh>();
		open.progress.gameObject.GetComponent<AnimEvent>().SetEventCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.FULL);
		}, 0);
		SetStockText();
		open.animals = open.anim.transform.Find("knob/animals").GetComponent<Animator>();
		open.animals_events = open.animals.GetComponent<AnimEvent>();
		if (!IsThereAnimal())
		{
			open.animals.gameObject.SetActive(value: false);
		}
		else if (!IsHungry())
		{
			Utils.Play(open.animals, "silo_animals_full_stay", 1f, 0f);
		}
		else
		{
			Utils.Play(open.animals, "silo_animals_stay", 1f, 0f);
		}
		open.grass_target = open.anim.transform.Find("knob/icon");
		sailo_anim = GetSilo(open.grass_target.parent);
		original = (Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/silo_grass_area") as GameObject);
		for (int i = 0; i < 4; i++)
		{
			open.areas[i] = new SailoOpen.Area();
			open.areas[i].root = open.anim.transform.Find("area_" + (i + 1));
			open.areas[i].area_anim = UnityEngine.Object.Instantiate(original, open.areas[i].root, worldPositionStays: false).GetComponent<Animation>();
			open.areas[i].area_anim.gameObject.name = original.name;
			if (i < sailo_data.level)
			{
				SetGrass(i, i == 0);
				Utils.Play(open.areas[i].area_anim, "sailo_area_appear", 0f, 1f);
				continue;
			}
			Utils.Play(open.areas[i].area_anim, "sailo_area_lock", 0f, 0f);
			if (i == sailo_data.level)
			{
				if (sailo_data.sailo_update_time != 0)
				{
					SetConstructIcon(i);
					CreateProgress(sailo_data.sailo_update_time, i);
					progress.SetTextVisibility(visibility: true, new Vector2(0f, -0.204f), Common.TextWhiteTranslucentColor);
				}
				else
				{
					SetLockIcon(i);
				}
			}
		}
		TouchEvent component = open.anim.GetComponent<TouchEvent>();
		component.ClickDown.AddListener(Manager.sound.CancelSound);
		component.ClickUp.AddListener(Close);
		Utils.Play(open.anim, "sailo_open", 1f, 0f);
		CreateSiloBoy(flip: true, 30);
		SetConstructVideo();
		if (worker.regist)
		{
			worker.parent = open.anim.transform;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				worker.Create();
			});
			sequence.Play();
		}
	}

	public void Close()
	{
		if (!(open.anim != null))
		{
			return;
		}
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
		}
		for (int i = 0; i < open.areas.Length; i++)
		{
			SailoOpen.Area area = open.areas[i];
			for (int j = 0; j < area.sequence.Length; j++)
			{
				if (area.sequence[j] != null)
				{
					area.sequence[j].Kill();
				}
			}
		}
		worker.Destroy();
		open.anim.GetComponent<AnimEvent>().auto_destroy = true;
		Utils.Play(open.anim, "sailo_open", -1f, (!open.anim.isPlaying) ? 1f : open.anim["sailo_open"].normalizedTime);
		open.anim = null;
		open = new SailoOpen();
		sailo_anim = GetSilo(base.transform);
		if (sailo_data.grass_stock > 0)
		{
			Utils.Play(sailo_anim, "sailo_spout_out", 0f, 0f);
		}
		CreateSiloBoy(flip: false, 0);
	}

	private bool SetConstructVideo()
	{
		if (sailo_data.sailo_update_time != 0 && open.anim != null)
		{
			int num = 600 - (int)((ulong)(DateTime.Now.Ticks - (long)sailo_data.sailo_update_time) / 10000000uL);
			Utils.Log("Construct reminder_sec= " + num);
			if ((float)num >= 180f)
			{
				if (present_video != null)
				{
					UnityEngine.Object.Destroy(present_video);
				}
				if (video_timer != null)
				{
					Timer.Remove(video_timer);
					video_timer = null;
				}
				if (manager.LoadVideo())
				{
					video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 0, ShowPresent);
					return true;
				}
			}
		}
		return false;
	}

	private void ShowPresent()
	{
		if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			present_video = Common.CreatePresentConstructBox(open.anim.transform, new Vector3(-0.32f, 2.88f, -30f), 22000, delegate
			{
				manager.PlayVideo(PlayVideoCompleted, PlayVideoFailed);
			}, null);
			video_timer = null;
			return;
		}
		video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 1, ShowPresent);
		if (manager.load_video.state == Manager.VideoRwd.eState.NONE)
		{
			manager.LoadVideo();
		}
	}

	private void PlayVideoCompleted()
	{
		Utils.Log("Silo : PlayVideoCompleted");
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
		FixSailoUpdate(video: true);
		manager.LoadVideo();
	}

	public void PlayVideoFailed()
	{
		Utils.Log("Silo : PlayVideoFailed");
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
		manager.LoadVideo();
		video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 3, ShowPresent);
	}

	private void SetGrass(int area_pos, bool set_default_pos)
	{
		SailoOpen.Area area = open.areas[area_pos];
		Data.SailoData.Area area2 = sailo_data.areas[area_pos];
		for (int i = 0; i < 15; i++)
		{
			area.grass_anim[i] = area.area_anim.transform.Find("grass_" + (i + 1)).GetComponent<Animation>();
			if (area2.grasses[i].time == 0)
			{
				area.grass_anim[i]["sailo_grass_grow"].speed = 1f;
			}
			else
			{
				area.grass_anim[i]["sailo_grass_grow"].speed = 0f;
			}
			area.grass_anim[i]["sailo_grass_grow"].normalizedTime = 0f;
			if (Data.farm_type == Data.eFarmType.RESORT && area2.grasses[i].time != 0)
			{
				area.grass_anim[i].Play("sailo_grass_grow");
			}
			else
			{
				area.grass_anim[i].Play();
			}
			TouchEvent touch = area.grass_anim[i].GetComponent<TouchEvent>();
			touch.param.value1 = area_pos;
			touch.param.value2 = i;
			touch.ClickDown.AddListener(delegate
			{
				Cut(touch);
			});
			touch.ClickMove.AddListener(delegate
			{
				Cut(touch);
			});
			if (set_default_pos)
			{
				open.default_grass_pos[i] = area.grass_anim[i].transform.localPosition;
			}
			if (Data.farm_type == Data.eFarmType.RESORT)
			{
				AnimEvent component = open.areas[area_pos].grass_anim[i].GetComponent<AnimEvent>();
				int grass_id = i;
				component.SetEventCallback(delegate
				{
					SetGrasAnimation(area_pos, grass_id);
				}, 0);
			}
		}
	}

	private void SetGrasAnimation(int area_id, int grass_id)
	{
		if (open.anim != null)
		{
			open.areas[area_id].grass_anim[grass_id].Play("sailo_grass");
		}
	}

	private void SetProgress()
	{
		if (open.anim != null && prev_stock != sailo_data.grass_stock)
		{
			float normalized_time = (float)sailo_data.grass_stock / (float)Data.SailoData.GRASS_STOCK_MAX[sailo_data.level];
			Utils.Play(open.progress, "sailo_progress", 0f, normalized_time);
			prev_stock = sailo_data.grass_stock;
			SetStockText();
		}
	}

	private void SetStockText()
	{
		open.stock_text.text = sailo_data.grass_stock + "/" + Data.SailoData.GRASS_STOCK_MAX[sailo_data.level];
	}

	public void Emergency()
	{
		sailo_anim.Play("sailo_emergency");
	}

	private void CheckStock()
	{
		if (sailo_data.grass_stock >= Data.SailoData.GRASS_STOCK_MAX[sailo_data.level])
		{
			Manager.sound.PlaySe(Sound.eSe.CANCEL);
		}
	}

	public void Cut(TouchEvent touch)
	{
		int value = touch.param.value1;
		int value2 = touch.param.value2;
		if (sailo_data.grass_stock >= Data.SailoData.GRASS_STOCK_MAX[sailo_data.level])
		{
			Utils.Play(open.progress, "sailo_progress_full", 1f, (!open.progress.isPlaying) ? 0f : open.progress["sailo_progress_full"].normalizedTime);
		}
		else if (sailo_data.areas[value].grasses[value2].time == 0)
		{
			sailo_data.GrassCut(value, value2);
			if (Data.farm_type == Data.eFarmType.RESORT)
			{
				open.areas[value].grass_anim[value2].Play("sailo_grass_get");
			}
			Manager.sound.GlassCutSe(Data.farm_type);
			if (Data.SailoData.GRASS_STOCK_MAX[sailo_data.level] - sailo_data.grass_stock > 1)
			{
				sailo_data.AddGrassStock(2);
			}
			else
			{
				sailo_data.AddGrassStock(1);
			}
			OccurExp(value, value2);
			grow_list.Add(sailo_data.areas[value].grasses[value2]);
			GameObject gameObject = open.areas[value].grass_anim[value2].gameObject;
			if (open.areas[value].sequence[value2] != null)
			{
				open.areas[value].sequence[value2].Kill();
			}
			Sequence sequence = DOTween.Sequence();
			Vector3 localPosition = gameObject.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = gameObject.transform.localPosition;
			float y = localPosition2.y + 0.1f;
			Vector3 localPosition3 = gameObject.transform.localPosition;
			Vector3 endValue = new Vector3(x, y, localPosition3.z);
			sequence.Append(gameObject.transform.DOLocalMove(endValue, 0.5f));
			endValue = open.areas[value].area_anim.transform.InverseTransformPoint(open.grass_target.position);
			endValue.y += 0.2f;
			sequence.Append(gameObject.transform.DOLocalMove(endValue, 0.2f));
			sequence.Join(gameObject.transform.DOScale(0.5f, 0.2f));
			Data.SailoData.Area.Grass gs = sailo_data.areas[value].grasses[value2];
			sequence.AppendCallback(delegate
			{
				GrassTweenCallback(gs);
			});
			sequence.Play();
			open.areas[value].sequence[value2] = sequence;
		}
	}

	private void OccurExp(int area, int grass)
	{
		if (UnityEngine.Random.Range(0, 5) == 1)
		{
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(Price.ExpSiloGrass(), 1, open.areas[area].grass_anim[grass].transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
		}
	}

	private void GrassTweenCallback(Data.SailoData.Area.Grass grass)
	{
		Animation animation = open.areas[grass.area].grass_anim[grass.id];
		animation.transform.localPosition = open.default_grass_pos[grass.id];
		Utils.Play(animation, "sailo_grass_grow", 0f, 0f);
	}

	private void SetLockIcon(int area)
	{
		work_icon = Common.CreateLockIcon(open.areas[area].root, manager.data.level, Price.OpenSailoLevel(area), area, delegate
		{
			LockIconUp(area);
		}, 22000);
	}

	private void SetConstructIcon(int area)
	{
		work_icon = Common.CreateConstructIcon(open.areas[area].root, area, delegate
		{
			ConstructIconUp(area);
		});
	}

	private void LockIconUp(int area)
	{
		if (prompt == null)
		{
			prompt = Prompt.CreateCoinPrompt(manager.data.coin, Price.OpenSailoPrice(area), open.areas[area].root, Vector2.zero, delegate
			{
				StartUpdateArea(area);
			}, Cancel);
			prompt.SetOrderInLayer(3000);
		}
	}

	private void StartUpdateArea(int area)
	{
		Utils.Log("StartUpdateArea");
		manager.data.SetCoinCount(manager.data.coin - Price.OpenSailoPrice(area));
		UnityEngine.Object.Destroy(prompt.gameObject);
		UnityEngine.Object.Destroy(work_icon.gameObject);
		prompt = null;
		sailo_data.UpdateSairo((ulong)DateTime.Now.Ticks);
		SetConstructIcon(area);
		CreateProgress(sailo_data.sailo_update_time, area);
		progress.SetTextVisibility(visibility: true, new Vector2(0f, -0.204f), Common.TextWhiteTranslucentColor);
	}

	private void Cancel()
	{
		Utils.Log("Cancel");
		UnityEngine.Object.Destroy(prompt.gameObject);
		prompt = null;
	}

	private void FixSailoUpdate(bool video)
	{
		fix_sailo_udpate = true;
		if (video)
		{
			if (progress != null)
			{
				progress.ChangeEndTime(2);
			}
			sailo_data.UpdateSairo((ulong)(DateTime.Now.Ticks - 6000000000L));
			manager.data.Save();
		}
		if (open.anim != null)
		{
			Common.ResetPushIcon(work_icon);
		}
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
	}

	private void ConstructIconUp(int area)
	{
		Utils.Log("Fix : area=" + area + " fix_sailo_udpate=" + fix_sailo_udpate);
		if (fix_sailo_udpate)
		{
			Manager.sound.ClickSound();
			sailo_data.AddGrassArea(area);
			Manager.sound.PlaySe(Sound.eSe.APPEAR);
			Utils.Play(open.areas[area].area_anim, "sailo_area_appear", 1f, 0f);
			Vector2 pos = open.areas[area].root.position;
			pos.y += 0.3f;
			Effect effect = Effect.Run(Resources.Load("Prefab/effect_paper") as GameObject, pos, open.areas[area].root);
			effect.SetOrderInLayer(5000);
			SetGrass(area, set_default_pos: false);
			UnityEngine.Object.Destroy(work_icon.gameObject);
			work_icon = null;
			if (sailo_data.level < 4)
			{
				SetLockIcon(area + 1);
			}
			fix_sailo_udpate = false;
			sailo_data.UpdateSairo(0uL);
			UnityEngine.Object.Destroy(progress.gameObject);
			progress = null;
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
		}
	}

	private void CreateProgress(ulong time, int area)
	{
		GameObject original = Resources.Load("Prefab/progress_1") as GameObject;
		progress = UnityEngine.Object.Instantiate(original, open.areas[area].root, worldPositionStays: false).GetComponent<Progress>();
		progress.transform.localPosition = new Vector2(0f, 0.2f);
		progress.Loop(time, 600, delegate
		{
			FixSailoUpdate(video: false);
		});
		Utils.Log(open.areas[area].root.gameObject.name);
		int sortingOrder = open.areas[area].root.Find("silo_grass_area/guard_front").GetComponent<SpriteRenderer>().sortingOrder;
		progress.SetOrderInLayer(sortingOrder + 300);
		progress.Show();
		SetConstructVideo();
	}

	private string Prefix()
	{
		return tANIM_PREFIX[(int)manager.data.main_type];
	}

	private void CreateSiloBoy(bool flip, int oil)
	{
		if (silo_boy.animator == null)
		{
			GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/" + Prefix()) as GameObject;
			silo_boy.SetAnimator(UnityEngine.Object.Instantiate(original, sailo_anim.transform, worldPositionStays: false).GetComponent<Animator>());
			silo_boy.animator.transform.localPosition = new Vector2(silo_boy.flipX * -0.6f, 0.08f);
			silo_boy.SetVisibility(visibility: false);
			silo_boy.SetFlipX(flip);
		}
		else
		{
			Vector3 localScale = silo_boy.animator.transform.localScale;
			float x = localScale.x;
			Vector2 v = silo_boy.animator.transform.localPosition;
			silo_boy.animator.transform.SetParent(sailo_anim.transform);
			silo_boy.animator.transform.localPosition = v;
			silo_boy.animator.transform.localScale = new Vector3(x, 1f, 1f);
		}
		silo_boy.SetOrderInLayer(oil);
	}

	public void ChangeSiloBoy()
	{
		if (silo_boy.animator != null)
		{
			UnityEngine.Object.Destroy(silo_boy.animator);
			silo_boy.animator = null;
		}
		CreateSiloBoy(flip: false, 0);
	}

	private void BoyGotoSilo()
	{
		if (!IsHungry())
		{
			AnimalsAnimation("silo_animals_full_stay");
		}
		Sequence sequence = DOTween.Sequence();
		silo_boy.animator.transform.localPosition = new Vector2(silo_boy.flipX * -0.6f, 0.08f);
		silo_boy.animator.transform.localScale = new Vector3(silo_boy.flipX, 1f, 1f);
		silo_boy.SetVisibility(visibility: true);
		Utils.Play(silo_boy.animator, Prefix() + "_walk_1", 1f, 0f);
		sequence.Append(ShortcutExtensions.DOLocalMove(endValue: new Vector2(0f, 0.08f), target: silo_boy.animator.transform, duration: 3.5f));
		sequence.AppendCallback(BoySpoutOut);
		sequence.Play();
		silo_boy.spout_outing = true;
	}

	private void BoySpoutOut()
	{
		int grass_stock = sailo_data.grass_stock;
		int num = 0;
		give_grass_list.Clear();
		do
		{
			silo_boy.count--;
			num += DoSpoutOut(silo_boy.count > 0);
		}
		while (sailo_data.grass_stock > 0 && silo_boy.count > 0);
		Sequence sequence = DOTween.Sequence();
		if (grass_stock == 0)
		{
			if (num == 0)
			{
				sequence.AppendCallback(delegate
				{
					AnimalsAnimation("silo_animals_full_stay");
				});
			}
			sequence.Append(silo_boy.animator.transform.DOScale(new Vector3(0f - silo_boy.flipX, 1f, 1f), 0f));
			sequence.AppendCallback(delegate
			{
				Utils.Play(silo_boy.animator, Prefix() + "_walk_1", 1f, 0f);
			});
			sequence.Append(ShortcutExtensions.DOLocalMove(endValue: new Vector2((0f - silo_boy.flipX) * 0.2f, 0.08f), target: silo_boy.animator.transform, duration: 1f));
			sequence.Append(silo_boy.animator.transform.DOScale(new Vector3(silo_boy.flipX, 1f, 1f), 0f));
			sequence.AppendCallback(delegate
			{
				Utils.Play(silo_boy.animator, Prefix() + "_empty", 1f, 0f);
			});
			if (num > 0)
			{
				sequence.AppendCallback(delegate
				{
					AnimalsAnimation("silo_animals_shock");
				});
				sequence.AppendCallback(GiveGrassAll);
			}
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				silo_boy.SetVisibility(visibility: false);
			});
		}
		else if (num == 0)
		{
			sequence.Append(silo_boy.animator.transform.DOScale(new Vector3(0f - silo_boy.flipX, 1f, 1f), 0f));
			sequence.AppendCallback(delegate
			{
				Utils.Play(silo_boy.animator, Prefix() + "_walk_2", 1f, 0f);
			});
			sequence.Append(ShortcutExtensions.DOLocalMove(endValue: new Vector2((0f - silo_boy.flipX) * 0.28f, 0.08f), target: silo_boy.animator.transform, duration: 2f));
			sequence.AppendCallback(delegate
			{
				AnimalsAnimation("silo_animals_full_eat");
			});
			sequence.Append(ShortcutExtensions.DOLocalMove(endValue: new Vector2((0f - silo_boy.flipX) * 0.3f, 0.08f), target: silo_boy.animator.transform, duration: 0.5f));
			sequence.Append(silo_boy.animator.transform.DOScale(new Vector3(silo_boy.flipX, 1f, 1f), 0f));
			sequence.Append(ShortcutExtensions.DOLocalMove(endValue: new Vector2(0f, 0.08f), target: silo_boy.animator.transform, duration: 3f));
			sequence.AppendCallback(delegate
			{
				silo_boy.SetVisibility(visibility: false);
			});
		}
		else
		{
			Vector2 v = new Vector2(silo_boy.flipX * -0.6f, 0.08f);
			sequence.Append(silo_boy.animator.transform.DOScale(new Vector3(0f - silo_boy.flipX, 1f, 1f), 0f));
			sequence.AppendCallback(delegate
			{
				Utils.Play(silo_boy.animator, Prefix() + "_walk_2", 1f, 0f);
			});
			sequence.Append(silo_boy.animator.transform.DOLocalMove(v, 1.2f));
			sequence.AppendCallback(GiveGrassAll);
			sequence.AppendCallback(SetMiniAnimationCB);
			sequence.AppendCallback(delegate
			{
				AnimalsAnimation("silo_animals_eat");
			});
			sequence.AppendCallback(delegate
			{
				silo_boy.SetVisibility(visibility: false);
			});
		}
		sequence.AppendCallback(delegate
		{
			silo_boy.spout_outing = false;
		});
		sequence.Play();
	}

	private void GiveGrassAll()
	{
		for (int i = 0; i < give_grass_list.Count; i++)
		{
			SpoutOut spoutOut = give_grass_list[i];
			map.facility_list[spoutOut.facility].GiveGrass(spoutOut.animal, spoutOut.grass, immediate: false);
		}
	}

	private void SetMiniAnimationCB()
	{
		if (open.animals_events != null)
		{
			open.animals_events.SetFinishCallback(SetFullAnimation);
		}
	}

	private void SetFullAnimation()
	{
		if (!IsHungry())
		{
			AnimalsAnimation("silo_animals_full_stay");
		}
		if (open.animals_events != null)
		{
			open.animals_events.SetFinishCallback(null);
		}
	}

	private void AnimalsAnimation(string name)
	{
		if (open.anim != null)
		{
			Utils.Play(open.animals, name, 1f, 0f);
		}
	}

	public void AssignWorker(Worker.eType type)
	{
		worker.regist = true;
		worker.type = type;
	}

	public void FreeWorker()
	{
		worker.regist = false;
	}

	private void StartAutoCut()
	{
		if (!worker.regist)
		{
			return;
		}
		Vector3 endValue = default(Vector3);
		for (int i = 0; i < sailo_data.level && manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)worker.type]] >= i + 1; i++)
		{
			int num = 0;
			for (int j = 0; j < 15; j++)
			{
				if (num >= 6)
				{
					break;
				}
				if (sailo_data.grass_stock >= Data.SailoData.GRASS_STOCK_MAX[sailo_data.level] || sailo_data.areas[i].grasses[j].time != 0)
				{
					continue;
				}
				sailo_data.GrassCut(i, j);
				int num2 = (Data.SailoData.GRASS_STOCK_MAX[sailo_data.level] - sailo_data.grass_stock <= 1) ? 1 : 2;
				sailo_data.AddGrassStock(num2);
				if (open.anim != null)
				{
					if (Data.farm_type == Data.eFarmType.RESORT)
					{
						open.areas[i].grass_anim[j].Play("sailo_grass_get");
					}
					Manager.sound.GlassCutSe(Data.farm_type);
					OccurExp(i, j);
					GameObject gameObject = open.areas[i].grass_anim[j].gameObject;
					if (open.areas[i].sequence[j] != null)
					{
						open.areas[i].sequence[j].Kill();
					}
					Sequence sequence = DOTween.Sequence();
					Vector3 localPosition = gameObject.transform.localPosition;
					float x = localPosition.x;
					Vector3 localPosition2 = gameObject.transform.localPosition;
					float y = localPosition2.y + 0.1f;
					Vector3 localPosition3 = gameObject.transform.localPosition;
					endValue = new Vector3(x, y, localPosition3.z);
					sequence.Append(gameObject.transform.DOLocalMove(endValue, 0.5f));
					endValue = open.areas[i].area_anim.transform.InverseTransformPoint(open.grass_target.position);
					endValue.y += 0.2f;
					sequence.Append(gameObject.transform.DOLocalMove(endValue, 0.2f));
					sequence.Join(gameObject.transform.DOScale(0.5f, 0.2f));
					Data.SailoData.Area.Grass gs = sailo_data.areas[i].grasses[j];
					sequence.AppendCallback(delegate
					{
						GrassTweenCallback(gs);
					});
					sequence.Play();
					open.areas[i].sequence[j] = sequence;
				}
				grow_list.Add(sailo_data.areas[i].grasses[j]);
				num += num2;
			}
		}
	}
}
