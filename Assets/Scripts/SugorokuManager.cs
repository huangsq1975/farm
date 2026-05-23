using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SugorokuManager : MonoBehaviour
{
	public class Mass
	{
		public enum eType
		{
			NORMAL,
			COIN,
			EXP,
			FARM,
			HOME,
			MAX
		}

		public eType type;

		public SpriteRenderer renderer;

		public Vector2 default_pos;

		public Mass(eType type, SpriteRenderer renderer)
		{
			this.type = type;
			this.renderer = renderer;
			default_pos = renderer.transform.localPosition;
		}
	}

	public class Car
	{
		public Animator animator;

		private SpriteRenderer body;

		private SpriteRenderer tire;

		private SpriteRenderer shadow;

		private Sequence sequence;

		private Animation reminder;

		private TextMesh reminder_text;

		public void Create(Transform parent)
		{
			GameObject original = Resources.Load("Prefab/sugoroku_car") as GameObject;
			animator = UnityEngine.Object.Instantiate(original, parent, worldPositionStays: false).GetComponent<Animator>();
			body = animator.transform.Find("body").GetComponent<SpriteRenderer>();
			tire = animator.transform.Find("tire").GetComponent<SpriteRenderer>();
			shadow = animator.transform.Find("shadow").GetComponent<SpriteRenderer>();
			reminder = animator.transform.Find("reminder").GetComponent<Animation>();
			reminder_text = reminder.transform.Find("text").GetComponent<TextMesh>();
			reminder.gameObject.SetActive(value: false);
			sequence = DOTween.Sequence();
		}

		public void SetDir(Data.SugorokuData.eDir dir)
		{
			switch (dir)
			{
			case Data.SugorokuData.eDir.RIGHT:
				Utils.Play(animator, "sugoroku_car_side", 1f, 0f);
				SetFlipX(flipx: true);
				return;
			case Data.SugorokuData.eDir.UP:
				Utils.Play(animator, "sugoroku_car_up", 1f, 0f);
				break;
			case Data.SugorokuData.eDir.DOWN:
				Utils.Play(animator, "sugoroku_car_down", 1f, 0f);
				break;
			case Data.SugorokuData.eDir.LEFT:
				Utils.Play(animator, "sugoroku_car_side", 1f, 0f);
				break;
			}
			SetFlipX(flipx: false);
		}

		public void Move(Vector2 pos, float duration, UnityAction finish_call)
		{
			sequence.Kill();
			sequence = DOTween.Sequence();
			sequence.Append(animator.transform.DOLocalMove(pos, duration).SetEase(Ease.Linear));
			sequence.AppendCallback(finish_call.Invoke);
			sequence.Play();
		}

		public void ShowReminder(int count)
		{
			if (reminder.gameObject.activeSelf)
			{
				Utils.Play(reminder, "sugoroku_reminder", 1.5f, 0f);
			}
			else
			{
				reminder.gameObject.SetActive(value: true);
				Utils.Play(reminder, "sugoroku_reminder", 1.5f, 0.333333343f);
			}
			reminder_text.text = count.ToString();
		}

		public void HideReminder()
		{
			reminder.gameObject.SetActive(value: false);
		}

		public void Destroy()
		{
			if (animator != null)
			{
				sequence.Kill();
				UnityEngine.Object.Destroy(animator.gameObject);
				animator = null;
				body = null;
				tire = null;
				shadow = null;
			}
		}

		private void SetFlipX(bool flipx)
		{
			body.flipX = flipx;
			tire.flipX = flipx;
			shadow.flipX = flipx;
		}
	}

	public class Cursor
	{
		public class Parts
		{
			public TouchEvent touch;

			public SpriteRenderer arrow;

			public SpriteRenderer shadow;
		}

		private SugorokuManager sm;

		public GameObject root;

		private GameObject prefab;

		private Parts[] parts = new Parts[4];

		public void Create(SugorokuManager sm, Transform parent)
		{
			this.sm = sm;
			prefab = (Resources.Load("Prefab/sugoroku_cursor") as GameObject);
			root = UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays: false);
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i] = new Parts();
				Parts obj = parts[i];
				Transform transform = root.transform;
				Data.SugorokuData.eDir eDir = (Data.SugorokuData.eDir)i;
				obj.touch = transform.Find(eDir.ToString().ToLower()).GetComponent<TouchEvent>();
				Animation anim = parts[i].touch.GetComponent<Animation>();
				parts[i].touch.ClickDown.AddListener(delegate
				{
					anim.Play();
				});
				Data.SugorokuData.eDir dir = (Data.SugorokuData.eDir)i;
				parts[i].touch.ClickUp.AddListener(delegate
				{
					sm.TouchDirectionButton(dir);
				});
				parts[i].arrow = parts[i].touch.transform.Find("contents/sprite").GetComponent<SpriteRenderer>();
				parts[i].shadow = parts[i].touch.transform.Find("contents/shadow").GetComponent<SpriteRenderer>();
			}
		}

		public void SetEnabled(bool enabled, Data.SugorokuData.eDir dir)
		{
			parts[(int)dir].arrow.enabled = enabled;
			parts[(int)dir].shadow.enabled = enabled;
			parts[(int)dir].touch.SetEnabled(enabled);
		}

		public void Destroy()
		{
			if (root != null)
			{
				UnityEngine.Object.Destroy(root);
				sm = null;
				for (int i = 0; i < parts.Length; i++)
				{
					parts[i] = null;
				}
			}
		}
	}

	public class DiceArea
	{
		public class DiceParts
		{
			private DiceArea dice_area;

			public Animation anim;

			public TouchEvent touch;

			public AnimEvent events;

			public SpriteRenderer sprite;

			public SpriteRenderer ledge;

			public DiceParts(DiceArea dice_area, Transform t)
			{
				this.dice_area = dice_area;
				anim = t.GetComponent<Animation>();
				touch = t.GetComponent<TouchEvent>();
				events = t.GetComponent<AnimEvent>();
				sprite = t.Find("sprite").GetComponent<SpriteRenderer>();
				ledge = t.Find("ledge").GetComponent<SpriteRenderer>();
			}

			public void SetSprite(bool enabled)
			{
				sprite.enabled = enabled;
				ledge.enabled = enabled;
			}

			public void SetTouch(bool enabled)
			{
				if (enabled)
				{
					touch.ClickUp.AddListener(delegate
					{
						dice_area.Touched(this);
					});
				}
				else
				{
					touch.ClickUp.RemoveAllListeners();
				}
				events.SetFinishCallback(null);
			}

			public void SetAnimEvent()
			{
				events.SetFinishCallback(delegate
				{
					dice_area.FinishedTouched(this);
					events.SetFinishCallback(null);
				});
			}
		}

		public const int RESTORE_TIME = 600;

		private const string ANIM = "sugoroku_dice";

		private const string ANIM_ENABLED = "sugoroku_dice_enabled";

		private SugorokuManager sm;

		private DiceParts[] dices = new DiceParts[3];

		private TextMesh text;

		private SpriteRenderer plus_icon;

		private TouchEvent plus_icon_touch;

		private Prompt prompt;

		private const float ALPHA = 0.5f;

		public void Init(SugorokuManager sm, Transform parent)
		{
			this.sm = sm;
			Transform transform = parent.Find("dice_area");
			for (int i = 0; i < dices.Length; i++)
			{
				dices[i] = new DiceParts(this, transform.Find("dice_" + (i + 1)));
				SetDice(i, sm.sugoroku_data.dice_remind_count, sounds: false);
			}
			text = transform.Find("timer_area/text").GetComponent<TextMesh>();
			plus_icon_touch = transform.transform.Find("timer_area/plus_icon").GetComponent<TouchEvent>();
			plus_icon = plus_icon_touch.transform.Find("contents").GetComponent<SpriteRenderer>();
			plus_icon.color = Utils.Alpha(plus_icon.color, 0.25f);
			SetTimeText(0);
		}

		private void SetDice(int id, int dice_remind_count, bool sounds = true)
		{
			if (dice_remind_count < id + 1)
			{
				dices[id].SetSprite(enabled: false);
				dices[id].SetTouch(enabled: false);
			}
			else if (dice_remind_count == id + 1 && !Dice.IsExist() && sm.sugoroku_data.move_count == sm.sugoroku_data.dice_value)
			{
				Utils.Play(dices[id].anim, "sugoroku_dice_enabled", 1f, 0f);
				dices[id].SetSprite(enabled: true);
				dices[id].SetTouch(enabled: true);
				if (sounds)
				{
					Manager.sound.PlaySe(Sound.eSe.FULL);
				}
			}
			else
			{
				Utils.Play(dices[id].anim, "sugoroku_dice_enabled", -1f, dices[id].anim["sugoroku_dice_enabled"].normalizedTime);
				dices[id].SetSprite(enabled: true);
				dices[id].SetTouch(enabled: false);
			}
		}

		public void EnableDice()
		{
			if (sm.sugoroku_data.dice_remind_count > 0)
			{
				SetDice(sm.sugoroku_data.dice_remind_count - 1, sm.sugoroku_data.dice_remind_count);
			}
		}

		public void ChangeDiceTouchState(bool true_false)
		{
			for (int i = 0; i < 3; i++)
			{
				dices[i].SetTouch(true_false);
			}
		}

		private void SetPlusIcon()
		{
			if (sm.sugoroku_data.dice_remind_count == 3 || sm.sugoroku_data.move_count < sm.sugoroku_data.dice_value || Dice.IsExist())
			{
				Color color = plus_icon.color;
				if (color.a != 0.5f)
				{
					plus_icon.color = Utils.Alpha(plus_icon.color, 0.5f);
					plus_icon_touch.ClickDown.RemoveAllListeners();
					plus_icon_touch.ClickUp.RemoveAllListeners();
				}
			}
			else if (sm.manager.load_video.state == Manager.VideoRwd.eState.LOAD_COMPLETED)
			{
				Color color2 = plus_icon.color;
				if (color2.a != 1f)
				{
					plus_icon.color = Utils.Alpha(plus_icon.color, 1f);
					plus_icon_touch.ClickDown.AddListener(delegate
					{
						plus_icon_touch.GetComponent<Animation>().Play();
					});
					plus_icon_touch.ClickUp.AddListener(delegate
					{
						TouchedPlusIcon();
						Manager.sound.PlaySe(Sound.eSe.HUG);
					});
				}
			}
			else if (sm.manager.load_video.state == Manager.VideoRwd.eState.NONE)
			{
				sm.manager.LoadVideo();
			}
		}

		private void TouchedPlusIcon()
		{
			UnityAction call_ok = delegate
			{
				sm.manager.PlayVideo(PlayVideoCompleted, null);
				if (prompt != null)
				{
					UnityEngine.Object.Destroy(prompt.gameObject);
				}
			};
			UnityAction call_cancel = delegate
			{
				if (prompt != null)
				{
					UnityEngine.Object.Destroy(prompt.gameObject);
				}
			};
			prompt = Prompt.CreateVideoSugorokuPrompt(plus_icon_touch.transform, new Vector2(0.56f, 0.56f), call_ok, call_cancel);
		}

		private void PlayVideoCompleted()
		{
			Utils.Log("Sugoroku : PlayVideoCompleted - reminder_count=" + sm.sugoroku_data.dice_remind_count);
			if (sm.sugoroku_data.dice_remind_count < 3)
			{
				sm.sugoroku_data.SetRestoreTime((ulong)(DateTime.Now.Ticks - 24000000000L));
			}
			sm.manager.LoadVideo();
		}

		private void Touched(DiceParts dice)
		{
			if (!Dice.IsExist())
			{
				Manager.sound.PlaySe(Sound.eSe.FULL);
				Utils.Play(dice.anim, "sugoroku_dice_enabled", -1f, 1f);
				dice.SetTouch(enabled: false);
				sm.dice_area.SetDiceRemind();
				sm.sugoroku_data.SetDiceValue(-1);
				dice.SetAnimEvent();
			}
		}

		public void FinishedTouched(DiceParts dice)
		{
			Dice.Throw(sm.PreFinishThrow, sm.FinishedThrow);
			for (int i = 0; i < dices.Length; i++)
			{
				SetDice(i, sm.sugoroku_data.dice_remind_count);
			}
		}

		public void SetDiceRemind()
		{
			sm.sugoroku_data.SetDiceRemindCount(sm.sugoroku_data.dice_remind_count - 1);
			if (sm.sugoroku_data.restore_time == 0)
			{
				sm.sugoroku_data.SetRestoreTime((ulong)DateTime.Now.Ticks);
			}
		}

		public void Update()
		{
			if (!(sm != null))
			{
				return;
			}
			if (sm.sugoroku_data.dice_remind_count < 3)
			{
				int num = (int)((ulong)(DateTime.Now.Ticks - (long)sm.sugoroku_data.restore_time) / 10000000uL);
				SetTimeText(600 - num);
				if (num >= 600)
				{
					sm.sugoroku_data.SetDiceRemindCount(sm.sugoroku_data.dice_remind_count + 1);
					if (sm.sugoroku_data.dice_remind_count == 3)
					{
						sm.sugoroku_data.SetRestoreTime(0uL);
						SetTimeText(0);
						if (prompt != null)
						{
							UnityEngine.Object.Destroy(prompt.gameObject);
							prompt = null;
						}
					}
					else
					{
						ulong num2 = (ulong)(DateTime.Now.Ticks - (long)(num - 600) * 10000000L);
						sm.sugoroku_data.SetRestoreTime(num2);
						SetTimeText(600 - (int)(num2 / 10000000uL));
					}
					for (int i = 0; i < dices.Length; i++)
					{
						SetDice(i, sm.sugoroku_data.dice_remind_count);
					}
				}
				SetPlusIcon();
			}
			else
			{
				SetPlusIcon();
			}
		}

		private void SetTimeText(int reminder)
		{
			text.text = (reminder / 60).ToString("00") + ":" + (reminder % 60).ToString("00");
			if (reminder == 0)
			{
				text.color = Utils.Alpha(text.color, 0.2f);
			}
			else
			{
				text.color = Utils.Alpha(text.color, 1f);
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < dices.Length; i++)
			{
				dices[i] = null;
			}
			sm = null;
		}
	}

	public class MassTable
	{
		public Mass.eType reserved;

		public int[] connects = new int[4];

		public Vector2 farm_offset;

		public MassTable(Mass.eType reserved, int[] connects, Vector2 farm_offset)
		{
			this.reserved = reserved;
			this.connects = connects;
			this.farm_offset = farm_offset;
		}
	}

	private Manager manager;

	private Data.SugorokuData sugoroku_data;

	private DiceArea dice_area = new DiceArea();

	public Animation anim;

	public Animation progress;

	public TextMesh text;

	public GameObject map;

	private Transform grass;

	private Car car = new Car();

	private Cursor cursor = new Cursor();

	private List<Mass> masses = new List<Mass>();

	private Common.Bag coin_bag;

	private Common.Bag exp_bag;

	private Sequence mass_sequence;

	private Common.SugorokuFarmPrompt farm_prompt;

	private TouchEvent touch;

	private GameObject balloon;

	public int pre_dice_value = -1;

	private const string OPEN_ANIM = "sugoroku_open";

	private List<MassTable[]> MapList = new List<MassTable[]>
	{
		new MassTable[22]
		{
			new MassTable(Mass.eType.FARM, new int[4]
			{
				-1,
				3,
				-1,
				-1
			}, new Vector2(0f, 0.05f)),
			new MassTable(Mass.eType.FARM, new int[4]
			{
				-1,
				5,
				-1,
				-1
			}, new Vector2(0f, 0.05f)),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				-1,
				6,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				0,
				7,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				-1,
				8,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				1,
				-1,
				6,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				2,
				9,
				-1,
				5
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				3,
				-1,
				8,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				4,
				10,
				-1,
				7
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				6,
				12,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				8,
				14,
				11,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.HOME, new int[4]
			{
				-1,
				15,
				12,
				10
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				9,
				-1,
				-1,
				11
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				-1,
				16,
				14,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				10,
				-1,
				15,
				13
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				11,
				17,
				-1,
				14
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				13,
				-1,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				15,
				20,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				-1,
				21,
				-1,
				-1
			}, Vector2.zero),
			new MassTable(Mass.eType.FARM, new int[4]
			{
				-1,
				-1,
				20,
				-1
			}, new Vector2(0f, 0.05f)),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				17,
				-1,
				21,
				19
			}, Vector2.zero),
			new MassTable(Mass.eType.NORMAL, new int[4]
			{
				18,
				-1,
				-1,
				20
			}, Vector2.zero)
		},
		new MassTable[1]
		{
			new MassTable(Mass.eType.FARM, new int[4]
			{
				-1,
				-1,
				-1,
				-1
			}, Vector2.zero)
		}
	};

	public void Init(Manager m)
	{
		manager = m;
		sugoroku_data = m.data.sugoroku_data;
		if (Data.farm_type == Data.eFarmType.NORMAL)
		{
			SetEnabled(manager.data.level > 1);
		}
		else
		{
			SetEnabled(enabled: false);
		}
	}

	private void Update()
	{
		dice_area.Update();
	}

	public void SetEnabled(bool enabled, bool effect = false)
	{
		Transform transform = base.transform.Find("icon");
		transform.gameObject.SetActive(enabled);
		base.transform.Find("bg").gameObject.SetActive(enabled);
		if (enabled && effect)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			Vector2 pos = new Vector2(x, position2.y - 0.1f);
			Effect.Run(Resources.Load("Prefab/effect_trash") as GameObject, pos, base.transform);
			Manager.sound.PlaySe(Sound.eSe.SMOKE);
		}
	}

	public void Open()
	{
		if (anim == null)
		{
			GameObject original = Resources.Load("Prefab/sugoroku") as GameObject;
			anim = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<Animation>();
			touch = anim.GetComponent<TouchEvent>();
			touch.ClickDown.AddListener(Manager.sound.CancelSound);
			touch.ClickUp.AddListener(Close);
			Utils.Play(anim, "sugoroku_open", 1f, 0f);
			grass = anim.transform.Find("grass");
			car.Create(grass);
			car.SetDir(sugoroku_data.car_dir);
			car.ShowReminder(3);
			cursor.Create(this, grass);
			dice_area.Init(this, grass);
			SetMap();
			Manager.sound.PlayBgm(Sound.eBgm.MENU);
			CheckMassEvent(masses[sugoroku_data.current]);
		}
	}

	public void Close()
	{
		if (anim != null)
		{
			if (pre_dice_value != -1)
			{
				FinishedThrow(pre_dice_value);
			}
			touch = null;
			masses.Clear();
			anim.GetComponent<AnimEvent>().auto_destroy = true;
			Utils.Play(anim, "sugoroku_open", -1f, (!anim.isPlaying) ? 1f : anim["sugoroku_open"].normalizedTime);
			anim = null;
			if (map != null)
			{
				UnityEngine.Object.Destroy(map);
			}
			car.Destroy();
			cursor.Destroy();
			dice_area.Destroy();
			Dice.Destroy();
			if (mass_sequence != null)
			{
				mass_sequence.Kill();
			}
			if (farm_prompt != null)
			{
				UnityEngine.Object.Destroy(farm_prompt.root);
			}
			Manager.sound.PlayBgm(Sound.eBgm.FIELD);
		}
	}

	private void SetMap()
	{
		masses.Clear();
		GameObject original = Resources.Load("Prefab/sugoroku_map_" + (sugoroku_data.map_id + 1)) as GameObject;
		GameObject gameObject = base.transform.Find("sugoroku(Clone)/grass").gameObject;
		map = UnityEngine.Object.Instantiate(original, gameObject.transform, worldPositionStays: false);
		for (int i = 0; i < MapList[0].GetLength(0); i++)
		{
			Mass item = new Mass(MapList[0][i].reserved, map.transform.Find("mass" + i).GetComponent<SpriteRenderer>());
			masses.Add(item);
		}
		SetFarmMass(sugoroku_data.map_id);
		SetNormalMass(sugoroku_data.map_id);
		SetCoinMass(sugoroku_data.map_id);
		SetExpMass(sugoroku_data.map_id);
		if (sugoroku_data.history[0] == -1)
		{
			sugoroku_data.SetHistry(sugoroku_data.move_count, sugoroku_data.current);
		}
		car.animator.transform.localPosition = SetCarPosition(Data.SugorokuData.eDir.DOWN);
		int num = 0;
		if (sugoroku_data.dice_value == -1)
		{
			sugoroku_data.dice_value = UnityEngine.Random.Range(1, 7);
			FinishedThrow(sugoroku_data.dice_value);
		}
		num = sugoroku_data.dice_value - sugoroku_data.move_count;
		car.ShowReminder(num);
		if (num == 0)
		{
			car.HideReminder();
		}
		else
		{
			dice_area.ChangeDiceTouchState(true_false: false);
		}
		cursor.root.transform.localPosition = SetCursorPosition();
		hideDirectionButton();
	}

	private Vector2 SetCarPosition(Data.SugorokuData.eDir dir)
	{
		Vector2 v = masses[sugoroku_data.current].renderer.transform.localPosition;
		switch (dir)
		{
		case Data.SugorokuData.eDir.DOWN:
			v = new Vector2(v.x, v.y - 0.1f);
			break;
		case Data.SugorokuData.eDir.UP:
			v = new Vector2(v.x, v.y - 0.1f);
			break;
		case Data.SugorokuData.eDir.RIGHT:
			v = ((MapList[0][sugoroku_data.current].reserved != Mass.eType.FARM) ? new Vector2(v.x, v.y - 0.03f) : new Vector2(v.x, v.y - 0.064f - 0.03f));
			break;
		case Data.SugorokuData.eDir.LEFT:
			v = ((MapList[0][sugoroku_data.current].reserved != Mass.eType.FARM) ? new Vector2(v.x, v.y - 0.03f) : new Vector2(v.x, v.y - 0.064f - 0.03f));
			break;
		}
		return grass.InverseTransformPoint(map.transform.TransformPoint(v));
	}

	private Vector3 SetCursorPosition()
	{
		Vector3 localPosition = masses[sugoroku_data.current].renderer.transform.localPosition;
		localPosition.y += 0.1f;
		localPosition.z = -20f;
		return grass.InverseTransformPoint(map.transform.TransformPoint(localPosition));
	}

	private void hideDirectionButton()
	{
		if (sugoroku_data.dice_value - sugoroku_data.move_count == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				cursor.SetEnabled(enabled: false, (Data.SugorokuData.eDir)i);
			}
			return;
		}
		for (int j = 0; j < 4; j++)
		{
			if (MapList[0][sugoroku_data.current].connects[j] != -1)
			{
				cursor.SetEnabled(enabled: true, (Data.SugorokuData.eDir)j);
			}
			else
			{
				cursor.SetEnabled(enabled: false, (Data.SugorokuData.eDir)j);
			}
		}
	}

	public void TouchDirectionButton(Data.SugorokuData.eDir dir)
	{
		if (sugoroku_data.dice_value - sugoroku_data.move_count > 0)
		{
			int num = 0;
			touch.SetEnabled(enabled: false);
			num = MapList[0][sugoroku_data.current].connects[(int)dir];
			if (sugoroku_data.move_count == 0)
			{
				sugoroku_data.SetMoveCount(sugoroku_data.move_count + 1);
				sugoroku_data.SetHistry(sugoroku_data.move_count, num);
			}
			else if (num != sugoroku_data.history[sugoroku_data.move_count - 1])
			{
				sugoroku_data.SetMoveCount(sugoroku_data.move_count + 1);
				sugoroku_data.SetHistry(sugoroku_data.move_count, num);
			}
			else
			{
				sugoroku_data.SetHistry(sugoroku_data.move_count, -1);
				sugoroku_data.SetMoveCount(sugoroku_data.move_count - 1);
			}
			for (int i = 0; i < 4; i++)
			{
				cursor.SetEnabled(enabled: false, (Data.SugorokuData.eDir)i);
			}
			sugoroku_data.SetCurrent(num);
			if (sugoroku_data.dice_value - sugoroku_data.move_count == 0)
			{
				CheckNextMass(masses[num]);
			}
			car.SetDir(dir);
			sugoroku_data.SetCarDir(dir);
			Vector2 current_pos = car.animator.transform.localPosition;
			car.animator.transform.localPosition = StartCarPosition(dir, current_pos);
			float duration = (dir != Data.SugorokuData.eDir.LEFT && dir != Data.SugorokuData.eDir.RIGHT) ? 0.8f : 1f;
			car.Move(SetCarPosition(dir), duration, EndCarMove);
			Manager.sound.PlaySe(Sound.eSe.BUS);
		}
	}

	private Vector2 StartCarPosition(Data.SugorokuData.eDir dir, Vector2 current_pos)
	{
		if (dir == Data.SugorokuData.eDir.LEFT || dir == Data.SugorokuData.eDir.RIGHT)
		{
			Vector2 vector = current_pos;
			Vector2 vector2 = SetCarPosition(dir);
			vector.y = vector2.y;
			current_pos = vector;
			return current_pos;
		}
		Vector2 vector3 = current_pos;
		Vector2 vector4 = SetCarPosition(dir);
		vector3.x = vector4.x;
		current_pos = vector3;
		return current_pos;
	}

	private void EndCarMove()
	{
		int num = sugoroku_data.dice_value - sugoroku_data.move_count;
		car.ShowReminder(num);
		if (num == 0)
		{
			car.HideReminder();
			Manager.sound.PlaySe(Sound.eSe.BUS_BRAKE);
			DoMassEvent(masses[sugoroku_data.current]);
		}
		else
		{
			Manager.sound.PlaySe(Sound.eSe.HEART);
			touch.SetEnabled(enabled: true);
		}
		cursor.root.transform.localPosition = SetCursorPosition();
		hideDirectionButton();
	}

	public void PreFinishThrow(int dice_value)
	{
		pre_dice_value = dice_value;
	}

	public void FinishedThrow(int dice_value)
	{
		sugoroku_data.SetDiceValue(dice_value);
		sugoroku_data.SetMoveCount(0);
		for (int i = 0; i < 7; i++)
		{
			sugoroku_data.SetHistry(i, -1);
		}
		sugoroku_data.SetHistry(sugoroku_data.move_count, sugoroku_data.current);
		car.ShowReminder(dice_value);
		hideDirectionButton();
		dice_area.ChangeDiceTouchState(true_false: false);
		Manager.sound.PlaySe(Sound.eSe.BALLOON);
		Dice.Clear();
		pre_dice_value = -1;
	}

	private void CheckNextMass(Mass mass)
	{
		if (mass.type == Mass.eType.FARM || mass.type == Mass.eType.COIN || mass.type == Mass.eType.EXP)
		{
			sugoroku_data.SetMassEvent(1);
		}
	}

	private void CheckMassEvent(Mass mass)
	{
		if (sugoroku_data.dice_value - sugoroku_data.move_count != 0)
		{
			return;
		}
		if (sugoroku_data.mass_event == 1 && (mass.type == Mass.eType.FARM || mass.type == Mass.eType.COIN || mass.type == Mass.eType.EXP))
		{
			DoMassEvent(mass);
		}
		else if (mass.type == Mass.eType.FARM)
		{
			sugoroku_data.SetFarmMass(-1);
			if (balloon != null)
			{
				UnityEngine.Object.Destroy(balloon);
			}
			RemoveFarmMass(sugoroku_data.current);
			SetFarmMass(sugoroku_data.map_id, sugoroku_data.current);
		}
		else if (mass.type == Mass.eType.COIN)
		{
			sugoroku_data.SetCoinMass(-1);
			if (coin_bag != null)
			{
				coin_bag.Destroy();
			}
			SetCoinMass(sugoroku_data.map_id, sugoroku_data.current);
		}
		else if (mass.type == Mass.eType.EXP)
		{
			sugoroku_data.SetExpMass(-1);
			if (exp_bag != null)
			{
				exp_bag.Destroy();
			}
			SetExpMass(sugoroku_data.map_id, sugoroku_data.current);
		}
	}

	private void DoMassEvent(Mass mass)
	{
		if (mass.type == Mass.eType.FARM)
		{
			int coin = Price.CoinSugorokuFarm();
			int exp = Price.ExpSugorokuFarm();
			if (mass_sequence != null)
			{
				mass_sequence.Kill();
			}
			mass_sequence = DOTween.Sequence();
			Vector3 position = mass.renderer.transform.position;
			float x = position.x;
			Vector3 position2 = mass.renderer.transform.position;
			Vector2 pos = new Vector2(x, position2.y + 0.7f);
			Effect.Run(Resources.Load("Prefab/effect_sugoroku_farm") as GameObject, pos, mass.renderer.transform);
			Manager.sound.PlaySe(Sound.eSe.LEVELUP_SMALL);
			farm_prompt = Common.CreateSugorokuFarmPrompt(coin, exp, mass.renderer.transform, new Vector2(0f, 0.3f));
			mass_sequence.AppendInterval(1f);
			mass_sequence.AppendCallback(delegate
			{
				Effect.Coin(coin, 6, farm_prompt.coin.transform.position, CoinManager.CoinTarget(), Color.white);
				Manager.sound.PlaySe(Sound.eSe.COINS_ARRIVAL);
			});
			mass_sequence.AppendCallback(delegate
			{
				Effect.Exp(exp, 6, farm_prompt.exp.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
				{
					sugoroku_data.SetMassEvent(0);
				});
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			mass_sequence.AppendInterval(2f);
			mass_sequence.AppendCallback(delegate
			{
				UnityEngine.Object.Destroy(farm_prompt.root);
				farm_prompt = null;
			});
			int prev_farm_mass = sugoroku_data.farm_mass;
			mass_sequence.AppendCallback(delegate
			{
				RemoveFarmMass(prev_farm_mass);
			});
			mass_sequence.AppendInterval(2f);
			mass_sequence.AppendCallback(delegate
			{
				SetFarmMass(sugoroku_data.map_id, prev_farm_mass);
			});
			mass_sequence.AppendCallback(delegate
			{
				dice_area.EnableDice();
				touch.SetEnabled(enabled: true);
			});
			mass_sequence.Play();
			sugoroku_data.SetFarmMass(-1);
		}
		else if (mass.type == Mass.eType.COIN)
		{
			coin_bag.AddValue(Price.CoinSugorokuBag());
			Common.TouchBag(coin_bag, delegate
			{
				sugoroku_data.SetMassEvent(0);
			});
			if (mass_sequence != null)
			{
				mass_sequence.Kill();
			}
			mass_sequence = DOTween.Sequence();
			mass_sequence.AppendInterval(2f);
			int coin_mass = sugoroku_data.coin_mass;
			mass_sequence.AppendCallback(delegate
			{
				SetCoinMass(sugoroku_data.map_id, coin_mass);
			});
			mass_sequence.AppendCallback(delegate
			{
				dice_area.EnableDice();
				touch.SetEnabled(enabled: true);
			});
			mass_sequence.Play();
			sugoroku_data.SetCoinMass(-1);
		}
		else if (mass.type == Mass.eType.EXP)
		{
			exp_bag.AddValue(Price.ExpSugorokuBag());
			Common.TouchBag(exp_bag, delegate
			{
				sugoroku_data.SetMassEvent(0);
			});
			if (mass_sequence != null)
			{
				mass_sequence.Kill();
			}
			mass_sequence = DOTween.Sequence();
			mass_sequence.AppendInterval(2f);
			int exp_mass = sugoroku_data.exp_mass;
			mass_sequence.AppendCallback(delegate
			{
				SetExpMass(sugoroku_data.map_id, exp_mass);
			});
			mass_sequence.AppendCallback(delegate
			{
				dice_area.EnableDice();
				touch.SetEnabled(enabled: true);
			});
			mass_sequence.Play();
			sugoroku_data.SetExpMass(-1);
		}
		else if (mass.type == Mass.eType.NORMAL)
		{
			Manager.sound.PlaySe(Sound.eSe.EXP);
			Effect.Exp(manager.data.level, 1, mass.renderer.transform.position, LevelManager.ExpTarget(), Color.white).AddFinishCallback(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.EXP_ARRIVAL);
			});
			dice_area.EnableDice();
			touch.SetEnabled(enabled: true);
		}
		else if (mass.type == Mass.eType.HOME)
		{
			Manager.sound.PlaySe(Sound.eSe.GET);
			dice_area.EnableDice();
			touch.SetEnabled(enabled: true);
		}
		else
		{
			touch.SetEnabled(enabled: true);
		}
	}

	private void RemoveFarmMass(int prev)
	{
		Mass mass = masses[prev];
		Vector3 position = mass.renderer.transform.position;
		float x = position.x;
		Vector3 position2 = mass.renderer.transform.position;
		Vector2 pos = new Vector2(x, position2.y + 0.1f);
		Effect.Run(Resources.Load("Prefab/effect_trash") as GameObject, pos, base.transform);
		Manager.sound.PlaySe(Sound.eSe.SMOKE);
		mass.type = Mass.eType.NORMAL;
		mass.renderer.sprite = SpriteManager.GetSugorokuMass(Mass.eType.NORMAL);
	}

	private void SetFarmMass(int map_id, int prev = -1)
	{
		if (sugoroku_data.farm_mass == -1)
		{
			Utils.Log("SetFarmMass prev = " + prev);
			List<int> list = new List<int>();
			for (int i = 0; i < MapList[map_id].Length; i++)
			{
				if (MapList[map_id][i].reserved == Mass.eType.FARM && prev != i)
				{
					Utils.Log("Add i = " + i);
					list.Add(i);
				}
			}
			sugoroku_data.SetFarmMass(list[UnityEngine.Random.Range(0, list.Count)]);
		}
		Mass mass = masses[sugoroku_data.farm_mass];
		mass.type = Mass.eType.FARM;
		mass.renderer.sprite = SpriteManager.GetSugorokuMass(Mass.eType.FARM);
		mass.renderer.transform.localPosition = mass.default_pos + MapList[map_id][sugoroku_data.farm_mass].farm_offset;
		if (prev != -1)
		{
			masses[prev].renderer.transform.localPosition = masses[prev].default_pos;
			Vector3 position = mass.renderer.transform.position;
			float x = position.x;
			Vector3 position2 = mass.renderer.transform.position;
			Vector2 pos = new Vector2(x, position2.y + 0.1f);
			Effect.Run(Resources.Load("Prefab/effect_trash") as GameObject, pos, base.transform);
			Manager.sound.PlaySe(Sound.eSe.SMOKE);
		}
		SetGoalBalloon(mass);
	}

	private void SetNormalMass(int map_id)
	{
		for (int i = 0; i < MapList[map_id].Length; i++)
		{
			if (sugoroku_data.farm_mass == i)
			{
				continue;
			}
			if (MapList[map_id][i].reserved == Mass.eType.HOME)
			{
				masses[i].type = Mass.eType.HOME;
				masses[i].renderer.sprite = SpriteManager.GetSugorokuMass(Mass.eType.HOME);
				if (sugoroku_data.current == -1)
				{
					sugoroku_data.SetCurrent(i);
				}
			}
			else
			{
				masses[i].type = Mass.eType.NORMAL;
				masses[i].renderer.sprite = SpriteManager.GetSugorokuMass(Mass.eType.NORMAL);
			}
		}
	}

	private void SetCoinMass(int map_id, int prev = -1)
	{
		if (sugoroku_data.coin_mass == -1)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < MapList[map_id].Length; i++)
			{
				if (MapList[map_id][i].reserved == Mass.eType.NORMAL && sugoroku_data.coin_mass != i && sugoroku_data.exp_mass != i && prev != i)
				{
					list.Add(i);
				}
			}
			sugoroku_data.SetCoinMass(list[UnityEngine.Random.Range(0, list.Count)]);
		}
		Mass mass = masses[sugoroku_data.coin_mass];
		mass.type = Mass.eType.COIN;
		coin_bag = Common.OccurCoinBagMini(0, mass.renderer.transform.TransformPoint(new Vector3(0f, -0.03f, 0f)), null, mass.renderer.transform);
		coin_bag.SetOrderInLayer(81);
		coin_bag.touch.SetEnabled(enabled: false);
		if (prev != -1)
		{
			masses[prev].type = Mass.eType.NORMAL;
		}
	}

	private void SetExpMass(int map_id, int prev = -1)
	{
		if (sugoroku_data.exp_mass == -1)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < MapList[map_id].Length; i++)
			{
				if (MapList[map_id][i].reserved == Mass.eType.NORMAL && sugoroku_data.coin_mass != i && sugoroku_data.exp_mass != i && prev != i)
				{
					list.Add(i);
				}
			}
			sugoroku_data.SetExpMass(list[UnityEngine.Random.Range(0, list.Count)]);
		}
		Mass mass = masses[sugoroku_data.exp_mass];
		mass.type = Mass.eType.EXP;
		exp_bag = Common.OccurExpBagMini(0, mass.renderer.transform.TransformPoint(new Vector3(0f, -0.03f, 0f)), null, mass.renderer.transform);
		exp_bag.SetOrderInLayer(81);
		exp_bag.touch.SetEnabled(enabled: false);
		if (prev != -1)
		{
			masses[prev].type = Mass.eType.NORMAL;
		}
	}

	private void SetGoalBalloon(Mass mass)
	{
		GameObject original = Resources.Load("prefab/sugoroku_goal") as GameObject;
		balloon = UnityEngine.Object.Instantiate(original, mass.renderer.transform, worldPositionStays: false);
	}
}
