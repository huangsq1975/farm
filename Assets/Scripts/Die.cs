using UnityEngine;

public class Die : MonoBehaviour
{
	public int value;

	public int pre_value;

	public bool get_value;

	public bool get_prevalue;

	protected Vector3 localHitNormalized;

	protected float validMargin = 0.45f;

	private bool dice_sound;

	private int same_count;

	private int prev_value;

	private float callbackTime;

	private const float NOTIFY_VALUE = 1f;

	public bool rolling => !(GetComponent<Rigidbody>().velocity.sqrMagnitude < 0.1f) || !(GetComponent<Rigidbody>().angularVelocity.sqrMagnitude < 0.1f);

	protected bool localHit
	{
		get
		{
			Ray ray = new Ray(base.transform.position + new Vector3(0f, 2f, 0f) * base.transform.localScale.magnitude, Vector3.up * -1f);
			RaycastHit hitInfo = default(RaycastHit);
			if (GetComponent<Collider>().Raycast(ray, out hitInfo, 3f * base.transform.localScale.magnitude))
			{
				Transform transform = base.transform;
				Vector3 point = hitInfo.point;
				float x = point.x;
				Vector3 point2 = hitInfo.point;
				float y = point2.y;
				Vector3 point3 = hitInfo.point;
				localHitNormalized = transform.InverseTransformPoint(x, y, point3.z).normalized;
				return true;
			}
			return false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Manager.sound.PlaySe(Sound.eSe.DICE);
	}

	private void GetValue()
	{
		value = 0;
		float num = 1f;
		int num2 = 1;
		Vector3 lhs;
		do
		{
			lhs = HitVector(num2);
			if (lhs != Vector3.zero && valid(localHitNormalized.x, lhs.x) && valid(localHitNormalized.y, lhs.y) && valid(localHitNormalized.z, lhs.z))
			{
				float num3 = Mathf.Abs(localHitNormalized.x - lhs.x) + Mathf.Abs(localHitNormalized.y - lhs.y) + Mathf.Abs(localHitNormalized.z - lhs.z);
				if (num3 < num)
				{
					value = num2;
					num = num3;
				}
			}
			num2++;
		}
		while (lhs != Vector3.zero);
	}

	private void Update()
	{
		if (rolling || !localHit || get_value)
		{
			return;
		}
		GetValue();
		if (value == 0)
		{
			return;
		}
		if (same_count == 0)
		{
			prev_value = value;
			same_count++;
		}
		else if (prev_value == value)
		{
			same_count++;
		}
		else
		{
			prev_value = value;
			same_count = 0;
		}
		if (same_count >= 5)
		{
			callbackTime += Time.deltaTime;
			if (callbackTime >= 1f)
			{
				get_value = true;
				Dice.EndRoll(this);
			}
		}
		if (!get_value)
		{
			pre_value = value;
			Dice.PreEndRoll(this);
		}
	}

	protected bool valid(float t, float v)
	{
		if (t > v - validMargin && t < v + validMargin)
		{
			return true;
		}
		return false;
	}

	protected Vector3 HitVector(int side)
	{
		switch (side)
		{
		case 1:
			return new Vector3(0f, 0f, 1f);
		case 2:
			return new Vector3(0f, -1f, 0f);
		case 3:
			return new Vector3(-1f, 0f, 0f);
		case 4:
			return new Vector3(1f, 0f, 0f);
		case 5:
			return new Vector3(0f, 1f, 0f);
		case 6:
			return new Vector3(0f, 0f, -1f);
		default:
			return Vector3.zero;
		}
	}
}
