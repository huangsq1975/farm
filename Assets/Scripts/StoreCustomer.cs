using System;
using UnityEngine;

public class StoreCustomer : PartsCharacter
{
	private StoreManager store_manager;

	public Vector2 target;

	private Vector2 start_pos;

	private const float GOTO_TIME = 3f;

	private const float BUY_TIME = 1f;

	private const float BACK_TIME = 3f;

	private ulong start_time;

	public const float BUY_ITEM_Y = 0.2f;

	private bool buyed;

	private float tutorial_diff_time;

	public void Run(StoreManager sm, float target_x, int base_order_in_layer)
	{
		store_manager = sm;
		hair_no = UnityEngine.Random.Range(0, hair_side_sprite.Length);
		face_no = UnityEngine.Random.Range(0, face_side_sprite.Length);
		tops_no = UnityEngine.Random.Range(0, clothes_set.Length);
		bottoms_no = UnityEngine.Random.Range(0, clothes_set.Length);
		for (int i = 0; i < base.parts.Length; i++)
		{
			SpriteRenderer[] parts = base.parts;
			int num = i;
			Transform transform = base.transform;
			ePARTS ePARTS = (ePARTS)i;
			parts[num] = transform.Find(ePARTS.ToString().ToLower()).GetComponent<SpriteRenderer>();
		}
		frame = 0;
		SetState(eSTATE.WALK, eDIR.SIDE, _flipX: false);
		SetSprite(state, dir, frame);
		SetOrderInLayer(base_order_in_layer);
		start_time = (ulong)DateTime.Now.Ticks;
		start_pos = base.transform.position;
		Vector3 position = base.transform.position;
		target = new Vector2(target_x, position.y);
	}

	private void Update()
	{
		float num = (float)(double)(ulong)(DateTime.Now.Ticks - (long)start_time) / 1E+07f;
		if (Manager.events.state != Event.eState.NONE)
		{
			tutorial_diff_time += Time.deltaTime;
			num = tutorial_diff_time;
		}
		if (num < 3f)
		{
			base.transform.position = Vector2.Lerp(start_pos, target, num / 3f);
		}
		else if (num < 4f)
		{
			if (Manager.events.state != Event.eState.ENDING && dir != eDIR.UP)
			{
				StoreManager.BuyItem buyItem = store_manager.BuyHarvest();
				if (buyItem != null)
				{
					store_manager.AppearItem(buyItem, parts[0].transform, new Vector2(0f, 0.2f), parts[1].sortingOrder);
				}
				else
				{
					SetFeelsReglet();
				}
				buyed = true;
				SetState(eSTATE.STAY, eDIR.UP, _flipX: false);
				UpdateSpriteImmediate();
			}
			else
			{
				SetState(eSTATE.STAY, eDIR.UP, _flipX: false);
			}
			base.transform.position = target;
		}
		else
		{
			if (!buyed)
			{
				StoreManager.BuyItem buyItem2 = store_manager.BuyHarvest();
				if (buyItem2 != null)
				{
					store_manager.Stock(buyItem2.coin);
					buyed = true;
				}
			}
			if (dir != eDIR.SIDE)
			{
				SetState(eSTATE.WALK, eDIR.SIDE, _flipX: true);
				UpdateSpriteImmediate();
			}
			else
			{
				SetState(eSTATE.WALK, eDIR.SIDE, _flipX: true);
			}
			num -= 4f;
			if (num < 3f)
			{
				base.transform.position = Vector2.Lerp(target, start_pos, num / 3f);
			}
			else
			{
				base.transform.position = start_pos;
				store_manager.DeleteStoreCustomer(this);
			}
		}
		UpdateSprite();
	}

	private void SetFeelsReglet()
	{
		GameObject original = Resources.Load("Prefab/store_buy_ng") as GameObject;
		Animator component = UnityEngine.Object.Instantiate(original, base.transform, worldPositionStays: false).GetComponent<Animator>();
		Utils.Play(component, "feels_regret", 1f);
	}
}
