using UnityEngine;

public class LevelManager : CoinManager
{
	public static LevelManager self;

	public TextMesh level_text;

	private Transform exp_target;

	private void Start()
	{
		self = this;
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		exp_target = base.transform.Find("exp_text");
		current_value = manager.data.exp;
		SetValueText();
		SetLevelText();
	}

	private void Update()
	{
		UpdateText();
		LevelUp();
		ExpGuageAnimation();
	}

	public override int GetData()
	{
		return manager.data.exp;
	}

	public override void SetValueText()
	{
		value_text.text = string.Empty + current_value + "/" + Data.tFARM_LEVEL[manager.data.level];
	}

	private void SetLevelText()
	{
		level_text.text = string.Empty + manager.data.level;
	}

	private void LevelUp()
	{
		if (current_value < Data.tFARM_LEVEL[manager.data.level])
		{
			return;
		}
		int expCount = manager.data.exp - Data.tFARM_LEVEL[manager.data.level];
		if (manager.data.level < 99)
		{
			manager.data.SetLevelUp();
			SetLevelText();
			manager.data.SetExpCount(expCount);
			current_value = 0;
			Common.CreateLevelUpEffect(manager.data.level, manager.data.main_type);
			if (manager.data.level == 2 && Data.farm_type == Data.eFarmType.NORMAL)
			{
				manager.sugoroku.SetEnabled(enabled: true, effect: true);
			}
			manager.map.UpdateFacilityFlower();
		}
	}

	private void ExpGuageAnimation()
	{
		Animation component = GetComponent<Animation>();
		float normalizedTime = (float)current_value / (float)Data.tFARM_LEVEL[manager.data.level];
		component["level_exp_gauge"].normalizedTime = normalizedTime;
		component["level_exp_gauge"].speed = 0f;
		component.Play();
	}

	public void ForceRefresh()
	{
		current_value = -1;
		UpdateText();
		SetLevelText();
	}

	public static Transform ExpTarget()
	{
		return self.exp_target.transform;
	}
}
