using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Casher : MonoBehaviour
{
	public enum eType
	{
		ADD_ANIMAL = 0,
		ADD_FACILITY = 1,
		ADD_TREE = 2,
		DEL_ANIMAL = 3,
		DEL_FACILITY = 4,
		DEL_TREE = 5,
		RELEASE_LAND = 6,
		MAX = 7,
		NONE = -1
	}

	private Map map;

	private Manager manager;

	private SpriteRenderer black_bg;

	public GameObject black_Prefab;

	public GameObject facility_touch_Prefab;

	public GameObject facility_item_area_Prefab;

	public GameObject add_animal_Prefab;

	public GameObject add_item_Prefab;

	public GameObject level_lock_Prefab;

	private List<GameObject> facility_prefab = new List<GameObject>();

	private GameObject casher_bg;

	public GameObject facility_bg;

	private GameObject facility_item_area;

	private SpriteRenderer picup_animal_bg;

	private SpriteRenderer picup_facility_bg;

	private GameObject facility_item_icon;

	private GameObject level_lock_icon;

	public Animator character;

	public GameObject cacher_animal_Prefab;

	private Vector3 parts_left;

	private GameObject right_max_obj;

	private GameObject left_max_obj;

	private GameObject office;

	private int sprite_order;

	private int shadow_order;

	private int level_order;

	private int area_order;

	private int level_text_order;

	private SpriteRenderer sprite_obj;

	private SpriteRenderer shadow_obj;

	private SpriteRenderer level_obj;

	private SpriteRenderer area_obj;

	private GameObject level_text_obj;

	private GameObject text_obj;

	public Facility.eItem w_item_type;

	public FarmAnimal.eType w_animal_type;

	public GameObject w_obj;

	public TouchEvent w_touch;

	public Facility.eType w_type;

	private int prev_value;

	public int casher_flag;

	public Facility touch_facility;

	private GameObject facility_small_category;

	private GameObject facility_touch_obj;

	private int[] facility_icon_order = new int[4];

	private TouchEvent[] icon_touch = new TouchEvent[4];

	public Prompt prompt;

	private ContentsScroller contents_scroller;

	private GameObject selector;

	private GameObject selector2;

	private Vector2 touch_down_wpos = Vector2.zero;

	public void Init()
	{
		if (black_bg != null)
		{
			UnityEngine.Object.Destroy(black_bg.gameObject);
			black_bg = null;
		}
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		office = GameObject.Find("OfficeManager").gameObject;
		black_bg = base.transform.Find("Pixcel_Black").GetComponent<SpriteRenderer>();
		black_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		black_bg.color = new Color(0f, 0f, 0f, 0.3f);
		Manager.office.menu.gameObject.SetActive(value: false);
		map = GameObject.Find("Manager").GetComponent<Map>();
		casher_flag = -1;
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			map.facility_list[i].Shoping(this);
		}
		for (int j = 0; j < 22; j++)
		{
			Facility.eType eType = (Facility.eType)j;
			GameObject item = Resources.Load("Prefab/facility_" + eType.ToString().ToLower()) as GameObject;
			facility_prefab.Add(item);
		}
	}

	public void DelFacility(Facility instance, Facility.eType facility)
	{
		FacilityPauseShopping();
		casher_flag = 4;
		Sprite facilityIcon = SpriteManager.GetFacilityIcon(facility);
		Vector2 vector = default(Vector2);
		vector = ((instance.my_id != 0 && instance.my_id != 2 && instance.my_id != 4) ? ((instance.my_id != 1 && instance.my_id != 3 && instance.my_id != 5) ? ((instance.my_id != 6 && instance.my_id != 8) ? ((instance.my_id == 7 || instance.my_id == 9) ? new Vector2(-0.15f, 0.5f) : new Vector2(0f, 0.5f)) : new Vector2(0.15f, 0.5f)) : new Vector2(-0.15f, -0.5f)) : new Vector2(0.15f, -0.5f));
		touch_facility = instance;
		w_item_type = Facility.eItem.NONE;
		prompt = Prompt.CreateTrashPrompt(facilityIcon, instance.transform, vector, delegate
		{
			DelFacilityOK(instance, facility);
		}, delegate
		{
			DelFacilityCancel(instance, Facility.eItem.NONE);
		});
	}

	public void DelFacility(Facility instance, Facility.eItem item)
	{
		FacilityPauseShopping();
		casher_flag = 5;
		Sprite itemIcon = SpriteManager.GetItemIcon(item);
		Vector2 vector = default(Vector2);
		vector = ((instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4 || instance.my_id == 6) ? new Vector2(0.2f, -0.55f) : new Vector2(-0.2f, -0.55f));
		touch_facility = instance;
		w_item_type = item;
		prompt = Prompt.CreateTrashPrompt(itemIcon, instance.transform, vector, delegate
		{
			DelFacilityOK(instance, item);
		}, delegate
		{
			DelFacilityCancel(instance, item);
		});
	}

	public void DelAnimal(Facility instance, FarmAnimal.eType type)
	{
		FacilityPauseShopping();
		casher_flag = 3;
		Vector2 vector = default(Vector2);
		vector = ((instance.my_id != 0 && instance.my_id != 2 && instance.my_id != 4) ? ((instance.my_id != 1 && instance.my_id != 3 && instance.my_id != 5) ? ((instance.my_id != 6 && instance.my_id != 8) ? ((instance.my_id == 7 || instance.my_id == 9) ? new Vector2(-0.36f, 0.58f) : new Vector2(0f, 0.58f)) : new Vector2(0.36f, 0.58f)) : new Vector2(-0.36f, -0.58f)) : new Vector2(0.36f, -0.58f));
		int price = Price.OpenAnimalReleasePrice(type);
		touch_facility = instance;
		prompt = Prompt.CreatePresentPrompt(type, price, instance.transform, vector, delegate
		{
			DelAnimalOK(instance, type, price);
		}, delegate
		{
			DelAnimalCancel(instance);
		});
	}

	public void OpenFacility(Facility instance)
	{
		casher_flag = 6;
		FacilityPauseShopping();
		int need_coin = Price.OpenFacilityUnlockPrice(instance.my_id);
		UnityAction call_ok = delegate
		{
			instance.FixOpenFacility();
			DestroyPromptObj();
			OkReset();
		};
		UnityAction call_cancel = delegate
		{
			FacilityRestartShopping();
			DestroyPromptObj();
		};
		prompt = Prompt.CreateCoinPrompt(manager.data.coin, need_coin, manager.transform, Vector2.zero, call_ok, call_cancel);
		Prompt.CreateAddFrame(prompt);
	}

	public void AddFacility(Facility instance, List<Facility.eItem> list)
	{
		FacilityPauseShopping();
		casher_flag = 2;
		prev_value = -1;
		casher_bg = UnityEngine.Object.Instantiate(add_item_Prefab, instance.transform, worldPositionStays: false).gameObject;
		if (instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4 || instance.my_id == 6)
		{
			casher_bg.transform.localPosition = new Vector2(0.2f, -0.4f);
		}
		else
		{
			casher_bg.transform.localPosition = new Vector2(-0.2f, -0.4f);
		}
		right_max_obj = casher_bg.transform.Find("obj_right_max").gameObject;
		left_max_obj = casher_bg.transform.Find("obj_left_max").gameObject;
		contents_scroller = casher_bg.transform.Find("content").gameObject.GetComponent<ContentsScroller>();
		contents_scroller.Init();
		contents_scroller.right_end_pos = right_max_obj.transform.localPosition;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject original = Resources.Load("Prefab/facility_" + list[i].ToString().ToLower()) as GameObject;
			facility_item_icon = UnityEngine.Object.Instantiate(original, contents_scroller.transform, worldPositionStays: false);
			Vector3 localPosition = facility_item_icon.transform.localPosition;
			localPosition.x = localPosition.x - 0.32f + 0.35f * (float)i;
			localPosition.y -= 0.056f;
			facility_item_icon.transform.localPosition = localPosition;
			facility_item_area = UnityEngine.Object.Instantiate(facility_item_area_Prefab, facility_item_icon.transform, worldPositionStays: false).gameObject;
			TouchEvent touch = facility_item_area.GetComponent<TouchEvent>();
			Facility.eItem item = list[i];
			touch.param.value_obj1 = facility_item_icon;
			Facility.ControlSortingOrder(facility_item_icon.transform, Facility.eSortMode.MENU, 20000);
			int num = Price.OpenFacilityItemBuyUnlockLevel(list[i]);
			if (num > manager.data.level)
			{
				ControlSpriteColor(facility_item_icon.transform, 0f);
				facility_item_area.GetComponent<SpriteRenderer>().color = Color.white;
				level_lock_icon = UnityEngine.Object.Instantiate(level_lock_Prefab, facility_item_icon.transform, worldPositionStays: false).gameObject;
				level_lock_icon.transform.localPosition = new Vector2(0.08f, 0.3f);
				TextMesh component = level_lock_icon.transform.Find("text").GetComponent<TextMesh>();
				SpriteRenderer component2 = facility_item_icon.transform.Find("contents/sprite").GetComponent<SpriteRenderer>();
				level_lock_icon.transform.Find("icon").GetComponent<SpriteRenderer>().sortingOrder = component2.sortingOrder + 200;
				component.GetComponent<CustomText>().SetOrderInLayer(component2.sortingOrder + 201);
				component.text = string.Empty + Price.OpenFacilityItemBuyUnlockLevel(list[i]);
			}
			else
			{
				touch.ClickDown.AddListener(ClickSE);
				touch.ClickUp.AddListener(delegate
				{
					ChooseFacilityItem(instance, touch, item);
				});
			}
			if (i == list.Count - 1)
			{
				contents_scroller.parts_right = facility_item_icon.transform.localPosition;
			}
		}
		if (contents_scroller.right_end_pos.x > contents_scroller.parts_right.x)
		{
			contents_scroller.SetTouchEventEnabled(enabled: false);
		}
	}

	public void ChooseFacilityItem(Facility instance, TouchEvent touch, Facility.eItem item)
	{
		instance.SelectFacility(item);
		GameObject tree_obj = touch.param.value_obj1;
		Facility.ControlSortingOrder(tree_obj.transform, Facility.eSortMode.MENU, 5000);
		selector = Common.CreateSelector(new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.12f)), pos: new Vector2(0f, 0.04f), parent: touch.transform, order_in_layer: 25013);
		Vector2 pos2 = new Vector2(0f, -0.37f);
		int price = Price.OpenFacilityItemPrice(item);
		touch_facility = instance;
		w_obj = tree_obj;
		w_item_type = item;
		prompt = Prompt.CreateCoinPrompt(manager.data.coin, price, casher_bg.transform, pos2, delegate
		{
			AddFacilityOK(instance, item, price);
		}, delegate
		{
			AddFacilityCancel(instance, tree_obj, item);
		});
		contents_scroller.SetEnabled(_enabled: false);
	}

	public void AddFacility(Facility instance, List<Facility.eType> list)
	{
		FacilityPauseShopping();
		casher_flag = 1;
		prev_value = -1;
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/cash_add_facility") as GameObject;
		casher_bg = UnityEngine.Object.Instantiate(original, instance.transform, worldPositionStays: false);
		if (instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4)
		{
			casher_bg.transform.localPosition = new Vector3(0.2f, -0.4f, -11f);
		}
		else if (instance.my_id == 1 || instance.my_id == 3 || instance.my_id == 5)
		{
			casher_bg.transform.localPosition = new Vector3(-0.2f, -0.4f, -11f);
		}
		else if (instance.my_id == 6 || instance.my_id == 8)
		{
			casher_bg.transform.localPosition = new Vector3(0.2f, 0.38f, -11f);
		}
		else if (instance.my_id == 7 || instance.my_id == 9)
		{
			casher_bg.transform.localPosition = new Vector3(-0.2f, 0.38f, -11f);
		}
		else
		{
			casher_bg.transform.localPosition = new Vector3(0f, 0.38f, -11f);
		}
		for (int i = 0; i < list.Count; i++)
		{
			icon_touch[i] = casher_bg.transform.Find("content/icon" + (i + 1)).GetComponent<TouchEvent>();
			icon_touch[i].param.value1 = i;
			icon_touch[i].param.value3 = Convert.FacilityTypePlus(Data.farm_type, i);
			TouchEvent touch = icon_touch[i];
			icon_touch[i].ClickDown.AddListener(ClickSE);
			icon_touch[i].ClickUp.AddListener(delegate
			{
				ChooseFacility(instance, touch);
			});
		}
	}

	public void ChooseFacility(Facility instance, TouchEvent touch)
	{
		if (prev_value == touch.param.value1)
		{
			ResetOrderInLayer();
			prev_value = -1;
			return;
		}
		if (facility_bg != null)
		{
			ResetOrderInLayer();
		}
		GameObject original = Resources.Load("Prefab/farm_" + (int)Data.farm_type + "/cash_choose_facility") as GameObject;
		facility_bg = UnityEngine.Object.Instantiate(original, casher_bg.transform, worldPositionStays: false);
		if (instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4)
		{
			facility_bg.transform.localPosition = new Vector2(0.6f, -0.47f);
		}
		else if (instance.my_id == 1 || instance.my_id == 3 || instance.my_id == 5)
		{
			facility_bg.transform.localPosition = new Vector2(-0.6f, -0.47f);
		}
		else if (instance.my_id == 6 || instance.my_id == 8)
		{
			facility_bg.transform.localPosition = new Vector2(0.6f, 0.47f);
		}
		else if (instance.my_id == 7 || instance.my_id == 9)
		{
			facility_bg.transform.localPosition = new Vector2(-0.6f, 0.47f);
		}
		else
		{
			facility_bg.transform.localPosition = new Vector2(0f, 0.47f);
		}
		right_max_obj = facility_bg.transform.Find("obj_right_max").gameObject;
		left_max_obj = facility_bg.transform.Find("obj_left_max").gameObject;
		contents_scroller = facility_bg.transform.Find("content").gameObject.GetComponent<ContentsScroller>();
		contents_scroller.Init();
		contents_scroller.right_end_pos = right_max_obj.transform.localPosition;
		List<Facility.eType> list = Facility.type_list[(int)Facility.TypeToPrefix((Facility.eType)touch.param.value3)];
		picup_facility_bg = facility_bg.transform.Find("black_wall").GetComponent<SpriteRenderer>();
		picup_facility_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		picup_facility_bg.color = new Color(0f, 0f, 0f, 0.3f);
		TouchEvent component = picup_facility_bg.GetComponent<TouchEvent>();
		component.ClickDown.AddListener(Manager.sound.CancelSound);
		component.ClickUp.AddListener(delegate
		{
			CancelChooseFacility();
		});
		sprite_obj = icon_touch[touch.param.value1].transform.Find("sprite").GetComponent<SpriteRenderer>();
		sprite_order = sprite_obj.sortingOrder;
		sprite_obj.sortingOrder = sprite_order + 10000;
		area_obj = icon_touch[touch.param.value1].transform.Find("area").GetComponent<SpriteRenderer>();
		area_order = area_obj.sortingOrder;
		area_obj.sortingOrder = area_order + 10000;
		prev_value = touch.param.value1;
		if (selector2 != null)
		{
			UnityEngine.Object.Destroy(selector2);
		}
		selector2 = Common.CreateSelector(new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.17f, 0.12f)), pos: new Vector2(0.02f, 0.04f), parent: touch.transform, order_in_layer: 20032);
		for (int i = 0; i < list.Count; i++)
		{
			Facility.eType type = list[i];
			facility_touch_obj = UnityEngine.Object.Instantiate(facility_touch_Prefab, contents_scroller.transform, worldPositionStays: false).gameObject;
			TouchEvent facility_touch = facility_touch_obj.GetComponent<TouchEvent>();
			facility_small_category = UnityEngine.Object.Instantiate(facility_prefab[(int)type], facility_touch_obj.transform, worldPositionStays: false).gameObject;
			Transform transform = facility_small_category.transform;
			Vector3 localPosition = facility_small_category.transform.localPosition;
			transform.localPosition = new Vector2(localPosition.x, 0.022f);
			if (type == Facility.eType.NET_3 || type == Facility.eType.UNDERWATERSHOW_2 || type == Facility.eType.UNDERWATERSHOW_3)
			{
				Transform transform2 = facility_small_category.transform;
				Vector3 localPosition2 = facility_small_category.transform.localPosition;
				transform2.localPosition = new Vector2(localPosition2.x, 0f);
			}
			Vector3 localPosition3 = facility_touch_obj.transform.localPosition;
			localPosition3.x += 0.75f * (float)i;
			facility_touch_obj.transform.localPosition = localPosition3;
			Facility.ControlSortingOrder(facility_small_category.transform, Facility.eSortMode.MENU, 15000);
			if (i == list.Count - 1)
			{
				contents_scroller.parts_right = facility_small_category.transform.localPosition;
			}
			if (Price.OpenFacilityBuyUnlockLevel(type) > manager.data.level)
			{
				ControlSpriteColor(facility_touch_obj.transform, 0f);
				level_lock_icon = UnityEngine.Object.Instantiate(level_lock_Prefab, facility_touch_obj.transform, worldPositionStays: false).gameObject;
				TextMesh component2 = level_lock_icon.transform.Find("text").GetComponent<TextMesh>();
				component2.text = string.Empty + Price.OpenFacilityBuyUnlockLevel(type);
			}
			else
			{
				facility_touch.ClickDown.AddListener(ClickSE);
				facility_touch.ClickUp.AddListener(delegate
				{
					TouchSmallCategory(instance, facility_touch, type);
				});
			}
		}
		if (contents_scroller.right_end_pos.x > contents_scroller.parts_right.x)
		{
			contents_scroller.SetTouchEventEnabled(enabled: false);
		}
		Transform transform3 = facility_bg.transform.Find("icons");
		Transform transform4 = transform3.Find("balloon");
		List<Sprite> animalIconList = instance.GetAnimalIconList(Facility.TypeToPrefix((Facility.eType)touch.param.value3), FarmAnimal.ePrefix.NONE);
		if (animalIconList.Count == 0)
		{
			animalIconList.Add(SpriteManager.GetCasherFishing(manager.data.main_type));
		}
		SpriteRenderer component3 = transform4.GetComponent<SpriteRenderer>();
		component3.size = SetBalloonSize(component3, animalIconList.Count);
		transform3.transform.localPosition = SetBallonLocalPos(transform3, animalIconList.Count);
		float start_x = SetIconStartPos(animalIconList.Count);
		for (int j = 0; j < animalIconList.Count; j++)
		{
			SpriteRenderer component4 = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/cash_icon"), transform3).GetComponent<SpriteRenderer>();
			component4.sprite = animalIconList[j];
			component4.transform.localPosition = Vector2.zero;
			component4.transform.localPosition = SetIconPos(start_x, j, animalIconList.Count);
		}
		if (instance.my_id % 2 == 1)
		{
			Transform transform5 = transform3;
			Vector3 localPosition4 = transform3.localPosition;
			float x = 0f - localPosition4.x;
			Vector3 localPosition5 = transform3.localPosition;
			transform5.localPosition = new Vector2(x, localPosition5.y);
			transform4.GetComponent<SpriteRenderer>().flipX = true;
		}
	}

	private Vector2 SetBalloonSize(SpriteRenderer sp, int list_count)
	{
		switch (list_count)
		{
		case 1:
		{
			Vector2 size3 = sp.size;
			return new Vector2(0.3f, size3.y);
		}
		case 8:
		{
			Vector2 size2 = sp.size;
			return new Vector2(0.72f, size2.y * 2f);
		}
		default:
		{
			float x = 0.18f * (float)list_count;
			Vector2 size = sp.size;
			return new Vector2(x, size.y);
		}
		}
	}

	private Vector2 SetBallonLocalPos(Transform icons, int list_count)
	{
		if (list_count == 8)
		{
			return new Vector2(0.8f, 0.565f);
		}
		Vector3 localPosition = icons.transform.localPosition;
		float x = localPosition.x - ((list_count % 2 != 0) ? (0.16f * (float)(list_count / 2)) : 0.08f);
		Vector3 localPosition2 = icons.transform.localPosition;
		return new Vector2(x, localPosition2.y);
	}

	private float SetIconStartPos(int list_count)
	{
		if (list_count == 8)
		{
			return -0.24f;
		}
		return 0f - (float)(list_count / 2) * ((list_count % 2 != 0) ? 0.16f : 0.08f);
	}

	private Vector2 SetIconPos(float start_x, int i, int list_count)
	{
		if (list_count > 5)
		{
			if (i < list_count / 2)
			{
				return new Vector2(start_x + (float)i * 0.16f, 0.09f);
			}
			return new Vector2(start_x + (float)(i - list_count / 2) * 0.16f, -0.09f);
		}
		return new Vector2(start_x + (float)i * 0.16f, 0f);
	}

	public void TouchSmallCategory(Facility instance, TouchEvent touch, Facility.eType type)
	{
		if (Price.OpenFacilityBuyUnlockLevel(type) <= manager.data.level)
		{
			Vector2 vector = default(Vector2);
			vector = ((instance.my_id < 6) ? new Vector2(0f, -0.48f) : new Vector2(0f, 0.48f));
			Facility.ControlSortingOrder(touch.transform, Facility.eSortMode.MENU, 5000);
			int price = Price.OpenFacilityPrice(type);
			touch_facility = instance;
			w_touch = touch;
			w_type = type;
			prompt = Prompt.CreateCoinPrompt(manager.data.coin, price, touch.transform, vector, delegate
			{
				AddFacilityOK(instance, type, price);
			}, delegate
			{
				AddFacilityCancel(instance, touch, type);
			});
			contents_scroller.SetEnabled(_enabled: false);
			instance.SelectFacility(type);
			if (selector != null)
			{
				UnityEngine.Object.Destroy(selector);
			}
			selector = Common.CreateSelector(new Rect(new Vector2(-0.2f, 0.05f), new Vector2(0.6f, 0.15f)), pos: new Vector2(-0.1f, 0.04f), parent: touch.transform, order_in_layer: 20032);
		}
	}

	private void ControlSpriteColor(Transform trans, float color_point)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				SpriteRenderer component = transform.gameObject.GetComponent<SpriteRenderer>();
				if (null != component)
				{
					component.color = new Color(0f, 0f, 0f, 1f);
				}
				if (transform.childCount != 0)
				{
					ControlSpriteColor(transform, color_point);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void CancelChooseFacility()
	{
		ResetOrderInLayer();
		prev_value = -1;
		if (selector2 != null)
		{
			UnityEngine.Object.Destroy(selector2);
			selector2 = null;
		}
	}

	public void AddAnimal(Facility instance, List<FarmAnimal.eType> list)
	{
		FacilityPauseShopping();
		casher_flag = 0;
		prev_value = -1;
		casher_bg = UnityEngine.Object.Instantiate(add_animal_Prefab, instance.transform, worldPositionStays: false).gameObject;
		if (instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4)
		{
			casher_bg.transform.localPosition = new Vector3(0.7f, -0.47f, -11f);
		}
		else if (instance.my_id == 1 || instance.my_id == 3 || instance.my_id == 5)
		{
			casher_bg.transform.localPosition = new Vector3(-0.7f, -0.47f, -11f);
		}
		else if (instance.my_id == 6 || instance.my_id == 8)
		{
			casher_bg.transform.localPosition = new Vector3(0.7f, 0.48f, -11f);
		}
		else if (instance.my_id == 7 || instance.my_id == 9)
		{
			casher_bg.transform.localPosition = new Vector3(-0.7f, 0.48f, -11f);
		}
		else
		{
			casher_bg.transform.localPosition = new Vector3(0f, 0.48f, -11f);
		}
		right_max_obj = casher_bg.transform.Find("obj_right_max").gameObject;
		left_max_obj = casher_bg.transform.Find("obj_left_max").gameObject;
		contents_scroller = casher_bg.transform.Find("content").gameObject.GetComponent<ContentsScroller>();
		contents_scroller.Init();
		contents_scroller.right_end_pos = right_max_obj.transform.localPosition;
		for (int i = 0; i < list.Count; i++)
		{
			SetContents(instance, list[i], i, list.Count);
		}
		if (contents_scroller.right_end_pos.x > contents_scroller.parts_right.x)
		{
			contents_scroller.SetTouchEventEnabled(enabled: false);
		}
	}

	private void SetContents(Facility instance, FarmAnimal.eType type, int list_index, int max)
	{
		GameObject gameObject = casher_bg.transform.Find("content").gameObject;
		TouchEventRect component = gameObject.GetComponent<TouchEventRect>();
		component.SetCamera(Camera.main);
		character = UnityEngine.Object.Instantiate(cacher_animal_Prefab, gameObject.transform, worldPositionStays: false).GetComponent<Animator>();
		TouchEvent touch = character.GetComponent<TouchEvent>();
		touch.param.value1 = (int)type;
		touch.ClickUp.AddListener(delegate
		{
			TouchUpCasherAnimal(instance, touch);
		});
		touch.ClickDown.AddListener(ClickSE);
		touch.ClickDown.AddListener(delegate
		{
			TouchDownCasherAnimal(touch);
		});
		Vector3 localPosition = character.transform.localPosition;
		localPosition.x += 0.35f * (float)list_index;
		character.transform.localPosition = localPosition;
		sprite_obj = character.transform.Find("sprite").GetComponent<SpriteRenderer>();
		shadow_obj = character.transform.Find("shadow").GetComponent<SpriteRenderer>();
		level_obj = character.transform.Find("hint").GetComponent<SpriteRenderer>();
		level_text_obj = character.transform.Find("level_text").gameObject;
		level_text_obj.GetComponent<TextMesh>().text = string.Empty + Price.OpenFarmAnimalLevel(type);
		text_obj = character.transform.Find("text").gameObject;
		text_obj.SetActive(value: false);
		if (list_index == max - 1)
		{
			contents_scroller.parts_right = casher_bg.transform.InverseTransformPoint(character.transform.position);
		}
		if (manager.data.level < Price.OpenFarmAnimalLevel(type))
		{
			sprite_obj.color = new Color(0f, 0f, 0f, 1f);
			shadow_obj.color = new Color(0f, 0f, 0f, 1f);
			level_obj.enabled = true;
			touch.param.value3 = 0;
		}
		else
		{
			level_obj.enabled = false;
			level_text_obj.SetActive(value: false);
			touch.param.value3 = 1;
		}
		SetCasherAnimal(character, type);
	}

	private void SetCasherAnimal(Animator animator, FarmAnimal.eType type)
	{
		RuntimeAnimatorController runtimeAnimatorController = null;
		AnimationClip animationClip = null;
		string str = "Animation/farmanimal/";
		runtimeAnimatorController = (Resources.Load(str + type.ToString().ToLower()) as RuntimeAnimatorController);
		animationClip = (Resources.Load(str + type.ToString().ToLower() + "_album_1_down") as AnimationClip);
		if (runtimeAnimatorController != null && animationClip != null)
		{
			animator.runtimeAnimatorController = runtimeAnimatorController;
			Utils.Play(animator, animationClip.name, 1f, 0f);
		}
	}

	public void TouchDownCasherAnimal(TouchEvent chara)
	{
		touch_down_wpos = chara.transform.parent.TransformPoint(chara.param.pos);
	}

	public void TouchUpCasherAnimal(Facility instance, TouchEvent chara)
	{
		Vector2 b = chara.transform.parent.TransformPoint(chara.param.pos);
		if (Vector2.Distance(touch_down_wpos, b) > 0.05f)
		{
			return;
		}
		Vector3 position = chara.transform.position;
		float x = position.x;
		Vector3 position2 = left_max_obj.transform.position;
		if (x < position2.x)
		{
			return;
		}
		Vector3 position3 = chara.transform.position;
		float x2 = position3.x;
		Vector3 position4 = right_max_obj.transform.position;
		if (x2 > position4.x + 0.09f || chara.param.value3 == 0)
		{
			return;
		}
		if (prev_value == chara.param.value1)
		{
			ResetOrderInLayer();
			prev_value = -1;
			return;
		}
		if (prev_value != -1)
		{
			ResetOrderInLayer();
		}
		ChangeOrderInLayer(chara);
		Vector2 vector = default(Vector2);
		float num = 0f;
		if (instance.my_id == 0 || instance.my_id == 2 || instance.my_id == 4 || instance.my_id == 6 || instance.my_id == 8)
		{
			Vector3 localPosition = chara.transform.localPosition;
			num = ((!(localPosition.x < -0.45f)) ? 0f : 0.18f);
		}
		else if (instance.my_id == 1 || instance.my_id == 3 || instance.my_id == 5 || instance.my_id == 7 || instance.my_id == 9)
		{
			Vector3 localPosition2 = chara.transform.localPosition;
			num = ((!(localPosition2.x > 0.89f)) ? 0f : (-0.18f));
		}
		else
		{
			num = 0f;
		}
		vector = ((instance.my_id < 6) ? new Vector2(num, -0.47f) : new Vector2(num, 0.48f));
		selector = Common.CreateSelector(new Rect(new Vector2(-0.1f, -0.1f), new Vector2(0.2f, 0.12f)), pos: new Vector2(0f, 0.04f), parent: chara.transform, order_in_layer: 20032);
		touch_facility = instance;
		FarmAnimal.eType type = (FarmAnimal.eType)chara.param.value1;
		w_animal_type = type;
		int price = Price.OpenFarmAnimalPrice(type);
		prompt = Prompt.CreateCoinPrompt(manager.data.coin, price, chara.transform, vector, delegate
		{
			AddAnimalOK(instance, type, price);
		}, delegate
		{
			AddAnimalCancel(instance, type);
		});
		contents_scroller.SetEnabled(_enabled: false);
		prev_value = chara.param.value1;
		instance.SelectAnimal(type);
	}

	private void ChangeOrderInLayer(TouchEvent chara)
	{
		GameObject gameObject = GameObject.Find("cash_add_animal(Clone)/content/cash_animal_character(Clone)");
		sprite_obj = chara.transform.Find("sprite").GetComponent<SpriteRenderer>();
		sprite_order = sprite_obj.sortingOrder;
		area_obj = chara.transform.Find("area").GetComponent<SpriteRenderer>();
		area_order = area_obj.sortingOrder;
		shadow_obj = chara.transform.Find("shadow").GetComponent<SpriteRenderer>();
		shadow_order = shadow_obj.sortingOrder;
		level_obj = chara.transform.Find("hint").GetComponent<SpriteRenderer>();
		level_order = level_obj.GetComponent<SpriteRenderer>().sortingOrder;
		level_text_obj = chara.transform.Find("level_text").gameObject;
		level_text_order = level_text_obj.GetComponent<CustomText>().order_in_layer;
		area_obj.sortingOrder = area_order + 10000;
		sprite_obj.sortingOrder = sprite_order + 10000;
		shadow_obj.sortingOrder = shadow_order + 10000;
		level_obj.sortingOrder = level_order + 10000;
		level_text_obj.GetComponent<CustomText>().order_in_layer = level_text_order + 10000;
		picup_animal_bg = UnityEngine.Object.Instantiate(black_Prefab, gameObject.transform, worldPositionStays: false).GetComponent<SpriteRenderer>();
		TouchEvent component = picup_animal_bg.GetComponent<TouchEvent>();
		component.ClickDown.AddListener(ClickSE);
		component.ClickUp.AddListener(delegate
		{
			ResetOrderInLayer();
		});
		picup_animal_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		picup_animal_bg.color = new Color(0f, 0f, 0f, 0.3f);
		picup_animal_bg.sortingOrder = 10100;
	}

	private void ResetOrderInLayer()
	{
		sprite_obj.sortingOrder = sprite_order;
		if (area_obj != null)
		{
			area_obj.sortingOrder = area_order;
		}
		if (level_obj != null)
		{
			level_obj.sortingOrder = level_order;
		}
		if (shadow_obj != null)
		{
			shadow_obj.sortingOrder = shadow_order;
		}
		if (level_text_obj != null)
		{
			level_text_obj.GetComponent<CustomText>().order_in_layer = level_text_order;
		}
		if (picup_animal_bg != null)
		{
			UnityEngine.Object.Destroy(picup_animal_bg.gameObject);
			picup_animal_bg = null;
		}
		if (facility_bg != null)
		{
			UnityEngine.Object.Destroy(facility_bg.gameObject);
			facility_bg = null;
		}
	}

	public void DelFacilityOK(Facility instance, Facility.eType type)
	{
		instance.FixDelFacility(type);
		DestroyPromptObj();
		OkReset();
	}

	public void DelFacilityOK(Facility instance, Facility.eItem item)
	{
		instance.FixDelFacility(item);
		DestroyPromptObj();
		OkReset();
	}

	public void DelFacilityCancel(Facility instance, Facility.eItem item)
	{
		instance.CancelDelFacility(item);
		FacilityRestartShopping();
		DestroyPromptObj();
	}

	public void DelAnimalOK(Facility instance, FarmAnimal.eType type, int coin)
	{
		instance.FixDelAnimal(type);
		DestroyPromptObj();
		OkReset();
	}

	public void DelAnimalCancel(Facility instance)
	{
		instance.CancelDelAnimal();
		FacilityRestartShopping();
		DestroyPromptObj();
	}

	public void AddFacilityOK(Facility instance, Facility.eType type, int coin = 0)
	{
		instance.FixFacility(type);
		manager.data.coin = manager.data.coin - coin;
		manager.data.SetCoinCount(manager.data.coin);
		ResetOrderInLayer();
		DestroyPromptObj();
		OkReset();
	}

	public void AddFacilityCancel(Facility instance, TouchEvent touch, Facility.eType type)
	{
		instance.CancelFacility(type);
		DestroyPromptObj();
		Facility.ControlSortingOrder(touch.transform, Facility.eSortMode.MENU, -5000);
	}

	public void AddFacilityOK(Facility instance, Facility.eItem item, int coin = 0)
	{
		instance.FixFacility(item);
		manager.data.coin = manager.data.coin - coin;
		manager.data.SetCoinCount(manager.data.coin);
		DestroyPromptObj();
		OkReset();
	}

	public void AddFacilityCancel(Facility instance, GameObject obj, Facility.eItem item)
	{
		instance.CancelFacility(item);
		DestroyPromptObj();
		Facility.ControlSortingOrder(obj.transform, Facility.eSortMode.MENU, -5000);
	}

	public void AddAnimalOK(Facility instance, FarmAnimal.eType type, int coin)
	{
		instance.FixAnimal(type);
		ResetOrderInLayer();
		DestroyPromptObj();
		OkReset();
	}

	public void AddAnimalCancel(Facility instance, FarmAnimal.eType type)
	{
		instance.CancelAnimal(type);
		ResetOrderInLayer();
		DestroyPromptObj();
	}

	public void FacilityRestartShopping()
	{
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			map.facility_list[i].RestartShoping();
		}
	}

	private void FacilityPauseShopping()
	{
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			map.facility_list[i].PauseShoping();
		}
	}

	private void FacilityFinishShopping()
	{
		for (int i = 0; i < map.facility_list.Count; i++)
		{
			map.facility_list[i].FinishShoping();
		}
	}

	public void DestroyPromptObj()
	{
		UnityEngine.Object.Destroy(prompt.gameObject);
		prompt = null;
		if (selector != null)
		{
			UnityEngine.Object.Destroy(selector);
			selector = null;
		}
		if (contents_scroller != null)
		{
			contents_scroller.SetEnabled(_enabled: true);
		}
		prev_value = -1;
	}

	public void BlackPrefabDestroy()
	{
		if (casher_flag >= 0 && casher_flag <= 2)
		{
			UnityEngine.Object.Destroy(casher_bg.gameObject);
			casher_bg = null;
			FacilityRestartShopping();
			casher_flag = -1;
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		black_bg = null;
		FacilityFinishShopping();
		if (casher_bg != null)
		{
			UnityEngine.Object.Destroy(casher_bg.gameObject);
			casher_bg = null;
		}
		Manager.office.menu.gameObject.SetActive(value: true);
		Manager.office.menu.CloseMenu(buy_flag: false);
	}

	private void OkReset()
	{
		UnityEngine.Object.Destroy(black_bg.gameObject);
		black_bg = null;
		FacilityFinishShopping();
		Manager.office.menu.menu_flag = 1;
		if (casher_bg != null)
		{
			UnityEngine.Object.Destroy(casher_bg.gameObject);
			casher_bg = null;
		}
		UnityEngine.Object.Destroy(Manager.office.menu.casher.gameObject);
		Manager.office.menu.casher = null;
		Manager.office.menu.CloseMenu(buy_flag: true);
	}

	public void SetEnabledContentsScroller(bool enabled)
	{
		if (contents_scroller != null)
		{
			contents_scroller.SetEnabled(enabled);
		}
	}

	public void ClickSE()
	{
		Manager.sound.ClickSound();
	}

	public void CancelSE()
	{
		Manager.sound.CancelSound();
	}
}
