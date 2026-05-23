using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
	public enum eScene
	{
		LOAD,
		GAME
	}

	private static eScene load_scene = eScene.GAME;

	private void Start()
	{
		StartCoroutine(LoadSceneAsync());
	}

	public static void SetNextScene(eScene next)
	{
		load_scene = next;
		SceneManager.LoadSceneAsync(eScene.LOAD.ToString().ToLower());
	}

	private IEnumerator LoadSceneAsync()
	{
		float time = Time.realtimeSinceStartup;
		AsyncOperation ope = SceneManager.LoadSceneAsync(eScene.GAME.ToString().ToLower());
		ope.allowSceneActivation = false;
		while (Time.realtimeSinceStartup - time < 1f)
		{
			yield return null;
		}
		ope.allowSceneActivation = true;
	}
}
