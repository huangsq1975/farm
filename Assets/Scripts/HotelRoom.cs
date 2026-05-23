using UnityEngine;

public class HotelRoom : MonoBehaviour
{
	private HotelManager hm;

	private PartsController controller;

	private Transform bag_place;

	private Common.Bag bag;

	private Worker.eType worker_type;

	private int price;

	private Vector2 disp_pos = new Vector2(-0.506f, 1.598f);

	public void Init(HotelManager hm, int price, Worker.eType worker_type, int room_id)
	{
		this.hm = hm;
		this.worker_type = worker_type;
		base.transform.parent = hm.transform.parent;
		controller = base.transform.Find("contents/worker").GetComponent<PartsController>();
		controller.Init(Worker.style[(int)worker_type]);
		controller.SetSortingOrder(10051);
		controller.Play(PartsController.eAnimType._GET_1_UP, 1f);
		bag_place = base.transform.Find("contents/bag");
		base.transform.Find("contents/text").GetComponent<TextMesh>().text = string.Empty + HotelManager.tHOTEL_ROOM_ID[(int)Data.farm_type, room_id];
		base.transform.localPosition = disp_pos;
		this.price = price;
	}

	private void CreateCoinBag()
	{
		bag = Common.OccurCoinBagMini(price, bag_place.transform.position, FixBill, base.transform);
		bag.touch.SetEnabled(enabled: false);
	}

	private void FixBill()
	{
		bag = null;
	}

	private void FinishAnim()
	{
		hm.AutoCollected();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void GetCoinBag()
	{
		bag.touch.ClickUp.Invoke();
	}
}
