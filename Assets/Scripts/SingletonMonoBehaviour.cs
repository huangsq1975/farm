using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviourWithInit where T : MonoBehaviourWithInit
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if ((Object)_instance == (Object)null)
			{
				_instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
				if ((Object)_instance == (Object)null)
				{
					UnityEngine.Debug.LogError(typeof(T) + " is nothing");
				}
				else
				{
					_instance.InitIfNeeded();
				}
			}
			return _instance;
		}
	}

	protected sealed override void Awake()
	{
		if (!(this == Instance))
		{
			UnityEngine.Debug.LogError(typeof(T) + " is duplicated");
		}
	}
}
