using UnityEngine;

public class Review : SingletonMonoBehaviour<Review>
{
	[SerializeField]
	[Header("-----各平台ID-----")]
	private int _iosID = 1328266576;

	[SerializeField]
	private string _androidID = "net.appmaga.pixelfarm";

	private bool _canReviewInApp;

	public bool CanReviewInApp => _canReviewInApp;

	protected override void Init()
	{
		base.Init();
	}

	public void RequestReview()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=" + _androidID);
	}
}
