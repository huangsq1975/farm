using DG.Tweening;
using UnityEngine;

public class LevelUpEvent : MonoBehaviour
{
	public void Run(int level, Data.eMainType type)
	{
		GameObject obj = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefab/effect_levelup"));
		Effect component = obj.GetComponent<Effect>();
		TextMesh component2 = component.transform.Find("text").GetComponent<TextMesh>();
		component2.text = level.ToString();
		PartsController controller = GetComponent<PartsController>();
		controller.Init(MainCharacter.style[(int)(type + (int)Data.farm_type * 2)]);
		Vector2 vector = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(new Vector2(Screen.width, 0f));
		vector.x += 0.5f;
		Vector3 position = base.transform.position;
		vector.y = position.y;
		Vector2 v = vector;
		v.x = 0f;
		Vector2 vector2 = vector;
		vector2.x = 0f - vector.x;
		controller.Play(PartsController.eAnimType._WALK_1_SIDE, 3f);
		base.transform.position = vector;
		Manager.sound.PlaySe(Sound.eSe.LEVELUP_BIG);
		Sequence s = DOTween.Sequence();
		s.Append(base.transform.DOMoveX(0f, 0.8f));
		s.AppendCallback(delegate
		{
			controller.Play(PartsController.eAnimType._STAY_1_DOWN, 1f);
		});
		s.AppendInterval(0.2f);
		s.Append(base.transform.DOJump(v, 0.05f, 6, 2f).SetEase(Ease.Linear));
		s.AppendInterval(0.2f);
		s.AppendCallback(delegate
		{
			controller.Play(PartsController.eAnimType._WALK_1_SIDE, 3f);
		});
		s.Append(base.transform.DOMoveX(vector2.x, 0.8f));
		s.AppendCallback(delegate
		{
			Object.Destroy(obj);
			Object.Destroy(base.gameObject);
		});
	}
}
