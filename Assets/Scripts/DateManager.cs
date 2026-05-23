using UnityEngine;

public class DateManager : MonoBehaviour
{
	private Manager manager;

	public float prev_time;

	public float elapsed_count;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Manager");
		manager = gameObject.GetComponent<Manager>();
	}

	private void Update()
	{
		manager.data.game_time++;
	}
}
