using UnityEngine;

public class ReviewManager : MonoBehaviour
{
	public enum eType
	{
		START,
		GOOD,
		BAD,
		MAX
	}

	private const int TEXT_IMAGE_MAX = 3;

	public static GameObject review_popup;

	private GameObject ok_button;

	private GameObject cancel_button;

	private SpriteRenderer[] text_image = new SpriteRenderer[3];

	private SpriteRenderer black_bg;

	private Manager manager;

	private int review_state;

	public void Init(GameObject obj)
	{
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		review_popup = obj;
		ok_button = review_popup.transform.Find("button_bg1").gameObject;
		cancel_button = review_popup.transform.Find("button_bg2").gameObject;
		for (int i = 0; i < 3; i++)
		{
			text_image[i] = obj.transform.Find("text_image" + i).GetComponent<SpriteRenderer>();
		}
		review_state = 0;
		ChangeMessage((eType)review_state);
		black_bg = review_popup.transform.Find("black_bg").GetComponent<SpriteRenderer>();
		black_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		black_bg.color = new Color(0f, 0f, 0f, 0.3f);
	}

	public void TouchOk()
	{
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		if (review_state == 0)
		{
			review_state = 1;
			ChangeMessage((eType)review_state);
		}
		else if (review_state == 1)
		{
			DeleteReviewPopUp();
			SingletonMonoBehaviour<Review>.Instance.RequestReview();
		}
		else
		{
			DeleteReviewPopUp();
		}
	}

	public void TouchCancel()
	{
		Manager.sound.PlaySe(Sound.eSe.CLICK);
		review_state = 2;
		ChangeMessage((eType)review_state);
		cancel_button.SetActive(value: false);
		if (manager.data.lang == Data.eLang.JP)
		{
			ok_button.transform.localPosition = new Vector3(0f, -0.235f, 0f);
		}
		else
		{
			ok_button.transform.localPosition = new Vector3(0f, -0.272f, 0f);
		}
	}

	private void ChangeMessage(eType type)
	{
		for (int i = 0; i < 3; i++)
		{
			text_image[i].enabled = false;
		}
		text_image[(int)type].enabled = true;
	}

	public static void DeleteReviewPopUp()
	{
		if (review_popup != null)
		{
			UnityEngine.Object.Destroy(review_popup.gameObject);
			review_popup = null;
		}
	}
}
