using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private enum etype
	{
		BGM,
		SE_1,
		SE_2,
		SE_3,
		MAX
	}

	private const float POS_X = 0.6f;

	private SpriteRenderer[] b_sr = new SpriteRenderer[2];

	private float[] pos_x = new float[2];

	private Manager manager;

	private Sound sd;

	public static readonly float[] tBUTTON_POS = new float[11]
	{
		-0.6f,
		-0.48f,
		-0.36f,
		-0.24f,
		-0.12f,
		0f,
		0.12f,
		0.24f,
		0.36f,
		0.48f,
		0.6f
	};

	public void Init()
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		sd = Manager.sound.GetComponent<Sound>();
		for (int i = 0; i < 2; i++)
		{
			b_sr[i] = base.transform.Find("volume_" + i + "/button").GetComponent<SpriteRenderer>();
			pos_x[i] = sd.audioSource[i].volume;
			int num = Mathf.RoundToInt(pos_x[i] * 10f);
			b_sr[i].transform.localPosition = new Vector2(tBUTTON_POS[num], 0f);
			SetBgmVolumeBar(i);
		}
		SpriteRenderer component = base.transform.Find("black_bg").GetComponent<SpriteRenderer>();
		component.transform.localScale = new Vector3(50f, 50f, 1f);
		component.color = new Color(0f, 0f, 0f, 0.3f);
	}

	public void SliderDown(int i)
	{
		SetButtonPos(i);
		SetBgmVolumeBar(i);
		SetSoundVolume(i);
	}

	public void SliderMove(int i)
	{
		SetButtonPos(i);
		SetBgmVolumeBar(i);
		SetSoundVolume(i);
	}

	private void SetButtonPos(int i)
	{
		Vector2 v = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		Vector2 vector = base.transform.InverseTransformPoint(v);
		if (vector.x > 0.6f)
		{
			vector.x = 0.6f;
		}
		else if (vector.x < -0.6f)
		{
			vector.x = -0.6f;
		}
		float num = vector.x - -0.5f;
		pos_x[i] = Mathf.Round(num * 10f) / 10f;
		if (pos_x[i] > 1f)
		{
			pos_x[i] = 1f;
		}
		else if (pos_x[i] < 0f)
		{
			pos_x[i] = 0f;
		}
		int num2 = Mathf.RoundToInt(pos_x[i] * 10f);
		b_sr[i].transform.localPosition = new Vector2(tBUTTON_POS[num2], 0f);
	}

	private void SetBgmVolumeBar(int i)
	{
		Animation component = base.transform.Find("volume_" + i).GetComponent<Animation>();
		component["volume_gauge"].normalizedTime = pos_x[i];
		component["volume_gauge"].speed = 0f;
		component.Play();
	}

	private void SetSoundVolume(int i)
	{
		if (i == 0)
		{
			sd.audioSource[0].volume = pos_x[i];
			return;
		}
		for (int j = 1; j < 4; j++)
		{
			sd.audioSource[j].volume = pos_x[i];
		}
	}
}
