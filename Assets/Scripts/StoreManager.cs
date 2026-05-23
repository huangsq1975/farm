using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoreManager : MonoBehaviour
{
	[Serializable]
	public class BasketArea
	{
		private const string ANIM_NAME = "store_basket_show";

		private Animation anim;

		private SpriteRenderer front;

		private SpriteRenderer back;

		private SpriteRenderer shadow;

		private SpriteRenderer max;

		public SpriteRenderer[] items;

		public int open_item_count;

		public BasketArea(GameObject obj, Data.StoreData.Table.Basket basket)
		{
			anim = obj.GetComponent<Animation>();
			front = obj.GetComponent<SpriteRenderer>();
			back = front.transform.Find("back").GetComponent<SpriteRenderer>();
			shadow = front.transform.Find("shadow").GetComponent<SpriteRenderer>();
			max = front.transform.Find("max").GetComponent<SpriteRenderer>();
			items = new SpriteRenderer[Data.StoreData.Table.Basket.GetItemMax(basket.type)];
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = front.transform.Find("item_" + (i + 1)).GetComponent<SpriteRenderer>();
				SetSprite(basket, i);
			}
			Transform transform = front.transform;
			Vector3 localPosition = front.transform.localPosition;
			float x = localPosition.x + 0.3f * (float)basket.id;
			Vector3 localPosition2 = front.transform.localPosition;
			transform.localPosition = new Vector2(x, localPosition2.y);
		}

		public void SetSprite(Data.StoreData.Table.Basket basket, int item_pos)
		{
			if (basket.items[item_pos].value == -1)
			{
				items[item_pos].enabled = false;
			}
			else
			{
				open_item_count++;
				items[item_pos].enabled = true;
				if (basket.type == Data.eHARVEST.FISH)
				{
					items[item_pos].sprite = SpriteManager.GetHarvest((Fish.eType)basket.items[item_pos].value);
				}
				else
				{
					items[item_pos].sprite = SpriteManager.GetHarvest((FarmAnimal.eType)basket.items[item_pos].value);
				}
			}
			SetMax(basket);
		}

		private void SetMax(Data.StoreData.Table.Basket basket)
		{
			if (basket.item_count == basket.GetItemMax())
			{
				max.enabled = true;
			}
			else
			{
				max.enabled = false;
			}
		}

		public void Show(bool immediate)
		{
			anim["store_basket_show"].normalizedTime = 0f;
			if (immediate)
			{
				anim["store_basket_show"].normalizedTime = 1f;
			}
			anim["store_basket_show"].speed = 1f;
			anim.Play();
		}

		public void Hide()
		{
			anim["store_basket_show"].normalizedTime = 1f;
			anim["store_basket_show"].speed = -1f;
			anim.Play();
		}
	}

	[Serializable]
	public class TableArea
	{
		private StoreManager sm;

		public GameObject root;

		public TouchEvent work_icon;

		public BasketArea[] baskets_area = new BasketArea[6];

		private int id;

		public static readonly Vector2 PROGRESS_POS = new Vector2(0f, 0.17f);

		public TableArea(StoreManager _sm, GameObject obj, int table_pos)
		{
			sm = _sm;
			root = obj;
			id = table_pos;
			if (sm.store_data.level > id)
			{
				int num = 0;
				for (int i = 0; i < 6; i++)
				{
					Data.StoreData.Table.Basket basket = sm.store_data.tables[id].baskets[i];
					if (basket.type != Data.eHARVEST.NONE)
					{
						ShowBasket(basket);
						num++;
					}
				}
				if (num == 0)
				{
					SetSoldoutIcon();
				}
				return;
			}
			obj.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
			if (id - sm.store_data.level < 1)
			{
				if (sm.store_data.store_update_time != 0)
				{
					SetConstructIcon();
					sm.CreateProgress(sm.store_data.store_update_time, id, root.transform, GetOrderInLayer() + 300, PROGRESS_POS);
					sm.progress.SetTextVisibility(visibility: true, new Vector2(0f, -0.217f), Common.TextWhiteTranslucentColor, 25000);
				}
				else
				{
					SetLockIcon();
				}
			}
		}

		public void ShowBasket(Data.StoreData.Table.Basket basket)
		{
			baskets_area[basket.id] = new BasketArea(UnityEngine.Object.Instantiate(sm.prefab_basket[(int)basket.type], root.transform, worldPositionStays: false), basket);
			SetBasket(active: true, basket, immediate: true);
		}

		public void SetLockIcon()
		{
			RemoveIcon();
			work_icon = Common.CreateLockIcon(root.transform, sm.manager.data.level, Price.OpenTableLevel(id), id, delegate
			{
				sm.LockIconUp(id);
			}, 22000);
			Transform transform = work_icon.transform;
			Vector3 localPosition = work_icon.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = work_icon.transform.localPosition;
			float y = localPosition2.y - 0.02f;
			Vector3 localPosition3 = work_icon.transform.localPosition;
			transform.localPosition = new Vector3(x, y, localPosition3.z);
		}

		public void SetConstructIcon()
		{
			RemoveIcon();
			work_icon = Common.CreateConstructIcon(root.transform, id, delegate
			{
				sm.ConstructIconUp(id);
			});
			Transform transform = work_icon.transform;
			Vector3 localPosition = work_icon.transform.localPosition;
			transform.localPosition = new Vector3(0f, 0f, localPosition.z);
		}

		public void SetSoldoutIcon()
		{
			RemoveIcon();
			GameObject original = Resources.Load("Prefab/soldout_icon") as GameObject;
			work_icon = UnityEngine.Object.Instantiate(original, root.transform, worldPositionStays: false).GetComponent<TouchEvent>();
			Transform transform = work_icon.transform;
			Vector3 localPosition = work_icon.transform.localPosition;
			transform.localPosition = new Vector3(0f, 0.05f, localPosition.z);
		}

		public void RemoveIcon()
		{
			if (work_icon != null)
			{
				UnityEngine.Object.Destroy(work_icon.gameObject);
			}
		}

		public void SetBasket(bool active, Data.StoreData.Table.Basket basket, bool immediate)
		{
			if (active)
			{
				baskets_area[basket.id].Show(immediate);
			}
			else
			{
				baskets_area[basket.id].Hide();
			}
		}

		public void SetItemSprite(Data.StoreData.Table.Basket basket, int item_pos)
		{
			baskets_area[basket.id].SetSprite(basket, item_pos);
		}

		public int GetOrderInLayer()
		{
			return root.GetComponent<SpriteRenderer>().sortingOrder;
		}
	}

	public class PresentVideo
	{
		public GameObject obj;

		public Timer.TimeData timer;

		public void Clear()
		{
			if (obj != null)
			{
				UnityEngine.Object.Destroy(obj);
				obj = null;
			}
			if (timer != null)
			{
				Timer.Remove(timer);
				timer = null;
			}
		}
	}

	public class BuyItem
	{
		public Data.eHARVEST harvest = Data.eHARVEST.NONE;

		public Sprite item_sprite;

		public SpriteRenderer item;

		public int coin;

		public BuyItem(Data.eHARVEST h)
		{
			harvest = h;
		}
	}

	public static int[] SEC = new int[4]
	{
		7,
		6,
		5,
		5
	};

	public const ulong CUSTOMER_INTERVAL = 10000000uL;

	private GameObject[] prefab_basket = new GameObject[7];

	private Manager manager;

	private Data.StoreData store_data;

	public GameObject store_inner;

	public GameObject store_exterior;

	private int store_order_in_layer;

	private List<StoreCustomer> store_customer = new List<StoreCustomer>();

	private TableArea[] table_area = new TableArea[4];

	private bool product_set = true;

	private Common.Bag stock_earning;

	private static readonly Vector3 PAY_BAG_POS = new Vector3(0.83f, 1.66f, -10f);

	private Progress progress;

	private PresentVideo present_video = new PresentVideo();

	private PresentVideo present_video_exterior = new PresentVideo();

	private PresentVideo present_video_construct = new PresentVideo();

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

	public List<Data.ConstructPosition> const_positions = new List<Data.ConstructPosition>(2);

	private const int OIL_BASE = 4;

	private const int OIL_COUNT_MAX = 4;

	private int oil_count = 1;

	private static readonly Vector3 PRESENT_POS_INNER = new Vector3(-1.74f, -0.47f, -30f);

	private static readonly Vector3 PRESENT_POS_EXTERIOR = new Vector3(-0.275f, 0.184f, -30f);

	private static readonly Dictionary<FarmAnimal.eType, Data.eHARVEST> tFARM_TO_HARVEST = new Dictionary<FarmAnimal.eType, Data.eHARVEST>
	{
		{
			FarmAnimal.eType.SHEEP_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.SHEEP_2,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.COW_1,
			Data.eHARVEST.MILK
		},
		{
			FarmAnimal.eType.COW_2,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.ALPACA_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.ALPACA_2,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.CHICKEN_1,
			Data.eHARVEST.EGG
		},
		{
			FarmAnimal.eType.CHICKEN_2,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.CHICKEN_3,
			Data.eHARVEST.EGG
		},
		{
			FarmAnimal.eType.HONEY_1,
			Data.eHARVEST.HONEY
		},
		{
			FarmAnimal.eType.HONEY_2,
			Data.eHARVEST.HONEY
		},
		{
			FarmAnimal.eType.GOAT_1,
			Data.eHARVEST.MILK
		},
		{
			FarmAnimal.eType.PIG_1,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.PIG_2,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.TURKEY_1,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.WATER_BUFFALO_1,
			Data.eHARVEST.MILK
		},
		{
			FarmAnimal.eType.WATER_BUFFALO_2,
			Data.eHARVEST.MILK
		},
		{
			FarmAnimal.eType.BLUE_WHALE_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.SPERM_WHALE_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.SEA_OTTER_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.SHARK_1,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.CROCODILE_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.TURTLE_1,
			Data.eHARVEST.FUR
		},
		{
			FarmAnimal.eType.GIANT_SQUID_1,
			Data.eHARVEST.MEAT
		},
		{
			FarmAnimal.eType.FRUIT_1,
			Data.eHARVEST.FRUIT
		},
		{
			FarmAnimal.eType.FRUIT_2,
			Data.eHARVEST.FRUIT
		},
		{
			FarmAnimal.eType.FRUIT_3,
			Data.eHARVEST.FRUIT
		}
	};

	private const int UPDATE_TIME = 600;

	private bool fix_store_udpate;

	private Prompt prompt;

	private const int STOCK_MANIFEST = 9999;

	public void Init(Manager m)
	{
		manager = m;
		store_data = manager.data.store_data;
		for (int i = 0; i < prefab_basket.Length; i++)
		{
			prefab_basket[i] = (Resources.Load("Prefab/store_basket_" + (i + 1)) as GameObject);
		}
		SetStore(store_data.level);
		for (int j = 0; j < 2; j++)
		{
			const_positions.Add(new Data.ConstructPosition(flipX[j], manager.transform.InverseTransformPoint(store_exterior.transform.TransformPoint(const_pos[j])), land: true, store_order_in_layer));
		}
		product_set = true;
		if (store_data.stock_earnings > 0)
		{
			stock_earning = Common.OccurCoinBag(store_data.stock_earnings, base.transform.TransformPoint(PAY_BAG_POS), TouchStockEarnings, base.transform);
		}
		if (store_data.store_update_time != 0)
		{
			CreateProgress(store_data.store_update_time, store_data.level, store_exterior.transform, store_order_in_layer + 300, new Vector2(0f, 0.3f));
			progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
		}
		ulong num = (ulong)(10000000L * (long)SEC[store_data.level - 1]);
		ulong num2 = (ulong)(DateTime.Now.Ticks + (long)num);
		if (store_data.next_customer_time > num2)
		{
			Utils.Log("RESET");
			store_data.SetNextCustomerTime(num2);
		}
	}

	private void Update()
	{
		Visit();
	}

	private void SetStore(int level, bool immediate = true)
	{
		if (store_exterior != null)
		{
			UnityEngine.Object.Destroy(store_exterior.gameObject);
		}
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/store_" + level) as GameObject;
		store_exterior = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false);
		store_order_in_layer = store_exterior.transform.Find("contents/building_1").GetComponent<SpriteRenderer>().sortingOrder;
		MapIconLabel.Set(store_exterior.transform, "商店", store_order_in_layer + 1);
		if (!immediate)
		{
			Animation component = store_exterior.GetComponent<Animation>();
			component.Play();
		}
	}

	private void Visit()
	{
		ulong ticks = (ulong)DateTime.Now.Ticks;
		if (store_data.next_customer_time >= ticks)
		{
			return;
		}
		ulong num = (ulong)(10000000L * (long)SEC[store_data.level - 1]);
		ulong num2 = ticks - store_data.next_customer_time;
		int num3 = (int)(num2 / num);
		if (Manager.events.state == Event.eState.NONE && num3 > 0)
		{
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				if (product_set)
				{
					BuyItem buyItem = BuyHarvest();
					if (buyItem != null)
					{
						num4 += buyItem.coin;
					}
				}
			}
			if (num4 > 0)
			{
				Stock(num4);
			}
			store_data.SetNextCustomerTime(ticks + num);
		}
		else if ((Manager.events.state == Event.eState.NONE || Manager.events.state == Event.eState.TUTORIAL_STORE) && product_set && IsThereAnimals())
		{
			store_data.SetNextCustomerTime(ticks + num);
			GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/store_customer") as GameObject;
			StoreCustomer component = UnityEngine.Object.Instantiate(original, base.transform.parent, worldPositionStays: false).GetComponent<StoreCustomer>();
			StoreCustomer storeCustomer = component;
			Vector3 position = store_exterior.transform.position;
			storeCustomer.Run(this, position.x, GetOrderInLayer());
			store_customer.Add(component);
		}
	}

	private int GetOrderInLayer()
	{
		int result = store_order_in_layer + oil_count * 4;
		oil_count = ((oil_count + 1 >= 4) ? 1 : (oil_count + 1));
		return result;
	}

	public void DeleteStoreCustomer(StoreCustomer customer)
	{
		UnityEngine.Object.Destroy(customer.gameObject);
		store_customer.Remove(customer);
	}

	private bool IsThereAnimals()
	{
		for (int i = 0; i < manager.map.facility_list.Count; i++)
		{
			if (manager.map.facility_list[i].AnimalCount() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void Open()
	{
		if (store_inner == null)
		{
			present_video_exterior.Clear();
			GameObject original = Resources.Load("Prefab/store_inner") as GameObject;
			store_inner = UnityEngine.Object.Instantiate(original, store_exterior.transform, worldPositionStays: false);
			for (int i = 0; i < 4; i++)
			{
				table_area[i] = new TableArea(this, store_inner.transform.Find("table_" + (i + 1)).gameObject, i);
			}
			GameObject gameObject = store_inner.transform.Find("bg").gameObject;
			TouchEvent component = gameObject.GetComponent<TouchEvent>();
			component.ClickDown.AddListener(Manager.sound.CancelSound);
			component.ClickUp.AddListener(delegate
			{
				Close();
			});
			SetConstructVideo();
			SetSalesVideo();
			manager.LoadInterstitial();
			Manager.sound.SetVolumeBgm(0.5f);
		}
	}

	public void Close()
	{
		if (store_inner != null)
		{
			if (manager.data.interstitial_ON)
			{
				manager.ShowInterstitial();
			}
			present_video.Clear();
			present_video_construct.Clear();
			UnityEngine.Object.Destroy(store_inner);
			store_inner = null;
			if (store_data.store_update_time != 0)
			{
				CreateProgress(store_data.store_update_time, store_data.level, store_exterior.transform, store_order_in_layer + 300, new Vector2(0f, 0.3f));
				progress.SetTextVisibility(visibility: true, new Vector2(0f, 0.0855f), Common.TextWhiteTranslucentColor);
			}
			Manager.sound.SetVolumeBgm(1f);
		}
	}

	private bool SetConstructVideo()
	{
		if (store_data.store_update_time != 0 && store_inner != null)
		{
			int num = 600 - (int)((ulong)(DateTime.Now.Ticks - (long)store_data.store_update_time) / 10000000uL);
			Utils.Log("Construct reminder_sec= " + num);
			if ((float)num >= 180f)
			{
				present_video_construct.Clear();
				if (manager.LoadVideo())
				{
					present_video_construct.timer = Timer.Create((ulong)DateTime.Now.Ticks, 0, ShowPresent);
				}
				return true;
			}
		}
		return false;
	}

	private void ShowPresent()
	{
		if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			present_video_construct.Clear();
			Vector2 vector = store_inner.transform.InverseTransformPoint(table_area[store_data.level].root.transform.position);
			present_video_construct.obj = Common.CreatePresentConstructBox(store_inner.transform, new Vector3(-0.3f, vector.y, -30f), 22000, delegate
			{
				manager.PlayVideo(PlayVideoCompleted, PlayVideoFailed);
			}, null);
			return;
		}
		present_video_construct.Clear();
		present_video_construct.timer = Timer.Create((ulong)DateTime.Now.Ticks, 1, ShowPresent);
		if (manager.load_video.state == Manager.VideoRwd.eState.NONE && !manager.LoadVideo())
		{
			present_video_construct.Clear();
		}
	}

	public void PlayVideoCompleted()
	{
		Utils.Log("Store : PlayVideoCompleted");
		present_video_construct.Clear();
		FixStoreUpdate(video: true);
		manager.LoadVideo();
	}

	public void PlayVideoFailed()
	{
		Utils.Log("Store : PlayVideoFailed");
		present_video_construct.Clear();
		manager.LoadVideo();
		present_video_construct.timer = Timer.Create((ulong)DateTime.Now.Ticks, 3, ShowPresent);
	}

	private void SetSalesVideo()
	{
		int num = 0;
		int num2 = (int)((float)(48 * store_data.level) * 0.2f);
		for (int i = 0; i < store_data.level; i++)
		{
			for (int j = 0; j < table_area[i].baskets_area.Length; j++)
			{
				if (table_area[i].baskets_area[j] == null)
				{
					continue;
				}
				num += table_area[i].baskets_area[j].open_item_count;
				if (num >= num2)
				{
					present_video.Clear();
					if (manager.LoadVideo())
					{
						present_video.timer = Timer.Create((ulong)DateTime.Now.Ticks, 0, ShowPresentSale);
					}
					break;
				}
			}
		}
	}

	private void ShowPresentSale()
	{
		if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			present_video.Clear();
			UnityAction play_completed = delegate
			{
				PlayVideoSalesCompleted(present_video);
			};
			UnityAction play_failed = delegate
			{
				PlayVideoSalesFailed(present_video, delegate
				{
					PlayVideoSalesFailed(present_video, ShowPresentSale);
				});
			};
			present_video.obj = Common.CreatePresentStoreBox(store_exterior.transform, PRESENT_POS_INNER, 22000, delegate
			{
				manager.PlayVideo(play_completed, play_failed);
			}, null);
			return;
		}
		Utils.Log("[FARM] ShowPresentSale: state= " + manager.load_video.state);
		present_video.Clear();
		present_video.timer = Timer.Create((ulong)DateTime.Now.Ticks, 2, ShowPresentSale);
		if (manager.load_video.state == Manager.VideoRwd.eState.NONE && !manager.LoadVideo())
		{
			present_video.Clear();
		}
	}

	private void SetSalesVideoExterior()
	{
		present_video_exterior.Clear();
		if (manager.LoadVideo())
		{
			present_video_exterior.timer = Timer.Create((ulong)DateTime.Now.Ticks, 0, ShowPresentSaleExterior);
		}
	}

	private void ShowPresentSaleExterior()
	{
		if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
		{
			present_video_exterior.Clear();
			UnityAction play_completed = delegate
			{
				Sequence sequence = DOTween.Sequence();
				sequence.AppendCallback(Open);
				sequence.AppendInterval(1f);
				sequence.AppendCallback(delegate
				{
					PlayVideoSalesCompleted(present_video_exterior);
				});
				sequence.Play();
			};
			UnityAction play_failed = delegate
			{
				PlayVideoSalesFailed(present_video_exterior, delegate
				{
					PlayVideoSalesFailed(present_video_exterior, ShowPresentSaleExterior);
				});
			};
			present_video_exterior.obj = Common.CreatePresentStoreBox(store_exterior.transform, PRESENT_POS_EXTERIOR, 22000, delegate
			{
				manager.PlayVideo(play_completed, play_failed);
			}, null);
			present_video_exterior.timer = Timer.Create((ulong)DateTime.Now.Ticks, 30, delegate
			{
				present_video_exterior.Clear();
			});
		}
		else
		{
			present_video_exterior.Clear();
			present_video_exterior.timer = Timer.Create((ulong)DateTime.Now.Ticks, 2, ShowPresentSaleExterior);
			if (manager.load_video.state == Manager.VideoRwd.eState.NONE && !manager.LoadVideo())
			{
				present_video_exterior.Clear();
			}
		}
	}

	public void PlayVideoSalesCompleted(PresentVideo pv)
	{
		Utils.Log("Store : PlayVideoSalesCompleted");
		pv.Clear();
		DoSpotSale();
		manager.LoadVideo();
	}

	public void PlayVideoSalesFailed(PresentVideo pv, UnityAction recall)
	{
		Utils.Log("Store : PlayVideoSalesFailed");
		pv.Clear();
		if (manager.LoadVideo())
		{
			pv.timer = Timer.Create((ulong)DateTime.Now.Ticks, 3, recall.Invoke);
		}
	}

	public bool AddHarvest(FarmAnimal.eType type)
	{
		if (type == FarmAnimal.eType.HORSE_1 || type == FarmAnimal.eType.HORSE_2 || type == FarmAnimal.eType.HORSE_3 || type == FarmAnimal.eType.DOLPHIN_1 || type == FarmAnimal.eType.DOLPHIN_2 || type == FarmAnimal.eType.DOLPHIN_3 || type == FarmAnimal.eType.SEA_LION_1 || type == FarmAnimal.eType.WALRUS_1 || type == FarmAnimal.eType.KILLER_WHALE_1)
		{
			return false;
		}
		Data.eHARVEST harvest = tFARM_TO_HARVEST[type];
		return AddHarvest(harvest, (int)type);
	}

	public bool AddHarvest(Fish.eType type)
	{
		return AddHarvest(Data.eHARVEST.FISH, (int)type);
	}

	private bool AddHarvest(Data.eHARVEST harvest, int item)
	{
		for (int i = 0; i < store_data.tables.Count; i++)
		{
			Data.StoreData.Table.Basket basket = GetBasket(store_data.tables[i], harvest);
			if (basket == null)
			{
				continue;
			}
			if (basket.type == Data.eHARVEST.NONE)
			{
				store_data.AddBasket(i, basket.id, harvest);
				store_data.AddItem(i, basket.id, 0, item);
				if (store_inner != null)
				{
					table_area[i].ShowBasket(basket);
					table_area[i].SetBasket(active: true, basket, immediate: false);
					table_area[i].SetItemSprite(basket, 0);
					table_area[i].RemoveIcon();
				}
			}
			else
			{
				Data.StoreData.Table.Basket.Item item2 = basket.items.Find((Data.StoreData.Table.Basket.Item n) => n.value == -1);
				store_data.AddItem(i, basket.id, item2.id, item);
				if (store_inner != null)
				{
					table_area[i].SetItemSprite(basket, item2.id);
				}
			}
			product_set = true;
			return true;
		}
		Utils.Log("[ADD] No capacity.");
		if (store_inner == null && present_video_exterior.obj == null)
		{
			SetSalesVideoExterior();
		}
		return false;
	}

	public BuyItem BuyHarvest()
	{
		List<Data.StoreData.Table> list = new List<Data.StoreData.Table>();
		for (int i = 0; i < store_data.tables.Count; i++)
		{
			list.Add(store_data.tables[i]);
		}
		while (list.Count > 0)
		{
			Data.StoreData.Table table = list[UnityEngine.Random.Range(0, list.Count)];
			List<Data.StoreData.Table.Basket> list2 = new List<Data.StoreData.Table.Basket>();
			for (int j = 0; j < table.baskets.Count; j++)
			{
				if (table.baskets[j].type != Data.eHARVEST.NONE)
				{
					list2.Add(table.baskets[j]);
				}
			}
			if (list2.Count == 0)
			{
				list.Remove(table);
				list2.Clear();
				continue;
			}
			while (list2.Count > 0)
			{
				Data.StoreData.Table.Basket basket = list2[UnityEngine.Random.Range(0, list2.Count)];
				if (basket.item_count == 0)
				{
					list2.Remove(basket);
					continue;
				}
				List<int> list3 = new List<int>();
				for (int k = 0; k < basket.items.Count; k++)
				{
					if (basket.items[k].value != -1)
					{
						list3.Add(basket.items[k].id);
					}
				}
				int num = list3[UnityEngine.Random.Range(0, list3.Count)];
				BuyItem buyItem = new BuyItem(basket.type);
				if (basket.type == Data.eHARVEST.FISH)
				{
					buyItem.item_sprite = SpriteManager.GetHarvest((Fish.eType)basket.items[num].value);
					buyItem.coin = Price.HarvestPrice((Fish.eType)basket.items[num].value);
				}
				else
				{
					buyItem.item_sprite = SpriteManager.GetHarvest((FarmAnimal.eType)basket.items[num].value);
					buyItem.coin = Price.HarvestPrice((FarmAnimal.eType)basket.items[num].value);
				}
				store_data.DelItem(table.id, basket.id, num);
				if (store_inner != null)
				{
					buyItem.item = table_area[table.id].baskets_area[basket.id].items[num];
					if (basket.item_count == 0)
					{
						table_area[table.id].SetBasket(active: false, basket, immediate: false);
					}
					else
					{
						table_area[table.id].SetItemSprite(basket, num);
					}
					int num2 = 0;
					for (int l = 0; l < table.baskets.Count; l++)
					{
						num2 += table.baskets[l].item_count;
						if (num2 > 0)
						{
							break;
						}
					}
					if (num2 == 0)
					{
						table_area[table.id].SetSoldoutIcon();
					}
				}
				return buyItem;
			}
		}
		product_set = false;
		return null;
	}

	private void DoSpotSale()
	{
		present_video.Clear();
		present_video_exterior.Clear();
		BuyItem buyItem = null;
		int num = 0;
		do
		{
			buyItem = BuyHarvest();
			if (buyItem != null)
			{
				if (num == 0)
				{
					Manager.sound.PlaySe(Sound.eSe.COINS);
					Effect.Coin(buyItem.coin, 3, buyItem.item.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
					{
						Manager.sound.PlaySe(Sound.eSe.COINS_ARRIVAL);
					});
				}
				else
				{
					Effect.Coin(buyItem.coin, 3, buyItem.item.transform.position, CoinManager.CoinTarget(), Color.white);
				}
				num++;
			}
		}
		while (buyItem != null);
		manager.data.interstitial_ON = false;
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
		progress = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Progress>();
		progress.Loop(time, 600, delegate
		{
			FixStoreUpdate(video: false);
		});
		progress.SetOrderInLayer(order_in_layer);
		progress.transform.localPosition = pos;
		progress.Show();
		SetConstructVideo();
	}

	private void FixStoreUpdate(bool video)
	{
		if (!fix_store_udpate)
		{
			fix_store_udpate = true;
			if (video)
			{
				present_video_construct.Clear();
				if (progress != null)
				{
					progress.ChangeEndTime(2);
					manager.data.interstitial_ON = false;
				}
				store_data.UpdateStore((ulong)(DateTime.Now.Ticks - 6000000000L));
			}
			manager.main.NotifyConstruct(const_positions);
			manager.data.Save();
		}
		if (store_inner != null)
		{
			Common.ResetPushIcon(table_area[store_data.level].work_icon);
		}
	}

	private void UpdateStore(int table_id)
	{
		Utils.Log("UpdateStore : table=" + table_id);
		ulong ticks = (ulong)DateTime.Now.Ticks;
		store_data.UpdateStore(ticks);
		CreateProgress(ticks, table_id, table_area[table_id].root.transform, table_area[table_id].GetOrderInLayer() + 300, TableArea.PROGRESS_POS);
		progress.SetTextVisibility(visibility: true, new Vector2(0f, -0.217f), Common.TextWhiteTranslucentColor);
		UnityEngine.Object.Destroy(prompt.gameObject);
		table_area[table_id].SetConstructIcon();
		manager.data.SetCoinCount(manager.data.coin - Price.OpenTablePrice(table_id));
	}

	public void Cancel()
	{
		Utils.Log("[CANCEL] table");
		UnityEngine.Object.Destroy(prompt.gameObject);
	}

	private Data.StoreData.Table.Basket GetBasket(Data.StoreData.Table table, Data.eHARVEST harvest)
	{
		Data.StoreData.Table.Basket basket = null;
		for (int i = 0; i < table.baskets.Count; i++)
		{
			Data.StoreData.Table.Basket basket2 = table.baskets[i];
			if (basket == null && basket2.type == Data.eHARVEST.NONE)
			{
				basket = basket2;
			}
			if (basket2.type == harvest && basket2.item_count < basket2.GetItemMax())
			{
				basket = basket2;
				break;
			}
		}
		return basket;
	}

	public void AppearItem(BuyItem buy, Transform t_parent, Vector2 pos, int order_in_layer)
	{
		if (store_inner == null)
		{
			AppearItem(buy.item_sprite, t_parent, pos, order_in_layer);
			Manager.sound.PlaySe(Sound.eSe.COIN);
			Effect.Coin(buy.coin, 3, t_parent.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
			});
		}
		else
		{
			AppearItem(buy.item_sprite, buy.item.transform, pos, buy.item.sortingOrder);
			Manager.sound.PlaySe(Sound.eSe.COIN);
			Effect.Coin(buy.coin, 3, buy.item.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
			});
		}
	}

	public static void AppearItem(Sprite sprite, Transform t, Vector2 pos, int order_in_layer)
	{
		GameObject original = Resources.Load("Prefab/store_buy_item") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, t, worldPositionStays: false);
		gameObject.transform.localPosition = pos;
		SpriteRenderer component = gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>();
		component.sprite = sprite;
		component.sortingOrder = order_in_layer;
	}

	public void LockIconUp(int table_id)
	{
		Vector2 vector = table_area[table_id].root.transform.position;
		vector = store_inner.transform.InverseTransformPoint(vector);
		vector.y -= 0.4f;
		prompt = Prompt.CreateCoinPrompt(manager.data.coin, Price.OpenTablePrice(table_id), store_inner.transform, vector, delegate
		{
			UpdateStore(table_id);
		}, Cancel);
	}

	public void DownLockIcon(TouchEvent touch)
	{
		Animation component = touch.GetComponent<Animation>();
		component.Play();
	}

	private void ConstructIconUp(int table_id)
	{
		Utils.Log("Fix : table_id=" + table_id + " fix_store_udpate=" + fix_store_udpate);
		if (fix_store_udpate)
		{
			Manager.sound.ClickSound();
			table_area[table_id].SetSoldoutIcon();
			UnityEngine.Object.Destroy(progress.gameObject);
			progress = null;
			store_data.AddTable();
			store_data.UpdateStore(0uL);
			Close();
			SetStore(store_data.level, immediate: false);
			fix_store_udpate = false;
			Vector3 position = store_exterior.transform.position;
			float x = position.x;
			Vector3 position2 = store_exterior.transform.position;
			Vector2 pos = new Vector2(x, position2.y + 0.5f);
			Manager.sound.PlaySe(Sound.eSe.APPEAR);
			Effect.Run(Resources.Load("Prefab/effect_paper") as GameObject, pos, store_exterior.transform);
			if (store_data.level < 4)
			{
				table_area[store_data.level].SetLockIcon();
			}
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
		}
	}

	public void Stock(int coin)
	{
		if (stock_earning == null || stock_earning.touch == null)
		{
			stock_earning = Common.OccurCoinBag(coin, base.transform.TransformPoint(PAY_BAG_POS), TouchStockEarnings, base.transform);
			stock_earning.touch.param.value3 = 9999;
		}
		else
		{
			stock_earning.AddValue(coin);
		}
		store_data.AddStockEarnings(coin);
	}

	public void TouchStockEarnings()
	{
		store_data.ClearStockEarnings();
	}

	public Vector2 GetStorePos()
	{
		Vector3 position = store_exterior.transform.position;
		float x = position.x;
		Vector3 position2 = store_exterior.transform.position;
		return new Vector2(x, position2.y + 0.3f);
	}

	public void StoreMax()
	{
		Animation component = store_exterior.GetComponent<Animation>();
		Utils.Play(component, "store_max", 1f, 0f);
	}
}
