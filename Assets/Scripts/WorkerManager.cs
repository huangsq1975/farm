using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WorkerManager : MonoBehaviour
{
	private Manager manager;

	private static WorkerManager instance;

	private SpriteRenderer black_bg;

	private GameObject worker_menu;

	private GameObject framePrefab;

	public GameObject promptPrefab;

	public Prompt worker_prompt;

	public GameObject worker_place_prompt;

	private GameObject[] worker_obj = new GameObject[13];

	private TouchEvent[] worker_touch = new TouchEvent[13];

	private GameObject[] icon_obj = new GameObject[13];

	private SpriteRenderer[] worker_lock = new SpriteRenderer[13];

	private SpriteRenderer[] worker_icon = new SpriteRenderer[13];

	private PartsController[] worker_parts = new PartsController[13];

	private SpriteRenderer[,] star_sprite = new SpriteRenderer[13, 4];

	private SpriteRenderer worker_facility;

	private SpriteRenderer assign_button;

	private GameObject purchase;

	private SpriteRenderer purchase_button;

	private CustomText purchase_text;

	private TouchEvent purchase_agency;

	private GameObject purchase_attention;

	private int menu_flag;

	private int preceding_worker;

	private TouchEvent touch_ok;

	private GameObject selector;

	private Sequence myAssignONSequence;

	private Sequence myAssignOFFSequence;

	private Sequence[] myIconMoveSequence = new Sequence[13];

	public Sprite button_on;

	public Sprite button_off;

	public Worker.eType w_type;

	public TouchEvent w_touch;

	public int prev_order;

	private static int WORKER_ALL;

	private const float offset_y = 1f;

	private const float purchase_offset_y = -0.85f;

	private float default_y;

	private float default_purchase_y;

	public static readonly Vector2[] SetIcon = new Vector2[13]
	{
		new Vector2(-0.17f, 0.154f),
		new Vector2(0.27f, 0.154f),
		new Vector2(-0.17f, -0.116f),
		new Vector2(0.27f, -0.116f),
		new Vector2(-0.17f, -0.387f),
		new Vector2(0.27f, -0.387f),
		new Vector2(-0.17f, -0.651f),
		new Vector2(0.27f, -0.651f),
		new Vector2(-0.17f, -0.922f),
		new Vector2(0.27f, -0.922f),
		new Vector2(0.05f, -0.922f),
		new Vector2(-0.17f, 0.397f),
		new Vector2(-0.17f, -1.227f)
	};

	public static readonly Vector2[] SetCoinPos = new Vector2[5]
	{
		new Vector2(-0.0467f, 0f),
		new Vector2(-0.0637f, 0f),
		new Vector2(-0.103f, 0f),
		new Vector2(-0.1243f, 0f),
		new Vector2(-0.1479f, 0f)
	};

	public static readonly Vector2[] SetTextPos = new Vector2[5]
	{
		new Vector2(0.0122f, 0f),
		new Vector2(-0.0039f, 0f),
		new Vector2(-0.03f, 0f),
		new Vector2(-0.0549f, 0f),
		new Vector2(-0.0895f, 0f)
	};

	public static readonly Vector3[] SetPromptPos = new Vector3[13]
	{
		new Vector3(-0.74f, -0.13f, -20f),
		new Vector3(0.74f, -0.13f, -20f),
		new Vector3(-0.74f, -0.609f, -20f),
		new Vector3(0.74f, -0.609f, -20f),
		new Vector3(-0.74f, -1.085f, -20f),
		new Vector3(0.74f, -1.085f, -20f),
		new Vector3(-0.74f, -1.55f, -20f),
		new Vector3(0.74f, -1.55f, -20f),
		new Vector3(-0.74f, -2.1f, -20f),
		new Vector3(0.74f, -2.1f, -20f),
		new Vector3(-0.04f, -2.1f, -20f),
		new Vector3(0.04f, -2.1f, -20f),
		new Vector3(0.7f, -2.1f, -20f)
	};

	public void Init(Manager m)
	{
		manager = m;
		instance = this;
		WORKER_ALL = 8 + Data.WorkerData.SpecialMax(Data.farm_type);
		for (int i = 0; i < WORKER_ALL; i++)
		{
			AssignWorker(i, Convert.ToWorkerType(i));
		}
		if (!manager.data.ChkSpecialWorker() && (Data.farm_type != 0 || manager.data.worker_data.IsExist()))
		{
			Manager.office.SetNotice();
		}
	}

	private void AssignWorker(int index, Worker.eType type)
	{
		if (manager.data.worker_data.worker_place[index] != -1)
		{
			if (manager.data.worker_data.worker_place[index] == 11)
			{
				manager.hotel.AssignWorker(type);
			}
			else if (manager.data.worker_data.worker_place[index] == 12)
			{
				manager.silo.AssignWorker(type);
			}
			else
			{
				manager.map.facility_list[manager.data.worker_data.worker_place[index]].AssignWorker(type);
			}
		}
	}

	public void Open(GameObject obj)
	{
		if (black_bg != null)
		{
			UnityEngine.Object.Destroy(black_bg.gameObject);
			black_bg = null;
		}
		Manager.office.menu.gameObject.SetActive(value: false);
		worker_menu = obj;
		black_bg = worker_menu.transform.Find("pixel_black").GetComponent<SpriteRenderer>();
		black_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		black_bg.color = new Color(0f, 0f, 0f, 0.5f);
		TouchEvent component = black_bg.GetComponent<TouchEvent>();
		component.ClickUp.AddListener(delegate
		{
			TouchBlackBg();
		});
		worker_facility = worker_menu.transform.Find("facility_bg").GetComponent<SpriteRenderer>();
		SetWorker();
		SetFacility();
		menu_flag = 0;
		InitPurchase();
		Vector3 localPosition = worker_menu.transform.localPosition;
		default_y = localPosition.y;
		Vector3 localPosition2 = purchase.transform.localPosition;
		default_purchase_y = localPosition2.y;
		if (IsPurchased() || Data.farm_type != 0 || manager.data.worker_data.IsExist())
		{
			SetPurchaseArea(!IsPurchased());
			SetEnabledSpecial(IsPurchased(), IsPurchased());
			manager.data.SetSpecialWorker();
			manager.data.Save();
			Manager.office.menu.RemoveWorkerNotice();
		}
		else
		{
			worker_menu.transform.Find("purchase").gameObject.SetActive(value: false);
			SetEnabledSpecial(enabled: false, touch_enabled: false);
		}
	}

	private void InitPurchase()
	{
		purchase = worker_menu.transform.Find("purchase").gameObject;
		purchase_button = purchase.transform.Find("button_bg/button/buy_button").GetComponent<SpriteRenderer>();
		purchase_agency = purchase.transform.Find("agency").GetComponent<TouchEvent>();
		if (manager.data.lang == Data.eLang.EN)
		{
			purchase_attention = purchase.transform.Find("attention_en").gameObject;
			purchase.transform.Find("attention_ja").gameObject.SetActive(value: false);
		}
		else
		{
			purchase_attention = purchase.transform.Find("attention_ja").gameObject;
			purchase.transform.Find("attention_en").gameObject.SetActive(value: false);
		}
		purchase_attention.SetActive(value: false);
		purchase_text = purchase_button.transform.parent.Find("text").GetComponent<CustomText>();
		purchase_text.Init();
	}

	private void SetPurchaseArea(bool enabled)
	{
		purchase.SetActive(enabled);
		if (!enabled)
		{
			return;
		}
		TouchEvent component = purchase.GetComponent<TouchEvent>();
		component.ClickUp.AddListener(ClosePurchase);
		component.SetEnabled(enabled: false);
		component = purchase_button.GetComponent<TouchEvent>();
		component.ClickDown.AddListener(PushedPurchaseDown);
		component.ClickUp.AddListener(PushedPurchaseClickUp);
		component.SetEnabled(enabled: false);
		purchase_agency.ClickUp.AddListener(OpenPurchase);
		purchase_agency.SetEnabled(enabled: true);
		int num = 0;
		while (Data.farm_type == Data.eFarmType.RESORT && num < 5)
		{
			SpriteRenderer component2 = purchase_agency.transform.Find("worker_icon_" + (num + 1)).GetComponent<SpriteRenderer>();
			if (num < Data.WorkerData.SpecialMax(Data.farm_type))
			{
				component2.sprite = SpriteManager.GetWorkingIcon(Convert.ToWorkerType(8 + num));
				Transform transform = component2.transform;
				Vector3 localPosition = component2.transform.localPosition;
				transform.SetLocalPositionX(localPosition.x + 0.06f);
			}
			else
			{
				component2.gameObject.SetActive(value: false);
			}
			num++;
		}
	}

	private void SetEnabledWorker(bool enabled)
	{
		bool enabled2 = worker_touch[0].GetEnabled();
		if (enabled != enabled2)
		{
			List<SpriteRenderer> list = new List<SpriteRenderer>();
			list.Add(worker_menu.transform.Find("bg").GetComponent<SpriteRenderer>());
			Utils.SetList(list[0].transform, list, true);
			for (int i = 0; i < 8; i++)
			{
				Utils.SetList(worker_menu.transform.Find("worker" + (i + 1)), list, true);
				worker_touch[i].SetEnabled(enabled);
			}
			int num = (!enabled) ? (-1000) : 1000;
			foreach (SpriteRenderer item in list)
			{
				item.sortingOrder += num;
			}
		}
	}

	private void SetEnabledSpecial(bool enabled, bool touch_enabled)
	{
		worker_menu.transform.Find("special_bg").gameObject.SetActive(enabled);
		for (int i = 8; i < WORKER_ALL; i++)
		{
			worker_touch[i].gameObject.SetActive(enabled);
			worker_touch[i].SetEnabled(touch_enabled);
		}
	}

	private void SetFacility()
	{
		SpriteRenderer component = worker_facility.transform.Find("hotel").GetComponent<SpriteRenderer>();
		component.sprite = SpriteManager.GetWorkeHotel(Data.farm_type);
		SpriteRenderer component2 = worker_facility.transform.Find("sailo").GetComponent<SpriteRenderer>();
		component2.sprite = SpriteManager.GetWorkeSailo(Data.farm_type);
		SpriteRenderer component3 = worker_facility.transform.Find("office").GetComponent<SpriteRenderer>();
		component3.sprite = SpriteManager.GetWorkeOffice(Data.farm_type);
		SpriteRenderer component4 = worker_facility.transform.Find("store").GetComponent<SpriteRenderer>();
		component4.sprite = SpriteManager.GetWorkeStore(Data.farm_type);
		SpriteRenderer[] array = new SpriteRenderer[11];
		for (int i = 0; i < 11; i++)
		{
			array[i] = worker_facility.transform.Find("graze" + i).GetComponent<SpriteRenderer>();
			array[i].sprite = SpriteManager.GetWorkeFacility(Data.farm_type);
		}
		if (Data.farm_type == Data.eFarmType.RESORT)
		{
			array[10].gameObject.SetActive(value: false);
		}
	}

	private void SetWorker()
	{
		for (int i = 0; i < WORKER_ALL; i++)
		{
			Worker.eType type = Convert.ToWorkerType(i);
			SetWorkerRes(i, type);
			SetWorkerRank(i);
			SetWorkerTouch(i, type);
			SetLockIcon(i);
		}
		for (int j = WORKER_ALL; j < 13; j++)
		{
			worker_menu.transform.Find("worker" + (j + 1)).gameObject.SetActive(value: false);
		}
	}

	private void SetWorkerRes(int index, Worker.eType type)
	{
		worker_icon[index] = worker_menu.transform.Find("worker" + (index + 1) + "/icon").GetComponent<SpriteRenderer>();
		worker_icon[index].sprite = SpriteManager.GetWorkerIcon(manager, type, index);
		worker_parts[index] = worker_menu.transform.Find("worker" + (index + 1) + "/human").GetComponent<PartsController>();
		worker_parts[index].Init(Worker.style[(int)type]);
		worker_parts[index].Play(PartsController.eAnimType._STAY_1_DOWN, 0f, 0f);
	}

	private void SetWorkerRank(int index)
	{
		for (int i = 0; i < 4; i++)
		{
			star_sprite[index, i] = worker_menu.transform.Find("worker" + (index + 1) + "/star" + (i + 1)).GetComponent<SpriteRenderer>();
			star_sprite[index, i].sprite = SpriteManager.GetWorkerStar((manager.data.worker_data.worker_level[index] > i) ? 1 : 0);
		}
	}

	private void SetWorkerTouch(int index, Worker.eType type)
	{
		worker_obj[index] = worker_menu.transform.Find("worker" + (index + 1)).gameObject;
		worker_touch[index] = worker_menu.transform.Find("worker" + (index + 1)).GetComponent<TouchEvent>();
		worker_touch[index].param.value1 = index;
		worker_touch[index].ClickUp.AddListener(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			TouchWorker(worker_touch[index], type);
		});
		SetFork(worker_parts[index], manager.data.worker_data.worker_level[index] > 0);
	}

	private void SetLockIcon(int index)
	{
		worker_lock[index] = worker_menu.transform.Find("worker" + (index + 1) + "/lock").GetComponent<SpriteRenderer>();
		worker_lock[index].enabled = (manager.data.worker_data.worker_level[index] == 0);
	}

	private void SetFork(PartsController worker, bool visibility)
	{
		worker.SetItem(PartsController.Parts.eType.ITEM1, visibility ? PartsController.ePartsItem.FORK : PartsController.ePartsItem.NONE);
	}

	private void TouchWorker(TouchEvent touch, Worker.eType type)
	{
		w_type = type;
		w_touch = touch;
		Animation component = worker_menu.GetComponent<Animation>();
		if (component.isPlaying)
		{
			return;
		}
		if (manager.data.worker_data.worker_level[touch.param.value1] != 0)
		{
			if (menu_flag == 0)
			{
				Utils.Play(component, "worker_menu_open" + ((!IsPurchased()) ? string.Empty : "_special"), 1f, 0f);
				Manager.sound.PlaySe(Sound.eSe.GOOD);
				worker_menu.GetComponent<AnimEvent>().SetFinishCallback(delegate
				{
					EndOpenAnimation(touch, type);
				});
			}
			else
			{
				EndOpenAnimation(touch, type, initialization: false);
				Vector3 vector = new Vector3(0f, 0f, 0f);
				ShortcutExtensions.DOLocalJump(endValue: (manager.data.worker_data.worker_place[touch.param.value1] != -1) ? ((Vector3)SetIcon[manager.data.worker_data.worker_place[touch.param.value1]]) : worker_menu.transform.InverseTransformPoint(worker_icon[touch.param.value1].transform.position), target: icon_obj[touch.param.value1].transform, jumpPower: 0.1f, numJumps: 1, duration: 0.2f).SetEase(Ease.Linear);
			}
			return;
		}
		int count = 0;
		for (int i = 0; i < WORKER_ALL; i++)
		{
			if (manager.data.worker_data.worker_level[i] != 0)
			{
				count++;
			}
		}
		int price = Price.OpenWorkerPrice(count);
		int value = touch.param.value1;
		Vector2 pos = new Vector2(-0.45f, 0.42f);
		if (value >= 10)
		{
			float x;
			switch (value)
			{
			case 10:
				x = 0.2f;
				break;
			case 11:
				x = -1.1f;
				break;
			default:
				x = 0.17f;
				break;
			}
			pos = new Vector2(x, 0.89f);
		}
		Utils.SetOrderInLayer(touch.gameObject, 10000);
		worker_prompt = Prompt.CreateCoinPrompt(manager.data.coin, price, touch.transform, pos, delegate
		{
			AddWorkerOK(touch, type, price, count);
		}, delegate
		{
			AddWorkerCancel(touch, type);
		});
	}

	public void AddWorkerOK(TouchEvent touch, Worker.eType type, int price, int count)
	{
		Utils.SetOrderInLayer(touch.gameObject, -10000);
		manager.data.worker_data.SetWorkerLvUP(touch);
		manager.data.worker_data.SetWorkerPlace(touch.param.value1, Worker.eWorkPlace.NONE);
		manager.data.worker_data.SetWorkerRelease(touch, count);
		worker_icon[touch.param.value1].sprite = SpriteManager.GetWorkerIcon(manager, type, touch.param.value1);
		SetWorkerStar(touch);
		int coinCount = manager.data.coin - price;
		manager.data.SetCoinCount(coinCount);
		worker_lock[touch.param.value1].enabled = false;
		GameObject original = Resources.Load("Prefab/worker_ok_baloon") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, touch.transform, worldPositionStays: false);
		gameObject.transform.localPosition = new Vector3(-0.43f, 0.974f, 0f);
		UnityEngine.Object.Destroy(worker_prompt.gameObject);
		worker_prompt = null;
		SetFork(worker_parts[touch.param.value1], visibility: true);
		Manager.sound.PlaySe(Sound.eSe.HUG);
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			if (worker_menu != null)
			{
				TouchWorker(touch, type);
			}
		});
		sequence.Play();
	}

	public void AddWorkerCancel(TouchEvent touch, Worker.eType type)
	{
		UnityEngine.Object.Destroy(worker_prompt.gameObject);
		worker_prompt = null;
		Utils.SetOrderInLayer(touch.gameObject, -10000);
	}

	public void EndOpenAnimation(TouchEvent touch, Worker.eType type, bool initialization = true)
	{
		if (framePrefab != null)
		{
			UnityEngine.Object.Destroy(framePrefab.gameObject);
		}
		int value = touch.param.value1;
		Utils.SetOrderInLayer(touch.gameObject, 100);
		if (value < 8)
		{
			GameObject original = Resources.Load("Prefab/worker_select_" + ((value % 2 != 0) ? "right" : "left")) as GameObject;
			framePrefab = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			if (value < 12)
			{
				float[] array = new float[2]
				{
					-0.003f,
					0.003f
				};
				float[] array2 = new float[6]
				{
					-0.191f,
					-0.67f,
					-1.149f,
					-1.628f,
					-2f,
					-3f
				};
				framePrefab.transform.localPosition = new Vector2(array[value % 2], array2[value / 2]);
			}
		}
		else if (value < 10)
		{
			GameObject original = Resources.Load("Prefab/worker_select_bg_2") as GameObject;
			framePrefab = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			float[] array3 = new float[2]
			{
				-0.658f,
				0.658f
			};
			framePrefab.transform.localPosition = new Vector2(array3[value - 8], -1.705f);
			framePrefab.transform.localScale = new Vector3((value == 8) ? 1 : (-1), 1f, 1f);
		}
		else if (value < 12)
		{
			GameObject original = Resources.Load("Prefab/worker_select_bg_3") as GameObject;
			framePrefab = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			float[] array4 = new float[2]
			{
				-0.658f,
				0.658f
			};
			framePrefab.transform.localPosition = new Vector2(array4[value - 10], -2.185f);
			framePrefab.transform.localScale = new Vector3((value == 10) ? 1 : (-1), 1f, 1f);
		}
		else
		{
			GameObject original = Resources.Load("Prefab/worker_select_bg_4") as GameObject;
			framePrefab = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
		}
		SetLvpButton(touch);
		TouchEvent b_touch = framePrefab.transform.Find("button").GetComponent<TouchEvent>();
		b_touch.param.value1 = value;
		Animation anim = framePrefab.GetComponent<Animation>();
		b_touch.ClickDown.AddListener(delegate
		{
			anim.Play();
		});
		b_touch.ClickUp.AddListener(delegate
		{
			TouchLevelUp(b_touch, type);
		});
		SetAreaSprite(touch);
		worker_menu.GetComponent<AnimEvent>().SetFinishCallback(null);
		black_bg.sortingOrder = 10010;
		menu_flag = 1;
		SetWorkerIcon();
		SetGreenIcon();
		for (int i = 0; i < 13; i++)
		{
			TouchEvent touch_facility = null;
			if (i <= 10)
			{
				touch_facility = worker_facility.transform.Find("graze" + i).GetComponent<TouchEvent>();
				touch_facility.param.value1 = value;
				touch_facility.ClickUp.RemoveAllListeners();
				Worker.eWorkPlace wplace = (Worker.eWorkPlace)i;
				touch_facility.ClickUp.AddListener(delegate
				{
					TouchWorkerFacility(touch_facility, type, wplace);
				});
			}
			else if (i == 11)
			{
				touch_facility = worker_facility.transform.Find("hotel").GetComponent<TouchEvent>();
				touch_facility.param.value1 = value;
				touch_facility.ClickUp.RemoveAllListeners();
				Worker.eWorkPlace wplace2 = (Worker.eWorkPlace)i;
				touch_facility.ClickUp.AddListener(delegate
				{
					TouchWorkerFacility(touch_facility, type, wplace2);
				});
			}
			else
			{
				touch_facility = worker_facility.transform.Find("sailo").GetComponent<TouchEvent>();
				touch_facility.param.value1 = value;
				touch_facility.ClickUp.RemoveAllListeners();
				Worker.eWorkPlace wplace3 = (Worker.eWorkPlace)i;
				touch_facility.ClickUp.AddListener(delegate
				{
					TouchWorkerFacility(touch_facility, type, wplace3);
				});
			}
			if (initialization)
			{
				touch_facility.transform.DOJump(touch_facility.transform.position, 0.03f, 2, 0.5f).SetEase(Ease.Linear);
			}
		}
	}

	private void SetGreenIcon()
	{
		for (int i = 0; i < WORKER_ALL; i++)
		{
			if (manager.data.worker_data.worker_level[i] > 0)
			{
				worker_icon[i].sprite = SpriteManager.GetGreenIcon();
			}
		}
	}

	private void SetWorkerIcon()
	{
		GameObject original = Resources.Load("Prefab/worker_icon") as GameObject;
		for (int i = 0; i < WORKER_ALL; i++)
		{
			if (manager.data.worker_data.worker_level[i] > 0)
			{
				if (icon_obj[i] != null)
				{
					UnityEngine.Object.Destroy(icon_obj[i].gameObject);
					icon_obj[i] = null;
				}
				icon_obj[i] = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
				if (manager.data.worker_data.worker_place[i] != -1)
				{
					icon_obj[i].transform.localPosition = SetIcon[manager.data.worker_data.worker_place[i]];
				}
				else
				{
					icon_obj[i].transform.localPosition = worker_menu.transform.InverseTransformPoint(worker_icon[i].transform.position);
				}
				icon_obj[i].GetComponent<SpriteRenderer>().sprite = SpriteManager.GetWorkingIcon(Convert.ToWorkerType(i));
			}
		}
	}

	private void SetAreaSprite(TouchEvent touch)
	{
		SpriteRenderer[] array = new SpriteRenderer[(!IsPurchased()) ? 8 : WORKER_ALL];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = worker_menu.transform.Find("worker" + (i + 1) + "/area").GetComponent<SpriteRenderer>();
			array[i].enabled = true;
			array[i].sortingOrder = 10001;
		}
		array[touch.param.value1].enabled = false;
	}

	private void TouchLevelUp(TouchEvent touch, Worker.eType type)
	{
		if (manager.data.worker_data.worker_level[touch.param.value1] < 4)
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			GameObject original = Resources.Load("Prefab/worker_lv_prompt") as GameObject;
			promptPrefab = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			promptPrefab.transform.localPosition = SetPromptPos[touch.param.value1];
			SpriteRenderer component = promptPrefab.transform.Find("black_bg").GetComponent<SpriteRenderer>();
			component.transform.localScale = new Vector3(50f, 50f, 1f);
			component.color = new Color(0f, 0f, 0f, 0.5f);
			Utils.SetOrderInLayer(worker_obj[touch.param.value1].gameObject, 10000);
			prev_order = icon_obj[touch.param.value1].GetComponent<SpriteRenderer>().sortingOrder;
			if (manager.data.worker_data.worker_place[touch.param.value1] == -1)
			{
				icon_obj[touch.param.value1].GetComponent<SpriteRenderer>().sortingOrder = 25000;
			}
			int money_value = Price.LvUpWorkerPrice();
			int num = Digit(money_value);
			SpriteRenderer component2 = promptPrefab.transform.Find("coin").GetComponent<SpriteRenderer>();
			Vector2 v = SetCoinPos[num - 1];
			component2.transform.localPosition = v;
			TextMesh component3 = promptPrefab.transform.Find("text").GetComponent<TextMesh>();
			component3.text = string.Empty + money_value;
			Vector2 v2 = SetTextPos[num - 1];
			component3.transform.localPosition = v2;
			SpriteRenderer component4 = promptPrefab.transform.Find("star_left").GetComponent<SpriteRenderer>();
			SpriteRenderer component5 = promptPrefab.transform.Find("star_right").GetComponent<SpriteRenderer>();
			component4.sprite = SpriteManager.GetWorkerPromptStar(manager.data.worker_data.worker_level[touch.param.value1] - 1);
			component5.sprite = SpriteManager.GetWorkerPromptStar(manager.data.worker_data.worker_level[touch.param.value1]);
			touch_ok = promptPrefab.transform.Find("ok_button").GetComponent<TouchEvent>();
			touch_ok.ClickUp.AddListener(delegate
			{
				WorkerLvUpOK(touch, type, money_value, prev_order);
			});
			TouchEvent component6 = promptPrefab.transform.Find("cancel_button").GetComponent<TouchEvent>();
			component6.ClickUp.AddListener(delegate
			{
				WorkerLvUpCancel(touch, type, prev_order);
			});
			SetPromptOkButton(money_value);
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
		}
	}

	private int Digit(int num)
	{
		return num.ToString().Length;
	}

	private void WorkerLvUpOK(TouchEvent touch, Worker.eType type, int money, int _prev_order)
	{
		if (manager.data.coin >= money)
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			UnityEngine.Object.Destroy(promptPrefab.gameObject);
			promptPrefab = null;
			manager.data.worker_data.SetWorkerLvUP(touch);
			int coinCount = manager.data.coin - money;
			manager.data.SetCoinCount(coinCount);
			SetWorkerStar(touch);
			SetLvpButton(touch);
			Utils.SetOrderInLayer(worker_obj[touch.param.value1].gameObject, -10000);
			if (manager.data.worker_data.worker_place[touch.param.value1] == -1)
			{
				icon_obj[touch.param.value1].GetComponent<SpriteRenderer>().sortingOrder = _prev_order;
			}
			else if (manager.data.worker_data.worker_place[touch.param.value1] == 11)
			{
				manager.hotel.LevelUpWorker();
			}
			Vector2 pos = worker_parts[touch.param.value1].transform.TransformPoint(new Vector2(0f, 0.2f));
			Effect.LevelupSmall(manager.data.worker_data.worker_level[touch.param.value1], pos, worker_parts[touch.param.value1].transform, Color.white);
			Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
		}
	}

	public void WorkerLvUpCancel(TouchEvent touch, Worker.eType type, int _prev_order)
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
		UnityEngine.Object.Destroy(promptPrefab.gameObject);
		promptPrefab = null;
		Utils.SetOrderInLayer(worker_obj[touch.param.value1].gameObject, -10000);
		if (manager.data.worker_data.worker_place[touch.param.value1] == -1)
		{
			icon_obj[touch.param.value1].GetComponent<SpriteRenderer>().sortingOrder = _prev_order;
		}
	}

	private void SetWorkerStar(TouchEvent touch)
	{
		for (int i = 0; i < 4; i++)
		{
			if (manager.data.worker_data.worker_level[touch.param.value1] > i)
			{
				star_sprite[touch.param.value1, i].sprite = SpriteManager.GetWorkerStar(1);
			}
			else
			{
				star_sprite[touch.param.value1, i].sprite = SpriteManager.GetWorkerStar(0);
			}
		}
	}

	private void SetLvpButton(TouchEvent touch)
	{
		SpriteRenderer component = framePrefab.transform.Find("button").GetComponent<SpriteRenderer>();
		if (manager.data.worker_data.worker_level[touch.param.value1] >= 4)
		{
			component.sprite = button_off;
		}
		else
		{
			component.sprite = button_on;
		}
	}

	private void SetPromptOkButton(int need_coin)
	{
		SpriteRenderer component = touch_ok.GetComponent<SpriteRenderer>();
		if (manager.data.coin >= need_coin)
		{
			component.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			component.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
	}

	public void TouchWorkerFacility(TouchEvent touch, Worker.eType type, Worker.eWorkPlace place)
	{
		if (selector != null)
		{
			UnityEngine.Object.Destroy(selector.gameObject);
		}
		for (int i = 0; i < 13; i++)
		{
			if (place == (Worker.eWorkPlace)manager.data.worker_data.worker_place[i] && touch.param.value1 != i)
			{
				preceding_worker = i;
				break;
			}
			preceding_worker = -1;
		}
		if (preceding_worker != -1)
		{
			Manager.sound.PlaySe(Sound.eSe.BEEP);
			Vector3 endValue = SetIcon[(int)place];
			icon_obj[preceding_worker].transform.DOLocalJump(endValue, 0.1f, 2, 0.5f).SetEase(Ease.Linear);
			return;
		}
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		Rect rect;
		Vector2 pos;
		if (place <= Worker.eWorkPlace.FACILITY_10)
		{
			rect = new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.12f));
			pos = new Vector2(0f, 0.05f);
		}
		else if (place != Worker.eWorkPlace.HOTEL)
		{
			rect = new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.2f));
			pos = new Vector2(0f, -0.007f);
		}
		else
		{
			rect = new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.2f));
			pos = new Vector2(0f, -0.007f);
		}
		selector = Common.CreateSelector(rect, touch.transform, pos, 32000);
		SetWorkerPrompt(touch, type, place);
	}

	private void SetWorkerPrompt(TouchEvent touch, Worker.eType type, Worker.eWorkPlace place)
	{
		if (worker_place_prompt != null)
		{
			UnityEngine.Object.Destroy(worker_place_prompt.gameObject);
			worker_place_prompt = null;
		}
		if (place == Worker.eWorkPlace.SAILO)
		{
			GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/worker_sailo_prompt") as GameObject;
			worker_place_prompt = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			worker_place_prompt.transform.localPosition = new Vector3(0.42f, 0f, -11f);
			SpriteRenderer[] array = new SpriteRenderer[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = worker_place_prompt.transform.Find("sailo" + (i + 1)).GetComponent<SpriteRenderer>();
				if (manager.data.worker_data.worker_level[touch.param.value1] > i)
				{
					array[i].color = new Color(1f, 1f, 1f, 1f);
				}
				else
				{
					array[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
				}
			}
			Animator component = worker_place_prompt.transform.Find("work").GetComponent<Animator>();
			Utils.Play(component, "work_sailo", 1f);
		}
		else if (place == Worker.eWorkPlace.HOTEL)
		{
			GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/worker_hotel_prompt") as GameObject;
			worker_place_prompt = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			worker_place_prompt.transform.localPosition = new Vector3(0f, -1.58f, -11f);
			Animator[] array2 = new Animator[4];
			TextMesh[] array3 = new TextMesh[4];
			for (int j = 0; j < 4; j++)
			{
				array3[j] = worker_place_prompt.transform.Find("work" + (j + 1) + "/text").GetComponent<TextMesh>();
				array3[j].text = string.Empty + HotelManager.tHOTEL_ROOM_ID[(int)Data.farm_type, j];
				array2[j] = worker_place_prompt.transform.Find("work" + (j + 1)).GetComponent<Animator>();
				if (manager.data.worker_data.worker_level[touch.param.value1] > j)
				{
					Utils.Play(array2[j], "work_hotel_open", 1f);
				}
				else
				{
					Utils.Play(array2[j], "work_hotel_close", 1f);
				}
			}
		}
		else
		{
			GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/worker_hotel_prompt") as GameObject;
			worker_place_prompt = UnityEngine.Object.Instantiate(original, worker_menu.transform, worldPositionStays: false);
			worker_place_prompt.transform.localPosition = new Vector3(0f, -1.58f, -11f);
			Animator[] array4 = new Animator[4];
			for (int k = 0; k < 4; k++)
			{
				array4[k] = worker_place_prompt.transform.Find("work" + (k + 1)).GetComponent<Animator>();
				if (manager.data.worker_data.worker_level[touch.param.value1] > k)
				{
					Utils.Play(array4[k], "work_facility_" + (k + 1) + "_open", 1f);
				}
				else
				{
					Utils.Play(array4[k], "work_facility_" + (k + 1) + "_close", 1f);
				}
			}
		}
		assign_button = worker_place_prompt.transform.Find("assign_button").GetComponent<SpriteRenderer>();
		SetWorkerPromptButton(touch, type, place);
		TouchEvent touch_button = assign_button.GetComponent<TouchEvent>();
		touch_button.ClickUp.AddListener(delegate
		{
			WokerPromptOK(touch_button, type, place, touch.param.value1);
		});
		SpriteRenderer component2 = worker_place_prompt.transform.Find("black_bg").GetComponent<SpriteRenderer>();
		component2.transform.localScale = new Vector3(50f, 50f, 1f);
		component2.color = new Color(0f, 0f, 0f, 0.3f);
		TouchEvent touch_bg = component2.GetComponent<TouchEvent>();
		touch_bg.ClickUp.AddListener(delegate
		{
			TouchPromptBG(touch_bg);
		});
	}

	private void TouchPromptBG(TouchEvent touch)
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
		DestroyWorkerPrompt();
	}

	private void WokerPromptOK(TouchEvent touch, Worker.eType type, Worker.eWorkPlace place, int i)
	{
		if (manager.data.worker_data.worker_place[i] == (int)place)
		{
			Manager.sound.PlaySe(Sound.eSe.CANCEL);
			WorkerMapRemove(type, place);
			place = Worker.eWorkPlace.NONE;
			manager.data.worker_data.SetWorkerPlace(i, place);
			DoAssignButtonOFF(touch);
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			if (manager.data.worker_data.worker_place[i] != -1)
			{
				WorkerMapRemove(type, (Worker.eWorkPlace)manager.data.worker_data.worker_place[i]);
			}
			manager.data.worker_data.SetWorkerPlace(i, place);
			DoAssignButtonON(touch);
			WorkerMapSet(type, place);
		}
		SetGreenIcon();
		DoWorkerIconMove(icon_obj[i], type, place, i);
	}

	private void WorkerMapSet(Worker.eType type, Worker.eWorkPlace place)
	{
		switch (place)
		{
		case Worker.eWorkPlace.HOTEL:
			manager.hotel.AssignWorker(type);
			break;
		case Worker.eWorkPlace.SAILO:
			manager.silo.AssignWorker(type);
			break;
		default:
			manager.map.facility_list[(int)place].AssignWorker(type);
			break;
		}
	}

	private void WorkerMapRemove(Worker.eType type, Worker.eWorkPlace place)
	{
		switch (place)
		{
		case Worker.eWorkPlace.HOTEL:
			manager.hotel.FreeWorker();
			break;
		case Worker.eWorkPlace.SAILO:
			manager.silo.FreeWorker();
			break;
		default:
			manager.map.facility_list[(int)place].FreeWorker();
			break;
		}
	}

	private void DoWorkerIconMove(GameObject obj, Worker.eType type, Worker.eWorkPlace place, int i)
	{
		Vector3 position;
		if (place != Worker.eWorkPlace.NONE)
		{
			position = new Vector3(SetIcon[(int)place].x, SetIcon[(int)place].y, 0f);
		}
		else
		{
			Vector2 vector = worker_menu.transform.InverseTransformPoint(worker_icon[i].transform.position);
			position = new Vector3(vector.x, vector.y, 0f);
		}
		Vector3 endValue = worker_menu.transform.TransformPoint(position);
		myIconMoveSequence[i] = DOTween.Sequence();
		myIconMoveSequence[i].Append(obj.transform.DOMove(endValue, 1f));
	}

	private void DoAssignButtonON(TouchEvent touch)
	{
		myAssignONSequence = DOTween.Sequence();
		myAssignONSequence.Append(touch.transform.DOScale(new Vector3(1f, 0.6f, 1f), 0.1f).OnComplete(delegate
		{
			DoChangeButtonSprite(touch, 1);
		}));
		myAssignONSequence.Append(touch.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
	}

	private void DoAssignButtonOFF(TouchEvent touch)
	{
		myAssignONSequence = DOTween.Sequence();
		myAssignONSequence.Append(touch.transform.DOScale(new Vector3(1f, 0.6f, 1f), 0.1f).OnComplete(delegate
		{
			DoChangeButtonSprite(touch, 0);
		}));
		myAssignONSequence.Append(touch.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
	}

	private void DoChangeButtonSprite(TouchEvent touch, int on_off)
	{
		touch.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetWorkerPromptButton(on_off);
	}

	public void DestroyWorkerPrompt()
	{
		UnityEngine.Object.Destroy(worker_place_prompt.gameObject);
		worker_place_prompt = null;
		if (selector != null)
		{
			UnityEngine.Object.Destroy(selector.gameObject);
			selector = null;
		}
	}

	private void SetWorkerPromptButton(TouchEvent touch, Worker.eType type, Worker.eWorkPlace place)
	{
		int i = (manager.data.worker_data.worker_place[touch.param.value1] == (int)place) ? 1 : 0;
		assign_button.sprite = SpriteManager.GetWorkerPromptButton(i);
	}

	public void TouchBlackBg()
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
		if (worker_menu != null)
		{
			UnityEngine.Object.Destroy(worker_menu.gameObject);
			worker_menu = null;
		}
		Manager.office.menu.gameObject.SetActive(value: true);
		Manager.office.menu.ResetButton();
		Manager.office.menu.menu_flag = 1;
		menu_flag = 0;
	}

	private IEnumerator MovePurchase(float target_y, float target_agency_y, UnityAction arrival_callback)
	{
		Vector3 pos = worker_menu.transform.localPosition;
		Vector3 agency_pos = purchase.transform.localPosition;
		while (true)
		{
			Vector3 localPosition = worker_menu.transform.localPosition;
			if (localPosition.y == target_y)
			{
				Vector3 localPosition2 = purchase.transform.localPosition;
				if (localPosition2.y == target_agency_y)
				{
					break;
				}
			}
			pos = Vector2.MoveTowards(pos, new Vector2(pos.x, target_y), 2f * Time.deltaTime);
			Transform transform = worker_menu.transform;
			float x = pos.x;
			float y = pos.y;
			Vector3 localPosition3 = worker_menu.transform.localPosition;
			transform.localPosition = new Vector3(x, y, localPosition3.z);
			agency_pos = Vector2.MoveTowards(agency_pos, new Vector2(agency_pos.x, target_agency_y), 2.5f * Time.deltaTime);
			Transform transform2 = purchase.transform;
			float x2 = agency_pos.x;
			float y2 = agency_pos.y;
			Vector3 localPosition4 = purchase.transform.localPosition;
			transform2.localPosition = new Vector3(x2, y2, localPosition4.z);
			yield return new WaitForSeconds(0.01f);
		}
		arrival_callback?.Invoke();
	}

	private void OpenPurchase()
	{
		purchase_agency.SetEnabled(enabled: false);
		SetEnabledSpecial(enabled: true, touch_enabled: false);
		black_bg.color = new Color(0f, 0f, 0f, 0.7f);
		SetEnabledWorker(enabled: false);
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		StartCoroutine(MovePurchase(default_y + 1f, default_purchase_y + -0.85f, FinishedOpenPurchase));
	}

	private void FinishedOpenPurchase()
	{
		purchase_button.GetComponent<TouchEvent>().SetEnabled(enabled: true);
		purchase.GetComponent<TouchEvent>().SetEnabled(enabled: true);
		purchase.transform.Find("agency").gameObject.SetActive(value: false);
		purchase_button.transform.parent.parent.gameObject.SetActive(value: true);
		purchase_attention.SetActive(value: true);
		if (Data.farm_type == Data.eFarmType.NORMAL)
		{
			setPurchaseButton(Purchaser.eTYPE.WORKER_FARM);
		}
		else if (Data.farm_type == Data.eFarmType.RESORT)
		{
			setPurchaseButton(Purchaser.eTYPE.WORKER_RESORT);
		}
	}

	private void ClosePurchase()
	{
		purchase.GetComponent<TouchEvent>().SetEnabled(enabled: false);
		SetEnabledSpecial(enabled: false, touch_enabled: false);
		purchase.transform.Find("agency").gameObject.SetActive(value: true);
		purchase_button.GetComponent<TouchEvent>().SetEnabled(enabled: false);
		purchase_button.transform.parent.parent.gameObject.SetActive(value: false);
		purchase_attention.SetActive(value: false);
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		StartCoroutine(MovePurchase(default_y, default_purchase_y, FinishedClosePurchase));
	}

	private void FinishedClosePurchase()
	{
		SetEnabledWorker(enabled: true);
		purchase_agency.SetEnabled(enabled: true);
		black_bg.color = new Color(0f, 0f, 0f, 0.5f);
	}

	public void PushedPurchaseDown()
	{
		Utils.Log("PushedPuchaseDown");
		if (manager.purchaser.CheckPurchaserState())
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			SetButtonAnim(3f, 0f);
		}
	}

	private void SetButtonAnim(float speed, float normalized_time)
	{
		Animation component = purchase_button.transform.parent.GetComponent<Animation>();
		Utils.Play(component, component.clip.name, speed, normalized_time);
	}

	public void PushedPurchaseClickUp()
	{
		Utils.Log("PushedPuchaseClickUp");
		if (manager.purchaser.CheckPurchaserState())
		{
			if (Data.farm_type == Data.eFarmType.NORMAL)
			{
				manager.purchaser.BuyNonConsumable(Purchaser.eTYPE.WORKER_FARM);
			}
			else if (Data.farm_type == Data.eFarmType.RESORT)
			{
				manager.purchaser.BuyNonConsumable(Purchaser.eTYPE.WORKER_RESORT);
			}
		}
	}




	public static void SetPurchaseButton(Purchaser.eTYPE type)
	{
		instance.setPurchaseButton(type);
	}

	private void setPurchaseButton(Purchaser.eTYPE type)
	{
		if (((Data.farm_type == Data.eFarmType.NORMAL && type == Purchaser.eTYPE.WORKER_FARM) || (Data.farm_type == Data.eFarmType.RESORT && type == Purchaser.eTYPE.WORKER_RESORT)) && !IsPurchased() && purchase_text != null)
		{
			purchase_text.SetText(string.Empty + Purchaser.price[(int)type]);
			if (!manager.purchaser.m_InitializeSuccess)
			{
				purchase_button.sprite = SpriteManager.GetPurchaserButton(1);
				purchase_button.transform.localPosition = new Vector3(-0.035f, -0.37f, -10f);
			}
			else
			{
				purchase_button.sprite = SpriteManager.GetPurchaserButton(0);
			}
		}
	}

	public static void DecidePurchase(Purchaser.eTYPE type)
	{
		instance.manager.data.SetPurchase(type, 1);
		if (((Data.farm_type == Data.eFarmType.NORMAL && type == Purchaser.eTYPE.WORKER_FARM) || (Data.farm_type == Data.eFarmType.RESORT && type == Purchaser.eTYPE.WORKER_RESORT)) && instance.worker_menu != null)
		{
			instance.SetPurchaseArea(enabled: false);
			instance.SetEnabledWorker(enabled: true);
			UnityAction arrival_callback = delegate
			{
				instance.SetEnabledSpecial(enabled: true, touch_enabled: true);
			};
			instance.StartCoroutine(instance.MovePurchase(instance.default_y, instance.default_purchase_y, arrival_callback));
		}
	}

	public static bool IsOpen()
	{
		return instance.worker_menu != null;
	}

	public bool IsPurchased()
	{
		Purchaser.eTYPE eTYPE = (Data.farm_type != 0) ? Purchaser.eTYPE.WORKER_RESORT : Purchaser.eTYPE.WORKER_FARM;
		return manager.data.purchase[(int)eTYPE] == 1;
	}
}
