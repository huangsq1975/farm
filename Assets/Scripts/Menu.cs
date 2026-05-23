using DG.Tweening;
using System;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public class Granpa
	{
		public Animator animator;

		public Sequence sequence;

		public bool sleeping;
	}

	public enum eMENU
	{
		NONE,
		MENU,
		CASHER,
		ALBUM,
		WORKER,
		PURCHASER,
		SOUND,
		MAX
	}

	public enum eCATEGORY
	{
		CASHER,
		ALBUM,
		VOLUME,
		APP,
		TWITTER,
		WORKER,
		PURCHASER,
		FARM,
		MAX
	}

	private Manager manager;

	public SpriteRenderer volume_button;

	public SpriteRenderer pixcel_black;

	public Sprite[] volume_sprite = new Sprite[2];

	public GameObject casherPrefab;

	public GameObject albumPrefab;

	private GameObject workerPrefab;

	private GameObject purchaserPrefab;

	private GameObject reviewPrefab;

	public Casher casher;

	public Album album;

	public PurchaserManager purchaser_mg;

	public WorkerManager worker;

	private GameObject office;

	private Purchaser purchaser;

	public Animation new_icon;

	private Granpa granpa = new Granpa();

	private Animation[] buttons = new Animation[8];

	private Animation consent;

	private GameObject policy;

	public int menu_flag;

	public int album_type;

	private static int recommend_count;

	private bool enabled_granpa = true;

	private Animation new_worker_icon;

	public void Init()
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		office = GameObject.Find("OfficeManager").gameObject;
		worker = office.GetComponent<WorkerManager>();
		purchaser = manager.GetComponent<Purchaser>();
		pixcel_black.transform.localScale = new Vector3(50f, 50f, 1f);
		pixcel_black.color = new Color(0f, 0f, 0f, 0.3f);
		menu_flag = 1;
		ChangeVolumeImage((int)AudioListener.volume);
		manager.LoadInterstitial();
		for (int i = 0; i < 8; i++)
		{
			eCATEGORY eCATEGORY = (eCATEGORY)i;
			TouchEvent component = base.transform.Find("b_" + eCATEGORY.ToString().ToLower()).GetComponent<TouchEvent>();
			buttons[i] = component.GetComponent<Animation>();
			Animation anim = buttons[i];
			if (i == 5 && manager.data.level < Price.OpenWorkerMenu())
			{
				TouchEvent touchEvent = Common.CreateLockIcon(component.transform, manager.data.level, Price.OpenWorkerMenu(), 0, null, 10020, chg_ignore_color: false);
				touchEvent.ClickUp.RemoveAllListeners();
				touchEvent.ClickDown.RemoveAllListeners();
				touchEvent.GetComponent<Collider2D>().enabled = false;
				touchEvent.transform.localPosition = new Vector2(-0.11f, 0.02f);
				component.ClickDown.AddListener(delegate
				{
					BeepSE();
					Utils.Play(anim, "PushIconDeny", 1f, 0f);
				});
			}
			else if (i == 7 && manager.data.level < Price.OpenFarmMenu() && Data.farm_type == Data.eFarmType.NORMAL)
			{
				TouchEvent touchEvent2 = Common.CreateLockIcon(component.transform, manager.data.level, Price.OpenFarmMenu(), 0, null, 10020, chg_ignore_color: false);
				touchEvent2.ClickUp.RemoveAllListeners();
				touchEvent2.ClickDown.RemoveAllListeners();
				touchEvent2.GetComponent<Collider2D>().enabled = false;
				touchEvent2.transform.localPosition = new Vector2(-0.11f, 0.02f);
				component.ClickDown.AddListener(delegate
				{
					BeepSE();
					Utils.Play(anim, "PushIconDeny", 1f, 0f);
				});
			}
			else
			{
				if (i == 7 && manager.data.resort_event == 0)
				{
					Animation component2 = component.transform.Find("contents/new").GetComponent<Animation>();
					component2.gameObject.SetActive(value: true);
					component2.Play();
				}
				component.ClickDown.AddListener(delegate
				{
					ClickSE();
					Utils.Play(anim, "PushIcon", 1f, 0f);
				});
			}
		}
		Manager.sound.SetVolumeBgm(0.5f);
		SetNewIcon();
		SetGranpa();
		SetConsentIcon();
		SetRecommendBanner();
		SetPrivacyPolicyIcon();
		if (!manager.data.ChkSpecialWorker() && (Data.farm_type != 0 || manager.data.worker_data.IsExist()))
		{
			Manager.office.RemoveNotice();
			SetWorkerNotice();
		}
	}

	public void SetNewIcon()
	{
		bool flag = false;
		new_icon.gameObject.SetActive(value: false);
		for (int i = 0; i < 4; i++)
		{
			if (flag)
			{
				break;
			}
			for (int j = 0; j < manager.data.character_data[i].contents.Count; j++)
			{
				if (flag)
				{
					break;
				}
				if (manager.data.character_data[i].contents[j].new_reg == 1)
				{
					flag = true;
					new_icon.gameObject.SetActive(value: true);
					new_icon.Play();
				}
			}
		}
	}

	private void SetConsentIcon()
	{
		TouchEvent component = base.transform.Find("b_consent").GetComponent<TouchEvent>();
		consent = component.GetComponent<Animation>();
		component.ClickDown.AddListener(delegate
		{
			ClickSE();
			Utils.Play(consent, "PushIcon", 1f, 0f);
		});
		consent.gameObject.SetActive(WebMediator.IsEEA());
	}

	private void SetPrivacyPolicyIcon()
	{
		TouchEvent component = base.transform.Find("b_policy").GetComponent<TouchEvent>();
		policy = component.gameObject;
		component.ClickDown.AddListener(delegate
		{
			ClickSE();
		});
		policy.SetActive(!WebMediator.IsEEA());
	}

	private void SetConsentVisibility(bool visibility)
	{
		if (WebMediator.IsEEA())
		{
			consent.gameObject.SetActive(visibility);
		}
		else
		{
			policy.SetActive(visibility);
		}
	}

	public void ResetButton()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			Utils.Play(buttons[i], "PushIcon", 0f, 0f);
		}
	}

	private void SetGranpa()
	{
		granpa.animator = base.transform.Find("granpa").GetComponent<Animator>();
		if (Manager.events.state != Event.eState.NONE || Data.farm_type != 0)
		{
			granpa.animator.gameObject.SetActive(value: false);
		}
		if (DateTime.Now.Minute / 10 % 2 == 0)
		{
			granpa.sleeping = true;
		}
		SetGranpaMotion(granpa.sleeping);
		granpa.animator.transform.DOShakeScale(0.2f, 0.3f);
	}

	private void SetGranpaMotion(bool sleeping)
	{
		if (granpa.sequence != null)
		{
			granpa.sequence.Kill();
		}
		granpa.sequence = DOTween.Sequence();
		if (!sleeping)
		{
			Utils.Play(granpa.animator, "granpa_1_sitting_1_down", 1f, 0f);
			return;
		}
		GameObject original = Resources.Load("Prefab/feel_zzz") as GameObject;
		Animation feels = UnityEngine.Object.Instantiate(original, granpa.animator.transform, worldPositionStays: false).GetComponent<Animation>();
		Utils.Play(feels, "op_appear", 0f, 0f);
		Utils.Play(granpa.animator, "granpa_1_sitting_2_down", 1f, 0f);
		granpa.sequence.AppendInterval(3f);
		granpa.sequence.AppendCallback(delegate
		{
			Utils.Play(feels, "op_appear", 1f, 0f);
		});
		granpa.sequence.AppendInterval(2f);
		granpa.sequence.AppendCallback(delegate
		{
			Utils.Play(feels, "op_appear", -1f, 1f);
		});
		granpa.sequence.AppendInterval(UnityEngine.Random.Range(5, 10));
		granpa.sequence.SetLoops(-1);
		granpa.sequence.Play();
	}

	public void TouchCasher()
	{
		if (casher != null)
		{
			UnityEngine.Object.Destroy(casher.gameObject);
		}
		casher = UnityEngine.Object.Instantiate(casherPrefab, office.transform, worldPositionStays: false).GetComponent<Casher>();
		casher.Init();
		menu_flag = 2;
	}

	public void TouchWorker()
	{
		if (manager.data.level >= Price.OpenWorkerMenu())
		{
			if (workerPrefab != null)
			{
				UnityEngine.Object.Destroy(workerPrefab.gameObject);
			}
			GameObject original = Resources.Load("Prefab/worker") as GameObject;
			workerPrefab = UnityEngine.Object.Instantiate(original, office.transform, worldPositionStays: false);
			worker.Open(workerPrefab);
			menu_flag = 4;
			purchaser.Set(manager);
		}
	}

	public void TouchPurchaser()
	{
		purchaser.Set(manager);
		if (purchaserPrefab != null)
		{
			UnityEngine.Object.Destroy(purchaserPrefab.gameObject);
		}
		GameObject original = (manager.data.lang != 0) ? (Resources.Load("Prefab/purchaser_en") as GameObject) : (Resources.Load("Prefab/purchaser_jp") as GameObject);
		purchaserPrefab = UnityEngine.Object.Instantiate(original, office.transform, worldPositionStays: false);
		purchaser_mg = purchaserPrefab.GetComponent<PurchaserManager>();
		purchaser_mg.Init(purchaserPrefab);
		menu_flag = 5;
	}

	public void TouchAlbum(int i)
	{
		if (album != null)
		{
			UnityEngine.Object.Destroy(album.gameObject);
		}
		if (i == 0)
		{
			album_type = (int)Data.farm_type;
		}
		album = UnityEngine.Object.Instantiate(albumPrefab, office.transform, worldPositionStays: false).GetComponent<Album>();
		album.Init();
		menu_flag = 3;
	}

	public void SetVisiblity(bool visibility)
	{
		GetComponent<SpriteRenderer>().enabled = visibility;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].gameObject.SetActive(visibility);
		}
		SetConsentVisibility(visibility);
	}

	public void TouchVolume()
	{
		if (AudioListener.volume == 0f)
		{
			manager.data.SetVolume(1);
			ChangeVolumeImage((int)AudioListener.volume);
		}
		else
		{
			manager.data.SetVolume(0);
			ChangeVolumeImage((int)AudioListener.volume);
		}
	}

	public void ChangeVolumeImage(int onoff)
	{
		volume_button.sprite = volume_sprite[onoff];
	}

	public void TouchFarm()
	{
		if ((Data.farm_type != 0 || manager.data.level < Price.OpenFarmMenu()) && Data.farm_type != Data.eFarmType.RESORT)
		{
			return;
		}
		DOTween.Clear();
		if (manager.data.resort_event == 0)
		{
			CloseMenu(buy_flag: true);
			Manager.events.OpenResort();
			return;
		}
		if (Data.farm_type == Data.eFarmType.NORMAL)
		{
			Data.farm_type = Data.eFarmType.RESORT;
		}
		else
		{
			Data.farm_type = Data.eFarmType.NORMAL;
		}
		manager.data.SetFarmType(Data.farm_type);
		LoadingManager.eScene nextScene = LoadingManager.eScene.GAME;
		LoadingManager.SetNextScene(nextScene);
	}

	public void CloseMenu(bool buy_flag)
	{
		if (menu_flag == 1)
		{
			granpa.sequence.Kill();
			UnityEngine.Object.Destroy(Manager.office.menu.gameObject);
			Manager.office.menu = null;
			menu_flag = 0;
			if (!buy_flag && ChkEnding())
			{
				Manager.events.Ending();
				return;
			}
			if (manager.data.interstitial_ON && !buy_flag)
			{
				manager.ShowInterstitial();
			}
			Manager.sound.SetVolumeBgm(1f);
			if (ChkReview() && !manager.data.interstitial_ON && !buy_flag)
			{
				SetReviewPopUp();
			}
		}
		else if (menu_flag == 2)
		{
			UnityEngine.Object.Destroy(casher.gameObject);
			Resume();
			menu_flag = 1;
			SetNewIcon();
		}
		else if (menu_flag == 3)
		{
			Animation component = base.transform.Find("Album(Clone)").GetComponent<Animation>();
			component.Play("closealbum");
			Resume();
			menu_flag = 1;
			SetVisiblity(visibility: true);
			SetNewIcon();
		}
	}

	public bool ChkReview()
	{
		if (manager.data.review_flag == 1)
		{
			return false;
		}
		if (manager.data.facility_data[3].enabled == 0)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < manager.data.worker_data.worker_level.Length; i++)
		{
			if (manager.data.worker_data.worker_level[i] != 0)
			{
				num++;
			}
		}
		if (num < 2)
		{
			return false;
		}
		return true;
	}

	private void SetReviewPopUp()
	{
		Manager.sound.PlaySe(Sound.eSe.ALBUM);
		GameObject original = (manager.data.lang != 0) ? (Resources.Load("Prefab/review_en") as GameObject) : (Resources.Load("Prefab/review_jp") as GameObject);
		reviewPrefab = UnityEngine.Object.Instantiate(original, office.transform, worldPositionStays: false);
		ReviewManager component = reviewPrefab.GetComponent<ReviewManager>();
		component.Init(reviewPrefab);
		manager.data.SetReview();
	}

	public void Resume()
	{
		SetGranpaMotion(granpa.sleeping);
		for (int i = 0; i < buttons.Length; i++)
		{
			Utils.Play(buttons[i], "PushIcon", 0f, 0f);
		}
	}

	public bool ChkEnding()
	{
		if (Data.farm_type != 0)
		{
			return false;
		}
		if (manager.data.ending == 1)
		{
			return false;
		}
		if (manager.data.hotel_data.level < 4 || manager.data.sailo_data.level < 4 || manager.data.store_data.level < 4)
		{
			return false;
		}
		for (int i = 0; i < manager.data.facility_data.Length; i++)
		{
			if (manager.data.facility_data[i].enabled == 0)
			{
				return false;
			}
		}
		for (int j = 0; j < Convert.FarmAnimalLength((int)Data.farm_type); j++)
		{
			Customer.eType eType = (Customer.eType)j;
			if (eType != Customer.eType.CUSTOMER_8 && eType != Customer.eType.CUSTOMER_9 && manager.data.character_data[0].contents[j].reg == 0)
			{
				return false;
			}
		}
		for (int k = 0; k < Convert.WildAnimalLength((int)Data.farm_type); k++)
		{
			if (manager.data.character_data[1].contents[k].reg == 0)
			{
				return false;
			}
		}
		for (int l = 0; l < Convert.FishLength((int)Data.farm_type); l++)
		{
			if (manager.data.character_data[2].contents[l].reg == 0)
			{
				return false;
			}
		}
		for (int m = 0; m < Convert.CustomerLength((int)Data.farm_type); m++)
		{
			Customer.eType eType2 = (Customer.eType)m;
			if (eType2 != Customer.eType.CUSTOMER_8 && eType2 != Customer.eType.CUSTOMER_9 && manager.data.character_data[3].contents[m].reg == 0)
			{
				return false;
			}
		}
		return true;
	}

	public void TouchApp()
	{
		LinkEvent.GoTodApps();
	}

	public void TouchTwitter()
	{
		LinkEvent.GoToSns();
	}

	public void TouchConsent()
	{
		WebMediator.SetConsentForm();
	}

	public void TouchPrivacyPolicy()
	{
		LinkEvent.GoToPrivacyPolicy();
	}

	public void ClickSE()
	{
		Manager.sound.PlaySe(Sound.eSe.CLICK);
	}

	public void CancelSE()
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
	}

	public void BeepSE()
	{
		Manager.sound.PlaySe(Sound.eSe.BEEP);
	}

	public TouchEvent GetCasherIcon()
	{
		return base.transform.Find("b_casher").GetComponent<TouchEvent>();
	}

	public void AddTutorialPrivacyPolicy(TouchManager tm)
	{
		if (policy != null)
		{
			tm.AddSpecific(policy.gameObject.GetComponent<TouchEvent>());
		}
	}

	public void ClickGranpa(TouchEvent touch)
	{
		if (!enabled_granpa)
		{
			return;
		}
		enabled_granpa = false;
		if (granpa.sleeping)
		{
			int num = UnityEngine.Random.Range(0, 50);
			if (num == 11)
			{
				granpa.sequence.Kill();
				granpa.sleeping = false;
				SetGranpaMotion(granpa.sleeping);
			}
			else if (num > 40)
			{
				Manager.sound.PlaySe(Sound.eSe.SNORE);
			}
		}
		Sequence sequence = DOTween.Sequence();
		if (!manager.data.get_granpa_coin || UnityEngine.Random.Range(0, 13) == 7)
		{
			Manager.sound.PlaySe(Sound.eSe.LAUGH);
			Manager.sound.PlaySe(Sound.eSe.COIN);
			int coin = UnityEngine.Random.Range(1, manager.data.level / 10 + 2);
			Effect.Coin(coin, 3, granpa.animator.transform.position, CoinManager.CoinTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.COIN_ARRIVAL);
			});
			manager.data.get_granpa_coin = true;
			sequence.Append(granpa.animator.transform.DOShakeScale(0.2f, 0.3f));
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
			sequence.Append(granpa.animator.transform.DOJump(granpa.animator.transform.position, -0.05f, 2, 0.2f));
		}
		sequence.AppendCallback(delegate
		{
			enabled_granpa = true;
		});
		sequence.Play();
	}

	public void ClickRecommend()
	{
		LinkEvent.PushedRecommend((int)manager.data.lang);
	}

	private void SetRecommendBanner()
	{
		TouchEvent component = base.transform.Find("b_recommend").GetComponent<TouchEvent>();
		recommend_count++;
		if (recommend_count >= 8)
		{
			component.gameObject.SetActive(value: true);
			SpriteRenderer component2 = component.transform.Find("contents").GetComponent<SpriteRenderer>();
			if (0 == 0)
			{
				if (manager.data.lang == Data.eLang.JP)
				{
					component2.sprite = SpriteManager.GetRecommendJp(LinkEvent.eApp.SHIBAINU);
				}
				else if (manager.data.lang == Data.eLang.EN)
				{
					component2.sprite = SpriteManager.GetRecommendEn(LinkEvent.eApp.SHIBAINU);
				}
			}
			recommend_count = 0;
		}
		else
		{
			component.gameObject.SetActive(value: false);
		}
	}

	public void SetWorkerNotice()
	{
		if (new_worker_icon == null)
		{
			new_worker_icon = Utils.Load("Prefab/new_office", base.transform).GetComponent<Animation>();
			Transform transform = new_worker_icon.transform;
			Vector3 localPosition = new_worker_icon.transform.localPosition;
			transform.localPosition = new Vector3(0.034f, 0.742f, localPosition.z);
			Utils.Play(new_worker_icon, new_worker_icon.clip.name, 1f, 0f);
		}
	}

	public void RemoveWorkerNotice()
	{
		if (new_worker_icon != null)
		{
			UnityEngine.Object.Destroy(new_worker_icon.gameObject);
			new_worker_icon = null;
		}
	}
}
