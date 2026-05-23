using UnityEngine;
using UnityEngine.Events;

public class Dice : MonoBehaviour
{
	public static bool rolling = true;

	private static GameObject cameraObject;

	private static RollingDie rollDice;

	private static GameObject throwPoint;

	public static GameObject prefab(Vector3 position, Vector3 rotation, Vector3 scale)
	{
		Object original = Resources.Load("Prefab/dice");
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
		gameObject.transform.position = position;
		gameObject.transform.Rotate(rotation);
		return gameObject;
	}

	private static Vector3 Force()
	{
		Vector3 b = Vector3.zero + new Vector3(0f, 0.5f + 4f * UnityEngine.Random.value, -3f * UnityEngine.Random.value);
		return Vector3.Lerp(throwPoint.transform.position, b, 1f).normalized * (-8f - UnityEngine.Random.value * 2f);
	}

	private static void CreateDiceCamera()
	{
		if (cameraObject == null)
		{
			cameraObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefab/DiceCamera"));
			throwPoint = cameraObject.transform.Find("throw_point").gameObject;
		}
	}

	public static void Destroy()
	{
		Clear();
		if (cameraObject != null)
		{
			UnityEngine.Object.Destroy(cameraObject);
		}
		cameraObject = null;
		throwPoint = null;
	}

	public static void Throw(UnityAction<int> pre_callback, UnityAction<int> callback)
	{
		CreateDiceCamera();
		Clear();
		Vector3 vector = Force();
		Roll(force: new Vector3(0f, -1500f, 50f), spawnPoint: throwPoint.transform.position, pre_callback: pre_callback, callback: callback);
	}

	public static void Roll(Vector3 spawnPoint, Vector3 force, UnityAction<int> pre_callback, UnityAction<int> callback)
	{
		rolling = true;
		GameObject gameObject = prefab(spawnPoint, Vector3.zero, Vector3.one);
		gameObject.transform.Rotate(new Vector3(UnityEngine.Random.value * 360f, UnityEngine.Random.value * 360f, UnityEngine.Random.value * 360f));
		gameObject.GetComponent<Rigidbody>().AddTorque(5000f, 5000f, 5000f);
		gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		rollDice = new RollingDie(gameObject, pre_callback, callback);
	}

	public static void PreEndRoll(Die die)
	{
		if (rollDice.die == die)
		{
			rollDice.pre_callback(rollDice.die.pre_value);
		}
	}

	public static void EndRoll(Die die)
	{
		if (rollDice.die == die)
		{
			rollDice.callback(rollDice.die.value);
		}
	}

	public static int Value(string dieType)
	{
		return rollDice.die.value;
	}

	public static void Clear()
	{
		if (rollDice != null)
		{
			rollDice.Destroy();
		}
		rollDice = null;
		rolling = false;
	}

	private bool IsRolling()
	{
		return rollDice.rolling;
	}

	public static bool IsExist()
	{
		bool result = false;
		if (rollDice != null && rollDice.gameObject != null)
		{
			result = true;
		}
		return result;
	}
}
