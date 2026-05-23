using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Prompt : MonoBehaviour
{
	[Serializable]
	public class Button
	{
		public SpriteRenderer sr;

		public Animation anim;

		public BoxCollider2D collider;

		public UnityAction call;

		public Button(GameObject root, UnityAction _call, UnityAction up_call, UnityAction down_call)
		{
			anim = root.GetComponent<Animation>();
			collider = root.GetComponent<BoxCollider2D>();
			sr = root.transform.Find("contents/sprite").GetComponent<SpriteRenderer>();
			call = _call;
			TouchEvent component = root.GetComponent<TouchEvent>();
			component.ClickUp.AddListener(up_call);
			component.ClickDown.AddListener(down_call);
		}
	}

	[SerializeField]
	private Button ok;

	[SerializeField]
    private Button cancel;

	public static Prompt CreateCoinPrompt(int coin, int need_coin, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		Debug.Log("Prompt.CreateCoinPrompt t_parent:" + t_parent.name);
        Prompt prompt = Create("Prefab/buy_prompt", t_parent, pos, call_ok, call_cancel);
		GameObject gameObject = prompt.transform.Find("bg/sprite").gameObject;
		gameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPromptCoinType(Data.farm_type);
		TextMesh component = gameObject.transform.Find("text").GetComponent<TextMesh>();
		component.text = need_coin.ToString();
		Transform transform = gameObject.transform;
		Vector3 localPosition = gameObject.transform.localPosition;
		float x = localPosition.x - 0.03f * (float)(component.text.Length - 1);
		Vector3 localPosition2 = gameObject.transform.localPosition;
		transform.localPosition = new Vector2(x, localPosition2.y);
		if (coin < need_coin)
		{
			prompt.ok.collider.enabled = false;
			prompt.ok.sr.color = new Color(1f, 1f, 1f, 0.2f);
			component.color = Common.TextDisableColor;
		}
		return prompt;
	}

	public static void CreateAddFrame(Prompt prompt)
	{
		GameObject original = Resources.Load("Prefab/buy_prompt_add_frame") as GameObject;
		Animator component = UnityEngine.Object.Instantiate(original, prompt.transform.Find("bg"), worldPositionStays: false).transform.Find("enemy").GetComponent<Animator>();
		Utils.Play(component, "op_enemy_1", 1f, 0f);
	}

	public static Prompt CreateTrashPrompt(Sprite sprite, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		Debug.Log("Prompt.CreateTrashPrompt");
        Prompt prompt = Create("Prefab/trash_prompt", t_parent, pos, call_ok, call_cancel);
		SpriteRenderer component = prompt.transform.Find("bg/sprite").gameObject.GetComponent<SpriteRenderer>();
		component.sprite = sprite;
		return prompt;
	}

	public static Prompt CreatePresentPrompt(FarmAnimal.eType type, int coin, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		Debug.Log("Prompt.CreatePresentPrompt");
		Prompt prompt = Create("Prefab/present_prompt", t_parent, pos, call_ok, call_cancel);
		SpriteRenderer component = prompt.transform.Find("bg").GetComponent<SpriteRenderer>();
		component.sprite = SpriteManager.GetPresentPromptBg(Data.farm_type);
		GameObject gameObject = prompt.transform.Find("bg/sprite").gameObject;
		gameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPromptCoinType(Data.farm_type);
		TextMesh component2 = gameObject.transform.Find("text").GetComponent<TextMesh>();
		component2.text = coin.ToString();
		Transform transform = gameObject.transform;
		Vector3 localPosition = gameObject.transform.localPosition;
		float x = localPosition.x - 0.03f * (float)(component2.text.Length - 1);
		Vector3 localPosition2 = gameObject.transform.localPosition;
		transform.localPosition = new Vector2(x, localPosition2.y);
		Animator component3 = prompt.transform.Find("bg/animal").gameObject.GetComponent<Animator>();
		string str = "Animation/farmanimal/";
		component3.runtimeAnimatorController = (Resources.Load(str + type.ToString().ToLower()) as RuntimeAnimatorController);
		string name = type.ToString().ToLower() + "_album_1_down";
		Utils.Play(component3, name, 1f, 1f);
		return prompt;
	}

	public static Prompt CreateVideoCoinPrompt(int coin, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel = null)
	{
		Prompt prompt = createVideoBasePrompt(0f, t_parent, pos, call_ok, call_cancel);
		GameObject gameObject = prompt.transform.Find("bg/area/sprite").gameObject;
		gameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPromptCoinType(Data.farm_type);
		TextMesh component = gameObject.transform.Find("text").GetComponent<TextMesh>();
		component.text = coin.ToString();
		Transform transform = gameObject.transform;
		Vector3 localPosition = gameObject.transform.localPosition;
		float x = localPosition.x - 0.03f * (float)(component.text.Length - 1);
		Vector3 localPosition2 = gameObject.transform.localPosition;
		transform.localPosition = new Vector2(x, localPosition2.y);
		return prompt;
	}

	public static Prompt CreateVideoExpPrompt(int exp, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel = null)
	{
		Prompt prompt = createVideoBasePrompt(1f, t_parent, pos, call_ok, call_cancel);
		GameObject gameObject = prompt.transform.Find("bg/area/sprite").gameObject;
		TextMesh component = gameObject.transform.Find("text").GetComponent<TextMesh>();
		component.text = "+" + exp;
		Transform transform = gameObject.transform;
		Vector3 localPosition = gameObject.transform.localPosition;
		float x = localPosition.x - 0.03f * (float)(component.text.Length - 1);
		Vector3 localPosition2 = gameObject.transform.localPosition;
		transform.localPosition = new Vector2(x, localPosition2.y);
		return prompt;
	}

	public static Prompt CreateVideoStorePrompt(Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		return createVideoBasePrompt(2f, t_parent, pos, call_ok, call_cancel);
	}

	public static Prompt CreateVideoConstructPrompt(Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		return createVideoBasePrompt(3f, t_parent, pos, call_ok, call_cancel);
	}

	public static Prompt CreateVideoSugorokuPrompt(Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		return createVideoBasePrompt(4f, t_parent, pos, call_ok, call_cancel);
	}

	private static Prompt createVideoBasePrompt(float id, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		GameObject original = Resources.Load("Prefab/present_video_prompt") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false);
		Transform transform = gameObject.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = gameObject.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		Prompt prompt = gameObject.GetComponent<Prompt>();
		Animator component = prompt.gameObject.GetComponent<Animator>();
		Utils.Play(component, "present_video", 0f, id / 4f);
		prompt.ok = new Button(prompt.transform.Find("bg/icon_ok").gameObject, call_ok, delegate
		{
			prompt.ClickOK();
			UnityEngine.Object.Destroy(prompt.gameObject);
		}, delegate
		{
			prompt.ClickOKDown();
		});
		prompt.cancel = new Button(prompt.transform.Find("bg/icon_cancel").gameObject, delegate
		{
			if (call_cancel != null)
			{
				call_cancel();
			}
			UnityEngine.Object.Destroy(prompt.gameObject);
		}, delegate
		{
			prompt.ClickCancel();
		}, delegate
		{
			prompt.ClickCancelDown();
		});
		Transform transform2 = prompt.transform.Find("bg/icon_ok");
		Transform transform3 = prompt.transform.Find("bg/icon_cancel");
		Vector3 localPosition2 = transform2.transform.localPosition;
		Vector3 localPosition3 = transform3.transform.localPosition;
		prompt.transform.localScale = Vector2.zero;
		prompt.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		transform2.DOLocalMove(localPosition2, 0f);
		transform3.DOLocalMove(localPosition3, 0f);
		return prompt;
	}

	private static Prompt Create(string prefab_name, Transform t_parent, Vector2 pos, UnityAction call_ok, UnityAction call_cancel)
	{
		GameObject original = Resources.Load(prefab_name) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, t_parent, worldPositionStays: false);
		Transform transform = gameObject.transform;
		float x = pos.x;
		float y = pos.y;
		Vector3 localPosition = gameObject.transform.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition.z);
		Prompt prompt = gameObject.GetComponent<Prompt>();
		prompt.ok = new Button(prompt.transform.Find("bg/icon_ok").gameObject, call_ok, delegate
		{
			prompt.ClickOK();
		}, delegate
		{
			prompt.ClickOKDown();
		});
		prompt.cancel = new Button(prompt.transform.Find("bg/icon_cancel").gameObject, call_cancel, delegate
		{
			prompt.ClickCancel();
		}, delegate
		{
			prompt.ClickCancelDown();
		});
		return prompt;
	}

	public void ClickOKDown()
	{
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		ok.anim.Play();
	}

	public void ClickCancelDown()
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
		cancel.anim.Play();
	}

	public void ClickOK()
	{
		Debug.Log("Prompt.ClickOK");
		ok.call();
	}

	public void ClickCancel()
	{
		if (cancel.call != null)
		{
			cancel.call();
		}
	}

	public void SetOrderInLayer(int base_order_in_layer)
	{
		Utils.SetOrderInLayer(base.gameObject, base_order_in_layer);
	}
}
