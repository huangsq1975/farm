using DG.Tweening;
using UnityEngine;

public class Fowl : WildAnimal
{
	public int f_id;

	public int f_space;

	public new int SORTING_ORDER_CHARACTER = 75;

	private Sequence sequence;

	public void Open(Manager m, int facility_id, int space, eState sts, int type, int id)
	{
		manager = m;
		state = sts;
		f_id = facility_id;
		f_space = space;
		Initialize(type, id);
		move = eMove.WAIT;
	}

	public override void Initialize(int t, int id)
	{
		my_id = id;
		type = (eType)t;
		category = WildAnimal.TypeToCategory(type);
		direction = eDIRECTION.DOWN;
		renderers = new CharRenderers(base.transform);
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = (RuntimeAnimatorController)Object.Instantiate(Resources.Load("Animation/wildanimal/" + type.ToString().ToLower()));
		motion = eMOTIONTYPE._STAY_;
		direction = eDIRECTION.DOWN;
		PlayTurnAnimation();
		SetSortingOrder();
		SetHeartTimer();
		CreateSequence();
	}

	private void Crying()
	{
		switch (type)
		{
		case eType.CHICK_1:
			Manager.sound.PlaySe(Sound.eSe.CHICK);
			break;
		case eType.DUCK_1:
			Manager.sound.PlaySe(Sound.eSe.DUCK);
			break;
		case eType.SWAN_1:
			Manager.sound.PlaySe(Sound.eSe.DUCK);
			break;
		}
		CreateSequence();
	}

	private void CreateSequence()
	{
		sequence = DOTween.Sequence();
		sequence.AppendInterval(Random.Range(3f, 35f));
		sequence.AppendCallback(Crying);
		sequence.Play();
	}

	public override void SetSortingOrder()
	{
		if (category == eCATEGORY.FOWL)
		{
			renderers.SetSortingOrderAll(SORTING_ORDER_CHARACTER + manager.map.facility_list[f_id].GetBaseOrder());
		}
		else
		{
			renderers.SetSortingOrder(SORTING_ORDER_CHARACTER + manager.map.facility_list[f_id].GetBaseOrder());
		}
	}

	public override void ReturnBase()
	{
		sequence.Kill();
		Utils.Log("ReturnBase : heart_timer=" + heart_timer + " type=" + type + " my_id=" + my_id);
		if (heart_timer != null)
		{
			Timer.Remove(heart_timer);
			heart_timer = null;
		}
	}
}
