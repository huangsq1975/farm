using UnityEngine;

[ExecuteInEditMode]
public class CustomText : MonoBehaviour
{
	private TextMesh text;

	public int order_in_layer;

	public bool mask;

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		text = GetComponent<TextMesh>();
		SetOrderInLayer(order_in_layer);
		if (mask)
		{
			SetShader();
		}
	}

	public void SetText(string str)
	{
		text.text = str;
	}

	public void SetShader()
	{
		Renderer component = GetComponent<Renderer>();
		//component.sharedMaterial = Shader.Find("Custom/CustomFontShader");
	}

	public void SetOrderInLayer(int oil)
	{
		text = GetComponent<TextMesh>();
		Renderer component = GetComponent<Renderer>();
		component.sortingOrder = oil;
		order_in_layer = oil;
	}
}
