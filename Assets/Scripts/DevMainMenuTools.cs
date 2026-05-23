using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 開發版主界面：清空存檔並退出遊戲。
/// </summary>
public class DevMainMenuTools : MonoBehaviour
{
	private GameObject m_canvasRoot;

	private void Start()
	{
		if (!Data.IsDevelopmentBuild())
		{
			enabled = false;
			return;
		}
		EnsureEventSystem();
		CreateClearSaveButton();
	}

	private void Update()
	{
		if (m_canvasRoot == null || Manager.events == null)
		{
			return;
		}
		m_canvasRoot.SetActive(Manager.events.state == Event.eState.NONE);
	}

	private static void EnsureEventSystem()
	{
		if (EventSystem.current != null)
		{
			return;
		}
		GameObject es = new GameObject("EventSystem");
		es.AddComponent<EventSystem>();
		es.AddComponent<StandaloneInputModule>();
	}

	private void CreateClearSaveButton()
	{
		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		m_canvasRoot = new GameObject("DevMainMenuCanvas");
		Canvas canvas = m_canvasRoot.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 32766;
		CanvasScaler scaler = m_canvasRoot.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(800f, 600f);
		m_canvasRoot.AddComponent<GraphicRaycaster>();
		GameObject buttonRoot = new GameObject("ClearSaveButton");
		buttonRoot.transform.SetParent(m_canvasRoot.transform, worldPositionStays: false);
		RectTransform buttonRect = buttonRoot.AddComponent<RectTransform>();
		buttonRect.anchorMin = new Vector2(1f, 1f);
		buttonRect.anchorMax = new Vector2(1f, 1f);
		buttonRect.pivot = new Vector2(1f, 1f);
		buttonRect.anchoredPosition = new Vector2(-12f, -12f);
		buttonRect.sizeDelta = new Vector2(160f, 36f);
		Image buttonImage = buttonRoot.AddComponent<Image>();
		buttonImage.color = new Color(0.85f, 0.25f, 0.2f, 0.92f);
		Button button = buttonRoot.AddComponent<Button>();
		button.targetGraphic = buttonImage;
		button.onClick.AddListener(OnClearSaveAndQuit);
		GameObject labelGo = new GameObject("Text");
		labelGo.transform.SetParent(buttonRoot.transform, worldPositionStays: false);
		RectTransform labelRect = labelGo.AddComponent<RectTransform>();
		labelRect.anchorMin = Vector2.zero;
		labelRect.anchorMax = Vector2.one;
		labelRect.offsetMin = Vector2.zero;
		labelRect.offsetMax = Vector2.zero;
		Text label = labelGo.AddComponent<Text>();
		label.font = font;
		label.text = "清檔並退出";
		label.alignment = TextAnchor.MiddleCenter;
		label.color = Color.white;
		label.fontSize = 14;
		label.supportRichText = false;
	}

	private void OnClearSaveAndQuit()
	{
		Data.ClearAllSaveData();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
