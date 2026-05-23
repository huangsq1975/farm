using UnityEngine;

public static class MapIconLabel
{
	private static readonly Vector3 DefaultLocalPosition = new Vector3(0f, 0.55f, -0.01f);

	private static readonly Vector3 DefaultLocalScale = new Vector3(0.02f, 0.02f, 1f);

	private static readonly Color FillColor = new Color(1f, 0.92f, 0.2f, 1f);

	private static readonly Color OutlineColor = new Color(0.12f, 0.08f, 0.02f, 1f);

	private static readonly float OutlineOffset = 0.05f;

	private static readonly Vector2[] OutlineOffsets = new Vector2[8]
	{
		new Vector2(-1f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, -1f),
		new Vector2(0f, 1f),
		new Vector2(-0.7f, -0.7f),
		new Vector2(0.7f, -0.7f),
		new Vector2(-0.7f, 0.7f),
		new Vector2(0.7f, 0.7f)
	};

	public static void Set(Transform parent, string text, int outlineSortingOrder)
	{
		if (parent == null)
		{
			return;
		}
		Transform existingLabel = parent.Find("label");
		if (existingLabel != null)
		{
			Object.Destroy(existingLabel.gameObject);
		}
		GameObject labelRootObj = new GameObject("label");
		Transform labelRoot = labelRootObj.transform;
		labelRoot.SetParent(parent, false);
		labelRoot.localPosition = DefaultLocalPosition;
		labelRoot.localScale = DefaultLocalScale;
		for (int i = 0; i < OutlineOffsets.Length; i++)
		{
			Vector2 offset = OutlineOffsets[i];
			Vector3 localPosition = new Vector3(offset.x * OutlineOffset, offset.y * OutlineOffset, 0.01f);
			CreateText(labelRoot, "outline_" + i, text, OutlineColor, outlineSortingOrder, localPosition);
		}
		CreateText(labelRoot, "text", text, FillColor, outlineSortingOrder + 1, Vector3.zero);
	}

	private static void CreateText(Transform parent, string objectName, string text, Color color, int sortingOrder, Vector3 localPosition)
	{
		GameObject textObj = new GameObject(objectName);
		textObj.transform.SetParent(parent, false);
		textObj.transform.localPosition = localPosition;
		TextMesh textMesh = textObj.AddComponent<TextMesh>();
		textMesh.text = text;
		textMesh.color = color;
		textMesh.anchor = TextAnchor.MiddleCenter;
		textMesh.alignment = TextAlignment.Center;
		textMesh.fontStyle = FontStyle.Bold;
		textMesh.characterSize = 1f;
		ApplyUiFontStyle(textMesh);
		Renderer renderer = textObj.GetComponent<Renderer>();
		renderer.sortingOrder = sortingOrder;
		CustomText customText = textObj.AddComponent<CustomText>();
		customText.order_in_layer = sortingOrder;
		customText.Init();
		customText.SetText(text);
	}

	private static void ApplyUiFontStyle(TextMesh textMesh)
	{
		TextMesh referenceText = GameObject.Find("UI/LevelManager/level_text")?.GetComponent<TextMesh>();
		if (referenceText == null)
		{
			return;
		}
		textMesh.font = referenceText.font;
		Renderer targetRenderer = textMesh.GetComponent<Renderer>();
		Renderer referenceRenderer = referenceText.GetComponent<Renderer>();
		if (targetRenderer != null && referenceRenderer != null)
		{
			targetRenderer.sharedMaterial = referenceRenderer.sharedMaterial;
		}
	}
}
