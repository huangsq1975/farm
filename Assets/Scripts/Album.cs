using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Album : MonoBehaviour
{
	private Manager manager;

	public GameObject newPrefab;

	public GameObject characterPrefab;

	public GameObject levelPrefab;

	public GameObject maxPrefab;

	public GameObject condPrefab;

	public GameObject blackPrefab;

	public GameObject moviePrefab;

	public Animator character;

	public PartsController customer;

	public GameObject level_up_bg;

	public GameObject level_max_bg;

	public GameObject cond_bg;

	public SpriteRenderer black_bg;

	public GameObject movie_bg;

	private GameObject new_image;

	public SpriteRenderer[] need_item = new SpriteRenderer[6];

	public TextMesh level_text;

	private int area_order;

	private int sprite_order;

	private int shadow_order;

	private int hint_order;

	private int lv_order;

	private int text_order;

	private int bg_order;

	private int level_up_text1_order;

	private int level_up_text2_order;

	private int level_up_text3_order;

	private int level_up_icon_order;

	private int need_level_flag;

	private bool touch_down_flag;

	private int need_level_text_order;

	private int[] need_icon_order = new int[6];

	private int prev_type = 1000;

	private int prev_id = 1000;

	private int album_mode;

	private bool active_coroutine;

	private ContentsScroller[] contents_scroller = new ContentsScroller[4];

	private SpriteRenderer sprite_obj;

	private SpriteRenderer area_obj;

	private SpriteRenderer shadow_obj;

	private SpriteRenderer hint_obj;

	private SpriteRenderer lv_obj;

	private CustomText text_obj;

	private GameObject pop_up_obj;

	private TextMesh now_level;

	private TextMesh next_level;

	private TextMesh need_point;

	private SpriteRenderer need_icon;

	private GameObject album_right_end;

	private GameObject album_left_end;

	private float parts_scroll_end;

	public GameObject[] alubum_right_character = new GameObject[4];

	private float MOVE_RIGHT_MAX = 0.3f;

	public Transform album_bg;

	public GameObject[] contents_bg = new GameObject[4];

	private Vector2[] CONTENTS_POS = new Vector2[4]
	{
		new Vector2(0.082f, 0.154f),
		new Vector2(0.082f, -0.462f),
		new Vector2(0.082f, -1.078f),
		new Vector2(0.082f, -1.694f)
	};

	private Vector2[] BG_SIZE = new Vector2[4]
	{
		new Vector2(1.89f, 0.8f),
		new Vector2(1.89f, 1.4f),
		new Vector2(1.89f, 2f),
		new Vector2(1.89f, 2.6f)
	};

	private int contents_num;

	public List<TouchEvent>[] touch_list = new List<TouchEvent>[4]
	{
		new List<TouchEvent>(),
		new List<TouchEvent>(),
		new List<TouchEvent>(),
		new List<TouchEvent>()
	};

	public GameObject selector;

	private const int CUSTOMER_BASE_ORDER = 10011;

	private Vector2 touch_down_wpos = Vector2.zero;

	public void Init()
	{
		album_mode = Manager.office.menu.album_type;
		GameObject gameObject = GameObject.Find("Manager");
		manager = gameObject.GetComponent<Manager>();
		manager.LoadVideo();
		album_left_end = base.transform.Find("objects/left_obj").gameObject;
		album_right_end = base.transform.Find("objects/right_obj").gameObject;
		album_bg = base.transform.Find("objects/album_bg");
		TouchEvent[] array = new TouchEvent[2];
		SpriteRenderer[] array2 = new SpriteRenderer[2];
		for (int i = 0; i < 2; i++)
		{
			array[i] = base.transform.Find("objects/tab_" + i).GetComponent<TouchEvent>();
			array2[i] = base.transform.Find("objects/tab_" + i + "/bg").GetComponent<SpriteRenderer>();
			SetTabSprite(array2[i], i);
		}
		for (int j = 0; j < contents_scroller.Length; j++)
		{
			contents_bg[j] = base.transform.Find("objects/contents_bg" + (j + 1)).gameObject;
			contents_scroller[j] = base.transform.Find("objects/contents_bg" + (j + 1) + "/contents" + (j + 1)).gameObject.GetComponent<ContentsScroller>();
			contents_scroller[j].Init();
			contents_scroller[j].SetCleanup(ResetBG);
			contents_scroller[j].right_end_pos = contents_bg[j].transform.InverseTransformPoint(album_right_end.transform.position);
			contents_scroller[j].SetTouchEventEnabled(enabled: false);
		}
		Manager.sound.PlaySe(Sound.eSe.ALBUM);
		Manager.office.menu.gameObject.SetActive(value: false);
		contents_num = 0;
		for (int k = 0; k < 4; k++)
		{
			manager.data.character_data[k].album = manager.data.GetAlbumDisp(k, album_mode);
		}
		for (int l = 0; l < 4; l++)
		{
			if (manager.data.character_data[l].album == 1)
			{
				contents_bg[l].transform.localPosition = CONTENTS_POS[contents_num];
				contents_num++;
			}
			else
			{
				contents_bg[l].SetActive(value: false);
			}
		}
		album_bg.GetComponent<SpriteRenderer>().size = BG_SIZE[contents_num - 1];
		StartCoroutine(SetPrefab(4));
		SetAchievement();
		SpriteRenderer component = base.transform.Find("Pixel_Black").GetComponent<SpriteRenderer>();
		component.transform.localScale = new Vector3(50f, 50f, 1f);
		component.color = new Color(0f, 0f, 0f, 0.3f);
	}

	private IEnumerator SetPrefab(int type)
	{
		active_coroutine = true;
		int[] content_max = new int[4]
		{
			Convert.FarmAnimalLength(album_mode),
			Convert.WildAnimalLength(album_mode),
			Convert.FishLength(album_mode),
			Convert.CustomerLength(album_mode)
		};
		int count_max = (type != 4) ? content_max[type] : Mathf.Max(content_max);
		for (int i = 0; i < count_max; i++)
		{
			if (i < 6)
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				yield return new WaitForSeconds(0f);
			}
			if (type == 4)
			{
				for (int j = 0; j < 4; j++)
				{
					if (manager.data.character_data[j].album == 1 && i < content_max[j])
					{
						SetContents(j, i);
					}
				}
			}
			else
			{
				SetContents(type, i);
			}
		}
		if (type == 4)
		{
			for (int k = 0; k < 4; k++)
			{
				if (manager.data.character_data[k].album == 1)
				{
					contents_scroller[k].SetTouchEventEnabled(enabled: true);
				}
			}
		}
		else
		{
			contents_scroller[type].SetTouchEventEnabled(enabled: true);
		}
		SetNewContens();
		active_coroutine = false;
	}

	private void SetNewContens()
	{
		int num = 0;
		int num2 = 0;
		while (true)
		{
			if (num2 < 4)
			{
				if (manager.data.character_data[num2].album == 1)
				{
					num++;
				}
				else if (manager.data.character_data[num2].album == -1)
				{
					break;
				}
				num2++;
				continue;
			}
			return;
		}
		manager.data.SetAlbumDisp(1, num2, Data.eFarmType.NORMAL);
		num++;
		Sequence sequence = DOTween.Sequence();
		int pos_index = num - 1;
		for (int i = num2 + 1; i < 4; i++)
		{
			if (manager.data.character_data[i].album == 1)
			{
				num++;
				int num3 = num - 1;
				sequence.Join(contents_bg[i].transform.DOLocalMove(CONTENTS_POS[num3], 0.3f));
			}
		}
		int num4 = num - 1;
		SpriteRenderer sr = album_bg.GetComponent<SpriteRenderer>();
		Vector2 endValue = BG_SIZE[num4];
		sequence.Join(DOTween.To(() => sr.size, delegate(Vector2 do_size)
		{
			sr.size = do_size;
		}, endValue, 0.3f));
		int type = num2;
		sequence.AppendCallback(delegate
		{
			contents_bg[type].transform.localPosition = CONTENTS_POS[pos_index];
			contents_bg[type].SetActive(value: true);
			StartCoroutine(SetPrefab(type));
			Manager.sound.PlaySe(Sound.eSe.SMOKE);
		});
		sequence.Play();
		Manager.sound.PlaySe(Sound.eSe.GET);
	}

	private void SetContents(int content_type, int id)
	{
		character = UnityEngine.Object.Instantiate(characterPrefab, contents_scroller[content_type].transform, worldPositionStays: false).GetComponent<Animator>();
		character.name = "chracter_" + id;
		TouchEvent touch = character.GetComponent<TouchEvent>();
		touch.param.value1 = content_type;
		switch (content_type)
		{
		case 0:
			touch.param.value2 = (int)FarmAnimal.tFARMANIMAL_ORDER[album_mode, id];
			break;
		case 1:
			touch.param.value2 = (int)WildAnimal.tWILDANIMAL_ORDER[album_mode, id];
			break;
		case 2:
			touch.param.value2 = (int)Fish.tFISH_ORDER[album_mode, id];
			break;
		case 3:
			touch.param.value2 = (int)Customer.tCUSTOMER_ORDER[album_mode, id];
			break;
		}
		touch.ClickUp.AddListener(delegate
		{
			TouchAlbumCharacter(touch);
		});
		touch.ClickDown.AddListener(delegate
		{
			TouchDownAlbumCharacter(touch);
		});
		touch_list[content_type].Add(touch);
		Vector3 localPosition = character.transform.localPosition;
		localPosition.x += 0.35f * (float)id;
		character.transform.localPosition = localPosition;
		SpriteRenderer component = character.transform.Find("area").GetComponent<SpriteRenderer>();
		component.sprite = SpriteManager.GetAlbumArea((Data.CharacterData.eType)content_type);
		SpriteRenderer component2 = character.transform.Find("sprite").GetComponent<SpriteRenderer>();
		sprite_order = component2.sortingOrder;
		SpriteRenderer component3 = character.transform.Find("shadow").GetComponent<SpriteRenderer>();
		shadow_order = component3.sortingOrder;
		SpriteRenderer component4 = character.transform.Find("hint").GetComponent<SpriteRenderer>();
		hint_order = component4.GetComponent<SpriteRenderer>().sortingOrder;
		TextMesh component5 = character.transform.Find("text").GetComponent<TextMesh>();
		text_order = component5.GetComponent<CustomText>().order_in_layer;
		customer = character.transform.Find("human").GetComponent<PartsController>();
		if (content_type == 3)
		{
			customer.Init(Customer.style[(int)Customer.tCUSTOMER_ORDER[album_mode, id]]);
			customer.SetSortingOrderAll(10011);
			customer.Play(PartsController.eAnimType._STAY_1_DOWN, 0f, 0f);
			component2.gameObject.SetActive(value: false);
			component3.gameObject.SetActive(value: false);
		}
		else
		{
			customer.gameObject.SetActive(value: false);
		}
		if (content_type == 0 && id == Convert.FarmAnimalLength(album_mode) - 1)
		{
			contents_scroller[0].parts_right = contents_bg[0].transform.InverseTransformPoint(character.transform.position);
			alubum_right_character[0] = character.gameObject;
		}
		else if (content_type == 1 && id == Convert.WildAnimalLength(album_mode) - 1)
		{
			contents_scroller[1].parts_right = contents_bg[1].transform.InverseTransformPoint(character.transform.position);
			alubum_right_character[1] = character.gameObject;
		}
		else if (content_type == 2 && id == Convert.FishLength(album_mode) - 1)
		{
			contents_scroller[2].parts_right = contents_bg[2].transform.InverseTransformPoint(character.transform.position);
			alubum_right_character[2] = character.gameObject;
		}
		else if (content_type == 3 && id == Convert.CustomerLength(album_mode) - 1)
		{
			contents_scroller[3].parts_right = contents_bg[3].transform.InverseTransformPoint(character.transform.position);
			alubum_right_character[3] = character.gameObject;
		}
		if (manager.data.character_data[content_type].contents[touch.param.value2].reg == 0)
		{
			if (content_type == 3)
			{
				customer.SetColor(new Color(0f, 0f, 0f, 1f));
			}
			else
			{
				component2.color = new Color(0f, 0f, 0f, 1f);
				component3.color = new Color(0f, 0f, 0f, 1f);
			}
			component4.enabled = true;
		}
		else
		{
			component4.enabled = false;
		}
		if (manager.data.character_data[content_type].contents[touch.param.value2].new_reg == 1)
		{
			new_image = UnityEngine.Object.Instantiate(newPrefab, character.transform, worldPositionStays: false).gameObject;
		}
		component5.text = string.Empty + manager.data.character_data[content_type].contents[touch.param.value2].level;
		SetAlbumCharacter(character, (Data.CharacterData.eType)content_type, id);
	}

	private void SetTouchEnable(bool enable)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < touch_list[i].Count; j++)
			{
				touch_list[i][j].SetEnabled(enable);
			}
		}
	}

	private void SetAlbumCharacter(Animator animator, Data.CharacterData.eType character_data_type, int id)
	{
		RuntimeAnimatorController runtimeAnimatorController = null;
		AnimationClip animationClip = null;
		string str = "Animation/" + character_data_type.ToString().ToLower() + "/";
		switch (character_data_type)
		{
		case Data.CharacterData.eType.FARMANIMAL:
		{
			FarmAnimal.eType eType4 = FarmAnimal.tFARMANIMAL_ORDER[album_mode, id];
			runtimeAnimatorController = (Resources.Load(str + eType4.ToString().ToLower()) as RuntimeAnimatorController);
			animationClip = (Resources.Load(str + eType4.ToString().ToLower() + "_album_1_down") as AnimationClip);
			break;
		}
		case Data.CharacterData.eType.WILDANIMAL:
		{
			WildAnimal.eType eType3 = WildAnimal.tWILDANIMAL_ORDER[album_mode, id];
			runtimeAnimatorController = (Resources.Load(str + eType3.ToString().ToLower()) as RuntimeAnimatorController);
			animationClip = (Resources.Load(str + eType3.ToString().ToLower() + "_stay_1_down") as AnimationClip);
			break;
		}
		case Data.CharacterData.eType.FISH:
		{
			Fish.eType eType2 = Fish.tFISH_ORDER[album_mode, id];
			runtimeAnimatorController = (Resources.Load(str + eType2.ToString().ToLower()) as RuntimeAnimatorController);
			animationClip = (Resources.Load(str + eType2.ToString().ToLower() + "_stay_1_down") as AnimationClip);
			break;
		}
		case Data.CharacterData.eType.CUSTOMER:
		{
			Customer.eType eType = Customer.tCUSTOMER_ORDER[album_mode, id];
			runtimeAnimatorController = (Resources.Load("Animation/human/human".ToLower()) as RuntimeAnimatorController);
			animationClip = (Resources.Load("Animation/human/human_stay_1_down".ToLower()) as AnimationClip);
			break;
		}
		}
		if (runtimeAnimatorController != null && animationClip != null)
		{
			animator.runtimeAnimatorController = runtimeAnimatorController;
			switch (character_data_type)
			{
			case Data.CharacterData.eType.FISH:
			{
				Vector3 localPosition = animator.transform.localPosition;
				localPosition.y = 0.045f;
				animator.transform.localPosition = localPosition;
				Utils.Play(animator, animationClip.name, 0f, 0f);
				Animation component3 = animator.GetComponent<Animation>();
				component3["album_set_fish"].normalizedTime = 0f;
				component3["album_set_fish"].speed = 1f;
				component3.Play("album_set_fish");
				break;
			}
			case Data.CharacterData.eType.CUSTOMER:
			{
				Animation component2 = animator.GetComponent<Animation>();
				component2["album_customer"].normalizedTime = 0f;
				component2["album_customer"].speed = 1f;
				component2.Play("album_customer");
				break;
			}
			default:
			{
				Utils.Play(animator, animationClip.name, 0f, 0f);
				Animation component = animator.GetComponent<Animation>();
				component["album_set"].normalizedTime = 0f;
				component["album_set"].speed = 1f;
				component.Play("album_set");
				break;
			}
			case Data.CharacterData.eType.FARMANIMAL:
				Utils.Play(animator, animationClip.name, 1f, 0f);
				break;
			}
		}
	}

	private void SetAchievement()
	{
		GameObject[] array = new GameObject[4];
		GameObject[] array2 = new GameObject[4];
		int[] array3 = new int[4];
		int[] array4 = new int[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = base.transform.Find("objects/silver" + (i + 1)).gameObject;
			array[i] = base.transform.Find("objects/gold" + (i + 1)).gameObject;
			array2[i].gameObject.SetActive(value: false);
			array[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < Convert.FarmAnimalLength(album_mode); j++)
		{
			int index = j + Convert.FarmAnimalInitValue(album_mode);
			if (manager.data.character_data[0].contents[index].reg == 1)
			{
				array3[0]++;
				if (array3[0] >= Convert.FarmAnimalLength(album_mode))
				{
					array2[0].gameObject.SetActive(value: true);
				}
			}
			if (manager.data.character_data[0].contents[index].level == 99)
			{
				array4[0]++;
				if (array4[0] >= Convert.FarmAnimalLength(album_mode))
				{
					array[0].gameObject.SetActive(value: true);
				}
			}
		}
		for (int k = 0; k < Convert.WildAnimalLength(album_mode); k++)
		{
			int index2 = k + Convert.WildAnimalInitValue(album_mode);
			if (manager.data.character_data[1].contents[index2].reg == 1)
			{
				array3[1]++;
				if (array3[1] >= Convert.WildAnimalLength(album_mode))
				{
					array2[1].gameObject.SetActive(value: true);
				}
			}
			if (manager.data.character_data[1].contents[index2].level == 99)
			{
				array4[1]++;
				if (array4[1] >= Convert.WildAnimalLength(album_mode))
				{
					array[1].gameObject.SetActive(value: true);
				}
			}
		}
		for (int l = 0; l < Convert.FishLength(album_mode); l++)
		{
			int index3 = l + Convert.FishInitValue(album_mode);
			if (manager.data.character_data[2].contents[index3].reg == 1)
			{
				array3[2]++;
				if (array3[2] >= Convert.FishLength(album_mode))
				{
					array2[2].gameObject.SetActive(value: true);
				}
			}
			if (manager.data.character_data[2].contents[index3].level == 99)
			{
				array4[2]++;
				if (array4[2] >= Convert.FishLength(album_mode))
				{
					array[2].gameObject.SetActive(value: true);
				}
			}
		}
		for (int m = 0; m < Convert.CustomerLength(album_mode) - ExclusionChara(album_mode); m++)
		{
			int index4 = m + Convert.CustomerInitValue(album_mode);
			if (manager.data.character_data[3].contents[index4].reg == 1)
			{
				array3[3]++;
				if (array3[3] >= Convert.CustomerLength(album_mode) - ExclusionChara(album_mode))
				{
					array2[3].gameObject.SetActive(value: true);
				}
			}
			if (manager.data.character_data[3].contents[index4].level == 99)
			{
				array4[3]++;
				if (array4[3] >= Convert.CustomerLength(album_mode) - ExclusionChara(album_mode))
				{
					array[3].gameObject.SetActive(value: true);
				}
			}
		}
	}

	private int ExclusionChara(int album_type)
	{
		switch (album_type)
		{
		case 0:
			return 2;
		case 1:
			return 1;
		default:
			return 0;
		}
	}

	public void TouchDownAlbumCharacter(TouchEvent chara)
	{
		touch_down_wpos = chara.transform.parent.TransformPoint(chara.param.pos);
	}

	public void TouchAlbumCharacter(TouchEvent chara)
	{
		Vector2 b = chara.transform.parent.TransformPoint(chara.param.pos);
		if (Vector2.Distance(touch_down_wpos, b) > 0.05f || active_coroutine)
		{
			return;
		}
		Vector3 position = chara.transform.position;
		float x = position.x;
		Vector3 position2 = album_left_end.transform.position;
		if (!(x < position2.x))
		{
			Vector3 position3 = chara.transform.position;
			float x2 = position3.x;
			Vector3 position4 = album_right_end.transform.position;
			if (!(x2 > position4.x + 0.09f))
			{
				int value = chara.param.value1;
				int value2 = chara.param.value2;
				if (value == prev_type && value2 == prev_id)
				{
					Manager.sound.CancelSound();
					BlackPrefabDestroy(chara);
					prev_type = -1;
					prev_id = -1;
					return;
				}
				Manager.sound.ClickSound();
				ResetBG();
				black_bg = UnityEngine.Object.Instantiate(blackPrefab, chara.transform, worldPositionStays: false).GetComponent<SpriteRenderer>();
				black_bg.transform.localScale = new Vector3(50f, 50f, 1f);
				black_bg.color = new Color(0f, 0f, 0f, 0.3f);
				TouchEvent touch = black_bg.GetComponent<TouchEvent>();
				touch.ClickUp.AddListener(delegate
				{
					BlackPrefabDestroy(touch);
				});
				touch.ClickDown.AddListener(delegate
				{
					Manager.sound.CancelSound();
				});
				Rect rect = new Rect(new Vector2(-0.1f, -0.1f), new Vector2((value == 2) ? 0.22f : 0.2f, (value == 2) ? 0.15f : 0.12f));
				selector = Common.CreateSelector(pos: new Vector2(0f, 0.04f), rect: rect, parent: chara.transform, order_in_layer: 20012);
				if (manager.data.character_data[value].contents[value2].new_reg == 1)
				{
					manager.data.SetNewRegFlag(value, value2, 0);
					GameObject gameObject = chara.transform.Find("new(Clone)").gameObject;
					UnityEngine.Object.Destroy(gameObject.gameObject);
				}
				if (manager.data.character_data[value].contents[value2].reg == 1)
				{
					if (manager.data.character_data[value].contents[value2].level < 99)
					{
						level_up_bg = UnityEngine.Object.Instantiate(levelPrefab, chara.transform, worldPositionStays: false).gameObject;
						pop_up_obj = level_up_bg;
						bg_order = level_up_bg.GetComponent<SpriteRenderer>().sortingOrder;
						now_level = level_up_bg.transform.Find("text1").GetComponent<TextMesh>();
						level_up_text1_order = now_level.GetComponent<CustomText>().order_in_layer;
						next_level = level_up_bg.transform.Find("text2").GetComponent<TextMesh>();
						level_up_text2_order = next_level.GetComponent<CustomText>().order_in_layer;
						need_point = level_up_bg.transform.Find("text3").GetComponent<TextMesh>();
						level_up_text3_order = need_point.GetComponent<CustomText>().order_in_layer;
						need_icon = level_up_bg.transform.Find("icon").GetComponent<SpriteRenderer>();
						SpriteRenderer component = level_up_bg.transform.Find("coin").GetComponent<SpriteRenderer>();
						component.sprite = SpriteManager.GetPromptCoinType((Data.eFarmType)album_mode);
						TextMesh component2 = level_up_bg.transform.Find("text4").GetComponent<TextMesh>();
						level_up_text3_order = need_point.GetComponent<CustomText>().order_in_layer;
						level_up_icon_order = need_icon.sortingOrder;
						now_level.text = string.Empty + manager.data.character_data[value].contents[value2].level;
						next_level.text = string.Empty + (manager.data.character_data[value].contents[value2].level + 1);
						need_point.text = string.Empty + manager.data.character_data[value].contents[value2].level_condition_count + "/" + Data.tCHARACTER_LEVEL[value, manager.data.character_data[value].contents[value2].level - 1];
						switch (value)
						{
						case 0:
							component2.text = string.Empty + Price.HarvestPrice((FarmAnimal.eType)value2);
							break;
						case 2:
							component2.text = string.Empty + Price.HarvestPrice((Fish.eType)value2);
							break;
						case 3:
							component2.text = string.Empty + Price.CustomerPrice((Customer.eType)value2);
							break;
						case 1:
							component.enabled = false;
							component2.gameObject.SetActive(value: false);
							need_icon.transform.localPosition = new Vector3(-0.15f, -0.064f, 0f);
							need_point.transform.localPosition = new Vector3(-0.04f, -0.062f, 0f);
							break;
						}
						if (value == 0)
						{
							need_icon.sprite = SpriteManager.GetHarvest((FarmAnimal.eType)value2);
						}
						else
						{
							need_icon.sprite = SpriteManager.GetLevelUpHarvest((Data.CharacterData.eType)value);
						}
					}
					else
					{
						level_max_bg = UnityEngine.Object.Instantiate(maxPrefab, chara.transform, worldPositionStays: false).gameObject;
						bg_order = level_max_bg.GetComponent<SpriteRenderer>().sortingOrder;
						pop_up_obj = level_max_bg;
					}
					SetCondBg(chara, value, value2);
				}
				else if (manager.data.character_data[value].contents[value2].watched_video == 0)
				{
					movie_bg = UnityEngine.Object.Instantiate(moviePrefab, chara.transform, worldPositionStays: false).gameObject;
					SpriteRenderer component3 = movie_bg.transform.Find("black_wall").GetComponent<SpriteRenderer>();
					component3.transform.localScale = new Vector3(50f, 50f, 1f);
					Transform transform = component3.transform;
					Vector3 localPosition = component3.transform.localPosition;
					float x3 = localPosition.x;
					Vector3 localPosition2 = component3.transform.localPosition;
					transform.localPosition = new Vector3(x3, localPosition2.y, 1f);
					component3.color = new Color(0f, 0f, 0f, 0.3f);
					for (int i = 0; i < contents_scroller.Length; i++)
					{
						contents_scroller[i].SetTouchEventEnabled(enabled: false);
					}
					bg_order = movie_bg.GetComponent<SpriteRenderer>().sortingOrder;
					pop_up_obj = movie_bg;
					TouchEvent touch_on = movie_bg.transform.Find("On").GetComponent<TouchEvent>();
					if (manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
					{
						touch_on.ClickUp.AddListener(delegate
						{
							MovieStart(chara);
						});
						touch_on.ClickDown.AddListener(delegate
						{
							Utils.Play(touch_on.GetComponent<Animation>(), "PushIcon", 1f, 0f);
						});
						touch_on.ClickDown.AddListener(delegate
						{
							Manager.sound.ClickSound();
						});
					}
					else
					{
						touch_on.transform.Find("contents/sprite").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
						touch_on.ClickDown.AddListener(delegate
						{
							Utils.Play(touch_on.GetComponent<Animation>(), "PushIconDeny", 1f, 0f);
						});
						touch_on.ClickUp.AddListener(delegate
						{
							Manager.sound.PlaySe(Sound.eSe.BEEP);
						});
						if (manager.load_video.state == Manager.VideoRwd.eState.NONE)
						{
							manager.LoadVideo();
						}
					}
					TouchEvent touch_off = movie_bg.transform.Find("Cancel").GetComponent<TouchEvent>();
					touch_off.ClickUp.AddListener(delegate
					{
						BlackPrefabDestroy(chara);
					});
					touch_off.ClickDown.AddListener(delegate
					{
						Utils.Play(touch_off.GetComponent<Animation>(), "PushIcon", 1f, 0f);
					});
					touch_off.ClickDown.AddListener(delegate
					{
						Manager.sound.CancelSound();
					});
				}
				else
				{
					SetCondBg(chara, value, value2);
				}
				ChangeOrderInLayer(chara);
				ResettingBG(pop_up_obj);
				SetTouchEnable(enable: false);
				return;
			}
		}
		if (black_bg != null)
		{
			ResetBG();
		}
		else
		{
			Manager.office.menu.CloseMenu(buy_flag: false);
		}
	}

	private void SetCondBg(TouchEvent chara, int type, int id)
	{
		cond_bg = UnityEngine.Object.Instantiate(condPrefab, chara.transform, worldPositionStays: false).gameObject;
		if (level_up_bg != null)
		{
			Transform transform = cond_bg.transform;
			Vector3 localPosition = cond_bg.transform.localPosition;
			transform.localPosition = new Vector2(localPosition.x, -0.6f);
			level_up_bg.GetComponent<SpriteRenderer>().sortingOrder = level_up_bg.GetComponent<SpriteRenderer>().sortingOrder + 10000;
		}
		else if (level_max_bg != null)
		{
			Transform transform2 = cond_bg.transform;
			Vector3 localPosition2 = cond_bg.transform.localPosition;
			transform2.localPosition = new Vector2(localPosition2.x, -0.53f);
			level_max_bg.GetComponent<SpriteRenderer>().sortingOrder = level_max_bg.GetComponent<SpriteRenderer>().sortingOrder + 10000;
		}
		else
		{
			Transform transform3 = cond_bg.transform;
			Vector3 localPosition3 = cond_bg.transform.localPosition;
			transform3.localPosition = new Vector2(localPosition3.x, -0.27f);
		}
		bg_order = cond_bg.GetComponent<SpriteRenderer>().sortingOrder;
		pop_up_obj = cond_bg;
		SpriteRenderer component = cond_bg.transform.Find("hint_icon").GetComponent<SpriteRenderer>();
		component.sortingOrder += 10000;
		for (int i = 0; i < 6; i++)
		{
			need_item[i] = cond_bg.transform.Find("icon" + (i + 1)).GetComponent<SpriteRenderer>();
			need_item[i].sprite = null;
			need_icon_order[i] = need_item[i].sortingOrder;
		}
		level_text = cond_bg.transform.Find("level_text").GetComponent<TextMesh>();
		need_level_text_order = level_text.GetComponent<CustomText>().order_in_layer;
		switch (type)
		{
		case 0:
		{
			int num = FarmAnimal.GetConditionLength();
			for (int m = 0; m < num; m++)
			{
				Data.Condition conditions4 = FarmAnimal.GetConditions((FarmAnimal.eType)id, m);
				SetCondImage(conditions4, need_item[m], level_text, m);
			}
			break;
		}
		case 1:
		{
			int num = WildAnimal.GetConditionLength() - 1;
			for (int k = 0; k < num; k++)
			{
				Data.Condition conditions2 = WildAnimal.GetConditions((WildAnimal.eType)id, k);
				SetCondImage(conditions2, need_item[k], level_text, k);
			}
			break;
		}
		case 2:
		{
			int num = Fish.GetConditionLength();
			for (int l = 0; l < num; l++)
			{
				Data.Condition conditions3 = Fish.GetConditions((Fish.eType)id, l);
				SetCondImage(conditions3, need_item[l], level_text, l);
			}
			break;
		}
		case 3:
		{
			int num = Customer.GetConditionLength() - 1;
			for (int j = 0; j < num; j++)
			{
				Data.Condition conditions = Customer.GetConditions((Customer.eType)id, j);
				SetCondImage(conditions, need_item[j], level_text, j);
			}
			break;
		}
		}
		SetNeedItemEnabled(type);
	}

	private void SetCondImage(Data.Condition cond, SpriteRenderer need_item, TextMesh level_text, int index)
	{
		if (cond != null && cond.category != Data.Condition.eCATEGORY.FARM_TYPE)
		{
			level_text.text = string.Empty;
			if (cond.category == Data.Condition.eCATEGORY.FARMANIMAL)
			{
				need_item.sprite = SpriteManager.GetAnimalIcon((FarmAnimal.eType)cond.type);
			}
			else if (cond.category == Data.Condition.eCATEGORY.FACILITY)
			{
				need_item.sprite = SpriteManager.GetFacilityIcon((Facility.eType)cond.type);
			}
			else if (cond.category == Data.Condition.eCATEGORY.FACILITYITEM)
			{
				need_item.sprite = SpriteManager.GetItemIcon((Facility.eItem)cond.type);
			}
			else if (cond.category == Data.Condition.eCATEGORY.LEVEL)
			{
				need_item.sprite = SpriteManager.GetLevelMark();
				level_text.text = string.Empty + cond.type;
				need_level_flag = index;
			}
			else if (cond.category == Data.Condition.eCATEGORY.SEASON_EVENT)
			{
				need_item.sprite = SpriteManager.GetCondSeasonHarvest((Common.eSEASON_EVENT)cond.type);
			}
			else if (cond.category == Data.Condition.eCATEGORY.HOTEL)
			{
				need_item.sprite = SpriteManager.GetCondHotelStar(cond.type);
			}
		}
	}

	private void SetNeedItemEnabled(int type)
	{
		if (type == 0)
		{
			need_item[1].enabled = true;
			need_item[2].enabled = false;
			need_item[3].enabled = false;
			need_item[4].enabled = false;
			need_item[5].enabled = false;
			need_item[0].sprite = SpriteManager.GetCondLevelUpHarvest((Data.CharacterData.eType)type);
		}
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (need_item[i].sprite != null)
			{
				num++;
			}
		}
		for (int j = 0; j < 6; j++)
		{
			Vector3 localPosition = need_item[j].gameObject.transform.localPosition;
			if (j == need_level_flag)
			{
				localPosition.x = Data.SetIconX[num - 1, j] + 0.03f;
				level_text.transform.localPosition = new Vector2(localPosition.x + 0.005f, 0f);
			}
			else
			{
				localPosition.x = Data.SetIconX[num - 1, j];
			}
			need_item[j].gameObject.transform.localPosition = localPosition;
			need_item[j].sortingOrder = need_icon_order[j] + 10000;
			level_text.GetComponent<CustomText>().order_in_layer = need_level_text_order + 10000;
		}
	}

	private void ResettingBG(GameObject obj)
	{
		if (cond_bg != null || movie_bg != null)
		{
			Vector3 position = album_left_end.transform.position;
			float x = position.x;
			Vector3 position2 = obj.transform.position;
			float num = x - position2.x;
			if (num > -0.1f)
			{
				Vector3 localPosition = obj.transform.localPosition;
				localPosition.x = 0.3f;
				obj.transform.localPosition = localPosition;
			}
			else if (num < -1.4f)
			{
				Vector3 localPosition2 = obj.transform.localPosition;
				localPosition2.x = -0.4f;
				obj.transform.localPosition = localPosition2;
			}
			if (level_up_bg != null)
			{
				Transform transform = level_up_bg.transform;
				Vector3 localPosition3 = cond_bg.transform.localPosition;
				float x2 = localPosition3.x;
				Vector3 localPosition4 = level_up_bg.transform.localPosition;
				transform.localPosition = new Vector2(x2, localPosition4.y);
			}
			else if (level_max_bg != null)
			{
				Transform transform2 = level_max_bg.transform;
				Vector3 localPosition5 = cond_bg.transform.localPosition;
				float x3 = localPosition5.x;
				Vector3 localPosition6 = level_max_bg.transform.localPosition;
				transform2.localPosition = new Vector2(x3, localPosition6.y);
			}
		}
	}

	public void ResetBG()
	{
		if (black_bg != null)
		{
			ResetOrderInLayer();
			UnityEngine.Object.Destroy(black_bg.gameObject);
			black_bg = null;
		}
		if (level_up_bg != null)
		{
			UnityEngine.Object.Destroy(level_up_bg.gameObject);
			level_up_bg = null;
		}
		if (level_max_bg != null)
		{
			UnityEngine.Object.Destroy(level_max_bg.gameObject);
			level_max_bg = null;
		}
		if (cond_bg != null)
		{
			UnityEngine.Object.Destroy(cond_bg.gameObject);
			cond_bg = null;
		}
		if (movie_bg != null)
		{
			UnityEngine.Object.Destroy(movie_bg.gameObject);
			movie_bg = null;
			for (int i = 0; i < contents_scroller.Length; i++)
			{
				contents_scroller[i].SetTouchEventEnabled(enabled: true);
			}
		}
		if (selector != null)
		{
			UnityEngine.Object.Destroy(selector);
			selector = null;
		}
		prev_type = -1;
		prev_id = -1;
		SetTouchEnable(enable: true);
	}

	private void BlackPrefabDestroy(TouchEvent chara)
	{
		ResetBG();
	}

	private void ChangeOrderInLayer(TouchEvent chara)
	{
		area_obj = chara.transform.Find("area").GetComponent<SpriteRenderer>();
		area_order = area_obj.sortingOrder;
		area_obj.sortingOrder = area_order + 10000;
		hint_obj = chara.transform.Find("hint").GetComponent<SpriteRenderer>();
		hint_order = hint_obj.sortingOrder;
		hint_obj.sortingOrder = hint_order + 10000;
		lv_obj = chara.transform.Find("lv").GetComponent<SpriteRenderer>();
		lv_order = lv_obj.sortingOrder;
		lv_obj.sortingOrder = lv_order + 10000;
		text_obj = chara.transform.Find("text").GetComponent<CustomText>();
		text_order = text_obj.order_in_layer;
		text_obj.order_in_layer = text_order + 10000;
		pop_up_obj.GetComponent<SpriteRenderer>().sortingOrder = bg_order + 10000;
		if (chara.param.value1 == 3)
		{
			customer = chara.transform.Find("human").GetComponent<PartsController>();
			customer.SetSortingOrderAll(20011);
			sprite_obj = null;
			shadow_obj = null;
		}
		else
		{
			sprite_obj = chara.transform.Find("sprite").GetComponent<SpriteRenderer>();
			sprite_order = sprite_obj.sortingOrder;
			sprite_obj.sortingOrder = sprite_order + 10000;
			shadow_obj = chara.transform.Find("shadow").GetComponent<SpriteRenderer>();
			shadow_order = shadow_obj.sortingOrder;
			shadow_obj.sortingOrder = shadow_order + 10000;
		}
		if (level_up_bg != null)
		{
			now_level.GetComponent<CustomText>().order_in_layer = level_up_text1_order + 10000;
			next_level.GetComponent<CustomText>().order_in_layer = level_up_text2_order + 10000;
			need_point.GetComponent<CustomText>().order_in_layer = level_up_text3_order + 10000;
			need_icon.sortingOrder = level_up_icon_order + 10000;
		}
		else if (cond_bg != null)
		{
			for (int i = 0; i < 6; i++)
			{
				need_item[i].sortingOrder = need_icon_order[i] + 10000;
				level_text.GetComponent<CustomText>().order_in_layer = need_level_text_order + 10000;
			}
		}
		prev_type = chara.param.value1;
		prev_id = chara.param.value2;
	}

	private void ResetOrderInLayer()
	{
		area_obj.sortingOrder = area_order;
		hint_obj.sortingOrder = hint_order;
		lv_obj.sortingOrder = lv_order;
		text_obj.order_in_layer = text_order;
		if (sprite_obj != null)
		{
			sprite_obj.sortingOrder = sprite_order;
			shadow_obj.sortingOrder = shadow_order;
		}
		else
		{
			customer.SetSortingOrderAll(10011);
		}
		if (pop_up_obj != null)
		{
			pop_up_obj.GetComponent<SpriteRenderer>().sortingOrder = bg_order;
		}
	}

	public void DestroyAlbum()
	{
		for (int i = 0; i < 4; i++)
		{
			touch_list[i].Clear();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ChangeAlbumType(int tab)
	{
		if (tab != album_mode)
		{
			bool flag = false;
			if (tab == 0)
			{
				Manager.office.menu.album_type = 0;
				flag = true;
			}
			else if (tab == 1 && (manager.data.level >= Price.OpenFarmMenu() || Data.farm_type != 0))
			{
				Manager.office.menu.album_type = 1;
				flag = true;
			}
			else if (tab == 1 && manager.data.level < Price.OpenFarmMenu() && Data.farm_type == Data.eFarmType.NORMAL)
			{
				Manager.office.menu.BeepSE();
			}
			if (flag)
			{
				Manager.office.menu.CloseMenu(buy_flag: false);
				Manager.office.menu.TouchAlbum(1);
			}
		}
	}

	private void SetTabSprite(SpriteRenderer sr, int tab_id)
	{
		if (album_mode == tab_id)
		{
			sr.sprite = SpriteManager.GetAlbumTabBg(1);
			return;
		}
		sr.sprite = SpriteManager.GetAlbumTabBg(0);
		if (tab_id != 0 && Data.farm_type == Data.eFarmType.NORMAL && manager.data.level < Price.OpenFarmMenu())
		{
			TouchEvent component = base.transform.Find("objects/tab_" + tab_id).GetComponent<TouchEvent>();
			SpriteRenderer component2 = base.transform.Find("objects/tab_" + tab_id + "/album").GetComponent<SpriteRenderer>();
			component2.color = new Color(0.5f, 0.5f, 0.5f);
			SpriteRenderer component3 = base.transform.Find("objects/tab_" + tab_id + "/item").GetComponent<SpriteRenderer>();
			component3.color = new Color(0.5f, 0.5f, 0.5f);
		}
	}

	public void MovieStart(TouchEvent chara)
	{
		prev_type = -1;
		prev_id = -1;
		BlackPrefabDestroy(chara);
		manager.PlayVideo(delegate
		{
			PlayVideoCompleted(chara);
		}, PlayVideoFailed);
		for (int i = 0; i < contents_scroller.Length; i++)
		{
			contents_scroller[i].SetTouchEventEnabled(enabled: true);
		}
	}

	private void PlayVideoCompleted(TouchEvent touch_ch)
	{
		Utils.Log("Album: PlayVideoCompleted");
		int value = touch_ch.param.value1;
		int value2 = touch_ch.param.value2;
		manager.data.SetWatchedVideo(1, value, value2);
		Manager.office.menu.album.TouchAlbumCharacter(touch_ch);
		manager.LoadVideo();
	}

	private void PlayVideoFailed()
	{
		Utils.Log("Album: PlayVideoFailed");
		manager.LoadVideo();
	}

	public void Close()
	{
		SpriteRenderer component = base.transform.Find("Pixel_Black").GetComponent<SpriteRenderer>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component.gameObject);
		}
		Animation component2 = GetComponent<Animation>();
		component2.Play("closealbum");
		Manager.office.menu.gameObject.SetActive(value: true);
		Manager.office.menu.menu_flag = 1;
		Manager.office.menu.SetNewIcon();
	}

	public void CancelSE()
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
	}
}
