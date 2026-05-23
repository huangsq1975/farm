using UnityEngine;
using UnityEngine.Events;

internal class RollingDie
{
	public GameObject gameObject;

	public Die die;

	public UnityAction<int> pre_callback;

	public UnityAction<int> callback;

	public bool rolling => die.rolling;

	public int value => die.value;

	public RollingDie(GameObject gameObject, UnityAction<int> pre_callback, UnityAction<int> callback)
	{
		this.gameObject = gameObject;
		this.pre_callback = pre_callback;
		this.callback = callback;
		die = (Die)gameObject.GetComponent(typeof(Die));
	}

	public void Destroy()
	{
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		gameObject = null;
		die = null;
		callback = null;
	}
}
