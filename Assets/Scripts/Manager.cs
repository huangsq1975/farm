using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
	[Serializable]
	public class VideoRwd
	{
		public enum eState
		{
			NONE,
			LOADING,
			LOAD_COMPLETED,
			PLAYING
		}

		public eState state;

		public UnityAction play_video_completed_cb;

		public UnityAction play_video_failed_cb;

		public void SetState(eState state)
		{
			Utils.Log("[FARM] SetState: current=" + this.state + " new_state=" + state);
			this.state = state;
		}

		public void SetCallback(UnityAction completed_cb, UnityAction failed_cb)
		{
			play_video_completed_cb = completed_cb;
			play_video_failed_cb = failed_cb;
		}
	}

	[Serializable]
	public class Interstitial
	{
		public const int LIMIT = 180;

		public ulong timer;

		public void Init()
		{
			timer = (ulong)(DateTime.Now.Ticks + 1800000000);
		}

		public bool IsExpired()
		{
			return (ulong)DateTime.Now.Ticks >= timer;
		}

		public void SetTimer()
		{
			timer = (ulong)(DateTime.Now.Ticks + 1800000000);
		}
	}

	private static Manager instance;

	public bool no_ads;

	public bool delete_all;

	public Data data;

	public MainCharacter main;

	public Map map;

	public StoreManager store;

	public HotelManager hotel;

	public WorkerManager worker;

	public WildAnimalManager wild;

	public Purchaser purchaser;

	public Sailo silo;

	public static Event events;

	public static Sound sound;

	public static OfficeManager office;

	public SugorokuManager sugoroku;

	public GameObject social_worker_prefab;

	public static bool running;

	public VideoRwd load_video = new VideoRwd();

	public Interstitial interstitial = new Interstitial();

	private GameObject commercial;

	public void Awake()
	{
		Debug.Log("Manager.Awake 1");
		instance = this;
		//if (!WebMediator.IsThereInstance())
		//{
		//	Debug.Log("Manager.Awake 1.");
        //    GameObject gameObject = UnityEngine.Object.Instantiate(social_worker_prefab, base.transform.position, base.transform.rotation);
		//	WebMediator.Install(no_ads);
		//}
		Debug.Log("Manager.Awake 2");
        Camera.main.GetComponent<FixUI>().Init();
		GameObject.Find("SpriteManager").GetComponent<SpriteManager>().Init();
		main = base.transform.Find("main_character").gameObject.GetComponent<MainCharacter>();
		data = new Data();
		data.Init(this, delete_all);
		LinkEvent.Init(data);
		events = GetComponent<Event>();
		events.Init(this);
		GetComponent<Timer>().Init();
		Price.Init(data);
		GameObject gameObject2 = GameObject.Find("BGM");
		sound = gameObject2.GetComponent<Sound>();
		sound.Init(this);
		map = GetComponent<Map>();
		map.Init(this);
		Common.Init();
		Convert.Init();
        Debug.Log("Manager.Awake 3");
        gameObject2 = base.transform.Find("OfficeManager").gameObject;
		office = gameObject2.GetComponent<OfficeManager>();
		gameObject2 = base.transform.Find("StoreManager").gameObject;
		store = gameObject2.GetComponent<StoreManager>();
		store.Init(this);
		gameObject2 = base.transform.Find("HotelManager").gameObject;
		hotel = gameObject2.GetComponent<HotelManager>();
		hotel.Init(this);
		wild = GetComponent<WildAnimalManager>();
		wild.Init(this);
		gameObject2 = GameObject.Find("UI/sugoroku");
		sugoroku = gameObject2.GetComponent<SugorokuManager>();
		sugoroku.Init(this);
		silo = GameObject.Find("UI/silo").GetComponent<Sailo>();
		silo.Init();
		worker = office.GetComponent<WorkerManager>();
		worker.Init(this);
		purchaser = GetComponent<Purchaser>();
		TouchEvent camera_on = GameObject.Find("UI/camera/camera_image").GetComponent<TouchEvent>();
		camera_on.ClickUp.AddListener(delegate
		{
			TouchShare(camera_on);
		});
		GameObject original = SetBgPrefab(Data.farm_type);
		GameObject gameObject3 = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false);
		original = SetHomePrefab(Data.farm_type);
		GameObject gameObject4 = UnityEngine.Object.Instantiate(original, office.transform, worldPositionStays: false);
		TouchEvent touch_home = gameObject4.GetComponent<TouchEvent>();
		touch_home.ClickDown.AddListener(delegate
		{
			sound.ClickSound();
		});
		touch_home.ClickUp.AddListener(delegate
		{
			office.TouchOffice(touch_home);
		});
		Camera.main.backgroundColor = SetCameraColor(Data.farm_type);
		SpriteRenderer component = GameObject.Find("UI/CoinManager/coin").GetComponent<SpriteRenderer>();
		component.sprite = SpriteManager.GetUiCoinSprite(Data.farm_type);
		SpriteRenderer component2 = GameObject.Find("UI/LevelManager/image").GetComponent<SpriteRenderer>();
		component2.sprite = SpriteManager.GetUiLevelSprite(Data.farm_type);
		running = true;
		SystemFontManager.Init();
		if (data.opening == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				data.SetAlbumDisp((i == 0) ? 1 : 0, i, Data.eFarmType.NORMAL);
			}
			events.Opening();
		}
		else if (data.first_coin == 0)
		{
			events.SetKeepOutGrassCut();
		}
		interstitial.Init();
		sound.PlayBgm(Sound.eBgm.FIELD);
		if (Data.IsDevelopmentBuild())
		{
			gameObject.AddComponent<DevMainMenuTools>();
		}
        Debug.Log("Manager.Awake 11");
    }

	public void ReloadProgressData()
	{
		data.Init(this, delete_all: false);
		store.Init(this);
		hotel.Init(this);
		wild.Init(this);
		sugoroku.Init(this);
		silo.Init();
		worker.Init(this);
		if (CoinManager.self != null)
		{
			CoinManager.self.ForceRefresh();
		}
		if (LevelManager.self != null)
		{
			LevelManager.self.ForceRefresh();
		}
	}

    private void Update()
	{
		BackKey();
	}

	private void BackKey()
	{
		if (Application.platform != RuntimePlatform.Android || (!Input.GetKeyUp(KeyCode.Escape) && !Input.GetKey(KeyCode.Menu)))
		{
			return;
		}
		if (events.state != Event.eState.NONE)
		{
			Application.Quit();
		}
		else if (office.menu != null)
		{
			if (office.menu.menu_flag == 1)
			{
				office.menu.CloseMenu(buy_flag: false);
			}
			else if (office.menu.menu_flag == 3)
			{
				if (office.menu.album.selector != null)
				{
					office.menu.album.ResetBG();
				}
				else
				{
					office.menu.CloseMenu(buy_flag: false);
				}
			}
			else if (office.menu.menu_flag == 5)
			{
				office.menu.purchaser_mg.TouchBlackBg();
			}
			else if (office.menu.menu_flag == 4)
			{
				if (office.menu.worker.promptPrefab != null)
				{
					office.menu.worker.WorkerLvUpCancel(office.menu.worker.w_touch, office.menu.worker.w_type, office.menu.worker.prev_order);
				}
				else if (office.menu.worker.worker_prompt != null)
				{
					office.menu.worker.AddWorkerCancel(office.menu.worker.w_touch, office.menu.worker.w_type);
				}
				else if (office.menu.worker.worker_place_prompt != null)
				{
					office.menu.worker.DestroyWorkerPrompt();
				}
				else
				{
					office.menu.worker.TouchBlackBg();
				}
			}
			else
			{
				if (office.menu.menu_flag != 2)
				{
					return;
				}
				if (office.menu.casher.prompt != null)
				{
					if (office.menu.casher.casher_flag == 6)
					{
						office.menu.casher.FacilityRestartShopping();
						office.menu.casher.DestroyPromptObj();
					}
					else if (office.menu.casher.casher_flag == 3)
					{
						office.menu.casher.DelAnimalCancel(office.menu.casher.touch_facility);
					}
					else if (office.menu.casher.casher_flag == 5)
					{
						office.menu.casher.DelFacilityCancel(office.menu.casher.touch_facility, office.menu.casher.w_item_type);
					}
					else if (office.menu.casher.casher_flag == 4)
					{
						office.menu.casher.DelFacilityCancel(office.menu.casher.touch_facility, office.menu.casher.w_item_type);
					}
					else if (office.menu.casher.casher_flag == 0)
					{
						office.menu.casher.AddAnimalCancel(office.menu.casher.touch_facility, office.menu.casher.w_animal_type);
					}
					else if (office.menu.casher.casher_flag == 2)
					{
						office.menu.casher.AddFacilityCancel(office.menu.casher.touch_facility, office.menu.casher.w_obj, office.menu.casher.w_item_type);
					}
					else if (office.menu.casher.casher_flag == 1)
					{
						office.menu.casher.AddFacilityCancel(office.menu.casher.touch_facility, office.menu.casher.w_touch, office.menu.casher.w_type);
					}
				}
				else if (office.menu.casher.facility_bg != null)
				{
					office.menu.casher.CancelChooseFacility();
				}
				else
				{
					office.menu.casher.BlackPrefabDestroy();
				}
			}
		}
		else if (ReviewManager.review_popup != null)
		{
			ReviewManager.DeleteReviewPopUp();
		}
		else if (silo.open.anim != null)
		{
			silo.Close();
		}
		else if (sugoroku.anim != null)
		{
			sugoroku.Close();
		}
		else if (hotel.hotel_inner != null)
		{
			hotel.Close();
		}
		else if (store.store_inner != null)
		{
			store.Close();
		}
		else
		{
			Application.Quit();
		}
	}

	private IEnumerator ShowBanner()
	{
		yield return new WaitForSeconds(2f);
		if (data.purchase[0] == 0)
		{
			//WebMediator.ShowBanner();
		}
	}

	private void OnApplicationQuit()
	{
		Utils.Log("[QUIT]");
		data.Save();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			Utils.Log("[PAUSE]");
			data.Save();
		}
	}

	public void TouchShare(TouchEvent touch)
	{
		LinkEvent.DoTweet(this);
	}

	private GameObject SetHomePrefab(Data.eFarmType ftype)
	{
		switch (ftype)
		{
			case Data.eFarmType.NORMAL:
				return Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/home_0") as GameObject;
			case Data.eFarmType.RESORT:
				return Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/home_1") as GameObject;
			default:
				return null;
		}
	}

	private GameObject SetBgPrefab(Data.eFarmType ftype)
	{
		switch (ftype)
		{
			case Data.eFarmType.NORMAL:
				return Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/bg_0") as GameObject;
			case Data.eFarmType.RESORT:
				return Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/bg_1") as GameObject;
			default:
				return null;
		}
	}

	public static Color32 SetCameraColor(Data.eFarmType ftype)
	{
		switch (ftype)
		{
			case Data.eFarmType.NORMAL:
				return new Color32(147, 201, 109, byte.MaxValue);
			case Data.eFarmType.RESORT:
				return new Color32(byte.MaxValue, 229, 179, byte.MaxValue);
			default:
				return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	public void bannerAdLoaded(string str)
	{
	}

	public bool LoadVideo()
	{
		Utils.Log("[FARM] <LoadVideo> : state=" + load_video.state + " --- Internet Reachability=" + Application.internetReachability);
		if (Application.internetReachability != 0)
		{
			if (load_video.state == VideoRwd.eState.NONE)
			{
				load_video.SetState(VideoRwd.eState.LOADING);
				//WebMediator.LoadVideo();
				return true;
			}
			if (load_video.state != VideoRwd.eState.PLAYING)
			{
				return true;
			}
		}
		return false;
	}

	public void videoAdLoaded(string str)
	{
		Utils.Log("[FARM] videoAdLoaded : state=" + load_video.state);
		if (load_video.state == VideoRwd.eState.LOADING)
		{
			load_video.SetState(VideoRwd.eState.LOAD_COMPLETED);
		}
	}

	public void videoAdFailedToLoad(string str)
	{
		Utils.Log("[FARM] videoAdFailedToLoad : state=" + load_video.state);
		if (load_video.state == VideoRwd.eState.LOADING)
		{
			load_video.SetState(VideoRwd.eState.NONE);
		}
	}

	public void PlayVideo(UnityAction completed_cb, UnityAction failed_cb)
	{
		Utils.Log("[FARM] <PlayVideo> : state=" + load_video.state);
		load_video.SetState(VideoRwd.eState.PLAYING);
		//WebMediator.PlayVideo();
		load_video.SetCallback(completed_cb, failed_cb);
	}

	public void videoAdPlayed(string str)
	{
		Utils.Log("[FARM] videoAdPlayed : state=" + load_video.state);
		data.InterstitialCountReset();
		load_video.SetState(VideoRwd.eState.NONE);
		if (load_video.play_video_completed_cb != null)
		{
			load_video.play_video_completed_cb();
		}
	}

	public void videoAdFailedToPlay(string str)
	{
		Utils.Log("[FARM] videoAdFailedToPlay : state=" + load_video.state);
		load_video.SetState(VideoRwd.eState.NONE);
		if (load_video.play_video_failed_cb != null)
		{
			load_video.play_video_failed_cb();
		}
	}

	public bool LoadInterstitial()
	{
		data.InterstitialCount();
		if (data.interstitial_count >= 8 && data.purchase[0] == 0 && interstitial.IsExpired())
		{
			//WebMediator.LoadInterstitial();
			return true;
		}
		return false;
	}

	public void interstitialAdLoaded()
	{
		UnityEngine.Debug.Log("interstitialAdLoaded");
		data.interstitial_ON = true;
	}

	public void interstitialAdFailedToLoad()
	{
		UnityEngine.Debug.Log("interstitialAdFailedToLoad");
	}

	public void ShowInterstitial()
	{
		commercial = Common.OccurInterstitialSet(base.transform, new Vector2(0f, 0f));
		Sequence s = DOTween.Sequence();
		s.AppendInterval(2f);
		s.AppendCallback(delegate
		{
			//WebMediator.ShowInterstitial();
		});
	}

	public void interstitialAdShowed()
	{
		UnityEngine.Debug.Log("interstitialAdShowed");
		data.interstitial_ON = false;
		data.InterstitialCountReset();
		if (commercial != null)
		{
			Sequence s = DOTween.Sequence();
			Common.SetIntertitialAfter(commercial);
			Effect effect = Effect.Coin(Price.CoinInterstitial(), 3, commercial.transform.position, CoinManager.CoinTarget(), Color.white);
			effect.AddFinishCallback(delegate
			{
				sound.PlaySe(Sound.eSe.COINS_ARRIVAL);
			});
			s.AppendInterval(2f);
			s.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(commercial);
				commercial = null;
			});
		}
		interstitial.SetTimer();
	}

	public void interstitialAdFailedToShow()
	{
		UnityEngine.Debug.Log("interstitialAdFailedToShow");
		data.interstitial_ON = false;
		if (commercial != null)
		{
			UnityEngine.Object.Destroy(commercial);
			commercial = null;
		}
	}

	public void changedConsentState()
	{
		UnityEngine.Debug.Log("FARM: changedConsentState");
		StartCoroutine(ShowBanner());
	}

	public static Data GetData()
	{
		return instance.data;
	}
}