using UnityEngine;

public class OfficeManager : MonoBehaviour
{
	private const string HomeLabelText = "办公室";

	private const int HomeLabelSortingOrder = 3;

	private Manager manager;

	public TouchEvent home;

	public GameObject menuPrefab;

	public Menu menu;

	private Animation new_icon;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Manager");
		manager = gameObject.GetComponent<Manager>();
		home = transform.Find("home_" + (int)Data.farm_type + "(Clone)").GetComponent<TouchEvent>();
		if (home != null)
		{
			MapIconLabel.Set(home.transform, HomeLabelText, HomeLabelSortingOrder);
		}
	}

	public void TouchOffice(TouchEvent touch)
	{
		menu = Instantiate(menuPrefab, transform, worldPositionStays: false).GetComponent<Menu>();
		menu.Init();
	}

	public void SetNotice()
	{
		if (new_icon == null)
		{
			new_icon = Utils.Load("Prefab/new_office", transform).GetComponent<Animation>();
			Utils.Play(new_icon, new_icon.clip.name, 1f, 0f);
		}
	}

	public void RemoveNotice()
	{
		if (new_icon != null)
		{
            Destroy(new_icon.gameObject);
			new_icon = null;
		}
	}
}
