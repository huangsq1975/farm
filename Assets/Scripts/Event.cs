using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
	public enum eState
	{
		NONE = -1,
		CHARACTER_SELECT,
		OPENING,
		TUTORIAL_GRASS_CUT,
		TUTORIAL_OFFICE_FACILITY,
		TUTORIAL_CASHER_FACILITY,
		TUTORIAL_SELECT_FACILITY_CATEGORY,
		TUTORIAL_SELECT_FACILITY,
		TUTORIAL_FAILICY_OK,
		TUTORIAL_CONSTRUCT,
		TUTORIAL_PRESENT_BOX,
		TUTORIAL_FIX_CONSTRUCT,
		TUTORIAL_FIX_FACILITY,
		TUTORIAL_OFFICE_ANIMAL,
		TUTORIAL_CASHER_ANIMAL,
		TUTORIAL_SELECT_ANIMAL,
		TUTORIAL_ANIMAL_OK,
		TUTORIAL_ANIMAL_COME,
		TUTORIAL_HARVEST,
		TUTORIAL_STORE,
		TUTORIAL_LAST,
		OPENING_FINISH,
		KEEP_OUT_GRASS_CUT,
		ENDING,
		RESORT,
		MAX
	}

	public class Balloon
	{
		public Animation anim;

		public Animator contents;

		public Animator granpa;

		public SpriteRenderer granpa_sr;

		public Animator[] enemy = new Animator[2];

		public Animation[] keep_out = new Animation[2];
	}

	public class BalloonEd
	{
		public Animation anim;

		public Animator dev;

		public SpriteRenderer lv99;

		public SpriteRenderer achievement;

		public SpriteRenderer[] add_animal = new SpriteRenderer[2];
	}

	public class BalloonResort
	{
		public Animation anim;

		public Animator contents;

		public Animator granpa;

		public Animator bg;
	}

	public class Main
	{
		public PartsController controller;

		public TouchEvent touch;
	}

	private enum eGranpaWaitState
	{
		NONE = -1,
		CLOCK,
		CLOCK_OFF,
		PRESENT,
		PRESENT_OFF
	}

	private static readonly Vector2[] tNOTICE_POS = new Vector2[24]
	{
		Vector2.zero,
		Vector2.zero,
		new Vector2(-0.61f, 0.88f),
		new Vector2(0f, 1.52f),
		new Vector2(0f, 1.2f),
		new Vector2(-0.85f, 1.05f),
		new Vector2(-1.04f, 0.66f),
		new Vector2(-0.8f, 0.4f),
		new Vector2(-0.99f, -0.35f),
		Vector2.zero,
		new Vector2(-0.61f, 1.65f),
		new Vector2(-0.85f, 1.13f),
		new Vector2(0f, 1.52f),
		new Vector2(0f, 1.2f),
		new Vector2(-0.99f, 1.25f),
		new Vector2(-0.95f, 0.83f),
		new Vector2(-0.96f, 0.05f),
		new Vector2(-0.98f, 1.23f),
		Vector2.zero,
		Vector2.zero,
		Vector2.zero,
		new Vector2(-0.61f, 0.26f),
		Vector2.zero,
		Vector2.zero
	};

	public static int LAYER_EVENT = 0;

	public static int LAYER_SELECT = 0;

	private static int CULLING_MASK = 0;

	private Manager m;

	private TouchManager tm;

	public eState state = eState.NONE;

	private Main main;

	private Main main_2;

	private GameObject character_select;

	private GameObject dev_name_canvas;

	private InputField dev_name_input;

	private GameObject selector;

	private Balloon bln;

	private BalloonEd bln2;

	private BalloonResort bln_rst;

	private Animator granpa;

	private SpriteRenderer granpa_hat;

	private Sequence seq;

	private GameObject HollowedCamera;

	private GameObject sphare;

	private SpriteRenderer skip;

	private Animation notice;
	private Sequence notice_seq;

	private static string prefix = string.Empty;

	private const string CASHER_ADD_FACILITY_NAME = "cash_add_facility(Clone)";

	private const string CASHER_ADD_FACILITY_TOUCH_NAME = "cash_add_facility(Clone)/cash_choose_facility(Clone)/content/facility_touch_prefab(Clone)";

	private const string BUY_PROMPT_OK = "buy_prompt(Clone)/bg/icon_ok";

	private float elapsed_time;

	private float EVENT_TIME_LIMIT = -1f;

	private eGranpaWaitState granpa_wait_state = eGranpaWaitState.NONE;

	private const string CASHER_ADD_ANIMAL_NAME = "cash_add_animal(Clone)";

	private const string CASHER_ADD_ANIMAL_TOUCH_NAME = "cash_add_animal(Clone)/content/cash_animal_character(Clone)";

	private int current_coin;

	private Animation feels;

	public void Init(Manager manager)
	{
		LAYER_EVENT = LayerMask.NameToLayer("Event");
		LAYER_SELECT = LayerMask.NameToLayer("Select");
		CULLING_MASK = Camera.main.cullingMask;
		m = manager;
		tm = m.GetComponent<TouchManager>();
	}

	private void SetLayer(int layer)
	{
		for (int i = 0; i < m.map.facility_list.Count; i++)
		{
			Utils.SetLayer(m.map.facility_list[i].gameObject, layer);
			m.map.facility_list[i].SetDefaultLayer();
		}
		Utils.SetLayer(Manager.office.gameObject, layer);
		Utils.SetLayer(m.hotel.hotel_exterior, layer);
		Utils.SetLayer(m.store.store_exterior, layer);
		Utils.SetLayer(m.transform.Find("bg_" + (int)Data.farm_type + "(Clone)").gameObject, layer);
	}

	private void Update()
	{
		if (state == eState.TUTORIAL_GRASS_CUT)
		{
			OnTutorialGrassCut();
		}
		else if (state == eState.TUTORIAL_OFFICE_FACILITY)
		{
			OnTutorialOfficeFacility();
		}
		else if (state == eState.TUTORIAL_CASHER_FACILITY)
		{
			OnTutorialCasherFacility();
		}
		else if (state == eState.TUTORIAL_SELECT_FACILITY_CATEGORY)
		{
			OnTutorialFacilityCategory();
		}
		else if (state == eState.TUTORIAL_SELECT_FACILITY)
		{
			OnTutorialFacility();
		}
		else if (state == eState.TUTORIAL_FAILICY_OK)
		{
			OnTutorialFacilityOK();
		}
		else if (state == eState.TUTORIAL_CONSTRUCT)
		{
			OnTutorialConstruct();
		}
		else if (state == eState.TUTORIAL_PRESENT_BOX)
		{
			OnTutorialPresentBox();
		}
		else if (state == eState.TUTORIAL_FIX_CONSTRUCT)
		{
			OnTutorialFixConstruct();
		}
		else if (state == eState.TUTORIAL_FIX_FACILITY)
		{
			OnTutorialFixFacility();
		}
		else if (state == eState.TUTORIAL_OFFICE_ANIMAL)
		{
			OnTutorialOfficeAnimal();
		}
		else if (state == eState.TUTORIAL_CASHER_ANIMAL)
		{
			OnTutorialCasherAnimal();
		}
		else if (state == eState.TUTORIAL_SELECT_ANIMAL)
		{
			OnTutorialAnimal();
		}
		else if (state == eState.TUTORIAL_ANIMAL_OK)
		{
			OnTutorialAnimalOK();
		}
		else if (state == eState.TUTORIAL_ANIMAL_COME)
		{
			OnTutorialAnimalCome();
		}
		else if (state == eState.TUTORIAL_HARVEST)
		{
			OnTutorialHarvest();
		}
		else if (state == eState.TUTORIAL_STORE)
		{
			OnTutorialStore();
		}
		else if (state == eState.TUTORIAL_LAST)
		{
			OnTutorialLast();
		}
		else if (state == eState.OPENING_FINISH)
		{
			OnOpeningFinish();
		}
	}

	private void OnTutorialGrassCut()
	{
		if (m.map.facility_list[0].state == Facility.eState.EMPTY)
		{
			tm.ClearSpecific();
			tm.AddSpecific(Manager.office.home);
			m.map.facility_list[0].PausePlant();
			m.main.EventTutorialGrassCut();
			state = eState.TUTORIAL_OFFICE_FACILITY;
			notice_seq.Kill();
			notice.transform.localPosition = tNOTICE_POS[(int)state];
			notice.Play();
		}
	}

	private void OnTutorialOfficeFacility()
	{
		if (Manager.office.menu != null)
		{
			tm.ClearSpecific();
			tm.AddSpecific(Manager.office.menu.GetCasherIcon());
			Manager.office.menu.AddTutorialPrivacyPolicy(tm);
			state = eState.TUTORIAL_CASHER_FACILITY;
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialCasherFacility()
	{
		Facility.IconIfo iconInfo = m.map.facility_list[0].GetIconInfo(Data.Condition.eCATEGORY.FACILITY);
		if (iconInfo != null)
		{
			tm.ClearSpecific();
			tm.AddSpecific(iconInfo.touch);
			state = eState.TUTORIAL_SELECT_FACILITY_CATEGORY;
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialFacilityCategory()
	{
		if (m.map.facility_list[0].GetIconInfo(Data.Condition.eCATEGORY.FACILITY) == null)
		{
			OnTutorialCommon(m.map.facility_list[0].transform.Find("cash_add_facility(Clone)/content/icon1"), eState.TUTORIAL_SELECT_FACILITY);
			if (state == eState.TUTORIAL_SELECT_FACILITY)
			{
				notice.transform.localPosition = tNOTICE_POS[(int)state];
			}
		}
	}

	private void OnTutorialFacility()
	{
		OnTutorialCommon(m.map.facility_list[0].transform.Find("cash_add_facility(Clone)/cash_choose_facility(Clone)/content/facility_touch_prefab(Clone)"), eState.TUTORIAL_FAILICY_OK);
		if (state == eState.TUTORIAL_FAILICY_OK)
		{
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialFacilityOK()
	{
		OnTutorialCommon(m.map.facility_list[0].transform.Find("cash_add_facility(Clone)/cash_choose_facility(Clone)/content/facility_touch_prefab(Clone)/buy_prompt(Clone)/bg/icon_ok"), eState.TUTORIAL_CONSTRUCT);
		if (state == eState.TUTORIAL_CONSTRUCT)
		{
			elapsed_time = 0f;
			EVENT_TIME_LIMIT = 2f;
			granpa_wait_state = eGranpaWaitState.CLOCK;
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialConstruct()
	{
		if (m.map.facility_list[0].state != Facility.eState.CONSTRUCT)
		{
			return;
		}
		if (notice.gameObject.activeSelf)
		{
			notice.gameObject.SetActive(value: false);
		}
		elapsed_time += Time.deltaTime;
		if (elapsed_time > EVENT_TIME_LIMIT)
		{
			if (granpa_wait_state == eGranpaWaitState.CLOCK)
			{
				Utils.Play(granpa, "granpa_1_clock_1_down", 1f, 0f);
				feels = FeelsClock(granpa.transform);
				Utils.Play(feels, "op_appear", 1f, 0f);
				granpa_wait_state = eGranpaWaitState.CLOCK_OFF;
				elapsed_time = 0f;
				EVENT_TIME_LIMIT = 2f;
			}
			else if (granpa_wait_state == eGranpaWaitState.CLOCK_OFF)
			{
				Utils.Play(feels, "op_appear", -1f, 1f);
				granpa_wait_state = eGranpaWaitState.PRESENT;
				elapsed_time = 0f;
				EVENT_TIME_LIMIT = 1f;
			}
			else if (granpa_wait_state == eGranpaWaitState.PRESENT)
			{
				Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
				UnityEngine.Object.Destroy(feels.gameObject);
				feels = FeelsPresent(granpa.transform);
				Utils.Play(feels, "op_appear", 1f, 0f);
				granpa_wait_state = eGranpaWaitState.PRESENT_OFF;
				elapsed_time = 0f;
				EVENT_TIME_LIMIT = 1f;
			}
			else if (granpa_wait_state == eGranpaWaitState.PRESENT_OFF)
			{
				UnityEngine.Object.Destroy(feels.gameObject);
				granpa_wait_state = eGranpaWaitState.NONE;
				m.map.facility_list[0].SetPresentConstructBox();
				state = eState.TUTORIAL_PRESENT_BOX;
				notice.transform.localPosition = tNOTICE_POS[(int)state];
			}
		}
	}

	private void OnTutorialPresentBox()
	{
		OnTutorialCommon(m.map.facility_list[0].transform.Find("present_video(Clone)"), eState.TUTORIAL_FIX_CONSTRUCT);
		if (state == eState.TUTORIAL_FIX_CONSTRUCT)
		{
			notice.gameObject.SetActive(value: true);
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialFixConstruct()
	{
		if (m.map.facility_list[0].state == Facility.eState.FIX)
		{
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
			m.map.facility_list[0].RemovePresentBox();
			m.main.EventTutorialGrassCut();
			OnTutorialCommon(m.map.facility_list[0].transform, eState.TUTORIAL_FIX_FACILITY);
			if (state == eState.TUTORIAL_FIX_FACILITY)
			{
				notice.transform.localPosition = tNOTICE_POS[(int)state];
			}
		}
	}

	private void OnTutorialFixFacility()
	{
		if (m.map.facility_list[0].state == Facility.eState.ACTIVE)
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
			OnTutorialCommon(Manager.office.home.transform, eState.TUTORIAL_OFFICE_ANIMAL);
			if (state == eState.TUTORIAL_OFFICE_ANIMAL)
			{
				notice.transform.localPosition = tNOTICE_POS[(int)state];
			}
		}
	}

	private void OnTutorialOfficeAnimal()
	{
		if (Manager.office.menu != null)
		{
			tm.ClearSpecific();
			tm.AddSpecific(Manager.office.menu.GetCasherIcon());
			Manager.office.menu.AddTutorialPrivacyPolicy(tm);
			state = eState.TUTORIAL_CASHER_ANIMAL;
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialCasherAnimal()
	{
		Facility.IconIfo iconInfo = m.map.facility_list[0].GetIconInfo(Data.Condition.eCATEGORY.FARMANIMAL);
		if (iconInfo != null)
		{
			tm.ClearSpecific();
			tm.AddSpecific(iconInfo.touch);
			state = eState.TUTORIAL_SELECT_ANIMAL;
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialAnimal()
	{
		if (m.map.facility_list[0].GetIconInfo(Data.Condition.eCATEGORY.FARMANIMAL) == null)
		{
			OnTutorialCommon(m.map.facility_list[0].transform.Find("cash_add_animal(Clone)/content/cash_animal_character(Clone)"), eState.TUTORIAL_ANIMAL_OK);
			if (state == eState.TUTORIAL_ANIMAL_OK)
			{
				notice.transform.localPosition = tNOTICE_POS[(int)state];
				Manager.office.menu.casher.SetEnabledContentsScroller(enabled: false);
			}
		}
	}

	private void OnTutorialAnimalOK()
	{
		OnTutorialCommon(m.map.facility_list[0].transform.Find("cash_add_animal(Clone)/content/cash_animal_character(Clone)/buy_prompt(Clone)/bg/icon_ok"), eState.TUTORIAL_ANIMAL_COME);
		if (state == eState.TUTORIAL_ANIMAL_COME)
		{
			notice.transform.localPosition = tNOTICE_POS[(int)state];
		}
	}

	private void OnTutorialAnimalCome()
	{
		if (notice.gameObject.activeSelf && m.map.facility_list[0].transform.Find("cash_add_animal(Clone)/content/cash_animal_character(Clone)/buy_prompt(Clone)/bg/icon_ok") == null)
		{
			notice.gameObject.SetActive(value: false);
		}
		if (m.map.facility_list[0].AnimalCount() > 0)
		{
			OnTutorialCommon(m.map.facility_list[0].animal_info.animals[0].transform, eState.TUTORIAL_HARVEST);
			if (state == eState.TUTORIAL_HARVEST)
			{
				notice.gameObject.SetActive(value: true);
				notice.transform.localPosition = tNOTICE_POS[(int)state];
				notice.Play();
			}
		}
	}

	private void OnTutorialHarvest()
	{
		if (m.map.facility_list[0].IsHungry(0))
		{
			UnityEngine.Object.Destroy(notice.gameObject);
			notice = null;
			main = SetMain(m.data.main_type, m.transform);
			main.controller.Play(PartsController.eAnimType._STAY_1_SIDE, 1f, 0f);
			main.controller.SetHat(PartsController.eHat.NONE);
			main.controller.transform.localPosition = m.main.transform.localPosition;
			main.controller.transform.localScale = new Vector3(-1f, 1f, 1f);
			m.main.gameObject.SetActive(value: false);
			m.data.store_data.SetNextCustomerTime((ulong)(DateTime.Now.Ticks + 20000000));
			current_coin = m.data.coin;
			state = eState.TUTORIAL_STORE;
		}
	}

	private void OnTutorialStore()
	{
		if (m.data.coin != current_coin)
		{
			current_coin = m.data.coin;
			elapsed_time = 0f;
			EVENT_TIME_LIMIT = 7f;
			main.controller.transform.localScale = new Vector3(1f, 1f, 1f);
			main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendInterval(0.5f);
			Vector2 vector = main.controller.transform.localPosition;
			sequence.AppendCallback(delegate
			{
				Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
			});
			sequence.AppendCallback(delegate
			{
				main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
			});
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
			});
			sequence.Append(main.controller.transform.DOLocalMove(new Vector2(-0.297f, 0.91f), 0.5f).SetEase(Ease.Linear));
			sequence.AppendCallback(delegate
			{
				Utils.Play(granpa, "granpa_1_hug_1_down", 1f, 0f);
			});
			sequence.Join(main.controller.transform.DOLocalMove(new Vector2(-0.307f, 0.953f), 0.3f).SetEase(Ease.Linear));
			sequence.AppendCallback(delegate
			{
				main.controller.Play(PartsController.eAnimType._HUG_1_UP, 1f, 0f);
				Manager.sound.PlaySe(Sound.eSe.HUG);
			});
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
			});
			sequence.AppendCallback(delegate
			{
				Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
			});
			sequence.Append(main.controller.transform.DOLocalMoveY(vector.y, 1f).SetEase(Ease.Linear));
			sequence.AppendCallback(delegate
			{
				Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
			});
			sequence.AppendCallback(delegate
			{
				main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
			});
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendInterval(0.5f);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.GOOD);
			});
			sequence.AppendCallback(delegate
			{
				state = eState.TUTORIAL_LAST;
			});
			sequence.Play();
			seq = sequence;
		}
	}

	private void OnTutorialLast()
	{
		if (elapsed_time == 0f)
		{
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
			tm.ClearSpecific();
			Vector2 vector = main.controller.transform.position;
			CreateHollowedCamera(new Vector2(vector.x, vector.y + 0.1f));
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(2f);
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.MASK);
			});
			sequence.Append(sphare.transform.DOScale(1f, 1.5f));
			sequence.AppendInterval(1f);
			sequence.Append(sphare.transform.DOScale(0f, 0.3f));
			sequence.AppendCallback(delegate
			{
				Manager.sound.SetVolumeBgm(1f);
				Manager.sound.PlayBgm(Sound.eBgm.FIELD);
			});
			sequence.Play();
		}
		elapsed_time += Time.deltaTime;
		if (elapsed_time > EVENT_TIME_LIMIT)
		{
			m.map.facility_list[0].GiveGrass(0, grass: true, immediate: true);
			m.map.facility_list[0].GiveGrass(0, grass: true, immediate: true);
			state = eState.OPENING_FINISH;
		}
	}

	private void OnOpeningFinish()
	{
		state = eState.NONE;
		m.data.SetOpening();
		m.data.SetTreePlant(1);
		UnityEngine.Object.Destroy(HollowedCamera.gameObject);
		UnityEngine.Object.Destroy(granpa.gameObject);
		UnityEngine.Object.Destroy(main.controller.gameObject);
		m.main.gameObject.SetActive(value: true);
		Camera.main.cullingMask = CULLING_MASK;
		tm.ClearSpecific();
		tm.SetSpecificEnabled(enabled: false);
		m.main.FinishEvent();
		SetKeepOutGrassCut();
	}

	public void SetKeepOutGrassCut()
	{
		notice_seq = DOTween.Sequence();
		notice = CreateTutorialNotice(notice_seq, tNOTICE_POS[21]);
		feels = FeelsCoin(m.transform);
		Utils.Play(feels, "op_appear", 1f, 0f);
		Transform target = feels.transform.Find("bg/coin");
		Transform target2 = feels.transform.Find("bg/exp");
		Sequence sequence = DOTween.Sequence();
		sequence.Append(target.DOLocalMoveY(0.11f, 0.5f).SetEase(Ease.Flash));
		sequence.Append(target.DOLocalMoveY(0.09f, 0.5f).SetEase(Ease.Flash));
		sequence.SetLoops(-1);
		sequence.Play();
		Sequence sequence2 = DOTween.Sequence();
		sequence2.AppendInterval(0.2f);
		sequence2.Append(target2.DOLocalMoveY(0.11f, 0.5f).SetEase(Ease.Flash));
		sequence2.Append(target2.DOLocalMoveY(0.09f, 0.5f).SetEase(Ease.Flash));
		sequence2.SetLoops(-1);
		sequence2.Play();
		StartCoroutine(OnKeepOutGrassCut(sequence, sequence2));
	}

	private IEnumerator OnKeepOutGrassCut(Sequence coin_seq, Sequence exp_seq)
	{
		while (notice != null)
		{
			yield return new WaitForSeconds(0.5f);
			if (m.map.facility_list[2].state == Facility.eState.EMPTY)
			{
				notice_seq.Kill();
				coin_seq.Kill();
				exp_seq.Kill();
				UnityEngine.Object.Destroy(notice.gameObject);
				UnityEngine.Object.Destroy(feels.gameObject);
				notice = null;
			}
		}
	}

	private void OnTutorialCommon(Transform t, eState next)
	{
		if (t != null)
		{
			tm.ClearSpecific();
			tm.AddSpecific(t.GetComponent<TouchEvent>());
			state = next;
		}
	}

	private Main SetSelectMain(Data.eMainType type, Transform parent, TouchEvent icon_ok)
	{
		Main mn = SetMain(type, parent);
		UnityAction @object = delegate
		{
			if (selector != null)
			{
				UnityEngine.Object.Destroy(selector);
			}
			Rect rect = new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.15f));
			Vector3 localPosition = mn.controller.transform.localPosition;
			Vector2 pos = new Vector2(localPosition.x, 0.107f);
			selector = Common.CreateSelector(rect, parent, pos, 30020);
			Manager.sound.PlaySe(Sound.eSe.GOOD);
			Utils.SetLayer(selector, LAYER_SELECT);
			if (type == Data.eMainType.MAIN_1)
			{
				mn.controller.Play(PartsController.eAnimType._WALK_1_DOWN, 1f, 0f);
				main_2.controller.Play(PartsController.eAnimType._STAY_1_DOWN, 0f, 0f);
			}
			else
			{
				mn.controller.Play(PartsController.eAnimType._WALK_1_DOWN, 1f, 0f);
				main.controller.Play(PartsController.eAnimType._STAY_1_DOWN, 0f, 0f);
			}
			m.data.SetMainType(type);
			m.main.controller.SetStyle(MainCharacter.style[(int)type]);
			m.silo.ChangeSiloBoy();
			tm.AddSpecific(icon_ok);
			icon_ok.transform.Find("contents/sprite").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		};
		mn.touch.ClickUp.AddListener(@object.Invoke);
		mn.controller.SetHat(PartsController.eHat.NONE);
		mn.controller.SetSortingOrderAll(30010);
		mn.controller.Play(PartsController.eAnimType._STAY_1_DOWN, 0f, 0f);
		Utils.SetLayer(mn.controller.gameObject, LAYER_SELECT);
		mn.controller.transform.localScale = new Vector3(0f, 0f, 1f);
		return mn;
	}

	private void EnsureEventSystem()
	{
		if (EventSystem.current != null)
		{
			return;
		}
		GameObject es = new GameObject("EventSystem");
		es.AddComponent<EventSystem>();
		es.AddComponent<StandaloneInputModule>();
	}

	private void SetupDevNameInput(GameObject parent)
	{
		if (!Data.IsDevelopmentBuild())
		{
			return;
		}
		EnsureEventSystem();
		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		dev_name_canvas = new GameObject("DevNameCanvas");
		dev_name_canvas.transform.SetParent(parent.transform, worldPositionStays: false);
		Canvas canvas = dev_name_canvas.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 32767;
		CanvasScaler scaler = dev_name_canvas.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(800f, 600f);
		dev_name_canvas.AddComponent<GraphicRaycaster>();
		GameObject inputRoot = new GameObject("DevNameInput");
		inputRoot.transform.SetParent(dev_name_canvas.transform, worldPositionStays: false);
		RectTransform inputRect = inputRoot.AddComponent<RectTransform>();
		inputRect.anchorMin = new Vector2(0.5f, 0f);
		inputRect.anchorMax = new Vector2(0.5f, 0f);
		inputRect.pivot = new Vector2(0.5f, 0f);
		inputRect.anchoredPosition = new Vector2(0f, 36f);
		inputRect.sizeDelta = new Vector2(280f, 36f);
		Image bgImage = inputRoot.AddComponent<Image>();
		bgImage.color = new Color(1f, 1f, 1f, 0.92f);
		GameObject textGo = new GameObject("Text");
		textGo.transform.SetParent(inputRoot.transform, worldPositionStays: false);
		RectTransform textRect = textGo.AddComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = new Vector2(8f, 4f);
		textRect.offsetMax = new Vector2(-8f, -4f);
		Text text = textGo.AddComponent<Text>();
		text.font = font;
		text.color = Color.black;
		text.alignment = TextAnchor.MiddleLeft;
		text.fontSize = 18;
		text.supportRichText = false;
		GameObject placeholderGo = new GameObject("Placeholder");
		placeholderGo.transform.SetParent(inputRoot.transform, worldPositionStays: false);
		RectTransform placeholderRect = placeholderGo.AddComponent<RectTransform>();
		placeholderRect.anchorMin = Vector2.zero;
		placeholderRect.anchorMax = Vector2.one;
		placeholderRect.offsetMin = new Vector2(8f, 4f);
		placeholderRect.offsetMax = new Vector2(-8f, -4f);
		Text placeholder = placeholderGo.AddComponent<Text>();
		placeholder.font = font;
		placeholder.color = new Color(0.45f, 0.45f, 0.45f, 1f);
		placeholder.alignment = TextAnchor.MiddleLeft;
		placeholder.fontSize = 18;
		placeholder.text = Data.DEV_PLAYER_NAME_DEFAULT;
		placeholder.supportRichText = false;
		dev_name_input = inputRoot.AddComponent<InputField>();
		dev_name_input.textComponent = text;
		dev_name_input.placeholder = placeholder;
		dev_name_input.text = Data.GetSavedDevPlayerName();
	}

	private void DestroyDevNameInput()
	{
		if (dev_name_canvas != null)
		{
			UnityEngine.Object.Destroy(dev_name_canvas);
			dev_name_canvas = null;
		}
		dev_name_input = null;
	}

	private void SelectCharacter()
	{
		state = eState.CHARACTER_SELECT;
		SetLayer(LAYER_SELECT);
		tm.SetSpecificEnabled(enabled: true);
        Camera.main.cullingMask = LayerMask.GetMask("Select");
		GameObject original = Resources.Load("Prefab/op_select_prompt") as GameObject;
		character_select = Instantiate(original, m.transform, worldPositionStays: false);
		SetupDevNameInput(character_select);
		GameObject bg = character_select.transform.Find("bg").gameObject;
		bg.transform.localScale = new Vector3(0f, 0f, 1f);
		TouchEvent icon_ok = bg.transform.Find("icon_ok").GetComponent<TouchEvent>();
		icon_ok.transform.Find("contents/sprite").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
		icon_ok.ClickDown.AddListener(delegate
		{
			icon_ok.GetComponent<Animation>().Play();
		});
		UnityAction @object = delegate
		{
			string inputName = (dev_name_input != null) ? dev_name_input.text : Data.DEV_PLAYER_NAME_DEFAULT;
			Data.eMainType selectedType = m.data.main_type;
			Data.ConfirmPlayerName(inputName, m);
			m.data.SetMainType(selectedType);
			Manager.sound.ClickSound();
			tm.ClearSpecific();
			SpriteRenderer front = character_select.transform.Find("front_white").GetComponent<SpriteRenderer>();
			Sequence sequence2 = DOTween.Sequence();
			Manager.sound.PlaySe(Sound.eSe.WHITEOUT);
			sequence2.Append(DOTween.ToAlpha(() => front.color, delegate(Color color)
			{
				front.color = color;
			}, 1f, 3f).SetEase(Ease.Linear));
			sequence2.AppendCallback(delegate
			{
				bg.SetActive(value: false);
				character_select.transform.Find("bg_white").gameObject.SetActive(value: false);
			});
			sequence2.AppendCallback(DoOpening);
			sequence2.AppendCallback(delegate
			{
				Utils.SetLayer(front.gameObject, LAYER_EVENT);
			});
			sequence2.Append(DOTween.ToAlpha(() => front.color, delegate(Color color)
			{
				front.color = color;
			}, 0f, 4f).SetEase(Ease.Linear));
			sequence2.AppendCallback(delegate
			{
				DestroyDevNameInput();
				UnityEngine.Object.Destroy(character_select);
			});
			sequence2.Play();
		};
		icon_ok.ClickUp.AddListener(@object.Invoke);
		Vector3 localPosition = icon_ok.transform.localPosition;
		main = SetSelectMain(Data.eMainType.MAIN_1, bg.transform, icon_ok);
		main_2 = SetSelectMain(Data.eMainType.MAIN_2, bg.transform, icon_ok);
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(1f);
		sequence.Append(bg.transform.DOScale(new Vector2(1f, 1f), 0.3f));
		sequence.Join(main.controller.transform.DOScale(new Vector2(1f, 1f), 0.3f));
		sequence.Join(main_2.controller.transform.DOScale(new Vector2(1f, 1f), 0.3f));
		sequence.Join(main.controller.transform.DOLocalMove(new Vector3(-0.244f, 0.05f, -10f), 0f));
		sequence.Join(main_2.controller.transform.DOLocalMove(new Vector3(0.235f, 0.05f, -10f), 0f));
		sequence.Join(icon_ok.transform.DOLocalMove(localPosition, 0f));
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			tm.AddSpecific(main.touch);
		});
		sequence.AppendCallback(delegate
		{
			tm.AddSpecific(main_2.touch);
		});
		sequence.Play();
	}

	public void Opening()
	{
		SelectCharacter();
	}

	private void DoOpening()
	{
		Debug.Log("Event.DoOpening 1");
		state = eState.OPENING;
		SetLayer(LAYER_EVENT);
		prefix = "main_" + (int)(m.data.main_type + 1);
        Camera.main.cullingMask = LayerMask.GetMask("Event");
		GameObject original = Resources.Load("Prefab/granpa") as GameObject;
		granpa = UnityEngine.Object.Instantiate(original, m.transform, worldPositionStays: false).GetComponent<Animator>();
		granpa_hat = granpa.transform.Find("hat").GetComponent<SpriteRenderer>();
		granpa.gameObject.SetActive(value: false);
		Utils.SetOrderInLayer(granpa.gameObject, -5);
		main = SetMain(m.data.main_type, m.transform);
		main.controller.SetHat(PartsController.eHat.NONE);
		main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.BAG_1);
		Utils.SetLayer(main.controller.gameObject, LAYER_EVENT);
		main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
		bln = CreateBalloon1(granpa.transform, 10000, null);
		CreateHollowedCamera(main.controller.transform.position);
		CreateSkip();
		Sequence sequence = DOTween.Sequence();

        #region old
        //      sequence.Append(main.controller.transform.DOLocalMoveY(0.26f, 3f));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 2");
        //          main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 3");
        //          Manager.sound.PlaySe(Sound.eSe.DOOR);
        //	granpa.gameObject.SetActive(value: true);
        //	Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 4");
        //          SetHat(granpa_hat, active: true);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 5");
        //          Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
        //	Manager.sound.ToSmallBgm(0.3f, 4f);
        //});
        //sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 4f).SetEase(Ease.Linear));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 6");
        //          Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 7");
        //          Manager.sound.PlayBgm(Sound.eBgm.OPENING);
        //	main.controller.Play(PartsController.eAnimType._STAY_1_SIDE, 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 8");
        //          main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.BAG_2);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 9");
        //          Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.Append(main.controller.transform.DOScaleX(-1f, 0f));
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 10");
        //          main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //	Vector3 position2 = main.controller.transform.position;
        //	float x = position2.x + 0.18f;
        //	Vector3 position3 = main.controller.transform.position;
        //	Common.CreateQuestion(new Vector2(x, position3.y + 0.12f), 1.5f);
        //});
        //sequence.Append(main.controller.transform.DOScaleX(1f, 0f));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 11");
        //          main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.BAG_1);
        //});
        //sequence.AppendInterval(2f);
        //SetBalloonSequence(sequence);
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 12");
        //          main.controller.SetItem(PartsController.Parts.eType.ITEM2, PartsController.ePartsItem.SWEAT);
        //	main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.SWEAT);
        //	main.controller.Play(PartsController.eAnimType._SUPRISE_1_UP, 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.PANIC);
        //});
        //sequence.AppendInterval(2f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 13");
        //          main.controller.SetItem(PartsController.Parts.eType.ITEM2, PartsController.ePartsItem.NONE);
        //	main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.NONE);
        //	main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 14");
        //          Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
        //});
        //sequence.Append(granpa.transform.DOLocalMoveY(0.5f, 1f).SetEase(Ease.Linear));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 15");
        //          Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 16");
        //          Utils.Play(granpa, "granpa_1_give_hat_1_down", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 17");
        //          SetHat(granpa_hat, active: false);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 18");
        //          main.controller.SetHat(PartsController.eHat.TYPE_1);
        //});
        //sequence.AppendInterval(2f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 19");
        //          main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
        //});
        //sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0.035f, 0.32f), 1f).SetEase(Ease.Linear));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 20");
        //          Utils.Play(granpa, "granpa_1_hug_1_down", 1f, 0f);
        //});
        //sequence.Join(main.controller.transform.DOLocalMove(new Vector2(0.05f, 0.41f), 0.5f).SetEase(Ease.Linear));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 21");
        //          main.controller.Play(PartsController.eAnimType._HUG_1_UP, 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.HUG);
        //});
        //sequence.AppendInterval(2f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 22");
        //          main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 23");
        //          Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0f, 0.26f), 1.5f));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 24");
        //          Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
        //});
        //sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 1f).SetEase(Ease.Linear));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.NONE);
        //	main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.BAG_1);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Manager.sound.PlaySe(Sound.eSe.GOOD);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Manager.sound.PlaySe(Sound.eSe.GOOD);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Manager.sound.PlaySe(Sound.eSe.GOOD);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Manager.sound.PlaySe(Sound.eSe.GOOD);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Transform transform = sphare.transform;
        //	Vector3 position = main.controller.transform.position;
        //	transform.position = new Vector2(0f, position.y + 0.1f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Manager.sound.PlaySe(Sound.eSe.MASK);
        //});
        //sequence.Append(sphare.transform.DOScale(1f, 1.5f));
        //sequence.AppendInterval(1f);
        //sequence.Append(sphare.transform.DOScale(0f, 0.3f));
        //sequence.AppendInterval(3f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        //});
        //sequence.Append(granpa.transform.DOLocalMove(new Vector2(-0.35f, 1.04f), 0f));
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          UnityEngine.Object.Destroy(skip.gameObject);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          UnityEngine.Object.Destroy(main.controller.gameObject);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          Camera.main.cullingMask = CULLING_MASK;
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.DoOpening 25");
        //          UnityEngine.Object.Destroy(HollowedCamera);
        //});
        //UnityAction @object = delegate
        //{
        //	List<TouchEvent> list = new List<TouchEvent>();
        //	Utils.SetComponentList(m.map.facility_list[0].gameObject, list);
        //	for (int i = 0; i < list.Count; i++)
        //	{
        //		tm.AddSpecific(list[i]);
        //	}
        //	m.main.EventTutorialGrassCut();
        //	state = eState.TUTORIAL_GRASS_CUT;
        //	notice_seq = DOTween.Sequence();
        //	notice = CreateTutorialNotice(notice_seq, tNOTICE_POS[(int)state]);
        //	Manager.sound.SetVolumeBgm(1f);
        //	Manager.sound.PlayBgm(Sound.eBgm.MENU);
        //};
        //sequence.AppendCallback(@object.Invoke);
        //sequence.Play().OnComplete(()=> 
        //{
        //          Debug.Log("Event.DoOpening 100");
        //      });
        //seq = sequence;
        #endregion

        #region new
        sequence.Append(main.controller.transform.DOLocalMoveY(0.26f, 0.1f));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 2");
            main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 3");
            Manager.sound.PlaySe(Sound.eSe.DOOR);
            granpa.gameObject.SetActive(value: true);
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 4");
            SetHat(granpa_hat, active: true);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 5");
            Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
            Manager.sound.ToSmallBgm(0.3f, 4f);
        });
        sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 0.1f).SetEase(Ease.Linear));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 6");
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 7");
            Manager.sound.PlayBgm(Sound.eBgm.OPENING);
            main.controller.Play(PartsController.eAnimType._STAY_1_SIDE, 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 8");
            main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.BAG_2);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 9");
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.Append(main.controller.transform.DOScaleX(-1f, 0.1f));
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 10");
            main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.CLICK);
            Vector3 position2 = main.controller.transform.position;
            float x = position2.x + 0.18f;
            Vector3 position3 = main.controller.transform.position;
            Common.CreateQuestion(new Vector2(x, position3.y + 0.12f), 1.5f);
        });
        sequence.Append(main.controller.transform.DOScaleX(1f, 0.1f));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 11");
            main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.BAG_1);
        });
        sequence.AppendInterval(0.1f);
        SetBalloonSequence(sequence);
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 12");
            main.controller.SetItem(PartsController.Parts.eType.ITEM2, PartsController.ePartsItem.SWEAT);
            main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.SWEAT);
            main.controller.Play(PartsController.eAnimType._SUPRISE_1_UP, 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.PANIC);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 13");
            main.controller.SetItem(PartsController.Parts.eType.ITEM2, PartsController.ePartsItem.NONE);
            main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.NONE);
            main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 14");
            Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
        });
        sequence.Append(granpa.transform.DOLocalMoveY(0.5f, 0.1f).SetEase(Ease.Linear));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 15");
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 16");
            Utils.Play(granpa, "granpa_1_give_hat_1_down", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 17");
            SetHat(granpa_hat, active: false);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 18");
            main.controller.SetHat(PartsController.eHat.NONE);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 19");
            main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
        });
        sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0.035f, 0.32f), 0.1f).SetEase(Ease.Linear));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 20");
            Utils.Play(granpa, "granpa_1_hug_1_down", 1f, 0f);
        });
        sequence.Join(main.controller.transform.DOLocalMove(new Vector2(0.05f, 0.41f), 0.1f).SetEase(Ease.Linear));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 21");
            main.controller.Play(PartsController.eAnimType._HUG_1_UP, 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.HUG);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 22");
            main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 23");
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0f, 0.26f), 1.5f));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 24");
            Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
        });
        sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 1f).SetEase(Ease.Linear));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            main.controller.SetItem(PartsController.Parts.eType.ITEM1, PartsController.ePartsItem.NONE);
            main.controller.SetItem(PartsController.Parts.eType.ITEM3, PartsController.ePartsItem.BAG_1);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Manager.sound.PlaySe(Sound.eSe.GOOD);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Manager.sound.PlaySe(Sound.eSe.GOOD);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Manager.sound.PlaySe(Sound.eSe.GOOD);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Manager.sound.PlaySe(Sound.eSe.GOOD);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Transform transform = sphare.transform;
            Vector3 position = main.controller.transform.position;
            transform.position = new Vector2(0f, position.y + 0.1f);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Manager.sound.PlaySe(Sound.eSe.MASK);
        });
        sequence.Append(sphare.transform.DOScale(1f, 1.5f));
        sequence.AppendInterval(0.1f);
        sequence.Append(sphare.transform.DOScale(0f, 0.3f));
        sequence.AppendInterval(3f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
        });
        sequence.Append(granpa.transform.DOLocalMove(new Vector2(-0.35f, 1.04f), 0f));
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            UnityEngine.Object.Destroy(skip.gameObject);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            UnityEngine.Object.Destroy(main.controller.gameObject);
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            Camera.main.cullingMask = CULLING_MASK;
        });
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.DoOpening 25");
            UnityEngine.Object.Destroy(HollowedCamera);
        });
        UnityAction @object = delegate
        {
            List<TouchEvent> list = new List<TouchEvent>();
            Utils.SetComponentList(m.map.facility_list[0].gameObject, list);
            for (int i = 0; i < list.Count; i++)
            {
                tm.AddSpecific(list[i]);
            }
            m.main.EventTutorialGrassCut();
            state = eState.TUTORIAL_GRASS_CUT;
            notice_seq = DOTween.Sequence();
            notice = CreateTutorialNotice(notice_seq, tNOTICE_POS[(int)state]);
            Manager.sound.SetVolumeBgm(1f);
            Manager.sound.PlayBgm(Sound.eBgm.MENU);
        };
        sequence.AppendCallback(@object.Invoke);
        sequence.Play().OnComplete(() =>
        {
            Debug.Log("Event.DoOpening 100");
        });
        seq = sequence;
        #endregion
    }

    private Animation CreateTutorialNotice(Sequence sequence, Vector2 pos)
	{
		Animation animation = Common.CreateNotice();
		animation.Stop();
		sequence.Append(animation.transform.DOLocalMove(pos, 0f));
		sequence.AppendInterval(0.1f);
		sequence.Append(animation.transform.DOLocalMoveX(pos.x - 0.48f, 1f));
		sequence.AppendInterval(0.1f);
		sequence.Append(animation.transform.DOLocalMove(new Vector2(pos.x, pos.y + 0.21f), 0f));
		sequence.AppendInterval(0.1f);
		sequence.Append(animation.transform.DOLocalMoveX(pos.x - 0.48f, 1f));
		sequence.AppendInterval(0.3f);
		sequence.SetLoops(-1);
		sequence.Play();
		return animation;
	}

	private Main SetMain(Data.eMainType type, Transform parent)
	{
		Main main = new Main();
		GameObject original = Resources.Load("Prefab/event_human") as GameObject;
		main.controller = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false).GetComponent<PartsController>();
		main.controller.Init(MainCharacter.style[(int)type]);
		main.touch = main.controller.GetComponent<TouchEvent>();
		main.controller.SetSortingOrderAll(10);
		return main;
	}

	private void CreateSkip()
	{
		GameObject original = Resources.Load("Prefab/skip") as GameObject;
		skip = UnityEngine.Object.Instantiate(original, m.transform, worldPositionStays: false).GetComponent<SpriteRenderer>();
		TouchEventRect component = skip.GetComponent<TouchEventRect>();
		component.SetCamera(Camera.main);
		component.ClickDown.AddListener(Skip);
		component.ClickMove.AddListener(Skip);
		component.ClickUp.AddListener(ReleaseSkip);
		component.ClickCancel.AddListener(ReleaseSkip);
	}

	private Balloon CreateBalloon1(Transform t_parent, int order_in_layer, UnityAction finish_call)
	{
		bln = new Balloon();
		GameObject original = Resources.Load("Prefab/op_balloon") as GameObject;
		bln.anim = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Animation>();
		bln.anim.GetComponent<SpriteRenderer>().sortingOrder += order_in_layer;
		Utils.SetOrderInLayer(bln.anim.gameObject, order_in_layer);
		bln.contents = bln.anim.transform.Find("contents").GetComponent<Animator>();
		Utils.Play(bln.contents, "silo_animals_stay", 1f, 0f);
		bln.granpa = bln.contents.transform.Find("granpa").GetComponent<Animator>();
		Utils.Play(bln.granpa, "op_granpa_1", 1f, 0f);
		bln.granpa_sr = bln.granpa.GetComponent<SpriteRenderer>();
		for (int i = 0; i < 2; i++)
		{
			bln.enemy[i] = bln.contents.transform.Find("graze_" + (i + 1) + "/enemy").GetComponent<Animator>();
			bln.keep_out[i] = bln.contents.transform.Find("graze_" + (i + 1) + "/keep_out").GetComponent<Animation>();
			Utils.Play(bln.enemy[i], "op_enemy_1", 1f, 0f);
			Utils.Play(bln.keep_out[i], "op_keep_out", 0f, 0f);
		}
		Utils.Play(bln.anim, "op_appear", 0f, 0f);
		return bln;
	}

	private BalloonEd CreateBalloonEd(Transform t_parent, int order_in_layer)
	{
		bln2 = new BalloonEd();
		GameObject original = Resources.Load("Prefab/op_balloon_ending") as GameObject;
		bln2.anim = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Animation>();
		bln2.anim.GetComponent<SpriteRenderer>().sortingOrder += order_in_layer;
		Utils.SetOrderInLayer(bln2.anim.gameObject, order_in_layer);
		bln2.dev = bln2.anim.transform.Find("contents/dev").GetComponent<Animator>();
		bln2.dev.transform.localScale = new Vector3(0f, 0f, 1f);
		Utils.Play(bln2.dev, "op_dev", 1f, 0f);
		for (int i = 0; i < 2; i++)
		{
			bln2.add_animal[i] = bln2.anim.transform.Find("contents/add_animal_" + (i + 1)).GetComponent<SpriteRenderer>();
			bln2.add_animal[i].transform.localScale = new Vector3(0f, 0f, 1f);
		}
		bln2.lv99 = bln2.anim.transform.Find("contents/level").GetComponent<SpriteRenderer>();
		bln2.lv99.transform.localScale = new Vector3(0f, 0f, 1f);
		bln2.achievement = bln2.anim.transform.Find("contents/achievement").GetComponent<SpriteRenderer>();
		bln2.achievement.transform.localScale = new Vector3(0f, 0f, 1f);
		Utils.Play(bln2.anim, "op_appear", 0f, 0f);
		return bln2;
	}

	private BalloonResort CreateBalloonResort(Transform t_parent, int order_in_layer)
	{
		bln_rst = new BalloonResort();
		GameObject original = Resources.Load("Prefab/op_balloon_resort") as GameObject;
		bln_rst.anim = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false).GetComponent<Animation>();
		bln_rst.bg = bln_rst.anim.transform.Find("bg").gameObject.GetComponent<Animator>();
		bln_rst.contents = bln_rst.anim.transform.Find("contents").gameObject.GetComponent<Animator>();
		bln_rst.granpa = bln_rst.contents.transform.Find("granpa").gameObject.GetComponent<Animator>();
		Utils.SetOrderInLayer(bln_rst.anim.gameObject, order_in_layer);
		Utils.Play(bln_rst.anim, "op_appear", 0f, 0f);
		return bln_rst;
	}

	private void CreateHollowedCamera(Vector2 focus_pos)
	{
		GameObject original = Resources.Load("Prefab/HollowedCamera") as GameObject;
		HollowedCamera = UnityEngine.Object.Instantiate(original, null, worldPositionStays: false);
		sphare = HollowedCamera.transform.Find("Object").gameObject;
		sphare.transform.position = focus_pos;
		sphare.transform.localScale = new Vector3(20f, 20f, 1f);
	}

	private void SetBalloonSequence(Sequence sequence)
	{
		Debug.Log("Event.SetBalloonSequence");
        #region old
        //      sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.SetBalloonSequence 1");
        //          Utils.Play(bln.anim, "op_appear", 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.BALLOON);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.granpa, "op_granpa_1", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.UP);
        //});
        //sequence.AppendInterval(3f);
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.granpa, "op_granpa_2", 1f, 0f);
        //	Manager.sound.PlaySe(Sound.eSe.DOWN);
        //});
        //sequence.AppendInterval(3f);
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.contents, "op_animal_fade_in_out", 1f, 0f);
        //});
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.BYE);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.BYE);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.BYE);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	bln.granpa_sr.flipX = true;
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.SLIDE);
        //});
        //sequence.Append(bln.contents.transform.DOLocalMoveX(-0.35f, 0.5f));
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.keep_out[1], "op_keep_out", 1f, 0f);
        //});
        //sequence.AppendInterval(0.1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
        //});
        //sequence.AppendInterval(0.9f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.SLIDE);
        //});
        //sequence.Append(bln.contents.transform.DOLocalMoveX(0f, 0.5f));
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	bln.granpa_sr.flipX = false;
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.SLIDE);
        //});
        //sequence.Append(bln.contents.transform.DOLocalMoveX(0.35f, 0.5f));
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.keep_out[0], "op_keep_out", 1f, 0f);
        //});
        //sequence.AppendInterval(0.1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
        //});
        //sequence.AppendInterval(0.9f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.SLIDE);
        //});
        //sequence.Append(bln.contents.transform.DOLocalMoveX(0f, 0.5f));
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	bln.granpa_sr.flipX = true;
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.AppendInterval(0.5f);
        //sequence.AppendCallback(delegate
        //{
        //	bln.granpa_sr.flipX = false;
        //	Manager.sound.PlaySe(Sound.eSe.CLICK);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Utils.Play(bln.granpa, "op_granpa_3", 1f, 0f);
        //});
        //sequence.AppendInterval(1f);
        //sequence.AppendCallback(delegate
        //{
        //	Manager.sound.PlaySe(Sound.eSe.NEGATIVE);
        //});
        //sequence.AppendInterval(2f);
        //sequence.AppendCallback(delegate
        //{
        //	Debug.Log("Event.SetBalloonSequence 50");
        //          Utils.Play(bln.anim, "op_appear", -1f, 1f);
        //	Manager.sound.PlaySe(Sound.eSe.BALLOON);
        //});
        #endregion

        #region new
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.SetBalloonSequence 1");
            Utils.Play(bln.anim, "op_appear", 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.BALLOON);
        });
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.granpa, "op_granpa_1", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.UP);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.granpa, "op_granpa_2", 1f, 0f);
            Manager.sound.PlaySe(Sound.eSe.DOWN);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.contents, "op_animal_fade_in_out", 1f, 0f);
        });
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.BYE);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.BYE);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.BYE);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            bln.granpa_sr.flipX = true;
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.SLIDE);
        });
        sequence.Append(bln.contents.transform.DOLocalMoveX(-0.35f, 0.5f));
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.keep_out[1], "op_keep_out", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.SLIDE);
        });
        sequence.Append(bln.contents.transform.DOLocalMoveX(0f, 0.5f));
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            bln.granpa_sr.flipX = false;
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.SLIDE);
        });
        sequence.Append(bln.contents.transform.DOLocalMoveX(0.35f, 0.5f));
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.keep_out[0], "op_keep_out", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.SLIDE);
        });
        sequence.Append(bln.contents.transform.DOLocalMoveX(0f, 0.5f));
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            bln.granpa_sr.flipX = true;
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            bln.granpa_sr.flipX = false;
            Manager.sound.PlaySe(Sound.eSe.CLICK);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Utils.Play(bln.granpa, "op_granpa_3", 1f, 0f);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Manager.sound.PlaySe(Sound.eSe.NEGATIVE);
        });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(delegate
        {
            Debug.Log("Event.SetBalloonSequence 50");
            Utils.Play(bln.anim, "op_appear", -1f, 1f);
            Manager.sound.PlaySe(Sound.eSe.BALLOON);
        });
        #endregion
    }

    public void Ending()
	{
		state = eState.ENDING;
		tm.SetSpecificEnabled(enabled: true);
		SetLayer(LAYER_EVENT);
		prefix = "main_" + (int)(m.data.main_type + 1);
        Camera.main.cullingMask = LayerMask.GetMask("Event");
		GameObject original = Resources.Load("Prefab/granpa") as GameObject;
		granpa = UnityEngine.Object.Instantiate(original, m.transform, worldPositionStays: false).GetComponent<Animator>();
		granpa_hat = granpa.transform.Find("hat").GetComponent<SpriteRenderer>();
		SetHat(granpa_hat, active: false);
		granpa.gameObject.SetActive(value: false);
		Utils.SetOrderInLayer(granpa.gameObject, -5);
		main = SetMain(m.data.main_type, m.transform);
		main.controller.transform.localPosition = new Vector2(0f, 1.18f);
		Utils.SetLayer(main.controller.gameObject, LAYER_EVENT);
		main.controller.Play(PartsController.eAnimType._WALK_1_DOWN, 1f, 0f);
		bln = CreateBalloon1(granpa.transform, 10000, null);
		bln2 = CreateBalloonEd(granpa.transform, 10000);
		CreateHollowedCamera(main.controller.transform.position);
		CreateSkip();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			Manager.sound.ToSmallBgm(0f, 2f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.SetVolumeBgm(1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlayBgm(Sound.eBgm.OPENING);
		});
		sequence.Append(main.controller.transform.DOLocalMoveY(0.26f, 6f).SetEase(Ease.Linear));
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_DOWN, 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.DOOR);
			granpa.gameObject.SetActive(value: true);
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
			Manager.sound.ToSmallBgm(0.3f, 4f);
		});
		sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 4f).SetEase(Ease.Linear));
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_side", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
		});
		sequence.Append(granpa.transform.DOScaleX(-1f, 0f));
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			Vector3 position4 = main.controller.transform.position;
			float x2 = position4.x + 0.18f;
			Vector3 position5 = main.controller.transform.position;
			Common.CreateQuestion(new Vector2(x2, position5.y + 0.12f), 1.5f);
		});
		sequence.Append(granpa.transform.DOScaleX(1f, 0f));
		sequence.AppendInterval(2f);
		SetBalloonEnding1Sequence(sequence);
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_crying_1_down", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.UP);
		});
		sequence.AppendInterval(3f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
		});
		sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0.035f, 0.4f), 1f).SetEase(Ease.Linear));
		sequence.AppendInterval(0.1f);
		sequence.Join(main.controller.transform.DOLocalMove(new Vector2(0.05f, 0.46f), 0.5f).SetEase(Ease.Linear));
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._HUG_1_UP, 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.HUG);
		});
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._WALK_1_UP, 1f, 0f);
		});
		sequence.Append(main.controller.transform.DOLocalMove(new Vector2(0f, 0.26f), 1.5f));
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
		});
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			Vector3 position2 = main.controller.transform.position;
			float x = position2.x + 0.18f;
			Vector3 position3 = main.controller.transform.position;
			Common.CreateQuestion(new Vector2(x, position3.y + 0.12f), 1.5f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		SetBalloonSequenceEd(sequence);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._SUPRISE_1_UP, 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.PANIC);
		});
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Transform transform = sphare.transform;
			Vector3 position = main.controller.transform.position;
			transform.position = new Vector2(0f, position.y + 0.1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.Append(sphare.transform.DOScale(1f, 1.5f));
		sequence.AppendInterval(1f);
		sequence.Append(sphare.transform.DOScale(0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.ToSmallBgm(0f, 2f);
		});
		sequence.AppendInterval(3f);
		UnityAction @object = delegate
		{
			state = eState.NONE;
			m.data.SetEnding();
			UnityEngine.Object.Destroy(skip.gameObject);
			UnityEngine.Object.Destroy(HollowedCamera.gameObject);
			UnityEngine.Object.Destroy(granpa.gameObject);
			UnityEngine.Object.Destroy(main.controller.gameObject);
			Camera.main.cullingMask = CULLING_MASK;
			tm.SetSpecificEnabled(enabled: false);
			m.main.FinishEvent();
			Manager.sound.PlayBgm(Sound.eBgm.FIELD);
		};
		sequence.AppendCallback(@object.Invoke);
		sequence.Play();
		seq = sequence;
	}

	private void SetBalloonEnding1Sequence(Sequence sequence)
	{
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.keep_out[0], "op_keep_out", 0f, 1f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.keep_out[1], "op_keep_out", 0f, 1f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.contents, "op_animal_fade_in_out", 0f, 1f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.anim, "op_appear", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.granpa, "op_granpa_" + ((m.data.main_type != 0) ? "girl" : "boy"), 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.UP);
		});
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.SLIDE);
		});
		sequence.Append(bln.contents.transform.DOLocalMoveX(-0.35f, 0.5f));
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.keep_out[1], "op_keep_out", -1f, 1f);
		});
		Vector3 localPosition = bln.enemy[1].transform.localPosition;
		float x = localPosition.x - 0.5f;
		Vector3 localPosition2 = bln.enemy[1].transform.localPosition;
		Vector2 v = new Vector2(x, localPosition2.y + 0.5f);
		sequence.Append(bln.enemy[1].transform.DOLocalMove(v, 0.3f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
		});
		sequence.AppendInterval(0.9f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.SLIDE);
		});
		sequence.Append(bln.contents.transform.DOLocalMoveX(0.35f, 1f));
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.keep_out[0], "op_keep_out", -1f, 1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
		});
		Vector3 localPosition3 = bln.enemy[0].transform.localPosition;
		float x2 = localPosition3.x - 0.5f;
		Vector3 localPosition4 = bln.enemy[0].transform.localPosition;
		v = new Vector2(x2, localPosition4.y + 0.5f);
		sequence.Append(bln.enemy[0].transform.DOLocalMove(v, 0.3f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.AppendInterval(0.9f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.SLIDE);
		});
		sequence.Append(bln.contents.transform.DOLocalMoveX(0f, 0.5f));
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Utils.Reverse(bln.contents, "op_animal_fade_in_out", 1.5f, 1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.BYE);
		});
		sequence.AppendInterval(0.3f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.BYE);
		});
		sequence.AppendInterval(0.3f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.BYE);
		});
		sequence.AppendInterval(3f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln.anim, "op_appear", -1f, 1f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
	}

	private void SetBalloonSequenceEd(Sequence sequence)
	{
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln2.anim, "op_appear", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
		sequence.AppendInterval(1f);
		sequence.Append(bln2.lv99.transform.DOScale(new Vector2(1f, 1f), 0.1f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.AppendInterval(1f);
		sequence.Append(bln2.achievement.transform.DOScale(new Vector2(1f, 1f), 0.1f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.AppendInterval(2f);
		sequence.Append(bln2.lv99.transform.DOScale(Vector2.zero, 0.1f));
		sequence.Join(bln2.achievement.transform.DOScale(Vector2.zero, 0.1f));
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln2.dev, "op_dev", 1f, 0f);
		});
		sequence.Append(bln2.dev.transform.DOScale(new Vector2(1f, 1f), 0.1f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.KEYBOARD);
		});
		sequence.AppendInterval(1f);
		for (int i = 0; i < bln2.add_animal.Length; i++)
		{
			Vector2 v = bln2.add_animal[i].transform.localPosition;
			bln2.add_animal[i].transform.localPosition = bln2.dev.transform.localPosition;
			sequence.Append(bln2.add_animal[i].transform.DOLocalMove(v, 0.3f));
			sequence.Join(bln2.add_animal[i].transform.DOScale(new Vector2(1f, 1f), 0.3f));
			sequence.AppendCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.KEEPOUT);
			});
			sequence.AppendInterval(1f);
			if (i == 0)
			{
				sequence.AppendCallback(delegate
				{
					Manager.sound.PlaySe(Sound.eSe.KEYBOARD);
				});
			}
		}
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln2.anim, "op_appear", -1f, 1f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
	}

	public void OpenResort()
	{
		state = eState.RESORT;
		tm.SetSpecificEnabled(enabled: true);
		SetLayer(LAYER_EVENT);
		prefix = "main_" + (int)(m.data.main_type + 1);
        Camera.main.cullingMask = LayerMask.GetMask("Event");
		GameObject original = Resources.Load("Prefab/granpa") as GameObject;
		granpa = UnityEngine.Object.Instantiate(original, m.transform, worldPositionStays: false).GetComponent<Animator>();
		granpa_hat = granpa.transform.Find("hat").GetComponent<SpriteRenderer>();
		SetHat(granpa_hat, active: false);
		granpa.gameObject.SetActive(value: false);
		Utils.SetOrderInLayer(granpa.gameObject, -5);
		main = SetMain(m.data.main_type, m.transform);
		main.controller.transform.localPosition = new Vector2(0f, 1.18f);
		Utils.SetLayer(main.controller.gameObject, LAYER_EVENT);
		main.controller.Play(PartsController.eAnimType._WALK_1_DOWN, 1f, 0f);
		bln_rst = CreateBalloonResort(granpa.transform, 10000);
		CreateHollowedCamera(main.controller.transform.position);
		CreateSkip();
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			Manager.sound.ToSmallBgm(0f, 2f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.SetVolumeBgm(1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlayBgm(Sound.eBgm.OPENING);
		});
		sequence.Append(main.controller.transform.DOLocalMoveY(0.26f, 6f).SetEase(Ease.Linear));
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_DOWN, 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.DOOR);
			granpa.gameObject.SetActive(value: true);
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_walk_1_down", 1f, 0f);
			Manager.sound.ToSmallBgm(0.3f, 4f);
		});
		sequence.Append(granpa.transform.DOLocalMoveY(0.6f, 4f).SetEase(Ease.Linear));
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_side", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.CLICK);
		});
		sequence.Append(granpa.transform.DOScaleX(-1f, 0f));
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_1_down", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			Vector3 position2 = main.controller.transform.position;
			float x = position2.x + 0.18f;
			Vector3 position3 = main.controller.transform.position;
			Common.CreateQuestion(new Vector2(x, position3.y + 0.12f), 1.5f);
		});
		sequence.Append(granpa.transform.DOScaleX(1f, 0f));
		sequence.AppendInterval(2f);
		SetBalloonOpenResortSequence(sequence);
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._SUPRISE_1_UP, 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.PANIC);
		});
		sequence.AppendInterval(2f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._STAY_1_UP, 1f, 0f);
		});
		sequence.AppendInterval(1f);
		sequence.AppendCallback(delegate
		{
			main.controller.Play(PartsController.eAnimType._POSE_1_DOWN, 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(granpa, "granpa_1_stay_2_down", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.GOOD);
		});
		sequence.AppendInterval(0.5f);
		sequence.AppendCallback(delegate
		{
			Transform transform = sphare.transform;
			Vector3 position = main.controller.transform.position;
			transform.position = new Vector2(0f, position.y + 0.1f);
		});
		sequence.AppendCallback(delegate
		{
			Manager.sound.PlaySe(Sound.eSe.MASK);
		});
		sequence.Append(sphare.transform.DOScale(1f, 1.5f));
		sequence.AppendInterval(1f);
		sequence.Append(sphare.transform.DOScale(0f, 0.3f));
		sequence.AppendCallback(delegate
		{
			Manager.sound.ToSmallBgm(0f, 2f);
		});
		sequence.AppendInterval(2f);
		UnityAction @object = delegate
		{
			state = eState.NONE;
			UnityEngine.Object.Destroy(skip.gameObject);
			UnityEngine.Object.Destroy(granpa.gameObject);
			UnityEngine.Object.Destroy(main.controller.gameObject);
			Camera.main.cullingMask = CULLING_MASK;
			Manager.sound.PlayBgm(Sound.eBgm.FIELD);
			tm.ClearSpecific();
			tm.SetSpecificEnabled(enabled: false);
			m.main.FinishEvent();
			m.data.SetResortEvent();
		};
		sequence.AppendCallback(@object.Invoke);
		sequence.AppendCallback(delegate
		{
			ChangeFarm(Data.eFarmType.RESORT);
		});
		sequence.Play();
		seq = sequence;
	}

	private void SetBalloonOpenResortSequence(Sequence sequence)
	{
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.contents, "silo_animals_stay", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.granpa, "op_granpa_resort_1", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.anim, "op_appear", 1f, 0f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
		sequence.AppendInterval(4f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.bg, "op_resort_bg", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.contents, "op_animal_fade_out_all", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.granpa, "op_granpa_resort_4", 1f, 0f);
		});
		sequence.AppendInterval(7f);
		sequence.AppendCallback(delegate
		{
			Utils.Reverse(bln_rst.granpa, "op_granpa_resort_4", 1.5f, 1f);
		});
		sequence.AppendInterval(3f);
		sequence.AppendCallback(delegate
		{
			Utils.Reverse(bln_rst.contents, "op_animal_fade_in_out", 1f, 1f);
		});
		sequence.AppendInterval(3f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.contents, "silo_animals_stay", 1f, 0f);
		});
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.granpa, "op_granpa_resort_2", 1f, 0f);
		});
		sequence.AppendInterval(5f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.granpa, "op_granpa_resort_3", 1f, 0f);
		});
		sequence.AppendInterval(5f);
		sequence.AppendCallback(delegate
		{
			Utils.Play(bln_rst.anim, "op_appear", -1f, 1f);
			Manager.sound.PlaySe(Sound.eSe.BALLOON);
		});
	}

	private void ChangeFarm(Data.eFarmType farm)
	{
		LoadingManager.eScene nextScene = LoadingManager.eScene.GAME;
		Data.farm_type = farm;
		m.data.SetFarmType(Data.farm_type);
		LoadingManager.SetNextScene(nextScene);
	}

	private void SetHat(SpriteRenderer hat, bool active)
	{
		hat.sprite = null;
		if (active)
		{
			hat.sprite = SpriteManager.GetHat();
		}
	}

	private void SetOrderInLayer(SpriteRenderer sr, int value)
	{
		sr.sortingOrder += value;
	}

	private Animation FeelsClock(Transform t)
	{
		GameObject original = Resources.Load("Prefab/feel_clock") as GameObject;
		return UnityEngine.Object.Instantiate(original, t, worldPositionStays: false).GetComponent<Animation>();
	}

	private Animation FeelsCoin(Transform t)
	{
		GameObject original = Resources.Load("Prefab/feel_coin") as GameObject;
		return UnityEngine.Object.Instantiate(original, t, worldPositionStays: false).GetComponent<Animation>();
	}

	private Animation FeelsPresent(Transform t)
	{
		GameObject original = Resources.Load("Prefab/feel_present") as GameObject;
		return UnityEngine.Object.Instantiate(original, t, worldPositionStays: false).GetComponent<Animation>();
	}

	public void Skip()
	{
		seq.timeScale = 20f;
		skip.color = new Color(1f, 58f / 85f, 0f);
	}

	public void ReleaseSkip()
	{
		seq.timeScale = 1f;
		skip.color = Color.white;
	}
}
