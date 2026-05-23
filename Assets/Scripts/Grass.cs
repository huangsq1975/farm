using UnityEngine;
using UnityEngine.Events;

public class Grass : MonoBehaviour
{
	public enum eState
	{
		LONG,
		CUT,
		SHORT,
		PLANT,
		FIXSHORT,
		MAX
	}

	public eState state;

	private const int GRASS_EMPTY = 0;

	private const int GRASS_FRAME = 6;

	private const string GRASS_APPEAR_ANIM = "grass_appear";

	private const string GRASS_CUT_ANIM = "grass_cut";

	private const float TIMER_PLANT = 10f;

	public Facility facility;

	private MainCharacter main;

	private Animator animator;

	private TouchEvent touch;

	private SpriteRenderer grass_sr;

	private GameObject coin_exp;

	private UnityAction<Grass> cut_callback;

	private float next_plant_time;

	private float now_time;

	private bool wait_plant;

	private const int OCCUR_COIN = 5;

	private const int OCCUR_EXP = 10;

	private void SetPlantTime()
	{
		wait_plant = true;
		now_time = 0f;
		next_plant_time = 10f;
	}

	public void Open(Facility f, MainCharacter m, bool empty)
	{
		facility = f;
		main = m;
		wait_plant = false;
		next_plant_time = 0f;
		animator = GetComponent<Animator>();
		touch = GetComponent<TouchEvent>();
		grass_sr = animator.transform.Find("grass_L").GetComponent<SpriteRenderer>();
		if (empty)
		{
			FixShort();
		}
		else
		{
			Long();
		}
	}

	public void Touch(TouchEvent t)
	{
		touch.SetEnabled(enabled: false);
		main.RegistGrassCutQueue(this);
	}

	public void AutoCut()
	{
		if (touch.GetEnabled() && facility.worker != null)
		{
			touch.SetEnabled(enabled: false);
			facility.worker.RegistGrassCutQueue(this);
		}
	}

	public void Long()
	{
		state = eState.LONG;
		Utils.Play(animator, "grass_appear", 0f, 6f);
	}

	public void Cut(UnityAction<Grass> call)
	{
		state = eState.CUT;
		Utils.Play(animator, "grass_cut", 1f);
		Manager.sound.GlassCutSe(Data.farm_type);
		cut_callback = call;
	}

	public void Plant()
	{
		if (coin_exp != null)
		{
			UnityEngine.Object.Destroy(coin_exp);
			coin_exp = null;
		}
		state = eState.PLANT;
		Utils.Play(animator, "grass_appear", 1f);
		Manager.sound.PlaySe(Sound.eSe.GRASS_GROW);
		facility.NotifyGrassPlant();
	}

	public void Short()
	{
		state = eState.SHORT;
		Utils.Play(animator, "grass_appear", 0f, 0f);
	}

	public void FixShort()
	{
		state = eState.FIXSHORT;
		touch.SetEnabled(enabled: false);
		Utils.Play(animator, "grass_appear", 0f, 0f);
	}

	private void Update()
	{
		CheckAnimation();
		CheckPlantTime();
	}

	private void CheckAnimation()
	{
		if (state == eState.CUT && !Utils.IsPlaying(animator))
		{
			state = eState.SHORT;
			SetPlantTime();
			facility.NotifyGrassCut();
			OccurCoinExp();
			cut_callback(this);
		}
		else if (state == eState.PLANT && !Utils.IsPlaying(animator))
		{
			state = eState.LONG;
			touch.SetEnabled(enabled: true);
		}
	}

	private void CheckPlantTime()
	{
		if (wait_plant && state == eState.SHORT)
		{
			now_time += Time.deltaTime;
			if (now_time >= next_plant_time)
			{
				wait_plant = false;
				next_plant_time = 0f;
				Plant();
			}
		}
	}

	public void Pause()
	{
		if (wait_plant)
		{
			wait_plant = false;
			next_plant_time -= now_time;
			now_time = 0f;
		}
	}

	public void Restart()
	{
		if (0f < next_plant_time)
		{
			wait_plant = true;
			now_time = 0f;
		}
	}

	private void OccurCoinExp()
	{
		if (Manager.events.state == Event.eState.NONE)
		{
			int num = UnityEngine.Random.Range(0, 20);
			if (facility.manager.data.first_coin == 0 && facility.my_id == 2)
			{
				num = 5;
				facility.manager.data.SetFirstCoin();
			}
			switch (num)
			{
			case 5:
			{
				int coin = Price.CoinGrassCut();
				Vector3 position3 = base.transform.position;
				float x2 = position3.x + 0.05f;
				Vector3 position4 = base.transform.position;
				coin_exp = Common.OccurMapCoin(coin, new Vector2(x2, position4.y), grass_sr.sortingOrder, CoinExpDestroy);
				break;
			}
			case 10:
			{
				int exp = Price.ExpGrassCut();
				Vector3 position = base.transform.position;
				float x = position.x + 0.05f;
				Vector3 position2 = base.transform.position;
				coin_exp = Common.OccurMapExp(exp, new Vector2(x, position2.y), grass_sr.sortingOrder, CoinExpDestroy);
				break;
			}
			}
		}
	}

	private void CoinExpDestroy()
	{
		coin_exp = null;
	}

	public GameObject GetCoinExp()
	{
		return coin_exp;
	}

	public bool IsLong()
	{
		return (state == eState.LONG) ? true : false;
	}
}
