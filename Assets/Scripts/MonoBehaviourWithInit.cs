using UnityEngine;

public class MonoBehaviourWithInit : MonoBehaviour
{
	private bool _isInitialized;

	public void InitIfNeeded()
	{
		if (!_isInitialized)
		{
			Init();
			_isInitialized = true;
		}
	}

	protected virtual void Init()
	{
	}

	protected virtual void Awake()
	{
	}
}
