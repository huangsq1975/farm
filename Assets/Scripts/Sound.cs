using DG.Tweening;
using UnityEngine;

public class Sound : MonoBehaviour
{
	public enum eBgm
	{
		OPENING,
		FIELD,
		MENU,
		ENDING,
		FIELD_RESORT,
		MAX
	}

	public enum eSe
	{
		CLICK,
		CANCEL,
		BEEP,
		LEVELUP_SMALL,
		LEVELUP_BIG,
		COIN,
		COIN_ARRIVAL,
		ALBUM,
		EGG_BREAK,
		GRASS_CUT,
		GRASS_GROW,
		EXP,
		EXP_ARRIVAL,
		CONSTRUCT,
		SMOKE,
		APPEAR,
		HEART,
		COINS,
		COINS_ARRIVAL,
		SALE,
		SALE_ARRIVAL,
		GET,
		FISHING,
		HARVEST,
		EAT,
		BUS,
		BUS_BRAKE,
		BUS_DOOR,
		HORSE_WALK,
		PRESENT,
		FALL,
		SHEEP,
		COW,
		CHICKEN,
		HORSE,
		PIG,
		ALPACA,
		GOAT,
		TURKEY,
		BIRD,
		WOODPECKER,
		DUCK,
		CHICK,
		BELL,
		DOOR,
		BALLOON,
		UP,
		DOWN,
		BYE,
		SLIDE,
		NEGATIVE,
		KEEPOUT,
		HUG,
		MASK,
		PANIC,
		GOOD,
		FULL,
		WHITEOUT,
		SNORE,
		LAUGH,
		KEYBOARD,
		DICE,
		SPLASH,
		SEAGULL,
		MONKEY,
		SEA_LION,
		WALRUS,
		OWL,
		APPLAUSE,
		SPOUTING,
		TANK,
		SEEWEED_CUT,
		MAX
	}

	public const int AUDIOSOURCE_MAX = 4;

	private const int BGM_INDEX = 0;

	private const int GET_INDEX = 1;

	private const int EAT_INDEX = 3;

	private const int PLAYONESHOT_START = 1;

	public AudioSource[] audioSource = new AudioSource[4];

	public AudioClip[] BGM = new AudioClip[5];

	public AudioClip[] SE = new AudioClip[72];

	private Manager manager;

	private int index;

	private Sequence sequence;

	public void Init(Manager m)
	{
		manager = m;
		index = 1;
		audioSource = GetComponents<AudioSource>();
		sequence = DOTween.Sequence();
	}

	public void PlayBgm(eBgm type)
	{
		switch (type)
		{
		case eBgm.OPENING:
			audioSource[0].clip = (Resources.Load("sound/bgm_" + (int)type) as AudioClip);
			break;
		case eBgm.FIELD:
			if (Data.farm_type == Data.eFarmType.NORMAL)
			{
				audioSource[0].clip = BGM[(int)type];
			}
			else if (Data.farm_type == Data.eFarmType.RESORT)
			{
				audioSource[0].clip = (Resources.Load("sound/bgm_4") as AudioClip);
			}
			break;
		default:
			audioSource[0].clip = BGM[(int)type];
			break;
		}
		audioSource[0].Play();
	}

	public void SetVolumeBgm(float volume)
	{
		sequence.Kill();
		audioSource[0].volume = volume;
	}

	public void ToSmallBgm(float fin_volume, float dulation)
	{
		sequence.AppendCallback(delegate
		{
			DOTween.To(() => audioSource[0].volume, delegate(float setting)
			{
				audioSource[0].volume = setting;
			}, fin_volume, dulation);
		});
		sequence.Play();
	}

	private int GetSeIndex()
	{
		int result = index;
		index = ((index + 1 == 4) ? 1 : (index + 1));
		return result;
	}

	public void PlaySe(eSe type)
	{
		audioSource[GetSeIndex()].PlayOneShot(SE[(int)type]);
	}

	public void PlayEat()
	{
		if (!audioSource[3].isPlaying)
		{
			audioSource[3].clip = SE[24];
			audioSource[3].Play();
		}
	}

	public void PlayGet()
	{
		int num = 1;
		while (true)
		{
			if (num < 3)
			{
				if (!audioSource[num].isPlaying)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		audioSource[num].clip = SE[21];
		audioSource[num].Play();
	}

	public void ClickSound()
	{
		PlaySe(eSe.CLICK);
	}

	public void CancelSound()
	{
		PlaySe(eSe.CANCEL);
	}

	public void GlassCutSe(Data.eFarmType ftype)
	{
		switch (ftype)
		{
		case Data.eFarmType.NORMAL:
			Manager.sound.PlaySe(eSe.GRASS_CUT);
			break;
		case Data.eFarmType.RESORT:
			Manager.sound.PlaySe(eSe.SEEWEED_CUT);
			break;
		}
	}
}
