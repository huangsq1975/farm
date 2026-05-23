using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HotelManager : MonoBehaviour
{
	[Serializable]
	public class RoomArea
	{
		private HotelManager hm;

		public int id;

		public GameObject root;

		public TouchEvent work_icon;

		public SpriteRenderer inner;

		public SpriteRenderer deco;

		public PartsController customer;

		public Common.Bag bag;

		private Progress progress;

		public static readonly Vector2 PROGRESS_POS = new Vector2(0f, 0.17f);

		private static readonly Vector2 STAY_PROGRESS_POS = new Vector2(0f, -0.101f);

		public static readonly Vector3 BAG_POS = new Vector3(0f, -0.101f, -10f);

		public RoomArea(HotelManager _hm, GameObject obj, int room_pos)
		{
			hm = _hm;
			root = obj;
			id = room_pos;
			bag = null;
			inner = root.transform.Find("inner").GetComponent<SpriteRenderer>();
			deco = root.transform.Find("decoration").GetComponent<SpriteRenderer>();
			if (hm.hotel_data.level > id)
			{
				Customer.eType type = hm.hotel_data.rooms[id].type;
				if (type != Customer.eType.NONE)
				{
					if (hm.hotel_data.rooms[id].time == 0)
					{
						bag = Common.OccurCoinBagMini(Price.CustomerPrice(type), root.transform.TransformPoint(BAG_POS), FixBill, root.transform);
						return;
					}
					GameObject original = Resources.Load("Prefab/hotel_customer") as GameObject;
					customer = UnityEngine.Object.Instantiate(original, root.transform, worldPositionStays: false).GetComponent<PartsController>();
					customer.transform.localPosition = new Vector3(0.005f, -0.09f, -10f);
					customer.Init(Customer.style[(int)type]);
					customer.Play(PartsController.eAnimType._STAY_1_DOWN, 1f, 0f);
					customer.SetSortingOrderAll(inner.sortingOrder + 1);
					CreateProgress(hm.hotel_data.rooms[id].time);
					progress.SetTextVisibility(visibility: true, new Vector2(0f, -0.161f), Common.TextWhiteTranslucentColor);
				}
				return;
			}
			inner.enabled = false;
			deco.enabled = false;
			if (id - hm.hotel_data.level < 1)
			{
				if (hm.hotel_data.hotel_update_time != 0)
				{
					SetConstructIcon();
					hm.CreateProgress(hm.hotel_data.hotel_update_time, id, root.transform, GetOrderInLayer() + 300, PROGRESS_POS);
					hm.progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
				}
				else
				{
					SetLockIcon();
				}
			}
		}

		public void SetLockIcon()
		{
			RemoveIcon();
			work_icon = Common.CreateLockIcon(root.transform, hm.manager.data.level, Price.OpenRoomLevel(id), id, delegate
			{
				hm.LockIconUp(id);
			}, 22000);
			Transform transform = work_icon.transform;
			Vector3 localPosition = work_icon.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = work_icon.transform.localPosition;
			float y = localPosition2.y - 0.06f;
			Vector3 localPosition3 = work_icon.transform.localPosition;
			transform.localPosition = new Vector3(x, y, localPosition3.z);
		}

		public void SetConstructIcon()
		{
			RemoveIcon();
			work_icon = Common.CreateConstructIcon(root.transform, id, delegate
			{
				hm.ConstructIconUp(id);
			});
			Transform transform = work_icon.transform;
			Vector3 localPosition = work_icon.transform.localPosition;
			transform.localPosition = new Vector3(0f, 0f, localPosition.z);
		}

		public void RemoveIcon()
		{
			if (work_icon != null)
			{
				UnityEngine.Object.Destroy(work_icon.gameObject);
			}
		}

		public int GetOrderInLayer()
		{
			return root.GetComponent<SpriteRenderer>().sortingOrder;
		}

		private void CreateProgress(ulong time)
		{
			GameObject original = Resources.Load("Prefab/progress_1") as GameObject;
			if (progress != null)
			{
				UnityEngine.Object.Destroy(progress.gameObject);
			}
			progress = UnityEngine.Object.Instantiate(original, root.transform, worldPositionStays: false).GetComponent<Progress>();
			progress.Loop(time, 300, StayTimeOver);
			progress.SetOrderInLayer(GetOrderInLayer() + 100);
			progress.transform.localPosition = STAY_PROGRESS_POS;
			progress.Show();
		}

		private void StayTimeOver()
		{
			Customer.eType type = hm.hotel_data.rooms[id].type;
		}

		public void OccurCoin()
		{
			Customer.eType type = hm.hotel_data.rooms[id].type;
			bag = Common.OccurCoinBagMini(Price.CustomerPrice(type), root.transform.TransformPoint(BAG_POS), FixBill, root.transform);
			UnityEngine.Object.Destroy(customer.gameObject);
			UnityEngine.Object.Destroy(progress.gameObject);
		}

		public void FixBill()
		{
			bag = null;
			hm.hotel_data.SetCustomer(0uL, Customer.eType.NONE, id);
			for (int i = 0; i < hm.hotel_data.rooms.Length; i++)
			{
				if (hm.hotel_data.rooms[i].type != Customer.eType.NONE && hm.hotel_data.rooms[i].time == 0)
				{
					return;
				}
			}
			hm.DeleteBagBalloon();
		}
	}

	[Serializable]
	public class HotelCustomer
	{
		public enum eSTATE
		{
			NONE,
			GOTO,
			WAITING,
			GO_BACK
		}

		public const int VISIT_INTERVAL = 60;

		public const int BELL_INTERVAL = 5;

		public Customer.eType type = Customer.eType.NONE;

		public eSTATE state;

		public PartsController controller;

		public Animation bell;

		public float elapsed_time;

		public int room_id = -1;

		private HotelManager hm;

		private BoxCollider2D collider;

		private Vector2 default_pos;

		public Sequence sequence;

		public void Init(GameObject obj, int _room_id, HotelManager hotel_manager)
		{
			hm = hotel_manager;
			room_id = _room_id;
			controller = obj.GetComponent<PartsController>();
			collider = obj.GetComponent<BoxCollider2D>();
			default_pos = controller.transform.position;
			TouchEvent component = obj.GetComponent<TouchEvent>();
			component.ClickUp.AddListener(ComeIn);
		}

		public virtual void SetFlipX(bool flipX)
		{
			controller.SetFlipX(flipX);
		}

		public void Visit(float hotel_x)
		{
			sequence = DOTween.Sequence();
			controller.Play(PartsController.eAnimType._WALK_1_SIDE, 1f, 0f);
			collider.enabled = false;
			SetFlipX(flipX: true);
			sequence.Append(controller.transform.DOMoveX(hotel_x, 3f));
			sequence.AppendCallback(delegate
			{
				SetFlipX(flipX: false);
			});
			sequence.AppendCallback(delegate
			{
				controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
			});
			sequence.AppendCallback(delegate
			{
				state = eSTATE.WAITING;
			});
			sequence.AppendCallback(delegate
			{
				collider.enabled = true;
			});
			sequence.AppendCallback(delegate
			{
				RingBell();
			});
			sequence.AppendInterval(5f);
			sequence.AppendCallback(delegate
			{
				RingBell();
			});
			sequence.AppendInterval(5f);
			sequence.AppendCallback(delegate
			{
				RingBell();
			});
			sequence.AppendInterval(5f);
			sequence.AppendCallback(delegate
			{
				collider.enabled = false;
			});
			sequence.AppendCallback(delegate
			{
				controller.Play(PartsController.eAnimType._WALK_1_SIDE, 1f, 0f);
			});
			sequence.Append(controller.transform.DOMove(new Vector2(default_pos.x, default_pos.y), 3f));
			sequence.AppendCallback(delegate
			{
				Clear();
			});
			sequence.Play();
		}

		private void RingBell()
		{
			if (Manager.events.state == Event.eState.NONE)
			{
				GameObject original = Resources.Load("Prefab/bell") as GameObject;
				bell = UnityEngine.Object.Instantiate(original, controller.transform, worldPositionStays: false).GetComponent<Animation>();
				Utils.SetOrderInLayer(bell.gameObject, controller.parts.rens[1].sortingOrder + 3);
				bell.Play();
				Manager.sound.PlaySe(Sound.eSe.BELL);
			}
		}

		private void ComeIn()
		{
			int num = Price.ExpLodging(type);
			hm.hotel_data.SetCustomer((ulong)DateTime.Now.Ticks, type, room_id);
			if (hm.manager.data.GetReg(Data.CharacterData.eType.CUSTOMER, (int)type) == 0)
			{
				hm.manager.data.SetReg(Data.CharacterData.eType.CUSTOMER, (int)type);
			}
			if (bell != null)
			{
				UnityEngine.Object.Destroy(bell.gameObject);
			}
			sequence.Kill();
			collider.enabled = false;
			sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
			});
			Sequence s = sequence;
			Transform transform = controller.transform;
			Vector3 position = controller.transform.position;
			float x = position.x;
			Vector3 position2 = controller.transform.position;
			s.Append(transform.DOMove(new Vector2(x, position2.y + 0.1f), 1f));
			if (hm.manager.data.AddLevelCondCount(Data.CharacterData.eType.CUSTOMER, (int)type))
			{
				int level = hm.manager.data.character_data[3].contents[(int)type].level;
				Vector2 pos = hm.hotel_exterior.transform.TransformPoint(new Vector2(-0.02f, 0.31f));
				sequence.AppendCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
					Effect.LevelupSmall(level, pos, hm.hotel_exterior.transform, Color.white);
				});
				sequence.AppendInterval(1f);
				num *= 2;
			}
			sequence.AppendCallback(delegate
			{
				Clear();
			});
			sequence.AppendInterval(3f);
			Customer.eType lodging_customer = type;
			int room = room_id;
			sequence.AppendCallback(delegate
			{
				hm.manager.map.GoToFarm(lodging_customer, room, entry: true);
			});
			sequence.AppendCallback(delegate
			{
				hm.CreateLodgingProgress((ulong)DateTime.Now.Ticks, lodging_customer, room, hm.transform);
			});
			sequence.Play();
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(num, 3, controller.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
		}

		public void Clear()
		{
			UnityEngine.Object.Destroy(controller.gameObject);
			controller = null;
			type = Customer.eType.NONE;
			state = eSTATE.NONE;
			elapsed_time = 0f;
		}
	}

	[Serializable]
	public class HotelWorker
	{
		private Manager manager;

		public bool regist;

		public Worker obj;

		public Worker.eType type;

		public HotelRoom hotel_room;

		public Transform parent;

		public HotelWorker(Manager manager, bool regist)
		{
			this.manager = manager;
			this.regist = regist;
			obj = null;
		}

		public void Create()
		{
			if (regist)
			{
				obj = Worker.Create(manager, type, Worker.eWorkPlace.HOTEL, parent);
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

	private Manager manager;

	private Data.HotelData hotel_data;

	public GameObject hotel_inner;

	private SpriteRenderer hotel_icon;

	private SpriteRenderer[] room_deco = new SpriteRenderer[4];

	private TextMesh[] room_id_text = new TextMesh[4];

	private SpriteRenderer[] hotel_star = new SpriteRenderer[3];

	public GameObject hotel_exterior;

	private int hotel_order_in_layer;

	public HotelCustomer hotel_customer;

	private RoomArea[] room_area = new RoomArea[4];

	private Progress progress;

	private GameObject present_video;

	public const int STAY_TIME = 300;

	private Progress[] lodgin_progress = new Progress[4];

	private const int WORK_MAX = 2;

	private Vector2[] const_pos = new Vector2[2]
	{
		new Vector2(-0.265f, -0.036f),
		new Vector2(0.24f, -0.036f)
	};

	private bool[] flipX = new bool[2]
	{
		true,
		false
	};

	private List<Data.ConstructPosition> const_positions = new List<Data.ConstructPosition>(2);

	private Animation bag_balloon;

	public HotelWorker worker;

	private Timer.TimeData video_timer;

	private const int UPDATE_TIME = 600;

	private bool fix_hotel_udpate;

	private Prompt prompt;

	public static readonly int[,] tHOTEL_ROOM_ID = new int[2, 4]
	{
		{
			101,
			102,
			201,
			202
		},
		{
			101,
			102,
			103,
			104
		}
	};

	public void Init(Manager m)
	{
		manager = m;
		hotel_data = manager.data.hotel_data;
		worker = new HotelWorker(m, regist: false);
		SetHotel(hotel_data.level);
		for (int i = 0; i < 2; i++)
		{
			const_positions.Add(new Data.ConstructPosition(flipX[i], manager.transform.InverseTransformPoint(hotel_exterior.transform.TransformPoint(const_pos[i])), land: true, hotel_order_in_layer));
		}
		if (hotel_data.hotel_update_time != 0)
		{
			CreateProgress(hotel_data.hotel_update_time, hotel_data.level, hotel_exterior.transform, hotel_order_in_layer + 300, new Vector2(0f, 0.3f));
			if (Data.farm_type == Data.eFarmType.RESORT)
			{
				progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteResortTranslucentColor);
			}
			else
			{
				progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
			}
		}
		hotel_customer = new HotelCustomer();
		ulong num = (ulong)(DateTime.Now.Ticks - (long)hotel_data.customer_visit_time);
		hotel_customer.elapsed_time = (float)(double)(num / 10000000uL) * 1f % 60f;
		bool flag = false;
		for (int j = 0; j < hotel_data.level; j++)
		{
			Customer.eType type = hotel_data.rooms[j].type;
			if (type != Customer.eType.NONE)
			{
				if (hotel_data.rooms[j].time != 0)
				{
					manager.map.GoToFarm(type, j, entry: false);
					CreateLodgingProgress(hotel_data.rooms[j].time, hotel_data.rooms[j].type, j, base.transform);
				}
				else
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			if (worker.regist)
			{
				StartAutoCollect();
			}
			else
			{
				CreateBagBalloon();
			}
		}
	}

	private void Update()
	{
		if (hotel_data.level > 0)
		{
			Visit();
		}
	}

	private void SetHotel(int level, bool immediate = true)
	{
		if (hotel_exterior != null)
		{
			UnityEngine.Object.Destroy(hotel_exterior.gameObject);
		}
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/hotel_" + level) as GameObject;
		hotel_exterior = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false);
		hotel_order_in_layer = hotel_exterior.transform.Find("contents/building_1").GetComponent<SpriteRenderer>().sortingOrder;
		if (!immediate)
		{
			Animation component = hotel_exterior.GetComponent<Animation>();
			component.Play();
		}
	}

	private void Visit()
	{
		if (hotel_customer.type != Customer.eType.NONE)
		{
			return;
		}
		hotel_customer.elapsed_time += Time.deltaTime;
		if (!(hotel_customer.elapsed_time > 60f))
		{
			return;
		}
		hotel_customer.elapsed_time = 0f;
		hotel_data.SetVisit((ulong)DateTime.Now.Ticks);
		int num = VacantRoom();
		if (num != -1)
		{
			hotel_customer.type = DecideCustomer();
			if (hotel_customer.type != Customer.eType.NONE)
			{
				GameObject original = Resources.Load("Prefab/hotel_customer") as GameObject;
				GameObject obj = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false);
				hotel_customer.Init(obj, num, this);
				hotel_customer.state = HotelCustomer.eSTATE.GOTO;
				hotel_customer.controller.Init(Customer.style[(int)hotel_customer.type]);
				HotelCustomer hotelCustomer = hotel_customer;
				Vector3 position = hotel_exterior.transform.position;
				hotelCustomer.Visit(position.x);
			}
		}
	}

	private Customer.eType DecideCustomer()
	{
		HashSet<Customer.eType> hashSet = new HashSet<Customer.eType>();
		for (int i = 0; i < hotel_data.level; i++)
		{
			if (hotel_data.rooms[i].type != Customer.eType.NONE)
			{
				hashSet.Add(hotel_data.rooms[i].type);
			}
		}
		List<Customer.eType> list = new List<Customer.eType>();
		Common.ConditionCompare compareData = Common.GetCompareData(manager);
		for (int j = 0; j < 17; j++)
		{
			Customer.eType eType = (Customer.eType)j;
			if (!hashSet.Contains(eType) && Common.IsMeetConditions(eType, compareData))
			{
				list.Add(eType);
			}
		}
		if (list.Count == 0)
		{
			return Customer.eType.NONE;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private int VacantRoom()
	{
		for (int i = 0; i < hotel_data.level; i++)
		{
			if (hotel_data.rooms[i].type == Customer.eType.NONE && hotel_data.rooms[i].time == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public void Open()
	{
		if (hotel_inner == null)
		{
			GameObject original = Resources.Load("Prefab/hotel_inner") as GameObject;
			hotel_inner = UnityEngine.Object.Instantiate(original, hotel_exterior.transform, worldPositionStays: false);
			for (int i = 0; i < 4; i++)
			{
				room_area[i] = new RoomArea(this, hotel_inner.transform.Find("room_" + (i + 1)).gameObject, i);
				room_id_text[i] = hotel_inner.transform.Find("room_" + (i + 1) + "/text").GetComponent<TextMesh>();
				room_id_text[i].text = string.Empty + tHOTEL_ROOM_ID[(int)Data.farm_type, i];
				room_deco[i] = hotel_inner.transform.Find("room_" + (i + 1) + "/decoration").GetComponent<SpriteRenderer>();
				room_deco[i].sprite = SpriteManager.GetRoomDeco(Data.farm_type);
			}
			GameObject gameObject = hotel_inner.transform.Find("bg").gameObject;
			TouchEvent component = gameObject.GetComponent<TouchEvent>();
			component.ClickDown.AddListener(Manager.sound.CancelSound);
			component.ClickUp.AddListener(delegate
			{
				Close();
			});
			hotel_icon = hotel_inner.transform.Find("icon").GetComponent<SpriteRenderer>();
			if (hotel_data.level == 0)
			{
				hotel_icon.sprite = null;
			}
			for (int j = 0; j < hotel_star.Length; j++)
			{
				hotel_star[j] = hotel_icon.transform.Find("star_" + (j + 1)).GetComponent<SpriteRenderer>();
				hotel_star[j].sprite = ((hotel_data.level != 0) ? ((hotel_data.level - 1 <= j) ? hotel_star[j].sprite : SpriteManager.GetHotelStar(0)) : null);
			}
			Manager.sound.SetVolumeBgm(0.5f);
			if (worker.regist)
			{
				worker.parent = hotel_inner.transform;
				worker.Create();
				CheckCoinBag();
			}
		}
	}

	public void Close()
	{
		if (!(hotel_inner != null))
		{
			return;
		}
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
		worker.Destroy();
		UnityEngine.Object.Destroy(hotel_inner);
		hotel_inner = null;
		if (hotel_data.hotel_update_time != 0)
		{
			CreateProgress(hotel_data.hotel_update_time, hotel_data.level, hotel_exterior.transform, hotel_order_in_layer + 300, new Vector2(0f, 0.3f));
			if (Data.farm_type == Data.eFarmType.RESORT)
			{
				progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteResortTranslucentColor);
			}
			else
			{
				progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
			}
		}
		if (worker.regist)
		{
			StartAutoCollect();
		}
		Manager.sound.SetVolumeBgm(1f);
	}

	private bool SetConstructVideo()
	{
		if (hotel_data.hotel_update_time != 0 && hotel_inner != null)
		{
			int num = 600 - (int)((ulong)(DateTime.Now.Ticks - (long)hotel_data.hotel_update_time) / 10000000uL);
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
			present_video = Common.CreatePresentConstructBox(hotel_inner.transform, new Vector3(0.85f, -0.17f, -30f), 22000, delegate
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
		Utils.Log("Hotel : PlayVideoCompleted");
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
		FixHotelUpdate(video: true);
		manager.LoadVideo();
	}

	public void PlayVideoFailed()
	{
		Utils.Log("Hotel : PlayVideoFailed");
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
		manager.LoadVideo();
		video_timer = Timer.Create((ulong)DateTime.Now.Ticks, 3, ShowPresent);
	}

	private void CreateProgress(ulong time, int table_id, Transform t_parent, int order_in_layer, Vector2 pos)
	{
		GameObject original = Resources.Load("Prefab/progress_1") as GameObject;
		if (progress != null)
		{
			UnityEngine.Object.Destroy(progress.gameObject);
		}
		else
		{
			manager.main.RegistConstructQueue(const_positions);
		}
		SetConstructVideo();
		progress = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Progress>();
		SpriteRenderer component = progress.transform.Find("text/bg").GetComponent<SpriteRenderer>();
		if (Data.farm_type == Data.eFarmType.RESORT)
		{
			component.gameObject.SetActive(value: true);
		}
		progress.Loop(time, 600, delegate
		{
			FixHotelUpdate(video: false);
		});
		progress.SetOrderInLayer(order_in_layer);
		progress.transform.localPosition = pos;
		progress.Show();
	}

	private void CreateLodgingProgress(ulong time, Customer.eType type, int room_id, Transform t_parent)
	{
		GameObject original = Resources.Load("Prefab/progress_1") as GameObject;
		if (lodgin_progress[room_id] != null)
		{
			UnityEngine.Object.Destroy(progress.gameObject);
		}
		lodgin_progress[room_id] = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Progress>();
		lodgin_progress[room_id].gameObject.name = "progress_room_" + (room_id + 1);
		lodgin_progress[room_id].Loop(time, 300, delegate
		{
			EndLodging(type, room_id);
		});
	}

	private void EndLodging(Customer.eType type, int room_id)
	{
		manager.map.ReturnFromFarm(type);
		UnityEngine.Object.Destroy(lodgin_progress[room_id].gameObject);
		lodgin_progress[room_id] = null;
	}

	private void FixHotelUpdate(bool video)
	{
		if (!fix_hotel_udpate)
		{
			fix_hotel_udpate = true;
			if (video)
			{
				if (progress != null)
				{
					progress.ChangeEndTime(2);
					manager.data.interstitial_ON = false;
				}
				hotel_data.UpdateHotel((ulong)(DateTime.Now.Ticks - 6000000000L));
				manager.data.Save();
			}
			manager.main.NotifyConstruct(const_positions);
		}
		if (hotel_inner != null)
		{
			Common.ResetPushIcon(room_area[hotel_data.level].work_icon);
		}
		if (video_timer != null)
		{
			Timer.Remove(video_timer);
			video_timer = null;
		}
	}

	private void UpdateHotel(int room_id)
	{
		manager.data.SetCoinCount(manager.data.coin - Price.OpenRoomPrice(room_id));
		ulong ticks = (ulong)DateTime.Now.Ticks;
		hotel_data.UpdateHotel(ticks);
		CreateProgress(ticks, room_id, room_area[room_id].root.transform, room_area[room_id].GetOrderInLayer() + 300, RoomArea.PROGRESS_POS);
		if (Data.farm_type == Data.eFarmType.RESORT)
		{
			progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteResortTranslucentColor);
		}
		else
		{
			progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
		}
		UnityEngine.Object.Destroy(prompt.gameObject);
		room_area[room_id].SetConstructIcon();
	}

	public void Cancel()
	{
		UnityEngine.Object.Destroy(prompt.gameObject);
	}

	public void LockIconUp(int room_id)
	{
		prompt = Prompt.CreateCoinPrompt(pos: new Vector2(-0.08f, -0.48f), coin: manager.data.coin, need_coin: Price.OpenRoomPrice(room_id), t_parent: hotel_inner.transform, call_ok: delegate
		{
			UpdateHotel(room_id);
		}, call_cancel: Cancel);
		prompt.SetOrderInLayer(1000);
	}

	public void DownLockIcon(TouchEvent touch)
	{
		Animation component = touch.GetComponent<Animation>();
		component.Play();
	}

	private void ConstructIconUp(int room_id)
	{
		if (fix_hotel_udpate)
		{
			Manager.sound.ClickSound();
			UnityEngine.Object.Destroy(progress.gameObject);
			progress = null;
			hotel_data.AddRoom();
			hotel_data.UpdateHotel(0uL);
			Close();
			SetHotel(hotel_data.level, immediate: false);
			fix_hotel_udpate = false;
			Vector3 position = hotel_exterior.transform.position;
			float x = position.x;
			Vector3 position2 = hotel_exterior.transform.position;
			Vector2 pos = new Vector2(x, position2.y + 0.5f);
			Manager.sound.PlaySe(Sound.eSe.APPEAR);
			Effect.Run(Resources.Load("Prefab/effect_paper") as GameObject, pos, hotel_exterior.transform);
			if (hotel_data.level < 4)
			{
				room_area[hotel_data.level].SetLockIcon();
			}
			hotel_customer.elapsed_time = 55f;
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
		}
	}

	private void CreateBagBalloon()
	{
		DeleteBagBalloon();
		GameObject original = Resources.Load("Prefab/feel_bag") as GameObject;
		bag_balloon = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<Animation>();
		Utils.Play(bag_balloon, "op_appear", 1f, 0f);
	}

	private void DeleteBagBalloon()
	{
		if (bag_balloon != null)
		{
			bag_balloon.GetComponent<AnimEvent>().auto_destroy = true;
			Utils.Play(bag_balloon, "op_appear", -1f, 1f);
			bag_balloon = null;
		}
	}

	public void CustomerReturnRoom(Customer.eType type, int room_id)
	{
		hotel_data.SetCustomer(0uL, type, room_id);
		if (hotel_inner == null)
		{
			if (worker.regist)
			{
				StartAutoCollect();
			}
			else
			{
				CheckPaymentAndCreateBalloon();
			}
			return;
		}
		room_area[room_id].OccurCoin();
		if (worker.obj != null)
		{
			worker.obj.RegistCoinBagQueue(room_id);
		}
	}

	private void CheckPaymentAndCreateBalloon()
	{
		int num = 0;
		while (true)
		{
			if (num < hotel_data.rooms.Length)
			{
				if (hotel_data.rooms[num].type != Customer.eType.NONE && hotel_data.rooms[num].time == 0)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		CreateBagBalloon();
	}

	public void AssignWorker(Worker.eType type)
	{
		worker.regist = true;
		worker.type = type;
		DeleteBagBalloon();
		StartAutoCollect();
	}

	public void FreeWorker()
	{
		worker.regist = false;
		CheckPaymentAndCreateBalloon();
	}

	public void LevelUpWorker()
	{
		StartAutoCollect();
	}

	private void CheckCoinBag()
	{
		if (!worker.regist || !(worker.obj != null))
		{
			return;
		}
		for (int i = 0; i < 4 && manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)worker.type]] >= i + 1; i++)
		{
			if (room_area[i].bag != null)
			{
				worker.obj.RegistCoinBagQueue(i);
			}
		}
	}

	public void CollectCoinBag(int room_id)
	{
		if (room_area[room_id].bag != null && room_area[room_id].bag.touch.GetEnabled())
		{
			room_area[room_id].bag.touch.ClickUp.Invoke();
		}
	}

	private bool StartAutoCollect()
	{
		bool result = false;
		if (hotel_inner == null && worker.regist)
		{
			result = true;
			for (int i = 0; i < hotel_data.rooms.Length && manager.data.worker_data.worker_level[Convert.WorkerTypeToIndex[(int)worker.type]] >= i + 1; i++)
			{
				if (!(worker.hotel_room == null))
				{
					break;
				}
				if (hotel_data.rooms[i].type != Customer.eType.NONE && hotel_data.rooms[i].time == 0)
				{
					DeleteBagBalloon();
					worker.hotel_room = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/hotel_room")).GetComponent<HotelRoom>();
					worker.hotel_room.Init(this, Price.CustomerPrice(hotel_data.rooms[i].type), worker.type, i);
					hotel_data.SetCustomer(0uL, Customer.eType.NONE, i);
				}
			}
			if (worker.hotel_room == null)
			{
				CheckPaymentAndCreateBalloon();
			}
		}
		return result;
	}

	public void AutoCollected()
	{
		worker.hotel_room = null;
		StartAutoCollect();
	}
}
