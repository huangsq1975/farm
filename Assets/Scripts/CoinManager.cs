using UnityEngine;

public class CoinManager : MonoBehaviour
{
	public TextMesh value_text;

	public int current_value;

	public Manager manager;

	private Transform coin_target;

	public static CoinManager self;

	private void Start()
	{
		self = this;
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		coin_target = base.transform.Find("bg");
		current_value = manager.data.coin;
		OverflowValue();
		SetValueText();
	}

	private void Update()
	{
		UpdateText();
	}

	public virtual int GetData()
	{
		return manager.data.coin;
	}

	public void UpdateText()
	{
		OverflowValue();
		int data = GetData();
		if (current_value == data)
		{
			return;
		}
		current_value = data;
		SetValueText();
	}

	public virtual void OverflowValue()
	{
		if (manager.data.coin >= 99999999)
		{
			manager.data.SetCoinCount(99999999);
		}
	}

	public virtual void SetValueText()
	{
		value_text.text = string.Empty + current_value;
	}

	public void ForceRefresh()
	{
		current_value = -1;
		UpdateText();
	}

	public static Transform CoinTarget()
	{
		return self.coin_target;
	}
}
