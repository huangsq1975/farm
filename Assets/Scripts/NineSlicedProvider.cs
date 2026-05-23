using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class NineSlicedProvider : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;

	private Material _material;

	public bool stencil;

	private bool old_stencil;

	public bool stencil3;

	private bool old_stencil3;

	public float top;

	public float bottom;

	public float left;

	public float right;

	public float sx;

	public float sy;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		Adjust();
	}

	private void Update()
	{
		Adjust();
	}

	public void Adjust()
	{
		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		if (_material == null || stencil != old_stencil || stencil3 != old_stencil3)
		{
			if (stencil)
			{
				_material = UnityEngine.Object.Instantiate(new Material(Shader.Find("Custom/NineSlicedShaderStencil")));
			}
			else if (stencil3)
			{
				_material = UnityEngine.Object.Instantiate(new Material(Shader.Find("Custom/NineSlicedShaderStencilRef3")));
			}
			else
			{
				_material = UnityEngine.Object.Instantiate(new Material(Shader.Find("Custom/NineSlicedShader")));
			}
			old_stencil = stencil;
			_spriteRenderer.material = _material;
		}
		float width = _spriteRenderer.sprite.rect.width;
		float height = _spriteRenderer.sprite.rect.height;
		Vector4 border = _spriteRenderer.sprite.border;
		float x = border.x;
		Vector4 border2 = _spriteRenderer.sprite.border;
		float y = border2.y;
		Vector4 border3 = _spriteRenderer.sprite.border;
		float z = border3.z;
		Vector4 border4 = _spriteRenderer.sprite.border;
		float w = border4.w;
		left = x / width;
		bottom = y / height;
		right = z / width;
		top = w / height;
		Vector3 localScale = base.transform.localScale;
		sx = localScale.x;
		Vector3 localScale2 = base.transform.localScale;
		sy = localScale2.y;
		_material.SetFloat("top", top);
		_material.SetFloat("bottom", bottom);
		_material.SetFloat("right", right);
		_material.SetFloat("left", left);
		_material.SetFloat("sx", sx);
		_material.SetFloat("sy", sy);
	}
}
